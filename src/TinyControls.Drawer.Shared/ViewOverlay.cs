using System;
using TinyControls.Drawer;
using Xamarin.Forms;

namespace TinyControls
{
    public class ViewOverlay : ContentView
    {

        public EventHandler<ViewSizeArgs> OnSizeChange;
        public View ShadowView { get; set; }
        public OverlayType Type { get; set; } = OverlayType.Bottom;
        public double MinSize { get; set; } = 55;
        public double MaxSize { get; set; } = 99999;
        public double InitialSize { get; set; }
        internal bool UseShadow { get; set; }
        public bool Active { get; set; }
        internal Rectangle OverlayBounds { get; set; }
        public Easing Easing { get; set; } = Easing.SpringOut;
        public bool IsHorizontal
        {
            get
            {
                return Type == OverlayType.Left || Type == OverlayType.Right;
            }
        }

        internal double offset { get; set; }
        internal bool active { get; set; }
        internal double lastOffset { get; set; }

    }
}
