using System;
using gymlocator.Controls;
using TK.CustomMap;
using Xamarin.Forms;

namespace gymlocator.Views
{
    public class MapPage : ContentPage
    {
        public MapPage()
        {
            var map = new TKCustomMap();
            var oc = new OverlayControl
            {
                Children = {
                    map
                }
            };
            Content = oc;
            var slider = new DrawerControl();
            oc.AddOverlay(new ViewOverlay(slider, OverlayType.Bottom));
        }
    }
}

