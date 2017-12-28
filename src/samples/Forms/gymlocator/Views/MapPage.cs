using System;
using gymlocator.Controls;
using gymlocator.ViewModels;
using Plugin.Geolocator;
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
            viewModel = new GymViewModel(this);
            NavigationPage.SetHasNavigationBar(this, false);
            SetupContents();
            SetupLocation();
        }

        private void SetupContents()
        {
            map = new TKCustomMap()
            {
                HasScrollEnabled = true,
                HasZoomEnabled = true,
                IsShowingUser = true
            };
            var oc = new OverlayControl
            {
                Children = {
                    map
                }
            };
            Content = oc;
            var slider = new DrawerControl();
            var sliderOverlay = new ViewOverlay(slider, OverlayType.Bottom) {
                MinSize = 65,
                InitialSize = 65
            };
            slider.OnGymSelected += (sender, e) =>
            {
                map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(e.Location.Lat, e.Location.Lng), Distance.FromKilometers(1)));
                oc.Minimize(sliderOverlay);
                map.Focus();
            };
            slider.OnSearchFocus += (sender, e) => {
                oc.MaximizeOverlay(sliderOverlay);
            };

            slider.BindingContext = viewModel;
            oc.AddOverlay(sliderOverlay);
        }

        private void SetupLocation()
        {
            var geo = CrossGeolocator.Current;
            geo.PositionChanged += PositionChanged;
            geo.StartListeningAsync(TimeSpan.FromMinutes(2), 15);
        }

        void PositionChanged(object sender, Plugin.Geolocator.Abstractions.PositionEventArgs e)
        {
            map.MoveToMapRegion(MapSpan.FromCenterAndRadius(new Position(e.Position.Latitude, e.Position.Longitude), Distance.FromKilometers(1)));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.Init(map);
        }
    }
}

