using System;
using System.Collections.Generic;
using System.Windows.Input;
using gymlocator.Rest.Models;
using Xamarin.Forms;

namespace gymlocator.ViewModels
{
    public class GymDetailViewModel : ViewModelBase
    {
        private Gym currentGym;

        public string Name { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public string Address { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }

        public IList<Feature> Features { get; set; }
        public IList<PersonalTrainer> PersonalTrainers { get; set; }

        public ICommand OpenFacebook => new Command(() =>
        {
            Device.OpenUri(new Uri(currentGym.FacebookPage));
        });

        public GymDetailViewModel(ContentPage page, Gym selectedGym) : base(page)
        {
            if (selectedGym != null)
            {
                currentGym = selectedGym;
            }
        }

        public override void OnFirstAppear()
        {
            base.OnFirstAppear();
            if (currentGym != null)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Name = currentGym.Name;
                    Title = currentGym.Name;
                    Description = currentGym.Description;
                    Address = currentGym.Address.StreetAddress;
                    Zip = currentGym.Address.PostalCode;
                    City = currentGym.Address.City;
                    PersonalTrainers = currentGym.PersonalTrainers;
                    Features = currentGym.Features;
                    Email = currentGym.Contact.Email;
                    Phone = currentGym.Contact.Phone;
                });
            }
        }
    }
}

