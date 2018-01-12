using System;
using gymlocator.Rest.Models;
using gymlocator.ViewModels;
using TinyControls;
using Xamarin.Forms;

namespace gymlocator.Controls
{
    public partial class CustomDrawerControl : DrawerControl
    {
        public CustomDrawerControl()
        {
            InitializeComponent();
        }


        public EventHandler<Gym> OnGymSelected;
        public EventHandler<FocusEventArgs> OnSearchFocus;

        void Handle_TextChanged(object sender, TextChangedEventArgs e)
        {
            var vm = BindingContext as GymViewModel;
            vm.FilterGyms(e.NewTextValue);
        }

        void Handle_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                OnGymSelected?.Invoke(this, e.SelectedItem as Gym);
                gymlist.SelectedItem = null;
            }
        }

        void Handle_Focused(object sender, FocusEventArgs e)
        {
            OnSearchFocus?.Invoke(sender, e);
        }
    }
}
