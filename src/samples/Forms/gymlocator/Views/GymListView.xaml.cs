using System;
using System.Collections.Generic;
using gymlocator.ViewModels;
using Plugin.Geolocator;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace gymlocator.Views
{
    public partial class GymListView : ContentPage
    {
        private bool keyboardOpen;

        private GymViewModel viewModel { get; set; }
        private Distance defaultDistance = Distance.FromKilometers(1);

        public GymListView()
        {
            InitializeComponent();
            viewModel = new GymViewModel(this);
            NavigationPage.SetHasNavigationBar(this, false);

            BindingContext = viewModel;

            keyboardOpen = false;
            sliderOverlay.OnSizeChange += (sender, e) =>
            {
                if (e.Old > e.New)
                {
                    HideKeyboard();
                }
            };

            sliderOverlay.OnGymSelected += (sender, e) =>
            {
                var moveToPos = new Xamarin.Forms.Maps.Position(e.Location.Lat, e.Location.Lng);
                map.MoveToRegion(MapSpan.FromCenterAndRadius(moveToPos, defaultDistance));
                HideKeyboard();
                ovelayController.Minimize(sliderOverlay);
            };

            sliderOverlay.OnSearchFocus += (sender, e) =>
            {
                keyboardOpen = true;
                ovelayController.MaximizeOverlay(sliderOverlay);
            };

        }

        private void HideKeyboard()
        {
            if (keyboardOpen)
            {
                keyboardOpen = false;
                TinyPubSubLib.TinyPubSub.Publish("HideKeyboard");
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
            //viewModel.OnAppear();
            viewModel.Init(map);
        }
    }
}
