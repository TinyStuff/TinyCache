using gymlocator.Controls;
using gymlocator.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(DrawerControl), typeof(DrawerControlRenderer))]

namespace gymlocator.iOS.Renderers
{
    public class DrawerControlRenderer : ViewRenderer<DrawerControl, UIControl>
    {
        private readonly UIVisualEffectView visualEffectView;
        private readonly UIBlurEffect blur;

        public DrawerControlRenderer()
        {
            blur = UIBlurEffect.FromStyle(UIBlurEffectStyle.Regular);
            visualEffectView = new UIVisualEffectView(blur);
            InsertSubview(visualEffectView, 0);
        }

        public override void Draw(CoreGraphics.CGRect rect)
        {
            visualEffectView.Frame = rect;
            base.Draw(rect);
        }

    }
}
