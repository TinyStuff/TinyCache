using System;
using System.Collections.Generic;
using gymlocator.ViewModels;
using Plugin.Geolocator;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace gymlocator.Views
{
    public partial class GymMapView : ContentPage
    {
        private GymViewModel viewModel { get; set; }
        private bool hasLoaded = false;

        public GymMapView()
        {
            InitializeComponent();
            viewModel = new GymViewModel(this);
            BindingContext = viewModel;
        }

        void Geo_PositionChanged(object sender, Plugin.Geolocator.Abstractions.PositionEventArgs e)
        {
            MyMap.MoveToMapRegion(MapSpan.FromCenterAndRadius(new Position(e.Position.Latitude,e.Position.Longitude), Distance.FromKilometers(1)));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (!hasLoaded)
            {
                hasLoaded = true;
                MyMap.HeightRequest = Bounds.Height;
                viewModel.Init(MyMap);
                var geo = CrossGeolocator.Current;
                geo.PositionChanged += Geo_PositionChanged;
                geo.StartListeningAsync(TimeSpan.FromMinutes(2), 15);
            }
        }

    }
}
