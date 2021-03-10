using System;

#if __UNIFIED__
using CoreAnimation;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using UIKit;
#else
using MonoTouch.CoreAnimation;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;

using CGRect = System.Drawing.RectangleF;
using CGSize = System.Drawing.SizeF;
using CGPoint = System.Drawing.PointF;
using nfloat = System.Single;
#endif

namespace SidePanels
{
    public class SidePanelController : UIViewController, IUIGestureRecognizerDelegate
    {
        private static UIImage defaultImage;

        private static readonly NSString ja_kvoContext = new NSString();

        private CGRect _centerPanelRestingFrame;
        private CGPoint _locationBeforePan;
        private SidePanelState _state;
        private SidePanelStyle _style;
        private UIViewController _centerPanel;
        private UIViewController _leftPanel;
        private UIViewController _rightPanel;
        private UIView _tapView;
        private bool _centerPanelHidden;

        public SidePanelController()
        {
            BaseInit();
        }

        public SidePanelController(NSCoder coder)
            : base(coder)
        {
            BaseInit();
        }

        public SidePanelController(IntPtr handle)
            : base(handle)
        {
            BaseInit();
        }

        public SidePanelController(string nibName, NSBundle bundle)
            : base(nibName, bundle)
        {
            BaseInit();
        }

        private void BaseInit()
        {
            CenterPanelContainer = new UIView();
            _centerPanelHidden = false;
            LeftPanelContainer = new UIView();
            LeftPanelContainer.Hidden = true;
            RightPanelContainer = new UIView();
            RightPanelContainer.Hidden = true;
            State = SidePanelState.CenterVisible;

            Style = SidePanelStyle.SingleActive;
            LeftPanelPercentWidth = 0.8f;
            RightPanelPercentWidth = 0.8f;
            MinimumMovePercentage = 0.15f;
            MaximumAnimationDuration = 0.2f;
            BounceDuration = 0.1f;
            BouncePercentage = 0.075f;
            PanningLimitedToTopViewController = true;
            RecognizesPanGesture = true;
            AllowLeftOverPan = true;
            AllowRightOverPan = true;
            BounceOnSidePanelOpen = true;
            BounceOnSidePanelClose = false;
            BounceOnCenterPanelChange = true;
            ShouldDelegateAutorotateToVisiblePanel = true;
            AllowRightSwipe = true;
            AllowLeftSwipe = true;
        }

        /// <summary>
        /// Gets or sets the left panel. (Optional)
        /// </summary>
        /// <value>The left panel.</value>
        public UIViewController LeftPanel
        {
            get { return _leftPanel; }
            set
            {
                if (_leftPanel != value)
                {
                    if (_leftPanel != null)
                    {
                        _leftPanel.WillMoveToParentViewController(null);
                        _leftPanel.View.RemoveFromSuperview();
                        _leftPanel.RemoveFromParentViewController();
                    }
                    _leftPanel = value;
                    if (_leftPanel != null)
                    {
                        AddChildViewController(_leftPanel);
                        _leftPanel.DidMoveToParentViewController(this);
                        PlaceButtonForLeftPanel();
                    }
                    if (State == SidePanelState.LeftVisible)
                    {
                        VisiblePanel = _leftPanel;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the center panel. (Required)
        /// </summary>
        /// <value>The center panel.</value>
        public UIViewController CenterPanel
        {
            get { return _centerPanel; }
            set
            {
                // because we are going to show the center panel, make sure the container is ready
                UnhideCenterPanel();

                UIViewController previous = _centerPanel;
                if (_centerPanel != value)
                {
                    if (_centerPanel != null)
                    {
                        _centerPanel.RemoveObserver(this, "view");
                        _centerPanel.RemoveObserver(this, "viewControllers");
                    }
                    _centerPanel = value;
                    if (_centerPanel != null)
                    {
                        _centerPanel.AddObserver(this, "viewControllers", (NSKeyValueObservingOptions)0, ja_kvoContext.Handle);
                        _centerPanel.AddObserver(this, "view", NSKeyValueObservingOptions.Initial, ja_kvoContext.Handle);
                    }
                    if (State == SidePanelState.CenterVisible)
                    {
                        VisiblePanel = _centerPanel;
                    }
                }

                if (IsViewLoaded && State == SidePanelState.CenterVisible)
                {
                    SwapCenter(previous, 0, _centerPanel);
                }
                else if (IsViewLoaded)
                {
                    // update the state immediately to prevent user interaction on the side panels while animating
                    SidePanelState previousState = State;
                    State = SidePanelState.CenterVisible;
                    UIView.AnimateNotify(0.2f, () =>
                    {
                        if (BounceOnCenterPanelChange)
                        {
                            // first move the centerPanel offscreen
                            nfloat x = (previousState == SidePanelState.LeftVisible)
                                ? View.Bounds.Width
                                : -View.Bounds.Width;
                            _centerPanelRestingFrame.X = x;
                        }
                        CenterPanelContainer.Frame = _centerPanelRestingFrame;
                    }, finished =>
                    {
                        SwapCenter(previous, previousState, _centerPanel);
                        ShowCenterPanel(true, false);
                    });
                }
            }
        }

        /// <summary>
        /// Gets or sets the right panel. (Optional)
        /// </summary>
        /// <value>The right panel.</value>
        public UIViewController RightPanel
        {
            get { return _rightPanel; }
            set
            {
                if (_rightPanel != value)
                {
                    if (_rightPanel != null)
                    {
                        _rightPanel.WillMoveToParentViewController(null);
                        _rightPanel.View.RemoveFromSuperview();
                        _rightPanel.RemoveFromParentViewController();
                    }
                    _rightPanel = value;
                    if (_rightPanel != null)
                    {
                        AddChildViewController(_rightPanel);
                        _rightPanel.DidMoveToParentViewController(this);
                    }
                    if (State == SidePanelState.RightVisible)
                    {
                        VisiblePanel = _rightPanel;
                    }
                }
            }
        }

        public SidePanelStyle Style
        {
            get { return _style; }
            set
            {
                if (_style != value)
                {
                    _style = value;
                    if (IsViewLoaded)
                    {
                        ConfigureContainers();
                        LayoutSideContainers(false, 0.0f);
                    }
                }
            }
        }

        /// <summary>
        /// push side panels instead of overlapping them
        /// </summary>
        public bool PushesSidePanels { get; set; }

        /// <summary>
        /// size the left panel based on % of total screen width
        /// </summary>
        public nfloat LeftPanelPercentWidth { get; set; }

        /// <summary>
        /// size the left panel based on this fixed size. overrides LeftPanelPercentWidth
        /// </summary>
        public nfloat LeftPanelFixedWidth { get; set; }

        /// <summary>
        /// the visible width of the left panel
        /// </summary>
        public nfloat LeftPanelVisibleWidth
        {
            get
            {
                if (CenterPanelHidden && ShouldResizeLeftPanel)
                {
                    return View.Bounds.Width;
                }
                else
                {
                    return LeftPanelFixedWidth != 0 ? LeftPanelFixedWidth : (nfloat)Math.Floor(View.Bounds.Width * LeftPanelPercentWidth);
                }
            }
        }

        /// <summary>
        /// size the right panel based on % of total screen width
        /// </summary>
        public nfloat RightPanelPercentWidth { get; set; }

        /// <summary>
        /// size the right panel based on this fixed size. overrides RightPanelPercentWidth
        /// </summary>
        public nfloat RightPanelFixedWidth { get; set; }

        /// <summary>
        /// the visible width of the right panel
        /// </summary>
        public nfloat RightPanelVisibleWidth
        {
            get
            {
                if (CenterPanelHidden && ShouldResizeRightPanel)
                {
                    return View.Bounds.Width;
                }
                else
                {
                    return RightPanelFixedWidth != 0 ? RightPanelFixedWidth : (nfloat)Math.Floor(View.Bounds.Width * RightPanelPercentWidth);
                }
            }
        }

        /// <summary>
        /// the minimum % of total screen width the centerPanel.view must move for panGesture to succeed
        /// </summary>
        /// <value>The minimum move percentage.</value>
        public nfloat MinimumMovePercentage { get; set; }

        /// <summary>
        /// the maximum time panel opening/closing should take. Actual time may be less if panGesture has already moved the view.
        /// </summary>
        /// <value>The maximum duration of the animation.</value>
        public nfloat MaximumAnimationDuration { get; set; }

        /// <summary>
        /// how long the bounce animation should take
        /// </summary>
        /// <value>The duration of the bounce.</value>
        public nfloat BounceDuration { get; set; }

        /// <summary>
        /// how far the view should bounce
        /// </summary>
        /// <value>The bounce percentage.</value>
        public nfloat BouncePercentage { get; set; }

        /// <summary>
        /// should the center panel bounce when you are panning open a left/right panel. (defaults to YES)
        /// </summary>
        /// <value><c>true</c> if bounce on side panel open; otherwise, <c>false</c>.</value>
        public bool BounceOnSidePanelOpen { get; set; }

        /// <summary>
        /// should the center panel bounce when you are panning closed a left/right panel. (defaults to NO)
        /// </summary>
        /// <value><c>true</c> if bounce on side panel close; otherwise, <c>false</c>.</value>
        public bool BounceOnSidePanelClose { get; set; }

        /// <summary>
        /// while changing the center panel, should we bounce it offscreen? (defaults to YES)
        /// </summary>
        /// <value><c>true</c> if bounce on center panel change; otherwise, <c>false</c>.</value>
        public bool BounceOnCenterPanelChange { get; set; }

        /// <summary>
        /// Determines whether the pan gesture is limited to the top ViewController in a UINavigationController/UITabBarController (default is YES)
        /// </summary>
        /// <value><c>true</c> if panning limited to top view controller; otherwise, <c>false</c>.</value>
        public bool PanningLimitedToTopViewController { get; set; }

        /// <summary>
        /// Determines whether showing panels can be controlled through pan gestures, or only through buttons (default is YES)
        /// </summary>
        /// <value><c>true</c> if recognizes pan gesture; otherwise, <c>false</c>.</value>
        public bool RecognizesPanGesture { get; set; }

        /// <summary>
        /// Gives you an image to use for your menu button. 
        /// The image is three stacked white lines, similar to Path 2.0 or Facebook's menu button.
        /// </summary>
        public static UIImage DefaultImage
        {
            get
            {
                if (defaultImage == null)
                {
                    UIGraphics.BeginImageContextWithOptions(new CGSize(20f, 13f), false, 0.0f);

                    UIColor.Black.SetFill();
                    UIBezierPath.FromRect(new CGRect(0, 0, 20, 1)).Fill();
                    UIBezierPath.FromRect(new CGRect(0, 5, 20, 1)).Fill();
                    UIBezierPath.FromRect(new CGRect(0, 10, 20, 1)).Fill();

                    UIColor.White.SetFill();
                    UIBezierPath.FromRect(new CGRect(0, 1, 20, 2)).Fill();
                    UIBezierPath.FromRect(new CGRect(0, 6, 20, 2)).Fill();
                    UIBezierPath.FromRect(new CGRect(0, 11, 20, 2)).Fill();

                    defaultImage = UIGraphics.GetImageFromCurrentImageContext();
                    UIGraphics.EndImageContext();
                }
                return defaultImage;
            }
        }

        /// <summary>
        /// Current state of panels. Use KVO to monitor state changes
        /// </summary>
        /// <value>The state.</value>
        public SidePanelState State
        {
            get { return _state; }
            set
            {
                if (_state != value)
                {
                    var oldState = _state;
                    _state = value;
                    switch (_state)
                    {
                        case SidePanelState.CenterVisible:
                            VisiblePanel = CenterPanel;
                            LeftPanelContainer.UserInteractionEnabled = false;
                            RightPanelContainer.UserInteractionEnabled = false;
                            break;
                        case SidePanelState.LeftVisible:
                            VisiblePanel = LeftPanel;
                            LeftPanelContainer.UserInteractionEnabled = true;
                            break;
                        case SidePanelState.RightVisible:
                            VisiblePanel = RightPanel;
                            RightPanelContainer.UserInteractionEnabled = true;
                            break;
                    }

                    OnStateChanged(new StateChangedEventArgs(oldState, _state));
                }
            }
        }

        /// <summary>
        /// Whether or not the center panel is completely hidden
        /// </summary>
        public bool CenterPanelHidden
        {
            get { return _centerPanelHidden; }
            set
            {
                if (CenterPanelHidden != value)
                {
                    SetCenterPanelHidden(value, true, MaximumAnimationDuration);
                }
            }
        }

        /// <summary>
        /// The currently visible panel
        /// </summary>
        /// <value>The visible panel.</value>
        public UIViewController VisiblePanel { get; private set; }

        /// <summary>
        /// If set to yes, "shouldAutorotateToInterfaceOrientation:" will be passed to visiblePanel instead of handled directly (defaults to YES)
        /// </summary>
        /// <value><c>true</c> if should delegate autorotate to visible panel; otherwise, <c>false</c>.</value>
        public bool ShouldDelegateAutorotateToVisiblePanel { get; set; }

        /// <summary>
        /// Determines whether or not the panel's views are removed when not visble. If YES, leftPanel's views are eligible for viewDidUnload (defaults to NO)
        /// </summary>
        /// <value><c>true</c> if can unload left panel; otherwise, <c>false</c>.</value>
        public bool CanUnloadLeftPanel { get; set; }

        /// <summary>
        /// Determines whether or not the panel's views are removed when not visble. If YES, rightPanel's views are eligible for viewDidUnload (defaults to NO)
        /// </summary>
        /// <value><c>true</c> if can unload right panel; otherwise, <c>false</c>.</value>
        public bool CanUnloadRightPanel { get; set; }

        /// <summary>
        /// Determines whether or not the panel's views should be resized when they are displayed. If yes, the views will be resized to their visible width (defaults to NO)
        /// </summary>
        /// <value><c>true</c> if should resize right panel; otherwise, <c>false</c>.</value>
        public bool ShouldResizeRightPanel { get; set; }

        /// <summary>
        /// Determines whether or not the panel's views should be resized when they are displayed. If yes, the views will be resized to their visible width (defaults to NO)
        /// </summary>
        /// <value><c>true</c> if should resize left panel; otherwise, <c>false</c>.</value>
        public bool ShouldResizeLeftPanel { get; set; }

        /// <summary>
        /// Determines whether or not the center panel can be panned beyound the the visible area of the side panels (defaults to YES)
        /// </summary>
        /// <value><c>true</c> if allow right overpan; otherwise, <c>false</c>.</value>
        public bool AllowRightOverPan { get; set; }

        /// <summary>
        /// Determines whether or not the center panel can be panned beyound the the visible area of the side panels (defaults to YES)
        /// </summary>
        /// <value><c>true</c> if allow left overpan; otherwise, <c>false</c>.</value>
        public bool AllowLeftOverPan { get; set; }

        /// <summary>
        /// Determines whether or not the left panel can be swiped into view. Use if only way to view a panel is with a button (defaults to YES)
        /// </summary>
        /// <value><c>true</c> if allow left swipe; otherwise, <c>false</c>.</value>
        public bool AllowLeftSwipe { get; set; }

        /// <summary>
        /// Determines whether or not the right panel can be swiped into view. Use if only way to view a panel is with a button (defaults to YES)
        /// </summary>
        /// <value><c>true</c> if allow right swipe; otherwise, <c>false</c>.</value>
        public bool AllowRightSwipe { get; set; }

        public UIView LeftPanelContainer { get; private set; }

        public UIView RightPanelContainer { get; private set; }

        public UIView CenterPanelContainer { get; private set; }

        private UIView TapView
        {
            get { return _tapView; }
            set
            {
                if (_tapView != value)
                {
                    if (_tapView != null)
                    {
                        _tapView.RemoveFromSuperview();
                    }
                    _tapView = value;
                    if (_tapView != null)
                    {
                        _tapView.Frame = CenterPanelContainer.Bounds;
                        _tapView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
                        UITapGestureRecognizer tapGesture = new UITapGestureRecognizer(CenterPanelTapped);
                        View.AddGestureRecognizer(tapGesture);
                        if (RecognizesPanGesture)
                        {
                            AddPanGestureToView(_tapView);
                        }
                        CenterPanelContainer.AddSubview(_tapView);
                    }
                }
            }
        }

        public event EventHandler<StateChangedEventArgs> StateChanged;

        public event EventHandler PanStarted;

        public event EventHandler<PanningEventArgs> Panning;

        public event EventHandler<PanCompletedEventArgs> PanCompleted;

        public void ShowLeftPanel(bool animated = true)
        {
            ShowLeftPanel(animated, false);
        }

        public void ShowCenterPanel(bool animated = true)
        {
            if (CenterPanelHidden)
            {
                _centerPanelHidden = false;
                UnhideCenterPanel();
            }
            ShowCenterPanel(animated, false);
        }

        public void ShowRightPanel(bool animated = true)
        {
            ShowRightPanel(animated, false);
        }

        public void ToggleLeftPanel(bool animated = true)
        {
            if (State == SidePanelState.LeftVisible)
            {
                ShowCenterPanel(animated, false);
            }
            else if (State == SidePanelState.CenterVisible)
            {
                ShowLeftPanel(animated, false);
            }
        }

        public void ToggleRightPanel(bool animated = true)
        {
            if (State == SidePanelState.RightVisible)
            {
                ShowCenterPanel(animated, false);
            }
            else if (State == SidePanelState.CenterVisible)
            {
                ShowRightPanel(animated, false);
            }
        }

        /// <summary>
        /// Calling this while the left or right panel is visible causes the center panel to be completely hidden
        /// </summary>
        public void SetCenterPanelHidden(bool hidden, bool animated = true, double duration = 0.0f)
        {
            if (CenterPanelHidden != hidden && State != SidePanelState.CenterVisible)
            {
                _centerPanelHidden = hidden;
                duration = animated ? duration : 0.0f;
                if (hidden)
                {
                    UIView.AnimateNotify(duration, () =>
                    {
                        CGRect frame = CenterPanelContainer.Frame;
                        frame.X = State == SidePanelState.LeftVisible ? CenterPanelContainer.Frame.Width : -CenterPanelContainer.Frame.Width;
                        CenterPanelContainer.Frame = frame;
                        LayoutSideContainers(false, 0);
                        if (ShouldResizeLeftPanel || ShouldResizeRightPanel)
                        {
                            LayoutSidePanels();
                        }
                    }, finished =>
                    {
                        // need to double check in case the user tapped really fast
                        if (CenterPanelHidden)
                        {
                            HideCenterPanel();
                        }
                    });
                }
                else
                {
                    UnhideCenterPanel();
                    UIView.Animate(duration, () =>
                    {
                        if (State == SidePanelState.LeftVisible)
                        {
                            ShowLeftPanel(false);
                        }
                        else
                        {
                            ShowRightPanel(false);
                        }
                        if (ShouldResizeLeftPanel || ShouldResizeRightPanel)
                        {
                            LayoutSidePanels();
                        }
                    });
                }
            }
        }

        /// <summary>
        /// by default applies a black shadow to the container. override in sublcass to change
        /// </summary>
        public virtual void StyleContainer(UIView container, bool animate, double duration)
        {
            UIBezierPath shadowPath = UIBezierPath.FromRoundedRect(container.Bounds, 0.0f);
            if (animate)
            {
                CABasicAnimation animation = CABasicAnimation.FromKeyPath("shadowPath");
                animation.SetFrom(container.Layer.ShadowPath);
                animation.SetTo(shadowPath.CGPath);
                animation.Duration = duration;
                container.Layer.AddAnimation(animation, "shadowPath");
            }
            container.Layer.ShadowPath = shadowPath.CGPath;
            container.Layer.ShadowColor = UIColor.Black.CGColor;
            container.Layer.ShadowRadius = 10.0f;
            container.Layer.ShadowOpacity = 0.75f;
            container.ClipsToBounds = false;
        }

        /// <summary>
        /// by default applies rounded corners to the panel. override in sublcass to change
        /// </summary>
        public virtual void StylePanel(UIView panel)
        {
            panel.ClipsToBounds = true;
        }

        /// <summary>
        /// Default button to place in gestureViewControllers top viewController. Override in sublcass to change look of default button
        /// </summary>
        /// <value>The left button for center panel.</value>
        public virtual UIBarButtonItem GetLeftButtonForCenterPanel()
        {
            return new UIBarButtonItem(DefaultImage, UIBarButtonItemStyle.Plain, (sender, e) => ToggleLeftPanel());
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;

            CenterPanelContainer.Frame = View.Bounds;
            _centerPanelRestingFrame = CenterPanelContainer.Frame;
            LeftPanelContainer.Frame = View.Bounds;
            RightPanelContainer.Frame = View.Bounds;

            ConfigureContainers();

            View.AddSubview(CenterPanelContainer);
            View.AddSubview(LeftPanelContainer);
            View.AddSubview(RightPanelContainer);

            SwapCenter(null, 0, CenterPanel);
            View.BringSubviewToFront(CenterPanelContainer);

            StyleContainer(CenterPanelContainer, false, 0);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            // ensure correct view dimensions
            LayoutSideContainers(false, 0.0f);
            LayoutSidePanels();
            CenterPanelContainer.Frame = AdjustCenterFrame();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            //Account for possible rotation while view appearing
            AdjustCenterFrame();
        }

        public override bool ShouldAutorotate()
        {
            UIViewController visiblePanel = VisiblePanel;

			if (ShouldDelegateAutorotateToVisiblePanel && visiblePanel != null && visiblePanel.RespondsToSelector(new Selector("shouldAutorotate")))
            {
                return visiblePanel.ShouldAutorotate();
            }
            else
            {
                return true;
            }
        }

        public override void WillAnimateRotation(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            CenterPanelContainer.Frame = AdjustCenterFrame();
            LayoutSideContainers(true, duration);
            LayoutSidePanels();
            StyleContainer(CenterPanelContainer, true, duration);
            if (CenterPanelHidden)
            {
                CGRect frame = CenterPanelContainer.Frame;
                frame.X = State == SidePanelState.LeftVisible
                    ? CenterPanelContainer.Frame.Width
                    : -CenterPanelContainer.Frame.Width;
                CenterPanelContainer.Frame = frame;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (CenterPanel != null)
            {
                CenterPanel.RemoveObserver(this, "viewControllers");
                CenterPanel.RemoveObserver(this, "view");
            }

            base.Dispose(disposing);
        }

        private void ConfigureContainers()
        {
            LeftPanelContainer.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleRightMargin;
            RightPanelContainer.AutoresizingMask = UIViewAutoresizing.FlexibleLeftMargin | UIViewAutoresizing.FlexibleHeight;
            CenterPanelContainer.Frame = View.Bounds;
            CenterPanelContainer.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
        }

        private void LayoutSideContainers(bool animate, double duration)
        {
            CGRect leftFrame = View.Bounds;
            CGRect rightFrame = View.Bounds;
            if (Style == SidePanelStyle.MultipleActive)
            {
                // left panel container
                leftFrame.Width = LeftPanelVisibleWidth;
                leftFrame.X = CenterPanelContainer.Frame.X - leftFrame.Width;

                // right panel container
                rightFrame.Width = RightPanelVisibleWidth;
                rightFrame.X = CenterPanelContainer.Frame.X + CenterPanelContainer.Frame.Width;
            }
            else if (PushesSidePanels && !CenterPanelHidden)
            {
                leftFrame.X = CenterPanelContainer.Frame.X - LeftPanelVisibleWidth;
                rightFrame.X = CenterPanelContainer.Frame.X + CenterPanelContainer.Frame.Width;
            }
            LeftPanelContainer.Frame = leftFrame;
            RightPanelContainer.Frame = rightFrame;
            StyleContainer(LeftPanelContainer, animate, duration);
            StyleContainer(RightPanelContainer, animate, duration);
        }

        private void LayoutSidePanels()
        {
            if (RightPanel != null && RightPanel.IsViewLoaded)
            {
                CGRect frame = RightPanelContainer.Bounds;
                if (ShouldResizeRightPanel)
                {
                    if (!PushesSidePanels)
                    {
                        frame.X = RightPanelContainer.Bounds.Width - RightPanelVisibleWidth;
                    }
                    frame.Width = RightPanelVisibleWidth;
                }
                RightPanel.View.Frame = frame;
            }
            if (LeftPanel != null && LeftPanel.IsViewLoaded)
            {
                CGRect frame = LeftPanelContainer.Bounds;
                if (ShouldResizeLeftPanel)
                {
                    frame.Width = LeftPanelVisibleWidth;
                }
                LeftPanel.View.Frame = frame;
            }
        }

        private void SwapCenter(UIViewController previous, SidePanelState previousState, UIViewController next)
        {
            if (previous != next)
            {
                if (previous != null)
                {
                    previous.WillMoveToParentViewController(null);
                    previous.View.RemoveFromSuperview();
                    previous.RemoveFromParentViewController();
                }
                if (next != null)
                {
                    LoadCenterPanelWithPreviousState(previousState);
                    AddChildViewController(next);
                    CenterPanelContainer.AddSubview(next.View);
                    next.DidMoveToParentViewController(this);
                }
            }
        }

        private void PlaceButtonForLeftPanel()
        {
            if (LeftPanel != null)
            {
                UIViewController buttonController = CenterPanel;
                UINavigationController nav = buttonController as UINavigationController;
                if (nav != null)
                {
                    if (nav.ViewControllers.Length > 0)
                    {
                        buttonController = nav.ViewControllers[0];
                    }
                }
                if (buttonController != null && buttonController.NavigationItem.LeftBarButtonItem == null)
                {
                    buttonController.NavigationItem.LeftBarButtonItem = GetLeftButtonForCenterPanel();
                }
            }
        }

        [Export("gestureRecognizerShouldBegin:")]
        public bool ShouldBegin(UIGestureRecognizer gestureRecognizer)
        {
            if (gestureRecognizer.View == TapView)
            {
                return true;
            }
            else if (PanningLimitedToTopViewController && !IsOnTopLevelViewController(CenterPanel))
            {
                return false;
            }

            UIPanGestureRecognizer pan = gestureRecognizer as UIPanGestureRecognizer;
            if (pan != null)
            {
                CGPoint translate = pan.TranslationInView(CenterPanelContainer);
                // determine if right swipe is allowed
                if (translate.X < 0 && !AllowRightSwipe)
                {
                    return false;
                }
                // determine if left swipe is allowed
                if (translate.X > 0 && !AllowLeftSwipe)
                {
                    return false;
                }
                bool possible = translate.X != 0 && ((Math.Abs(translate.Y) / Math.Abs(translate.X)) < 1.0f);
                if (possible && ((translate.X > 0 && LeftPanel != null) || (translate.X < 0 && RightPanel != null)))
                {
                    return true;
                }
            }
            return false;
        }

        private void AddPanGestureToView(UIView view)
        {
            UIPanGestureRecognizer panGesture = new UIPanGestureRecognizer(HandlePan);
#if __UNIFIED__
            panGesture.Delegate = this;
#else
            panGesture.WeakDelegate = this;
#endif
            panGesture.MaximumNumberOfTouches = 1;
            panGesture.MinimumNumberOfTouches = 1;
            view.AddGestureRecognizer(panGesture);
        }

        private void HandlePan(UIGestureRecognizer sender)
        {
            if (!RecognizesPanGesture)
            {
                return;
            }

            UIPanGestureRecognizer pan = sender as UIPanGestureRecognizer;
            if (pan != null)
            {
                if (pan.State == UIGestureRecognizerState.Began)
                {
                    _locationBeforePan = CenterPanelContainer.Frame.Location;
                    OnPanStarted();
                }

                CGPoint translate = pan.TranslationInView(CenterPanelContainer);
                CGRect frame = _centerPanelRestingFrame;
                nfloat offset = (nfloat)Math.Round(CorrectMovement(translate.X));
                frame.X += offset;

                if (Style == SidePanelStyle.MultipleActive)
                {
                    frame.Width = View.Bounds.Width - frame.X;
                }

                CenterPanelContainer.Frame = frame;

                // if center panel has focus, make sure correct side panel is revealed
                if (State == SidePanelState.CenterVisible)
                {
                    if (frame.X > 0.0f)
                    {
                        LoadLeftPanel();
                    }
                    else if (frame.X < 0.0f)
                    {
                        LoadRightPanel();
                    }
                }

                // adjust side panel locations, if needed
                if (Style == SidePanelStyle.MultipleActive || PushesSidePanels)
                {
                    LayoutSideContainers(false, 0);
                }

                OnPanning(new PanningEventArgs(offset, _locationBeforePan.X, frame.X));

                if (sender.State == UIGestureRecognizerState.Ended)
                {
                    nfloat deltaX = frame.X - _locationBeforePan.X;
                    if (ValidateThreshold(deltaX))
                    {
                        CompletePan(deltaX);
                    }
                    else
                    {
                        UndoPan();
                    }
                }
                else if (sender.State == UIGestureRecognizerState.Cancelled)
                {
                    UndoPan();
                }
            }
        }

        private void CompletePan(nfloat deltaX)
        {
            switch (State)
            {
                case SidePanelState.CenterVisible:
                    if (deltaX > 0.0f)
                    {
                        ShowLeftPanel(true, BounceOnSidePanelOpen);
                    }
                    else
                    {
                        ShowRightPanel(true, BounceOnSidePanelOpen);
                    }
                    break;
                case SidePanelState.LeftVisible:
                    ShowCenterPanel(true, BounceOnSidePanelClose);
                    break;
                case SidePanelState.RightVisible:
                    ShowCenterPanel(true, BounceOnSidePanelClose);
                    break;
            }
            OnPanCompleted(new PanCompletedEventArgs(false));
        }

        private void UndoPan()
        {
            switch (State)
            {
                case SidePanelState.CenterVisible:
                    ShowCenterPanel(true, false);
                    break;
                case SidePanelState.LeftVisible:
                    ShowLeftPanel(true, false);
                    break;
                case SidePanelState.RightVisible:
                    ShowRightPanel(true, false);
                    break;
            }
            OnPanCompleted(new PanCompletedEventArgs(true));
        }

        private void CenterPanelTapped(UIGestureRecognizer gestureRecognizer)
        {
            ShowCenterPanel(true, false);
        }

        private nfloat CorrectMovement(nfloat movement)
        {
            nfloat position = _centerPanelRestingFrame.X + movement;
            if (State == SidePanelState.CenterVisible)
            {
                if ((position > 0.0f && LeftPanel == null) || (position < 0.0f && RightPanel == null))
                {
                    return 0.0f;
                }
                else if (!AllowLeftOverPan && position > LeftPanelVisibleWidth)
                {
                    return LeftPanelVisibleWidth;
                }
                else if (!AllowRightOverPan && position < -RightPanelVisibleWidth)
                {
                    return -RightPanelVisibleWidth;
                }
            }
            else if (State == SidePanelState.RightVisible && !AllowRightOverPan)
            {
                if (position < -RightPanelVisibleWidth)
                {
                    return 0.0f;
                }
                else if ((Style == SidePanelStyle.MultipleActive || PushesSidePanels) && position > 0.0f)
                {
                    return -_centerPanelRestingFrame.X;
                }
                else if (position > RightPanelContainer.Frame.X)
                {
                    return RightPanelContainer.Frame.X - _centerPanelRestingFrame.X;
                }
            }
            else if (State == SidePanelState.LeftVisible && !AllowLeftOverPan)
            {
                if (position > LeftPanelVisibleWidth)
                {
                    return 0.0f;
                }
                else if ((Style == SidePanelStyle.MultipleActive || PushesSidePanels) && position < 0.0f)
                {
                    return -_centerPanelRestingFrame.X;
                }
                else if (position < LeftPanelContainer.Frame.X)
                {
                    return LeftPanelContainer.Frame.X - _centerPanelRestingFrame.X;
                }
            }
            return movement;
        }

        private bool ValidateThreshold(nfloat movement)
        {
            var minimum = Math.Floor(View.Bounds.Width * MinimumMovePercentage);
            switch (State)
            {
                case SidePanelState.LeftVisible:
                    return movement <= -minimum;
                case SidePanelState.CenterVisible:
                    return Math.Abs(movement) >= minimum;
                case SidePanelState.RightVisible:
                    return movement >= minimum;
            }
            return false;
        }

        private bool IsOnTopLevelViewController(UIViewController root)
        {
            UINavigationController nav = root as UINavigationController;
            if (nav != null)
            {
                return nav.ViewControllers.Length == 1;
            }
            UITabBarController tab = root as UITabBarController;
            if (tab != null)
            {
                return IsOnTopLevelViewController(tab.SelectedViewController);
            }
            return root != null;
        }

        private void LoadCenterPanelWithPreviousState(SidePanelState previousState)
        {
            PlaceButtonForLeftPanel();

            // for the multi-active style, it looks better if the new center starts out in it's fullsize and slides in
            if (Style == SidePanelStyle.MultipleActive)
            {
                CGRect frame;
                switch (previousState)
                {
                    case SidePanelState.LeftVisible:
                        frame = CenterPanelContainer.Frame;
                        frame.Width = View.Bounds.Width;
                        CenterPanelContainer.Frame = frame;
                        break;
                    case SidePanelState.RightVisible:
                        frame = CenterPanelContainer.Frame;
                        frame.Width = View.Bounds.Width;
                        frame.X = -RightPanelVisibleWidth;
                        CenterPanelContainer.Frame = frame;
                        break;
                    default:
                        break;
                }
            }

            _centerPanel.View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
            _centerPanel.View.Frame = CenterPanelContainer.Bounds;
            StylePanel(_centerPanel.View);
        }

        private void LoadLeftPanel()
        {
            RightPanelContainer.Hidden = true;
            if (LeftPanelContainer.Hidden && LeftPanel != null)
            {
                if (LeftPanel.View.Superview == null)
                {
                    LayoutSidePanels();
                    LeftPanel.View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
                    StylePanel(LeftPanel.View);
                    LeftPanelContainer.AddSubview(LeftPanel.View);
                }

                LeftPanelContainer.Hidden = false;
            }
        }

        private void LoadRightPanel()
        {
            LeftPanelContainer.Hidden = true;
            if (RightPanelContainer.Hidden && RightPanel != null)
            {
                if (RightPanel.View.Superview == null)
                {
                    LayoutSidePanels();
                    RightPanel.View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
                    StylePanel(RightPanel.View);
                    RightPanelContainer.AddSubview(RightPanel.View);
                }

                RightPanelContainer.Hidden = false;
            }
        }

        private void UnloadPanels()
        {
            if (CanUnloadLeftPanel && LeftPanel != null && LeftPanel.IsViewLoaded)
            {
                LeftPanel.View.RemoveFromSuperview();
            }
            if (CanUnloadRightPanel && RightPanel != null && RightPanel.IsViewLoaded)
            {
                RightPanel.View.RemoveFromSuperview();
            }
        }

        private nfloat CalculatedDuration()
        {
            nfloat remaining = (nfloat)Math.Abs(CenterPanelContainer.Frame.X - _centerPanelRestingFrame.X);
            nfloat max = _locationBeforePan.X == _centerPanelRestingFrame.X ? remaining : (nfloat)Math.Abs(_locationBeforePan.X - _centerPanelRestingFrame.X);
            return max > 0.0f ? MaximumAnimationDuration * (remaining / max) : MaximumAnimationDuration;
        }

        private void AnimateCenterPanel(bool shouldBounce, UICompletionHandler completion)
        {
            nfloat bounceDistance = (_centerPanelRestingFrame.X - CenterPanelContainer.Frame.X) * BouncePercentage;

            // looks bad if we bounce when the center panel grows
            if (_centerPanelRestingFrame.Width > CenterPanelContainer.Frame.Width)
            {
                shouldBounce = false;
            }

            nfloat duration = CalculatedDuration();
            UIView.AnimateNotify(duration, 0.0f, UIViewAnimationOptions.CurveLinear | UIViewAnimationOptions.LayoutSubviews, () =>
            {
                CenterPanelContainer.Frame = _centerPanelRestingFrame;
                StyleContainer(CenterPanelContainer, true, duration);
                if (Style == SidePanelStyle.MultipleActive || PushesSidePanels)
                {
                    LayoutSideContainers(false, 0.0f);
                }
            }, finished =>
            {
                if (shouldBounce)
                {
                    // make sure correct panel is displayed under the bounce
                    if (State == SidePanelState.CenterVisible)
                    {
                        if (bounceDistance > 0.0f)
                        {
                            LoadLeftPanel();
                        }
                        else
                        {
                            LoadRightPanel();
                        }
                    }
                    // animate the bounce
                    UIView.AnimateNotify(BounceDuration, 0.0f, UIViewAnimationOptions.CurveEaseOut, () =>
                    {
                        CGRect bounceFrame = _centerPanelRestingFrame;
                        bounceFrame.X += bounceDistance;
                        CenterPanelContainer.Frame = bounceFrame;
                    }, finished2 =>
                    {
                        UIView.AnimateNotify(BounceDuration, 0.0f, UIViewAnimationOptions.CurveEaseIn, () =>
                        {
                            CenterPanelContainer.Frame = _centerPanelRestingFrame;
                        }, completion);
                    });
                }
                else if (completion != null)
                {
                    completion(finished);
                }
            });
        }

        private CGRect AdjustCenterFrame()
        {
            CGRect frame = View.Bounds;
            switch (State)
            {
                case SidePanelState.CenterVisible:
                    frame.X = 0.0f;
                    if (Style == SidePanelStyle.MultipleActive)
                    {
                        frame.Width = View.Bounds.Width;
                    }
                    break;
                case SidePanelState.LeftVisible:
                    frame.X = LeftPanelVisibleWidth;
                    if (Style == SidePanelStyle.MultipleActive)
                    {
                        frame.Width = View.Bounds.Width - LeftPanelVisibleWidth;
                    }
                    break;
                case SidePanelState.RightVisible:
                    frame.X = -RightPanelVisibleWidth;
                    if (Style == SidePanelStyle.MultipleActive)
                    {
                        frame.X = 0.0f;
                        frame.Width = View.Bounds.Width - RightPanelVisibleWidth;
                    }
                    break;
            }
            _centerPanelRestingFrame = frame;
            return _centerPanelRestingFrame;
        }

        private void ShowLeftPanel(bool animated, bool shouldBounce)
        {
            State = SidePanelState.LeftVisible;
            LoadLeftPanel();

            AdjustCenterFrame();

            if (animated)
            {
                AnimateCenterPanel(shouldBounce, null);
            }
            else
            {
                CenterPanelContainer.Frame = _centerPanelRestingFrame;
                StyleContainer(CenterPanelContainer, false, 0.0f);
                if (Style == SidePanelStyle.MultipleActive || PushesSidePanels)
                {
                    LayoutSideContainers(false, 0.0f);
                }
            }

            if (Style == SidePanelStyle.SingleActive)
            {
                TapView = new UIView();
            }
            ToggleScrollsToTopForCenter(false, true, false);
        }

        private void ShowRightPanel(bool animated, bool shouldBounce)
        {
            State = SidePanelState.RightVisible;
            LoadRightPanel();

            AdjustCenterFrame();

            if (animated)
            {
                AnimateCenterPanel(shouldBounce, null);
            }
            else
            {
                CenterPanelContainer.Frame = _centerPanelRestingFrame;
                StyleContainer(CenterPanelContainer, false, 0.0f);
                if (Style == SidePanelStyle.MultipleActive || PushesSidePanels)
                {
                    LayoutSideContainers(false, 0.0f);
                }
            }

            if (Style == SidePanelStyle.SingleActive)
            {
                TapView = new UIView();
            }
            ToggleScrollsToTopForCenter(false, false, true);
        }

        private void ShowCenterPanel(bool animated, bool shouldBounce)
        {
            State = SidePanelState.CenterVisible;

            AdjustCenterFrame();

            if (animated)
            {
                AnimateCenterPanel(shouldBounce, finished =>
                {
                    LeftPanelContainer.Hidden = true;
                    RightPanelContainer.Hidden = true;
                    UnloadPanels();
                });
            }
            else
            {
                CenterPanelContainer.Frame = _centerPanelRestingFrame;
                StyleContainer(CenterPanelContainer, false, 0.0f);
                if (Style == SidePanelStyle.MultipleActive || PushesSidePanels)
                {
                    LayoutSideContainers(false, 0.0f);
                }
                LeftPanelContainer.Hidden = true;
                RightPanelContainer.Hidden = true;
                UnloadPanels();
            }

            TapView = null;
            ToggleScrollsToTopForCenter(true, false, false);
        }

        private void HideCenterPanel()
        {
            CenterPanelContainer.Hidden = true;
            if (CenterPanel != null && CenterPanel.IsViewLoaded)
            {
                CenterPanel.View.RemoveFromSuperview();
            }
        }

        private void UnhideCenterPanel()
        {
            CenterPanelContainer.Hidden = false;
            if (CenterPanel != null && CenterPanel.IsViewLoaded && CenterPanel.View.Superview == null)
            {
                CenterPanel.View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
                CenterPanel.View.Frame = CenterPanelContainer.Bounds;
                StylePanel(CenterPanel.View);
                CenterPanelContainer.AddSubview(CenterPanel.View);
            }
        }

        private void ToggleScrollsToTopForCenter(bool center, bool left, bool right)
        {
            // iPhone only supports 1 active UIScrollViewController at a time
            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
            {
                ToggleScrollsToTop(center, CenterPanelContainer);
                ToggleScrollsToTop(left, LeftPanelContainer);
                ToggleScrollsToTop(right, RightPanelContainer);
            }
        }

        private bool ToggleScrollsToTop(bool enabled, UIView view)
        {
            UIScrollView scrollView = view as UIScrollView;
            if (scrollView != null)
            {
                scrollView.ScrollsToTop = enabled;
                return true;
            }
            else
            {
                foreach (UIView subview in view.Subviews)
                {
                    if (ToggleScrollsToTop(enabled, subview))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override void ObserveValue(NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
        {
            if (context == ja_kvoContext.Handle)
            {
                if (keyPath == "view")
                {
                    if (CenterPanel != null && CenterPanel.IsViewLoaded && RecognizesPanGesture)
                    {
                        AddPanGestureToView(CenterPanel.View);
                    }
                }
                else if (keyPath == "viewControllers" && ofObject == CenterPanel)
                {
                    // view controllers have changed, need to replace the button
                    PlaceButtonForLeftPanel();
                }
            }
            else
            {
                base.ObserveValue(keyPath, ofObject, change, context);
            }
        }

        protected virtual void OnStateChanged(StateChangedEventArgs e)
        {
            var handler = StateChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnPanStarted()
        {
            var handler = PanStarted;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        protected virtual void OnPanning(PanningEventArgs e)
        {
            var handler = Panning;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnPanCompleted(PanCompletedEventArgs e)
        {
            var handler = PanCompleted;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
