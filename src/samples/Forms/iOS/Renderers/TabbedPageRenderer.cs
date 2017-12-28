using gymlocator.Controls;
using gymlocator.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(TabbedPage), typeof(TabbedPageRenderer))]

namespace gymlocator.iOS.Renderers
{
    public class TabbedPageRenderer : TabbedRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            UITabBarItem.Appearance.TitlePositionAdjustment = new UIOffset(0, -2f);
        }
    }
}
