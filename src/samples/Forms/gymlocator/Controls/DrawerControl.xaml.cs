using System;
using System.Collections.Generic;
using gymlocator.Rest.Models;
using gymlocator.ViewModels;
using Xamarin.Forms;

namespace gymlocator.Controls
{
    public partial class DrawerControl : ContentView
    {
        public DrawerControl()
        {
            InitializeComponent();
        }

        public EventHandler<Gym> OnGymSelected;
        public EventHandler<FocusEventArgs> OnSearchFocus;

        void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            var vm = BindingContext as GymViewModel;
            vm.FilterGyms(e.NewTextValue);
        }

        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                OnGymSelected?.Invoke(this, e.SelectedItem as Gym);
                gymlist.SelectedItem = null;
            }
        }

        void Handle_Focused(object sender, Xamarin.Forms.FocusEventArgs e)
        {
            OnSearchFocus?.Invoke(sender, e);
        }
    }
}
