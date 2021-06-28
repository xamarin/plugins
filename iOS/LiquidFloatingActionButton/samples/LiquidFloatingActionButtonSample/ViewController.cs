using System;
using System.Collections.Generic;
using CoreGraphics;
using UIKit;

using AnimatedButtons;

namespace LiquidFloatingActionButtonSample
{
    partial class ViewController : UIViewController
    {
        public ViewController()
        {
        }

        public ViewController(IntPtr handle)
            : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var font = UIFont.SystemFontOfSize(12);

            LiquidFloatingCell.DefaultTitleColor = UIColor.White;

            var cells = new List<LiquidFloatingCell>
            {
                new LiquidFloatingCell(UIImage.FromBundle("ic_cloud"), "cloud")
                    .WithTitleFont(font)
                    .WithTitleColor(UIColor.Green),
                new LiquidFloatingCell(UIImage.FromBundle("ic_system"), "system"),
                new LiquidFloatingCell(UIImage.FromBundle("ic_place"), "place")
            };

            topLeftButton.Image = UIImage.FromBundle("ic_art");
            topLeftButton.AnimateStyle = AnimateStyle.Down;
            topLeftButton.Cells = cells;
            topLeftButton.CellSelected += delegate
            {
                topLeftButton.Close();
            };
            topLeftButton.TitlePosition = TitlePositions.Right;
            
            bottomRightButton.Cells = cells;
            bottomRightButton.CellSelected += delegate
            {
                bottomRightButton.Close();
            };
            bottomRightButton.TitlePosition = TitlePositions.Left;
        }
    }
}
