using System;

using Xamarin.Forms;

namespace gymlocator.Views
{
    public class MapPage : ContentPage
    {
        public MapPage()
        {
            Content = new OverlayControl
            {
                Children = {
                    new Label { Text = "Hello ContentPage" }
                }
            };
        }
    }
}

