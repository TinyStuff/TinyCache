using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using TK.CustomMap.iOSUnified;
using UIKit;

namespace gymlocator.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            UINavigationBar.Appearance.Translucent = true;
            UINavigationBar.Appearance.BarStyle = UIBarStyle.Default;
            global::Xamarin.Forms.Forms.Init();
            global::Xamarin.FormsMaps.Init();
            TKCustomMapRenderer.InitMapRenderer();
            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }
}
