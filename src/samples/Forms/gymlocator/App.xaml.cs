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
            Translator = new TranslationHelper(new System.Uri("http://localhost:5000/"));
            ansExtension.Translator = Translator;
            //var oldMethod = Translator.FetchLanguageMethod;
            //Translator.FetchLanguageMethod = async (locale) => await TinyCache.TinyCache.UsePolicy<TranslationDictionary>("trans", () =>
            //{
            //    return oldMethod(locale);
            //});

            Translator.Init("sv");
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
