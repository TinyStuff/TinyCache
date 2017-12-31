using System.Threading.Tasks;
using TinyTranslations.Forms;
using Xamarin.Forms;

namespace gymlocator
{
    public partial class App : Application
    {
        TranslationHelper Translator;

        public App()
        {
            InitializeComponent();


            TinyPubSubLib.TinyPubSubForms.Init(this);
            Translator = new TinyTranslations.Forms.TranslationHelper(new System.Uri("http://localhost:5000/"));
            TinyTranslations.Forms.TranslateExtension.Translator = Translator;
            Task.Run(() => Translator.ChangeCultureAsync("sv").RunSynchronously());
            MainPage = new NavigationPage(new Views.MapPage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
