using gymlocator.Controls;
using gymlocator.iOS.Renderers;
using TinyPubSubLib;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(SearchBox), typeof(SearchBoxEntryRenderer))]

namespace gymlocator.iOS.Renderers
{
    public class SearchBoxEntryRenderer : SearchBarRenderer
    {
        //protected override void OnElementChanged(ElementChangedEventArgs<SearchBar> e)
        //{
        //    base.OnElementChanged(e);
        //    Control.ReturnKeyType = UIReturnKeyType.Search;
        //    Control.ClearButtonMode = UITextFieldViewMode.Always;
        //    Control.BorderStyle = UITextBorderStyle.None;
        //    Control.TextAlignment = UITextAlignment.Center;
        //    Layer.CornerRadius = 8;
        //    Layer.MasksToBounds = true;
        //    Layer.BorderColor = UIColor.Clear.CGColor;

        //}
    }
}
