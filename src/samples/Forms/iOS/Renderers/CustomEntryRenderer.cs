using gymlocator.Controls;
using gymlocator.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(SearchBox), typeof(SearchBoxEntryRenderer))]

namespace gymlocator.iOS.Renderers
{
    public class SearchBoxEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            Control.ReturnKeyType = UIReturnKeyType.Search;
            Control.ClearButtonMode = UITextFieldViewMode.Always;
            Control.BorderStyle = UITextBorderStyle.None;
            Layer.CornerRadius = 8;
            Layer.MasksToBounds = true;
            Layer.BorderColor = UIColor.Clear.CGColor;

        }
    }
}
