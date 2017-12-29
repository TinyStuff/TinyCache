using System;
using gymlocator.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(BoxView), typeof(CustomBoxViewRenderer))]

namespace gymlocator.iOS.Renderers
{
    public class CustomBoxViewRenderer : BoxRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<BoxView> e)
        {
            base.OnElementChanged(e);
            Layer.MasksToBounds = true;
            Layer.CornerRadius = 3;
        }
    }
}
