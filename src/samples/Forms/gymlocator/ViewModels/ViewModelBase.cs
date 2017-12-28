using System.Collections.Generic;
using Xamarin.Forms;

namespace gymlocator.ViewModels
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class ViewModelBase
    {
        public ViewModelBase(ContentPage page)
        {
            page.BindingContext = this;
            Navigation = page.Navigation;
        }

        private Dictionary<string, string> translations = new Dictionary<string, string>();
        private bool hasAppeared;

        public string this[string translationKey]
        {
            get
            {
                if (translations.ContainsKey(translationKey))
                    return translations[translationKey];
                else
                {
                    translations.Add(translationKey,translationKey);
                }
                return translationKey;
            }
        }

        public INavigation Navigation { get; internal set; }

        public string Title { get; set; } = "No name";

        public bool IsBusy { get; set; }

        public virtual void OnAppear()
        {
            if (!hasAppeared)
            {
                hasAppeared = true;
                OnFirstAppear();
            }
        }

        public virtual void OnFirstAppear()
        {

        }
    }
}

