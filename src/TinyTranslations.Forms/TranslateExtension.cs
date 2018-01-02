using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TinyTranslations.Forms
{
    [ContentProperty("Text")]
    public class ansExtension : IMarkupExtension
    {
        public ansExtension()
        {
        }

        public static TranslationHelper Translator {get;set;}

        public string Text { get; set; }

        public string Key { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Text == null)
                return "";

            if (Translator == null)
                return Text;

            if (string.IsNullOrEmpty(Key))
                return Translator.Translations[Text];
            else
                return Translator.Translate(Key, Text);

        }
    }

}
