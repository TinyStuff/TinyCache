using System.Threading.Tasks;
using TinyTranslations;
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
            Translator = new TranslationHelper(new System.Uri("http://tinytranslation.azurewebsites.net"));
            var tempLocale = "es";
            ansExtension.Translator = Translator;
            var oldMethod = Translator.FetchLanguageMethod;
            Translator.FetchLanguageMethod = async (locale) => await TinyCache.TinyCache.RunAsync<TranslationDictionary>("trans-"+tempLocale, () =>
            {
                return oldMethod(locale);
            });
            Translator.Init(tempLocale);
            MainPage = new NavigationPage(new Views.GymListView());
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
