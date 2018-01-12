using System;
using Xamarin.Forms;

namespace TinyControls
{
    public class DrawerControl : ViewOverlay
    {
        public static readonly BindableProperty BackgroundOpacityProperty =
            BindableProperty.Create("BackgroundOpacity", typeof(double), typeof(DrawerControl), 0.0, validateValue: IsValidOpacity);

        public double BackgroundOpacity
        {
            get
            {
                return (double)GetValue(BackgroundOpacityProperty);
            }
            set
            {
                SetValue(BackgroundOpacityProperty, value);
            }
        }

        static bool IsValidOpacity(BindableObject bindable, object value)
        {
            double result;
            bool isDouble = double.TryParse(value.ToString(), out result);
            return isDouble && (result >= 0 && result <= 1);
        }

    }
}
