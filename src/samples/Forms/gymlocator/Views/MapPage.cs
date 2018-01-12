using System;
using gymlocator.Controls;
using gymlocator.ViewModels;
using Plugin.Geolocator;
using TinyPubSubLib;
using TK.CustomMap;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace gymlocator.Views
{
    public class MapPage : ContentPage
    {
        private GymViewModel viewModel;
        private TKCustomMap map;

        public MapPage()
        {
            Title = "Locator";
            viewModel = new GymViewModel(this);
            NavigationPage.SetHasNavigationBar(this, false);
            SetupContents();
            SetupLocation();
        }

        private Distance defaultDistance = Distance.FromKilometers(1);
        private bool keyboardOpen;

        private void SetupContents()
        {
            map = new TKCustomMap()
            {
                HasScrollEnabled = true,
                HasZoomEnabled = true,
                IsShowingUser = true
            };
            var overlayControl = new OverlayControl
            {
                Children = {
                    map
                }
            };
            Content = overlayControl;

            var slider = new DrawerControl()
            {
                BindingContext = viewModel
            };

            var sliderOverlay = new ViewOverlay()
            {
                MinSize = 65,
                MaxSize = 580,
                //UseShadow = true,
                InitialSize = 65
            };
            keyboardOpen = false;
            sliderOverlay.OnSizeChange += (sender, e) =>
            {
                if (e.Old > e.New)
                {
                    HideKeyboard();
                }
            };

            slider.OnGymSelected += (sender, e) =>
            {
                var moveToPos = new Position(e.Location.Lat, e.Location.Lng);
                map.MoveToRegion(MapSpan.FromCenterAndRadius(moveToPos, defaultDistance));
                HideKeyboard();
                overlayControl.Minimize(sliderOverlay);
            };

            slider.OnSearchFocus += (sender, e) =>
            {
                keyboardOpen = true;
                overlayControl.MaximizeOverlay(sliderOverlay);
            };

            overlayControl.AddOverlay(sliderOverlay);
        }

        private void HideKeyboard()
        {
            if (keyboardOpen)
            {
                keyboardOpen = false;
                TinyPubSub.Publish("HideKeyboard");
            }
        }

        private void SetupLocation()
        {
            var geo = CrossGeolocator.Current;
            geo.PositionChanged += (sender, e) =>
            {
                var pos = new Position(e.Position.Latitude, e.Position.Longitude);
                map.MoveToMapRegion(MapSpan.FromCenterAndRadius(pos, defaultDistance));
            };
            geo.StartListeningAsync(TimeSpan.FromMinutes(2), 150);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.Init(map);
        }
    }
}

