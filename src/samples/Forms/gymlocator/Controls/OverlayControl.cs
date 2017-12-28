using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public ViewOverlay(View view, OverlayType type)
        {
            OverlayView = view;
            SnapPercent = 20;
            //if (type != OverlayType.Dialog)
            //{
            //    view.Opacity = 0;
            //}

        }
        public View OverlayView { get; set; }
        public OverlayType Type { get; set; }
        public double MinSize { get; set; } = 55;
        public float SnapPercent { get; set; }
        public bool Active { get; set; }
        public Rectangle Bounds { get; set; }

        internal double offset { get; set; }
        internal bool active { get; set; }
        internal double lastOffset { get; set; }

    }

    public class OverlayControl : Layout<View>
    {

        public List<ViewOverlay> Overlays { get; internal set; } = new List<ViewOverlay>();

        protected override void OnAdded(View view)
        {
            base.OnAdded(view);
        }

        public void AddOverlay(ViewOverlay overlay)
        {
            Overlays.Add(overlay);
            Children.Add(overlay.OverlayView);
            var gest = new PanGestureRecognizer();
            gest.PanUpdated += (sender, e) =>
            {
                switch (e.StatusType)
                {
                    case GestureStatus.Started:
                        overlay.active = true;
                        break;
                    case GestureStatus.Running:
                        overlay.lastOffset = overlay.offset;
                        overlay.offset = e.TotalY;
                        break;
                    case GestureStatus.Completed:
                        overlay.active = false;
                        break;
                }
                UpdateLayout(overlay);
            };
            overlay.OverlayView.GestureRecognizers.Add(gest);
            ForceLayout();
        }

        private void UpdateLayout(ViewOverlay overlay)
        {
            var orgSize = overlay.Bounds.Height - overlay.Bounds.Y;
            var newSize = (orgSize - overlay.offset);

            if (overlay.active)
            {
                overlay.OverlayView.Layout(GetRect(overlay, newSize));
            }
            else
            {
                var delta = overlay.lastOffset - overlay.offset;
                var absDelta = Math.Abs(delta);
                if (absDelta > 40)
                {

                    newSize = (delta > 0) ? TotalHeight : overlay.MinSize;
                }
                else if (absDelta > 4)
                {
                    newSize += (delta * 4);
                }
                var rect = GetRect(overlay, newSize);
                overlay.OverlayView.LayoutTo(rect, 300, Easing.CubicOut);
                overlay.Bounds = rect;
                overlay.offset = 0;
            }
        }

        public void MaximizeOverlay(ViewOverlay ov)
        {
            if (!ov.active) {
                ov.Bounds = new Rectangle(0, 0, TotalWidth, TotalHeight);
                ov.OverlayView.LayoutTo(ov.Bounds,300,Easing.CubicInOut);
            }
        }

        private Rectangle GetRect(ViewOverlay overlay, double newSize)
        {
            if (newSize <= overlay.MinSize)
                newSize = overlay.MinSize;
            if (newSize >= TotalHeight)
                newSize = TotalHeight;
            return new Rectangle(overlay.Bounds.X, TotalHeight - newSize, overlay.Bounds.Width, TotalHeight);
        }

        private double TotalWidth;
        private double TotalHeight;

        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            TotalWidth = width;
            TotalHeight = height;
            var fullRect = new Rectangle(x, y, width, height);
            foreach (var child in Children)
            {
                if (!Overlays.Any(d => d.OverlayView == child))
                    child.Layout(fullRect);
            }
            foreach (var overlay in Overlays)
            {
                if (overlay.Bounds.IsEmpty)
                {
                    overlay.Bounds = GetInitialBounds(overlay, fullRect);
                }
                overlay.OverlayView.Layout(overlay.Bounds);
            }
        }

        private Rectangle GetInitialBounds(ViewOverlay overlay, Rectangle fullRect)
        {
            var size = (double)(fullRect.Height * (overlay.SnapPercent / 100f));
            var ret = new Rectangle(fullRect.X, fullRect.Height - size, fullRect.Width, fullRect.Height);
            return ret;
        }
    }
}
