using System;
using Xamarin.Forms;

namespace gymlocator.Controls
{
    public class RoundedBoxView : BoxView
    {
        public static readonly BindableProperty CornerRadiusProperty =
            BindableProperty.Create("CornerRadius", typeof(float), typeof(DrawerControl), 0.0f, validateValue: IsValidRadius);

        public float CornerRadius
        {
            get
            {
                return (float)GetValue(CornerRadiusProperty);
            }
            set
            {
                SetValue(CornerRadiusProperty, value);
            }
        }

        static bool IsValidRadius(BindableObject bindable, object value)
        {
            float result;
            bool isFloat = float.TryParse(value.ToString(), out result);
            return isFloat && (result >= 0 && result <= 1000);
        }
    }
}
