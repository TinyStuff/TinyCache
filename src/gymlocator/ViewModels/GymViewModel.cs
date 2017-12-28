using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using gymlocator.Core;
using gymlocator.Rest.Models;
using gymlocator.Views;
using TK.CustomMap;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace gymlocator.ViewModels
{

    public class GymViewModel : ViewModelBase
    {
        private DataStore dataModel;
        private TKCustomMap map;

        public GymViewModel(ContentPage page) : base(page)
        {
            dataModel = new DataStore();

        }

        public ICommand OpenGym => new Command((arg) =>
        {
            Gym gym = null;
            if (arg is TKCustomMapPin pin)
            {
                gym = Gyms.FirstOrDefault(d => d.Id == pin.ID);
            }
            else if (arg is Gym g)
            {
                gym = g;
            }
            Navigation.PushAsync(new GymDetailView(gym));
        });

        public ICommand DoRefresh => new Command(async () =>
        {
            var gyms = await dataModel.GetGymsAsync();
            PopulateGyms(gyms);
        });

        public ObservableCollection<Gym> Gyms { get; set; } = new ObservableCollection<Gym>();
        public ObservableCollection<TKCustomMapPin> Pins { get; set; } = new ObservableCollection<TKCustomMapPin>();

        public async void Init(TKCustomMap map = null)
        {
            this.map = map;
            var gyms = await dataModel.GetGymsAsync();

            if (map != null)
            {
                map.CustomPins = Pins;
                map.CalloutClicked += (sender, e) => OpenGym.Execute(e.Value);
            }
            if (gyms != null && gyms.Any())
            {
                PopulateGyms(gyms);
            }
        }

        private void PopulateGyms(IList<Gym> gyms)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                foreach (var gym in gyms)
                {
                    if (!Gyms.Any(d => d.Id == gym.Id))
                    {
                        Gyms.Add(gym);
                    }
                    if (map != null && gym.Location != null)
                    {
                        var pos = new Position(gym.Location.Lat, gym.Location.Lng);

                        if (!Pins.Any(d => d.ID == gym.Id))
                        {
                            //map.MoveToRegion(MapSpan.FromCenterAndRadius(pos, Distance.FromKilometers(100)));
                            Pins.Add(new TKCustomMapPin()
                            {
                                Position = pos,
                                ID = gym.Id,
                                IsCalloutClickable = true,
                                Title = gym.Name,
                                ShowCallout = true,
                                Subtitle = gym.Address.StreetAddress
                                //Type = PinType.Place,
                                //Label = gym.Name,
                                //Address = gym.Address.StreetAddress
                            });
                        }
                    }
                }
            });
        }
    }
}