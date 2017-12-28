using System.Collections.Generic;
using Xamarin.Forms;

namespace gymlocator.Controls
{

    public enum OverlayType 
    {
        Top,
        Bottom,
        Left,
        Right,
        Dialog
    }

    public class ViewOverlay
    {
        public View OverlayView { get; set; }
        public OverlayType Type { get; set; }
        public int SnapPercent { get; set; }
    }

    public class OverlayControl : Layout<View>
    {

        public List<ViewOverlay> Overlays { get; internal set; }

        protected override void OnAdded(View view)
        {
            base.OnAdded(view);
        }

        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            
        }
    }
}
