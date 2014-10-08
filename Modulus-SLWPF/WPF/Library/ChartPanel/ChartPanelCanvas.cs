using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModulusFE
{
    internal partial class ChartPanelCanvas : Panel
    {

        private readonly List<Visual> _visuals = new List<Visual>();

        protected override int VisualChildrenCount
        {
            get
            {
                return _visuals.Count;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            return _visuals[index];
        }

        public void AddVisual(Visual visual)
        {
            _visuals.Add(visual);

            AddVisualChild(visual);
            AddLogicalChild(visual);
        }

        public void DeleteVisual(Visual visual)
        {
            _visuals.Remove(visual);

            RemoveVisualChild(visual);
            RemoveLogicalChild(visual);
        }

        public DrawingVisual GetVisual(Point point)
        {
            HitTestResult hitResult = VisualTreeHelper.HitTest(this, point);
            return hitResult.VisualHit as DrawingVisual;
        }

        private readonly List<DrawingVisual> hits = new List<DrawingVisual>();
        public List<DrawingVisual> GetVisuals(System.Windows.Media.Geometry region)
        {
            hits.Clear();
            GeometryHitTestParameters parameters = new GeometryHitTestParameters(region);
            HitTestResultCallback callback = HitTestCallback;
            VisualTreeHelper.HitTest(this, null, callback, parameters);
            return hits;
        }

        private HitTestResultBehavior HitTestCallback(HitTestResult result)
        {
            GeometryHitTestResult geometryResult = (GeometryHitTestResult)result;
            DrawingVisual visual = result.VisualHit as DrawingVisual;
            if (visual != null &&
                geometryResult.IntersectionDetail == IntersectionDetail.Intersects)
            {
                hits.Add(visual);
            }
            return HitTestResultBehavior.Continue;
        }
    }
}
