using CoreAnimation;
using CoreGraphics;
using TinyControls;
using TinyControls.Drawer.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(DrawerControl), typeof(DrawerControlRenderer))]

namespace TinyControls.Drawer.iOS.Renderers
{
    public class DrawerControlRenderer : ViewRenderer<DrawerControl, UIControl>
    {
        private readonly UIVisualEffectView visualEffectView;
        private readonly UIBlurEffect blur;

        public DrawerControlRenderer()
        {
            blur = UIBlurEffect.FromStyle(UIBlurEffectStyle.Regular);
            visualEffectView = new UIVisualEffectView(blur);
            visualEffectView.TintColor = UIColor.White;
            visualEffectView.Layer.MasksToBounds = true;
            visualEffectView.Layer.CornerRadius = 10;
            InsertSubview(visualEffectView, 0);
        }

        private CALayer CreateShadowLayer(CGRect rect)
        {
            var shadowLayer = new CALayer();
            shadowLayer.BackgroundColor = UIColor.Black.CGColor;
            shadowLayer.ShadowOpacity = 1f;
            shadowLayer.ShadowRadius = 7;
            shadowLayer.ShadowOffset = new CGSize(0, 0);
            shadowLayer.ShadowColor = UIColor.Black.CGColor;
            shadowLayer.ShadowPath = UIBezierPath.FromRect(rect).CGPath;
            shadowLayer.MasksToBounds = false;
            return shadowLayer;
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName=="BackgroundOpacity") 
            {
                UpdateBackgroundColor();
            }
        }

        private void UpdateBackgroundColor()
        {
            if (Element is DrawerControl dc) {
                var clr = dc.BackgroundColor.ToUIColor().ColorWithAlpha((float)dc.BackgroundOpacity);
                //var clr = UIColor.FromWhiteAlpha(1, (float)dc.BackgroundOpacity);
                Layer.BackgroundColor = clr.CGColor;
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<DrawerControl> e)
        {
            base.OnElementChanged(e);
            Layer.MasksToBounds = true;
            Layer.CornerRadius = 10;
            //UpdateBackgroundColor();
        }

        public override void Draw(CoreGraphics.CGRect rect)
        {
            
            visualEffectView.Frame = rect;
            Layer.InsertSublayer(CreateShadowLayer(new CGRect(rect.Width + 5, 6, 7, rect.Height + 5)), 0);
            Layer.InsertSublayer(CreateShadowLayer(new CGRect(6, rect.Height + 5, rect.Width, 7.0f)), 0);
            base.Draw(rect);
        }

    }
}
