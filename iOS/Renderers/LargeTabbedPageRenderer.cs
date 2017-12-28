using System;
using gymlocator.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(TabbedPage), typeof(TabbedPageRenderer))]
[assembly: ExportRenderer(typeof(Page), typeof(LargeTabbedPageRenderer))]

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
}
