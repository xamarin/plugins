using System;
using System.ComponentModel;

#if __UNIFIED__
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;
#else
using MonoTouch.CoreAnimation;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

using CGRect = System.Drawing.RectangleF;
using CGSize = System.Drawing.SizeF;
using CGPoint = System.Drawing.PointF;
using nfloat = System.Single;
#endif

namespace AnimatedButtons
{
    public class LiquidFloatingCell : LiquittableCircle
    {
        private nfloat imageRatio = 0.5f;
        private WeakReference<LiquidFloatingActionButton> actionButton = new WeakReference<LiquidFloatingActionButton>(null);

        private UIImageView imageView;

        private UIColor originalColor = UIColor.Clear;
        private UIFont _titleFont;
        private UIColor _titleColor;

        public static UIColor DefaultTitleColor { get; set; } = UIColor.LabelColor;

        public static UIFont DefaultTitleFont { get; set; } = UIFont.SystemFontOfSize(UIFont.SystemFontSize);

        public UIFont TitleFont { get => _titleFont; }

        public UIColor TitleColor { get => _titleColor; }

        public UIView View { get; private set; }

        public UILabel Label { get; private set; }

        public LiquidFloatingActionButton ActionButton
        {
            get
            {
                LiquidFloatingActionButton result = null;
                actionButton.TryGetTarget(out result);
                return result;
            }
            internal set { actionButton.SetTarget(value); }
        }

        public bool Responsible { get; set; }        

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if (imageView != null)
            {
                var radius = Frame.Width * imageRatio;
                var offset = (Frame.Width - radius) / 2f;
                imageView.Frame = new CGRect(offset, offset, radius, radius);
            }
        }

        public override void WillMoveToSuperview(UIView newsuper)
        {
            base.WillMoveToSuperview(newsuper);

            Label.Font = _titleFont ?? DefaultTitleFont;
            Label.TextColor = _titleColor ?? DefaultTitleColor;

            if (Label != null)
            {
                var size = (Label.Text + " ").StringSize(Label.Font);
                var actionButton = ActionButton;
                if (actionButton != null)
                {
                    if (actionButton.TitlePosition == TitlePositions.Left)
                        Label.Frame = new CGRect(-size.Width, (Frame.Height - size.Height) / 2, size.Width, size.Height);
                    else
                        Label.Frame = new CGRect(Frame.Width + (" ").StringSize(Label.Font).Width, (Frame.Height - size.Height) / 2, size.Width, size.Height);
                }
            }
        }

        public LiquidFloatingCell(UIImage icon)
        {
            Setup(icon);
        }

        public LiquidFloatingCell(UIImage icon, string title)
        {
            Setup(icon);
            SetupLabel(title);
        }

        public LiquidFloatingCell(UIImage icon, nfloat imageRatio)
        {
            this.imageRatio = imageRatio;
            Setup(icon);
        }

        public LiquidFloatingCell(UIView view)
        {
            SetupView(view);
        }

        private void SetupLabel(string title)
        {
            Label = new UILabel() { Text = title, Alpha = 0 };
            AddSubview(Label);
        }

        private void Setup(UIImage image, UIColor tintColor = null)
        {
            imageView = new UIImageView();
            imageView.Image = image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
            imageView.TintColor = tintColor ?? UIColor.White;
            SetupView(imageView);
        }

        private void SetupView(UIView view)
        {
            View = view;
            Responsible = true;
            UserInteractionEnabled = false;
            AddSubview(view);
        }

        public void Update(nfloat key, bool open)
        {
            foreach (var view in Subviews)
            {
                var ratio = (nfloat)Math.Max(2f * (key * key - 0.5f), 0f);
                view.Alpha = open ? ratio : -ratio;
            }
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);
            if (Responsible)
            {
                originalColor = Color;
                Color = originalColor.White(0.5f);
                SetNeedsDisplay();
            }
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            base.TouchesCancelled(touches, evt);
            if (Responsible)
            {
                Color = originalColor;
                SetNeedsDisplay();
            }
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);

            Color = originalColor;
            var button = ActionButton;
            if (button != null)
            {
                button.OnCellSelected(this);
            }
        }

        public LiquidFloatingCell WithTitleFont(UIFont font)
        {
            _titleFont = font;
            return this;
        }

        public LiquidFloatingCell WithTitleColor(UIColor color)
        {
            _titleColor = color;
            
            return this;
        }
    }
}
