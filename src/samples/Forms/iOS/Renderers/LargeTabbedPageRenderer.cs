using System;
using gymlocator.Controls;
using gymlocator.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(TabbedPage), typeof(TabbedPageRenderer))]
[assembly: ExportRenderer(typeof(Page), typeof(LargeTabbedPageRenderer))]
[assembly: ExportRenderer(typeof(DrawerControl), typeof(DrawerControlRenderer))]
[assembly: ExportRenderer(typeof(Entry), typeof(CustomEntryRenderer))]

namespace gymlocator.iOS.Renderers
{
    public class TabbedPageRenderer : TabbedRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            //this.NavigationController.NavigationBar.PrefersLargeTitles = true;
            UITabBarItem.Appearance.TitlePositionAdjustment = new UIOffset(0, -2f);
        }
    }

    public class LargeTabbedPageRenderer : PageRenderer
    {
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0) && NavigationController != null && NavigationController.NavigationBar != null)
            {
                NavigationController.NavigationBar.PrefersLargeTitles = !NavigationController.NavigationBar.Hidden;
            }

        }

    }

    public class CustomEntryRenderer : EntryRenderer 
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            Control.BorderStyle = UITextBorderStyle.RoundedRect;
        }
    }

    public class DrawerControlRenderer : ViewRenderer<DrawerControl,UIControl> 
    {
        private readonly UIVisualEffectView visualEffectView;
        private readonly UIBlurEffect blur;

        public DrawerControlRenderer() {
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
