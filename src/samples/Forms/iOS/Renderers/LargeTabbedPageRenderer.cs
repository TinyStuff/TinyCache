using System;
using gymlocator.Controls;
using gymlocator.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Page), typeof(LargeTabbedPageRenderer))]

namespace gymlocator.iOS.Renderers
{
    public class LargeTabbedPageRenderer : PageRenderer
    {
        public LargeTabbedPageRenderer() {
            TinyPubSubLib.TinyPubSub.Subscribe(this, "HideKeyboard",()=> {
                View.EndEditing(true);
            });
        }

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
