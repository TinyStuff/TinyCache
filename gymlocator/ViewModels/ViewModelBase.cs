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

        private bool hasAppeared;

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

