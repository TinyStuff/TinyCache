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

    public class ViewSizeArgs
    {
        public double Old { get; set; }
        public double New { get; set; }
    }

    public class ViewOverlay : ContentView
    {

        public EventHandler<ViewSizeArgs> OnSizeChange;
        //public View OverlayView { get; set; }
        public BoxView ShadowView { get; set; }
        public OverlayType Type { get; set; } = OverlayType.Bottom;
        public double MinSize { get; set; } = 55;
        public double MaxSize { get; set; } = 99999;
        public double InitialSize { get; set; }
        internal bool UseShadow { get; set; }
        //public float SnapPercent { get; set; } = 20;
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

    public class OverlayControl : Layout<View>
    {

        public List<ViewOverlay> Overlays { get; internal set; } = new List<ViewOverlay>();

        protected override void OnAdded(View view)
        {
            base.OnAdded(view);
        }

        protected override void OnChildAdded(Element child)
        {
            base.OnChildAdded(child);
            if (child is ViewOverlay overlay)
            {
                AddOverlay(overlay);
            }
        }

        public void AddOverlay(ViewOverlay overlay)
        {
            Overlays.Add(overlay);

            if (overlay.ShadowView != null)
            {
                overlay.UseShadow = true;
                overlay.ShadowView.IsEnabled = false;
                overlay.ShadowView.Opacity = 0;
            }
            // Ugly as piiip....
            Device.BeginInvokeOnMainThread(()=> {
                Children.Add(overlay.ShadowView);
                RaiseChild(overlay);
            });

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
            overlay.GestureRecognizers.Add(gest);
            ForceLayout();
        }

        private void UpdateLayout(ViewOverlay overlay)
        {
            var orgSize = overlay.OverlayBounds.Height - overlay.OverlayBounds.Y;
            var maxValue = TotalHeight;
            switch (overlay.Type)
            {
                case OverlayType.Top:
                    orgSize = -overlay.OverlayBounds.Y;
                    break;
                case OverlayType.Left:
                    orgSize = -overlay.OverlayBounds.X;
                    maxValue = TotalWidth;
                    break;
                case OverlayType.Right:
                    orgSize = overlay.OverlayBounds.Width - overlay.OverlayBounds.X;
                    maxValue = TotalWidth;
                    break;
            }
            var maxAllowed = Math.Min(maxValue, overlay.MaxSize);
            var newSize = (orgSize - overlay.offset);

            var backgroundOpacity = GetBackgroundOpacity(maxAllowed, newSize);
            if (overlay.active)
            {
                if (overlay.UseShadow)
                {
                    overlay.ShadowView.Opacity = backgroundOpacity * 0.5f;
                    if (overlay is DrawerControl dc)
                    {
                        dc.BackgroundOpacity = backgroundOpacity;
                    }
                }
                overlay.Layout(GetRect(overlay, newSize, orgSize));
            }
            else
            {
                var delta = overlay.lastOffset - overlay.offset;
                var absDelta = Math.Abs(delta);
#if DEBUG
                Console.WriteLine("Drag delta: " + delta);
#endif
                if (absDelta > 14)
                {
                    newSize = (delta > 0) ? maxValue : overlay.MinSize;
                }
                else if (absDelta > 4)
                {
                    newSize += (delta * 4);
                }
                var rect = GetRect(overlay, newSize, orgSize);
                backgroundOpacity = GetBackgroundOpacity(maxAllowed, newSize);
                overlay.LayoutTo(rect, 300, overlay.Easing);
                if (overlay.UseShadow)
                {
                    overlay.ShadowView.FadeTo(backgroundOpacity * 0.5f, 300, Easing.Linear);
                    if (overlay is DrawerControl dc)
                    {
                        dc.BackgroundOpacity = backgroundOpacity;
                    }
                }
                overlay.OverlayBounds = rect;
                overlay.offset = 0;
            }
        }

        private static double GetBackgroundOpacity(double maxAllowed, double newSize)
        {
            return Math.Min(1, Math.Max(0, (newSize / maxAllowed)));
        }

        private void SetOverlaySize(ViewOverlay ov, double ns)
        {
            if (!ov.active)
            {
                //ov.Bounds = GetRect(ov, ns);
                ov.offset = -ns;
                UpdateLayout(ov);
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
            SetOverlaySize(ov, 99999);
        }

        private Rectangle GetRect(ViewOverlay overlay, double newSize, double oldSize = -1)
        {
            var x = overlay.OverlayBounds.X;
            var y = overlay.OverlayBounds.Y;
            var w = overlay.OverlayBounds.Width;
            var h = overlay.OverlayBounds.Height;

            var maxValue = TotalHeight;
            if (overlay.IsHorizontal)
                maxValue = TotalWidth;

            var maxAllowed = Math.Min(maxValue, overlay.MaxSize);

            if (newSize <= overlay.MinSize)
                newSize = overlay.MinSize;
            if (newSize >= maxAllowed)
                newSize = maxAllowed;

            if (oldSize > -1)
            {
                if (overlay.OnSizeChange != null)
                {
                    overlay.OnSizeChange.Invoke(this, new ViewSizeArgs()
                    {
                        Old = oldSize,
                        New = newSize
                    });
                }
            }
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
                if (!Overlays.Any(d => d == child))
                    child.Layout(fullRect);
            }
            foreach (var overlay in Overlays)
            {
                if (overlay.OverlayBounds.IsEmpty)
                {
                    overlay.OverlayBounds = GetInitialBounds(overlay, fullRect);
                }
                overlay.Layout(overlay.OverlayBounds);
            }
        }

        private Rectangle GetInitialBounds(ViewOverlay overlay, Rectangle fullRect)
        {
            var initValue = Math.Max(overlay.MinSize, overlay.InitialSize);
            overlay.OverlayBounds = fullRect;
            return GetRect(overlay, initValue);
        }

    }
}
