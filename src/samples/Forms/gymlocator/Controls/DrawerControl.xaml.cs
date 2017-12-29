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

        public static readonly BindableProperty BackgroundOpacityProperty =
            BindableProperty.Create("BackgroundOpacity", typeof(double), typeof(DrawerControl), 0.0, validateValue: IsValidOpacity);

        public double BackgroundOpacity
        {
            get
            {
                return (double)GetValue(BackgroundOpacityProperty);
            }
            set
            {
                SetValue(BackgroundOpacityProperty, value);
            }
        }

        static bool IsValidOpacity(BindableObject bindable, object value)
        {
            double result;
            bool isDouble = double.TryParse(value.ToString(), out result);
            return isDouble && (result >= 0 && result <= 1);
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
