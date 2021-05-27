// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;
using EasyWPFUI.Extensions;

namespace EasyWPFUI.Controls
{
    [ContentProperty(nameof(ScrollViewer))]
    public class ItemsRepeaterScrollHost : Panel, IRepeaterScrollingSurface
    {
        public ItemsRepeaterScrollHost()
        {
            m_candidates = new List<CandidateInfo>();
            m_anchorElement = new WeakReference<UIElement>(this);
            m_pendingBringIntoView = new BringIntoViewState(this);
        }

        #region IFrameworkElementOverrides

        protected override Size MeasureOverride(Size availableSize)
        {
            Size desiredSize = default;
            if (ScrollViewer is ScrollViewer scrollViewer)
            {
                scrollViewer.Measure(availableSize);
                desiredSize = scrollViewer.DesiredSize;
            }

            return desiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Size result = finalSize;

            if (ScrollViewer is ScrollViewer scrollViewer)
            {
                bool shouldApplyPendingChangeView = scrollViewer != null && HasPendingBringIntoView() && !m_pendingBringIntoView.ChangeViewCalled;

                Rect anchorElementRelativeBounds = default;
                UIElement anchorElement =
                    // BringIntoView takes precedence over tracking.
                    shouldApplyPendingChangeView ?
                    null :
                    // Pick the best candidate depending on HorizontalAnchorRatio and VerticalAnchorRatio.
                    // The best candidate is the element that's the closest to the edge of interest.
                    GetAnchorElement(ref anchorElementRelativeBounds);

                scrollViewer.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));

                m_pendingViewportShift = 0.0;

                if (shouldApplyPendingChangeView)
                {
                    ApplyPendingChangeView(scrollViewer);
                }
                else if (anchorElement != null)
                {
                    // The anchor element might have changed its position relative to us.
                    // If that's the case, we should shift the viewport to follow it as much as possible.
                    m_pendingViewportShift = TrackElement(anchorElement, anchorElementRelativeBounds, scrollViewer);
                }
                else if (scrollViewer == null)
                {
                    m_pendingBringIntoView.Reset();
                }

                m_candidates.Clear();
                m_isAnchorElementDirty = true;

                //m_postArrange(*this);
                PostArrange?.Invoke(this);
            }

            return result;
        }

        #endregion

        #region IRepeaterScrollingSurface

        public bool IsHorizontallyScrollable
        {
            get
            {
                return true;
            }
        }

        public bool IsVerticallyScrollable
        {
            get
            {
                return true;
            }
        }

        public UIElement AnchorElement
        {
            get
            {
                return GetAnchorElement();
            }
        }

        public event ConfigurationChangedEventHandler ConfigurationChanged;
        public event PostArrangeEventHandler PostArrange;
        public event ViewportChangedEventHandler ViewportChanged;

        public Rect GetRelativeViewport(UIElement element)
        {
            if (ScrollViewer is ScrollViewer scrollViewer)
            {
                UIElement elem = element;
                bool hasLockedViewport = HasPendingBringIntoView();
                GeneralTransform transformer = elem.AltTransformToVisual(hasLockedViewport ? scrollViewer.ContentTemplateRoot() : scrollViewer);

                double zoomFactor = 1.0;// scrollViewer.ZoomFactor();
                double viewportWidth = scrollViewer.ViewportWidth / zoomFactor;
                double viewportHeight = scrollViewer.ViewportHeight / zoomFactor;

                Point elementOffset = transformer.Transform(new Point());

                elementOffset.X = -elementOffset.X;
                elementOffset.Y = -elementOffset.Y + m_pendingViewportShift;

                //REPEATER_TRACE_INFO(L"Pending Shift - %lf\n", m_pendingViewportShift);

                if (hasLockedViewport)
                {
                    elementOffset.X += m_pendingBringIntoView.ChangeViewOffset.X;
                    elementOffset.Y += m_pendingBringIntoView.ChangeViewOffset.Y;
                }

                return new Rect(elementOffset.X, elementOffset.Y, viewportWidth, viewportHeight);
            }

            return Rect.Empty;
        }

        public void RegisterAnchorCandidate(UIElement element)
        {
            if (HorizontalAnchorRatio != double.NaN || VerticalAnchorRatio != double.NaN)
            {
                if (ScrollViewer is ScrollViewer scrollViewer)
                {

                    #if DEBUG
                    // We should not be registring the same element twice. Even through it is functionally ok,
                    // we will end up spending more time during arrange than we must.
                    // However checking if an element is already in the list every time a new element is registered is worse for perf.
                    // So, I'm leaving an assert here to catch regression in our code but in release builds we run without the check.
                    UIElement elem = element;
                    int it = m_candidates.FindIndex(c => c.Element == elem);
                    if (it >= 0)
                    {
                        //Debug.Assert(false);
                    }
                    #endif // _DEBUG

                    m_candidates.Add(new CandidateInfo(element));
                    m_isAnchorElementDirty = true;
                }
            }
        }

        public void UnregisterAnchorCandidate(UIElement element)
        {
            UIElement elem = element;
            int it = m_candidates.FindIndex(c => c.Element == elem);
            if (it >= 0)
            {
                //REPEATER_TRACE_INFO(L"Unregistered candidate %d\n", it - m_candidates.begin());
                m_candidates.RemoveAt(it);
                m_isAnchorElementDirty = true;
            }
        }

        #endregion

        #region IScrollAnchorProvider

        public double HorizontalAnchorRatio
        {
            get
            {
                return m_horizontalEdge;
            }
            set
            {
                m_horizontalEdge = value;
            }
        }

        public double VerticalAnchorRatio
        {
            get
            {
                return m_verticalEdge;
            }
            set
            {
                m_verticalEdge = value;
            }
        }

        public UIElement CurrentAnchor
        {
            get
            {
                return GetAnchorElement();
            }
        }

        public ScrollViewer ScrollViewer
        {
            get
            {
                ScrollViewer value = null;

                UIElementCollection children = Children;

                if(children.Count > 0)
                {
                    value = children[0] as ScrollViewer;
                }

                return value;
            }
            set
            {
                if(ScrollViewer != null)
                {
                    ScrollViewer.SizeChanged -= ScrollViewerSizeChanged;
                    ScrollViewer.ScrollChanged -= ScrollViewerScrollChanged;
                }

                UIElementCollection children = Children;
                children.Clear();
                children.Add(value);

                value.SizeChanged += ScrollViewerSizeChanged;
                value.ScrollChanged += ScrollViewerScrollChanged;
            }
        }

        #endregion

        // TODO: this API should go on UIElement.
        public void StartBringIntoView(UIElement element, double alignmentX, double alignmentY, double offsetX, double offsetY, bool animate)
        {
            m_pendingBringIntoView = new BringIntoViewState(element, alignmentX, alignmentY, offsetX, offsetY, animate);
        }

        /* Private */

        private void ApplyPendingChangeView(ScrollViewer scrollViewer)
        {
            BringIntoViewState bringIntoView = m_pendingBringIntoView;

            bringIntoView.ChangeViewCalled = true;

            Rect layoutSlot = CachedVisualTreeHelpers.GetLayoutSlot((FrameworkElement)bringIntoView.TargetElement);

            // Arrange bounds are absolute.
            Rect arrangeBounds = bringIntoView.TargetElement.AltTransformToVisual(scrollViewer.ContentTemplateRoot()).TransformBounds(new Rect(0, 0, layoutSlot.Width, layoutSlot.Height));

            Point scrollableArea = new Point((scrollViewer.ViewportWidth - arrangeBounds.Width), (scrollViewer.ViewportHeight - arrangeBounds.Height));

            // Calculate the target offset based on the alignment and offset parameters.
            // Make sure that we are constrained to the ScrollViewer's extent.
            Point changeViewOffset = new Point(Math.Max(0.0, (Math.Min(arrangeBounds.X + bringIntoView.OffsetX - scrollableArea.X * bringIntoView.AlignmentX, scrollViewer.ExtentWidth - scrollViewer.ViewportWidth))),
                Math.Max(0.0f, (Math.Min(arrangeBounds.Y + bringIntoView.OffsetY - scrollableArea.Y * bringIntoView.AlignmentY, scrollViewer.ExtentHeight - scrollViewer.ViewportHeight))));

            bringIntoView.ChangeViewOffset = changeViewOffset;

            // REPEATER_TRACE_INFO(L"ItemsRepeaterScrollHost scroll to absolute offset (%.0f, %.0f), animate=%d \n", changeViewOffset.X, changeViewOffset.Y, bringIntoView.Animate());

            //scrollViewer.ChangeView(changeViewOffset.X, changeViewOffset.Y, null, !bringIntoView.Animate()); // TODO

            //m_pendingBringIntoView = std::move(bringIntoView);
        }

        private double TrackElement(UIElement element, Rect previousBounds, ScrollViewer scrollViewer)
        {
            Rect bounds = LayoutInformation.GetLayoutSlot((FrameworkElement)element);
            GeneralTransform transformer = element.AltTransformToVisual(scrollViewer.ContentTemplateRoot());
            Rect newBounds = transformer.TransformBounds(new Rect(0.0f, 0.0, bounds.Width, bounds.Height));

            double oldEdgeOffset = previousBounds.Y + HorizontalAnchorRatio * previousBounds.Height;
            double newEdgeOffset = newBounds.Y + HorizontalAnchorRatio * newBounds.Height;

            double unconstrainedPendingViewportShift = newEdgeOffset - oldEdgeOffset;
            double pendingViewportShift = unconstrainedPendingViewportShift;

            // ScrollViewer.ChangeView is not synchronous, so we need to account for the pending ChangeView call
            // and make sure we are locked on the target viewport.
            double verticalOffset = HasPendingBringIntoView() && !m_pendingBringIntoView.Animate ? m_pendingBringIntoView.ChangeViewOffset.Y : scrollViewer.VerticalOffset;

            // Constrain the viewport shift to the extent
            if (verticalOffset + pendingViewportShift < 0)
            {
                pendingViewportShift = -verticalOffset;
            }
            else if (verticalOffset + scrollViewer.ViewportHeight + pendingViewportShift > scrollViewer.ExtentHeight)
            {
                pendingViewportShift = scrollViewer.ExtentHeight - scrollViewer.ViewportHeight - verticalOffset;
            }

            if (Math.Abs(pendingViewportShift) > 1)
            {
                // TODO: do we need to account for the zoom factor?
                // BUG:
                //  Unfortunately, if we have to correct while animating, we almost never
                //  update the ongoing animation correctly and we end up missing our target
                //  viewport. We should address that when building element tracking as part
                //  of the framework.
                //REPEATER_TRACE_INFO(L"Viewport shift:%.0f. \n", pendingViewportShift);
                    scrollViewer.ScrollToVerticalOffset(verticalOffset);
                // scrollViewer.ChangeView(null, verticalOffset + pendingViewportShift, null, true /* disableAnimation */); TODO
            }
            else
            {
                pendingViewportShift = 0.0;

                // We can't shift the viewport to follow the tracked element. The viewport relative
                // to the tracked element will have changed. We need to raise ViewportChanged to make
                // sure the repeaters will get a second layout pass to fill any empty space they have.
                if (Math.Abs(unconstrainedPendingViewportShift) > 1)
                {
                    ViewportChanged?.Invoke(this, true /* isFinal */);
                }
            }

            return pendingViewportShift;
        }

        private UIElement GetAnchorElement(ref Rect relativeBounds)
        {
            if (m_isAnchorElementDirty)
            {
                if (ScrollViewer is ScrollViewer scrollViewer)
                {
                    // ScrollViewer.ChangeView is not synchronous, so we need to account for the pending ChangeView call
                    // and make sure we are locked on the target viewport.
                    double verticalOffset = HasPendingBringIntoView() && !m_pendingBringIntoView.Animate ? m_pendingBringIntoView.ChangeViewOffset.Y : scrollViewer.VerticalOffset;

                    double viewportEdgeOffset = verticalOffset + HorizontalAnchorRatio * scrollViewer.ViewportHeight + m_pendingViewportShift;

                    CandidateInfo bestCandidate = null;
                    double bestCandidateDistance = double.MaxValue;

                    foreach (CandidateInfo candidate in m_candidates)
                    {
                        UIElement element = candidate.Element;

                        if (!candidate.IsRelativeBoundsSet())
                        {
                            Rect bounds = LayoutInformation.GetLayoutSlot((FrameworkElement)element);
                            GeneralTransform transformer = element.AltTransformToVisual(scrollViewer.ContentTemplateRoot());
                            candidate.RelativeBounds = transformer.TransformBounds(new Rect(0.0, 0.0, bounds.Width, bounds.Height));
                        }

                        double elementEdgeOffset = candidate.RelativeBounds.Y + HorizontalAnchorRatio * candidate.RelativeBounds.Height;
                        double candidateDistance = Math.Abs(elementEdgeOffset - viewportEdgeOffset);

                        if (candidateDistance < bestCandidateDistance)
                        {
                            bestCandidate = candidate;
                            bestCandidateDistance = candidateDistance;
                        }
                    }

                    if (bestCandidate != null)
                    {
                        m_anchorElement = new WeakReference<UIElement>(bestCandidate.Element);
                        m_anchorElementRelativeBounds = bestCandidate.RelativeBounds;
                    }
                    else
                    {
                        m_anchorElement = null;
                        m_anchorElementRelativeBounds = CandidateInfo.InvalidBounds;
                    }
                }

                m_isAnchorElementDirty = false;
            }

            if (relativeBounds != null)
            {
                relativeBounds = m_anchorElementRelativeBounds;
            }

            if (m_anchorElement != null && m_anchorElement.TryGetTarget(out UIElement target))
            {
                return target;
            }
            else
            {
                return null;
            }
        }

        private UIElement GetAnchorElement()
        {
            Rect rect = Rect.Empty;
            return GetAnchorElement(ref rect);
        }

        private void ScrollViewerScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            m_pendingViewportShift = 0.0;

            if (HasPendingBringIntoView() && m_pendingBringIntoView.ChangeViewCalled == true)
            {
                m_pendingBringIntoView.Reset();
            }

            ViewportChanged?.Invoke(this, true /* isFinal */);
        }

        private void ScrollViewerSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ViewportChanged?.Invoke(this, true /* isFinal */);
        }

        private bool HasPendingBringIntoView()
        {
            return m_pendingBringIntoView.TargetElement != null;
        }

        private class CandidateInfo
        {
            public CandidateInfo(UIElement element)
            {
                m_element = new WeakReference<UIElement>(element);

                InvalidBounds = Rect.Empty;
                RelativeBounds = InvalidBounds;
            }

            public UIElement Element
            {
                get
                {
                    if (m_element != null && m_element.TryGetTarget(out UIElement target))
                    {
                        return target;
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            public Rect RelativeBounds { get; set; }

            public bool IsRelativeBoundsSet()
            {
                return RelativeBounds != InvalidBounds;
            }

            public static Rect InvalidBounds;
            
            private WeakReference<UIElement> m_element;
        }

        private class BringIntoViewState
        {
            public BringIntoViewState(UIElement owner) : this(owner, default, default, default, default, default)
            {

            }

            public BringIntoViewState(UIElement targetElement, double alignmentX, double alignmentY, double offsetX, double offsetY, bool animate)
            {
                m_targetElement = new WeakReference<UIElement>(targetElement);
                m_alignmentX = alignmentX;
                m_alignmentY = alignmentY;
                m_offsetX = offsetX;
                m_offsetY = offsetY;
                m_animate = animate;

                m_changeViewOffset = default;
                m_changeViewCalled = default;
            }

            public UIElement TargetElement
            {
                get
                {
                    if (m_targetElement != null && m_targetElement.TryGetTarget(out UIElement target))
                    {
                        return target;
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            public double AlignmentX
            {
                get
                {
                    return m_alignmentX;
                }
            }

            public double AlignmentY
            {
                get
                {
                    return m_alignmentY;
                }
            }

            public double OffsetX
            {
                get
                {
                    return m_offsetX;
                }
            }

            public double OffsetY
            {
                get
                {
                    return m_offsetY;
                }
            }

            public bool Animate
            {
                get
                {
                    return m_animate;
                }
            }

            public bool ChangeViewCalled
            {
                get
                {
                    return m_changeViewCalled;
                }
                set
                {
                    m_changeViewCalled = value;
                }
            }

            public Point ChangeViewOffset
            {
                get
                {
                    return m_changeViewOffset;
                }
                set
                {
                    m_changeViewOffset = value;
                }
            }

            public void Reset()
            {
                m_targetElement = null;
                m_alignmentX = m_alignmentY = m_offsetX = m_offsetY = 0.0;
                m_animate = m_changeViewCalled = false;
                m_changeViewOffset = default;
            }

            private WeakReference<UIElement> m_targetElement;
            private double m_alignmentX;
            private double m_alignmentY;
            private double m_offsetX;
            private double m_offsetY;
            private bool m_animate;
            private bool m_changeViewCalled;
            private Point m_changeViewOffset;
        }

        private List<CandidateInfo> m_candidates;

        private WeakReference<UIElement> m_anchorElement;

        private Rect m_anchorElementRelativeBounds = default;

        // Whenever the m_candidates list changes, we set this to true.
        bool m_isAnchorElementDirty = true;

        double m_horizontalEdge;
        double m_verticalEdge;    // Not used in this temporary implementation.

        // We can only bring an element into view after it got arranged and
        // we know its bounds as well as the viewport (so that we can account
        // for alignment and offset).
        // The BringIntoView call can however be made at any point, even
        // in the constructor of a page (deserialization scenario) so we
        // need to hold on the parameter that are passed in BringIntoViewOperation.
        BringIntoViewState m_pendingBringIntoView;

        // A ScrollViewer.ChangeView operation, even if not animated, is not synchronous.
        // In other words, right after the call, ScrollViewer.[Vertical|Horizontal]Offset and
        // TransformToVisual are not going to reflect the new viewport. We need to keep
        // track of the pending viewport shift until the ChangeView operation completes
        // asynchronously.
        double m_pendingViewportShift;
    }
}
