using System;
using System.ComponentModel;
using System.Linq;
using System.Collections.Generic;

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
    [DesignTimeVisible(true), Category("Controls")]
    [Register("LiquidFloatingActionButton")]
    public class LiquidFloatingActionButton : UIControl
    {
        private nfloat internalRadiusRatio = 20.0f / 56.0f;

        private bool _enableShadow = true;
        private UIColor color = UIColor.FromRGB(82, 112, 235);
        private UIImage image;

        private CAShapeLayer plusLayer = new CAShapeLayer();
        private CAShapeLayer circleLayer = new CAShapeLayer();

        private bool touching = false;
        private nfloat plusRotation = 0.0f;

        private CircleLiquidBaseView baseView = new CircleLiquidBaseView();
        private UIView liquidView = new UIView();

        public TitlePositions TitlePosition { get; set; }

        public LiquidFloatingActionButton()
        {
            Setup();
        }

        public LiquidFloatingActionButton(IntPtr handle)
            : base(handle)
        {
        }

        public LiquidFloatingActionButton(NSCoder coder)
            : base(coder)
        {
            Setup();
        }

        public LiquidFloatingActionButton(CGRect frame)
            : base(frame)
        {
            Setup();
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            Setup();
        }

        [Browsable(true)]
        [Export("CellRadiusRatio")]
        public nfloat CellRadiusRatio { get; set; }

        [Browsable(true)]
        [Export("AnimateStyle")]
        public AnimateStyle AnimateStyle
        {
            get { return baseView.AnimateStyle; }
            set { baseView.AnimateStyle = value; }
        }

        [Browsable(true)]
        [Export("EnableShadow")]
        public bool EnableShadow
        {
            get { return _enableShadow; }
            set
            {
                _enableShadow = value;
                SetNeedsDisplay();
            }
        }

        public event EventHandler<CellSelectedEventArgs> CellSelected;

        public IEnumerable<LiquidFloatingCell> Cells { get; set; }

        [Browsable(true)]
        [Export("Responsible")]
        public bool Responsible { get; set; }

        public bool IsOpening
        {
            get { return baseView.IsOpeningOrClosing; }
        }

        public bool IsClosed { get; private set; } = true;

        [Browsable(true)]
        [Export("Color")]
        public UIColor Color
        {
            get { return color; }
            set
            {
                color = value;
                baseView.Color = color;
            }
        }

        [Browsable(true)]
        [Export("Image")]
        public UIImage Image
        {
            get { return image; }
            set
            {
                image = value;
                plusLayer.Contents = image?.CGImage;
                plusLayer.Path = null;
            }
        }

        [Browsable(true)]
        [Export("RotationDegrees")]
        public nfloat RotationDegrees { get; set; } = 45.0f;

        public event EventHandler Opening;

        public event EventHandler Opened;

        public event EventHandler Closing;

        public event EventHandler Closed;

        private void InsertCell(LiquidFloatingCell cell)
        {
            var radius = Frame.Width * CellRadiusRatio;
            var offset = (Frame.Width - radius) / 2f;
            cell.Frame = new CGRect(offset, offset, radius, radius);
            cell.Color = Color;
            cell.ActionButton = this;
            InsertSubviewAbove(cell, baseView);
        }

        private LiquidFloatingCell[] CellArray()
        {
            if (Cells != null)
            {
                return Cells.ToArray();
            }
            return new LiquidFloatingCell[0];
        }

        /// <summary>
        /// Opens all cells.
        /// </summary>
        public void Open()
        {
            Opening?.Invoke(this, EventArgs.Empty);

            // rotate plus icon
            CATransaction.AnimationDuration = 0.8;
            plusLayer.Transform = CATransform3D.MakeRotation((CoreGraphicsExtensions.PI * RotationDegrees) / 180, 0, 0, 1);

            var cells = CellArray();
            foreach (var cell in cells)
            {
                InsertCell(cell);
            }

            baseView.Open(cells);

            IsClosed = false;

            Opened?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Closes all cells.
        /// </summary>
        public void Close()
        {
            Closing?.Invoke(this, EventArgs.Empty);

            // rotate plus icon
            CATransaction.AnimationDuration = 0.8;
            plusLayer.Transform = CATransform3D.MakeRotation(0, 0, 0, 1);

            baseView.Close(CellArray());

            IsClosed = true;

            Closed?.Invoke(this, EventArgs.Empty);
        }

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

            DrawCircle();
            DrawShadow();
        }

        public virtual CAShapeLayer CreatePlusLayer(CGRect frame)
        {
            // draw plus shape
            var layer = new CAShapeLayer();
            layer.LineCap = CAShapeLayer.CapRound;
            layer.StrokeColor = UIColor.White.CGColor;
            layer.LineWidth = 3.0f;

            var path = new UIBezierPath();
            path.MoveTo(new CGPoint(frame.Width * internalRadiusRatio, frame.Height * 0.5f));
            path.AddLineTo(new CGPoint(frame.Width * (1 - internalRadiusRatio), frame.Height * 0.5f));
            path.MoveTo(new CGPoint(frame.Width * 0.5f, frame.Height * internalRadiusRatio));
            path.AddLineTo(new CGPoint(frame.Width * 0.5f, frame.Height * (1 - internalRadiusRatio)));

            layer.Path = path.CGPath;
            return layer;
        }

        private void DrawCircle()
        {
            circleLayer.CornerRadius = Frame.Width * 0.5f;
            circleLayer.MasksToBounds = true;
            if (touching && Responsible)
            {
                circleLayer.BackgroundColor = Color.White(0.5f).CGColor;
            }
            else
            {
                circleLayer.BackgroundColor = Color.CGColor;
            }
        }

        private void DrawShadow()
        {
            if (EnableShadow)
            {
                circleLayer.AppendShadow();
            }
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);

            touching = true;
            SetNeedsDisplay();
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);
            touching = false;
            SetNeedsDisplay();
            DidTapped();
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            base.TouchesCancelled(touches, evt);
            touching = false;
            SetNeedsDisplay();
        }

        public override UIView HitTest(CGPoint point, UIEvent uievent)
        {
            foreach (var cell in CellArray())
            {
                var pointForTargetView = cell.ConvertPointFromView(point, this);
                if (cell.Bounds.Contains(pointForTargetView))
                {
                    if (cell.UserInteractionEnabled)
                    {
                        return cell.HitTest(pointForTargetView, uievent);
                    }
                }
            }
            return base.HitTest(point, uievent);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            var frame = new CGRect(CGPoint.Empty, Frame.Size);
            liquidView.Frame = frame;
            baseView.Frame = frame;
        }

        private void Setup()
        {
            CellRadiusRatio = 0.75f;
            Cells = new List<LiquidFloatingCell>();
            BackgroundColor = UIColor.Clear;
            ClipsToBounds = false;
            Responsible = true;

            baseView.Setup(this);
            AddSubview(baseView);

            liquidView.Frame = baseView.Frame;
            liquidView.UserInteractionEnabled = false;
            AddSubview(liquidView);

            liquidView.Layer.AddSublayer(circleLayer);
            circleLayer.Frame = liquidView.Layer.Bounds;

            plusLayer = CreatePlusLayer(circleLayer.Bounds);
            circleLayer.AddSublayer(plusLayer);
            plusLayer.Frame = circleLayer.Bounds;
        }

        private void DidTapped()
        {
            if (IsClosed)
            {
                Open();
            }
            else
            {
                Close();
            }
        }

        protected internal virtual void OnCellSelected(LiquidFloatingCell target)
        {
            var handler = CellSelected;
            if (handler != null)
            {
                var cells = CellArray();
                var index = Array.IndexOf(cells, target);
                handler(this, new CellSelectedEventArgs(target, index));
            }
        }
    }
}
