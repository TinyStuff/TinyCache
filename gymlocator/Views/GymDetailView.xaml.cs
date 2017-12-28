using System;
using System.Collections.Generic;
using gymlocator.Rest.Models;
using gymlocator.ViewModels;
using Xamarin.Forms;

namespace gymlocator.Views
{
    public partial class GymDetailView : ContentPage
    {
        private GymDetailViewModel viewModel;

        public GymDetailView(Gym gym)
        {
            InitializeComponent();
            viewModel = new GymDetailViewModel(this, gym);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.OnAppear();
        }
    }
}
