using System;

using Xamarin.Forms;

namespace gymlocator.Views
{
    public class GymLocatorMain : TabbedPage
    {
        private ContentPage AddView(ContentPage page) {
            this.Children.Add(page);
            return page;
        }

        public GymLocatorMain()
        {
            Title = "Gym finder";
            NavigationPage.SetHasNavigationBar(this, false);
            AddView(new GymListView()
            {
                Title = "List",
            });
            AddView(new MapPage()
            {
                Title = "Map",
            });
        }
    }
}

