using System;
using System.Collections.Generic;
using gymlocator.ViewModels;
using Xamarin.Forms;

namespace gymlocator.Views
{
    public partial class GymListView : ContentPage
    {
        private GymViewModel viewModel { get; set; }

        public GymListView()
        {
            InitializeComponent();
            viewModel = new GymViewModel(this);
            BindingContext = viewModel;
        }

        void Handle_ItemTapped(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            viewModel.OpenGym.Execute(e.Item);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.OnAppear();
        }
    }
}
