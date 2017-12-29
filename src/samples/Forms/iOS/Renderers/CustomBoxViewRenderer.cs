using System;
using gymlocator.Controls;
using gymlocator.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(RoundedBoxView), typeof(CustomBoxViewRenderer))]

namespace gymlocator.iOS.Renderers
{
    public class CustomBoxViewRenderer : BoxRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<BoxView> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement is RoundedBoxView rbc)
            {
                SetRadius(rbc);
            }
        }

        private void SetRadius(RoundedBoxView rbc)
        {
            Layer.MasksToBounds = (rbc.CornerRadius > 0);
            Layer.CornerRadius = rbc.CornerRadius;
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (Element is RoundedBoxView rbc && e.PropertyName=="CornerRadius") {
                SetRadius(rbc);
            }
        }
    }
}
