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
            Type = type;
        }

        public View OverlayView { get; set; }
        public OverlayType Type { get; set; }
        public double MinSize { get; set; } = 55;
        public double MaxSize { get; set; } = 99999;
        public double InitialSize { get; set; }
        //public float SnapPercent { get; set; } = 20;
        public bool Active { get; set; }
        public Rectangle Bounds { get; set; }

        internal double offset { get; set; }
        internal bool active { get; set; }
        internal double lastOffset { get; set; }
        public bool IsHorizontal
        {
            get
            {
                return Type == OverlayType.Left || Type == OverlayType.Right;
            }
        }

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
                        overlay.offset = overlay.IsHorizontal ? e.TotalX : e.TotalY;
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
            var maxValue = TotalHeight;
            switch (overlay.Type)
            {
                case OverlayType.Top:
                    orgSize = -overlay.Bounds.Y;
                    break;
                case OverlayType.Left:
                    orgSize = -overlay.Bounds.X;
                    maxValue = TotalWidth;
                    break;
                case OverlayType.Right:
                    orgSize = overlay.Bounds.Width - overlay.Bounds.X;
                    maxValue = TotalWidth;
                    break;
            }

            maxValue = Math.Min(maxValue, overlay.MaxSize);
            var newSize = (orgSize - overlay.offset);

            if (overlay.active)
            {
                overlay.OverlayView.Layout(GetRect(overlay, newSize));
            }
            else
            {
                var delta = overlay.lastOffset - overlay.offset;
                var absDelta = Math.Abs(delta);
                if (absDelta > 20)
                {
                    newSize = (delta > 0) ? maxValue : overlay.MinSize;
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

        private void SetOverlaySize(ViewOverlay ov, double ns)
        {
            if (!ov.active)
            {
                ov.Bounds = GetRect(ov, ns);
                ov.OverlayView.LayoutTo(ov.Bounds, 300, Easing.CubicInOut);
            }
        }

        public void Rezise(ViewOverlay ov, float percent)
        {
            SetOverlaySize(ov, TotalHeight * (percent / 100f));
        }

        public void Minimize(ViewOverlay sliderOverlay)
        {
            SetOverlaySize(sliderOverlay, 0);
        }

        public void MaximizeOverlay(ViewOverlay ov)
        {
            SetOverlaySize(ov, TotalHeight);
        }

        private Rectangle GetRect(ViewOverlay overlay, double newSize)
        {
            var x = overlay.Bounds.X;
            var y = overlay.Bounds.Y;
            var w = overlay.Bounds.Width;
            var h = overlay.Bounds.Height;

            var maxValue = TotalHeight;
            if (overlay.IsHorizontal)
                maxValue = TotalWidth;
            maxValue = Math.Min(maxValue, overlay.MaxSize);

            if (newSize <= overlay.MinSize)
                newSize = overlay.MinSize;
            if (newSize >= maxValue)
                newSize = maxValue;
            switch (overlay.Type)
            {
                case OverlayType.Bottom:
                    y = maxValue - newSize;
                    break;
                case OverlayType.Top:
                    y = -newSize;
                    break;
                case OverlayType.Left:
                    x = -newSize;
                    break;
                case OverlayType.Right:
                    x = maxValue - newSize;
                    break;
            }

            return new Rectangle(x, y, w, h);
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
            var initValue = Math.Max(overlay.MinSize, overlay.InitialSize);
            overlay.Bounds = fullRect;
            return GetRect(overlay, initValue);
        }
    }
}
