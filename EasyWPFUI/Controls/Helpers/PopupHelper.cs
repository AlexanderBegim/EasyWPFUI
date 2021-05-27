using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Interop;
using System.Reflection;
using EasyWPFUI.Common;

namespace EasyWPFUI.Controls.Helpers
{
    public class PopupHelper
    {
        #region RepositionHelperProperty

        internal static readonly DependencyProperty RepositionHelperProperty = DependencyProperty.RegisterAttached("RepositionHelper", typeof(PopupRepositionHelper), typeof(PopupHelper), new FrameworkPropertyMetadata());

        #endregion

        #region IsRepositionEnabled Property

        public static readonly DependencyProperty IsRepositionEnabledProperty = DependencyProperty.RegisterAttached("IsRepositionEnabled", typeof(bool), typeof(PopupHelper), new FrameworkPropertyMetadata(false, OnIsRepositionPropertyChanged));

        public static bool GetRepositionEnabled(UIElement ui)
        {
            return (bool)ui.GetValue(IsRepositionEnabledProperty);
        }

        public static void SetRepositionEnabled(UIElement ui, bool value)
        {
            ui.SetValue(IsRepositionEnabledProperty, value);
        }

        private static void OnIsRepositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Popup popup = d as Popup;

            if (popup == null)
                return;

            if((bool)e.NewValue)
            {
                PopupRepositionHelper helper = new PopupRepositionHelper(popup);
                helper.PopupRepositionAttach();
                popup.SetValue(RepositionHelperProperty, helper);
            }
            else
            {
                PopupRepositionHelper helper = popup.GetValue(RepositionHelperProperty) as PopupRepositionHelper;

                if (helper == null)
                    return;

                helper.PopupRepositionDetach();

                popup.ClearValue(RepositionHelperProperty);
            }
        }

        #endregion
    }

    internal class PopupRepositionHelper
    {
        private Popup popup;
        private FrameworkElement popupRoot;
        private UIElement target;
        private Window window;
        private HwndSource hwndSource;
        private const double Tolerance = 1.0e-2; // allow errors in double calculations
        private PositionInfo _positionInfo;

        #region Properties

        private bool IsTransparent
        {
            get
            {
                return popup.AllowsTransparency;
            }
        }

        private bool DropOpposite
        {
            get
            {
                return PopupInternalMethods.GetDropOpposition(popup);
            }
        }

        #endregion

        #region Methods
        public PopupRepositionHelper(Popup popup)
        {
            if (popup == null)
            {
                throw new ArgumentNullException("popup");
            }

            this.popup = popup;
        }

        public void PopupRepositionAttach()
        {
            popup.Opened += OnPopupOpened;
            popup.Closed += OnPopupClosed;

            target = popup.PlacementTarget;
            window = Window.GetWindow(target);
        }

        public void PopupRepositionDetach()
        {
            if (popup.IsOpen)
                popup.IsOpen = false;

            popup.Opened -= OnPopupOpened;
            popup.Closed -= OnPopupClosed;

            window = null;
        }

        private void OnPopupOpened(object sender, EventArgs e)
        {
            if (target != null)
            {
                target.LayoutUpdated += OnLayoutUpdated;
            }

            if (window != null)
            {
                window.LocationChanged += OnWindowLocationChanged;
            }

            if(popup.Child is UIElement child && PresentationSource.FromVisual(child) is HwndSource hwnd)
            {
                popupRoot = hwnd.RootVisual as FrameworkElement;
                hwndSource = hwnd;
            }
        }

        private void OnPopupClosed(object sender, EventArgs e)
        {
            if (target != null)
            {
                target.LayoutUpdated -= OnLayoutUpdated;
            }

            if (window != null)
            {
                window.LocationChanged -= OnWindowLocationChanged;
            }

            hwndSource = null;
        }

        private void OnWindowLocationChanged(object sender, EventArgs e)
        {
            UpdatePopupPosition();
        }

        private void OnLayoutUpdated(object sender, EventArgs e)
        {
            UpdatePopupPosition();
        }

        private void UpdatePopupPosition()
        {
            UpdatePosition();
        }

        // To position the popup, we find the InterestPoints of the placement rectangle/point
        // in the screen coordinate space.  We also find the InterestPoints of the child in
        // the popup's space.  Then we attempt all valid combinations of matching InterestPoints
        // (based on PlacementMode) to find the position that best fits on the screen.
        // NOTE: any reference to the screen implies the monitor for full trust and
        //       the browser area for partial trust
        private void UpdatePosition()
        {
            if (popupRoot == null)
                return;

            if(_positionInfo == null)
            {
                _positionInfo = new PositionInfo();
            }

            PlacementMode placement = PopupInternalMethods.GetPlacementInternal(popup);

            // Get a list of the corners of the target/child in screen space
            Point[] placementTargetInterestPoints = GetPlacementTargetInterestPoints(placement);
            Point[] childInterestPoints = GetChildInterestPoints(placement);

            // Find bounds of screen and child in screen space
            Rect targetBounds = GetBounds(placementTargetInterestPoints);
            Rect screenBounds;
            Rect childBounds = GetBounds(childInterestPoints);

            double childArea = childBounds.Width * childBounds.Height;

            // Rank possible positions
            int bestIndex = -1;
            Vector bestTranslation = new Vector(_positionInfo.X, _positionInfo.Y);
            double bestScore = -1;
            PopupPrimaryAxis bestAxis = PopupPrimaryAxis.None;

            int positions;

            CustomPopupPlacement[] customPlacements = null;

            // Find the number of possible positions
            if (placement == PlacementMode.Custom)
            {
                CustomPopupPlacementCallback customCallback = popup.CustomPopupPlacementCallback;
                if (customCallback != null)
                {
                    customPlacements = customCallback(childBounds.Size, targetBounds.Size, new Point(popup.HorizontalOffset, popup.VerticalOffset));
                }
                positions = customPlacements == null ? 0 : customPlacements.Length;

                // Return if callback closed the popup
                if (!popup.IsOpen)
                    return;
            }
            else
            {
                positions = GetNumberOfCombinations(placement);
            }

            // Try each position until the best one is found
            for (int i = 0; i < positions; i++)
            {
                Vector popupTranslation;

                PopupPrimaryAxis axis;

                // Get the ith Position to rank
                if (placement == PlacementMode.Custom)
                {
                    // The custom callback only calculates relative to 0,0
                    // so the placementTarget's top/left need to be re-applied.
                    popupTranslation = ((Vector)placementTargetInterestPoints[(int)InterestPoint.TopLeft])
                                      + ((Vector)customPlacements[i].Point);  // vector from origin

                    axis = customPlacements[i].PrimaryAxis;
                }
                else
                {
                    PointCombination pointCombination = GetPointCombination(placement, i, out axis);

                    InterestPoint targetInterestPoint = pointCombination.TargetInterestPoint;
                    InterestPoint childInterestPoint = pointCombination.ChildInterestPoint;

                    // Compute the vector from the screen origin to the top left corner of the popup
                    // that will cause the the two interest points to overlap
                    popupTranslation = placementTargetInterestPoints[(int)targetInterestPoint]
                                       - childInterestPoints[(int)childInterestPoint];
                }

                // Find percent of popup on screen by translating the popup bounds
                // and calculating the percent of the bounds that is on screen
                // Note: this score is based on the percent of the popup that is on screen
                //       not the percent of the child that is on screen.  For certain
                //       scenarios, this may produce in counter-intuitive results.
                //       If this is a problem, more complex scoring is needed
                Rect tranlsatedChildBounds = Rect.Offset(childBounds, popupTranslation);
                screenBounds = GetScreenBounds(targetBounds, placementTargetInterestPoints[(int)InterestPoint.TopLeft]);
                Rect currentIntersection = Rect.Intersect(screenBounds, tranlsatedChildBounds);

                // Calculate area of intersection
                double score = currentIntersection != Rect.Empty ? currentIntersection.Width * currentIntersection.Height : 0;

                // If current score is better than the best score so far, save the position info
                if (score - bestScore > Tolerance)
                {
                    bestIndex = i;
                    bestTranslation = popupTranslation;
                    bestScore = score;
                    bestAxis = axis;

                    // Stop when we find a popup that is completely on screen
                    if (Math.Abs(score - childArea) < Tolerance)
                    {
                        break;
                    }
                }
            }

            // Check to see if the pop needs to be nudged onto the screen.
            // Popups are not nudged if their axes do not align with the screen axes

            // Use the size of the popupRoot in case it is clipping the popup content
            Matrix transformToDevice = PopupInternalMethods.SCGetTransformToDevice(popup);

            childBounds = new Rect((Size)transformToDevice.Transform((Point)(popup.Child != null ? popup.Child.RenderSize : popupRoot.RenderSize)));

            childBounds.Offset(bestTranslation);

            Vector childTranslation = (Vector)transformToDevice.Transform(popup.Child != null ? popup.Child.TranslatePoint(new Point(), popupRoot) : new Point());

            childBounds.Offset(childTranslation);

            screenBounds = GetScreenBounds(targetBounds, placementTargetInterestPoints[(int)InterestPoint.TopLeft]);
            Rect intersection = Rect.Intersect(screenBounds, childBounds);

            // See if width/height of intersection are less than child's
            if (Math.Abs(intersection.Width - childBounds.Width) > Tolerance ||
                Math.Abs(intersection.Height - childBounds.Height) > Tolerance)
            {
                // Nudge Horizontally
                Point topLeft = placementTargetInterestPoints[(int)InterestPoint.TopLeft];
                Point topRight = placementTargetInterestPoints[(int)InterestPoint.TopRight];

                // Create a vector pointing from the top of the placement target to the bottom
                // to determine which direction the popup should be nudged in.
                // If the vector is zero (NaN's after normalization), nudge horizontally
                Vector horizontalAxis = topRight - topLeft;
                horizontalAxis.Normalize();

                // See if target's horizontal axis is aligned with screen
                // (For opaque windows always translate horizontally)
                if (!IsTransparent || double.IsNaN(horizontalAxis.Y) || Math.Abs(horizontalAxis.Y) < Tolerance)
                {
                    // Nudge horizontally
                    if (childBounds.Right > screenBounds.Right)
                    {
                        bestTranslation.X = screenBounds.Right - childBounds.Width;
                        bestTranslation.X -= childTranslation.X;
                    }
                    else if (childBounds.Left < screenBounds.Left)
                    {
                        bestTranslation.X = screenBounds.Left;
                        bestTranslation.X -= childTranslation.X;
                    }
                }
                else if (IsTransparent && Math.Abs(horizontalAxis.X) < Tolerance)
                {
                    // Nudge vertically, limit horizontally
                    if (childBounds.Bottom > screenBounds.Bottom)
                    {
                        bestTranslation.Y = screenBounds.Bottom - childBounds.Height;
                        bestTranslation.Y -= childTranslation.Y;
                    }
                    else if (childBounds.Top < screenBounds.Top)
                    {
                        bestTranslation.Y = screenBounds.Top;
                        bestTranslation.Y -= childTranslation.Y;
                    }
                }

                // Nudge Vertically
                Point bottomLeft = placementTargetInterestPoints[(int)InterestPoint.BottomLeft];

                // Create a vector pointing from the top of the placement target to the bottom
                // to determine which direction the popup should be nudged in
                // If the vector is zero (NaN's after normalization), nudge vertically
                Vector verticalAxis = topLeft - bottomLeft;
                verticalAxis.Normalize();

                // Axis is aligned with screen, nudge
                if (!IsTransparent || double.IsNaN(verticalAxis.X) || Math.Abs(verticalAxis.X) < Tolerance)
                {
                    if (childBounds.Bottom > screenBounds.Bottom)
                    {
                        bestTranslation.Y = screenBounds.Bottom - childBounds.Height;
                        bestTranslation.Y -= childTranslation.Y;
                    }
                    else if (childBounds.Top < screenBounds.Top)
                    {
                        bestTranslation.Y = screenBounds.Top;
                        bestTranslation.Y -= childTranslation.Y;
                    }
                }
                else if (IsTransparent && Math.Abs(verticalAxis.Y) < Tolerance)
                {
                    if (childBounds.Right > screenBounds.Right)
                    {
                        bestTranslation.X = screenBounds.Right - childBounds.Width;
                        bestTranslation.X -= childTranslation.X;
                    }
                    else if (childBounds.Left < screenBounds.Left)
                    {
                        bestTranslation.X = screenBounds.Left;
                        bestTranslation.X -= childTranslation.X;
                    }
                }
            }

            // Finally, take the best position and apply it to the popup
            int bestX = DoubleToInt(bestTranslation.X);
            int bestY = DoubleToInt(bestTranslation.Y);
            if (bestX != _positionInfo.X || bestY != _positionInfo.Y)
            {
                _positionInfo.X = bestX;
                _positionInfo.Y = bestY;
                PopupInternalMethods.SCSetPopupPos(popup, true, bestX, bestY, false, 0, 0);
            }
        }

        private Rect GetBounds(Point[] placement)
        {
            return PopupInternalMethods.GetBounds(popup, placement);
        }

        private Point[] GetChildInterestPoints(PlacementMode placement)
        {
            return PopupInternalMethods.GetChildInterestPoints(popup, placement);
        }

        private Point[] GetPlacementTargetInterestPoints(PlacementMode placement)
        {
            return PopupInternalMethods.GetPlacementTargetInterestPoints(popup, placement);
        }

        private Rect GetScreenBounds(Rect targetBounds, Point point)
        {
            return PopupInternalMethods.GetScreenBounds(popup, targetBounds, point);
        }

        private PointCombination GetPointCombination(PlacementMode placement, int i, out PopupPrimaryAxis axis)
        {
            bool dropFromRight = SystemParameters.MenuDropAlignment;

            switch (placement)
            {
                case PlacementMode.Bottom:
                case PlacementMode.Mouse:
                    axis = PopupPrimaryAxis.Horizontal;
                    if (dropFromRight)
                    {
                        if (i == 0) return new PointCombination(InterestPoint.BottomRight, InterestPoint.TopRight);
                        if (i == 1) return new PointCombination(InterestPoint.TopRight, InterestPoint.BottomRight);
                    }
                    else
                    {
                        if (i == 0) return new PointCombination(InterestPoint.BottomLeft, InterestPoint.TopLeft);
                        if (i == 1) return new PointCombination(InterestPoint.TopLeft, InterestPoint.BottomLeft);
                    }
                    break;


                case PlacementMode.Top:
                    axis = PopupPrimaryAxis.Horizontal;
                    if (dropFromRight)
                    {
                        if (i == 0) return new PointCombination(InterestPoint.TopRight, InterestPoint.BottomRight);
                        if (i == 1) return new PointCombination(InterestPoint.BottomRight, InterestPoint.TopRight);
                    }
                    else
                    {
                        if (i == 0) return new PointCombination(InterestPoint.TopLeft, InterestPoint.BottomLeft);
                        if (i == 1) return new PointCombination(InterestPoint.BottomLeft, InterestPoint.TopLeft);
                    }
                    break;


                case PlacementMode.Right:
                case PlacementMode.Left:
                    axis = PopupPrimaryAxis.Vertical;
                    dropFromRight |= DropOpposite;

                    if ((dropFromRight && placement == PlacementMode.Right) ||
                        (!dropFromRight && placement == PlacementMode.Left))
                    {
                        if (i == 0) return new PointCombination(InterestPoint.TopLeft, InterestPoint.TopRight);
                        if (i == 1) return new PointCombination(InterestPoint.BottomLeft, InterestPoint.BottomRight);
                        if (i == 2) return new PointCombination(InterestPoint.TopRight, InterestPoint.TopLeft);
                        if (i == 3) return new PointCombination(InterestPoint.BottomRight, InterestPoint.BottomLeft);
                    }
                    else
                    {
                        if (i == 0) return new PointCombination(InterestPoint.TopRight, InterestPoint.TopLeft);
                        if (i == 1) return new PointCombination(InterestPoint.BottomRight, InterestPoint.BottomLeft);
                        if (i == 2) return new PointCombination(InterestPoint.TopLeft, InterestPoint.TopRight);
                        if (i == 3) return new PointCombination(InterestPoint.BottomLeft, InterestPoint.BottomRight);
                    }
                    break;

                case PlacementMode.Relative:
                case PlacementMode.RelativePoint:
                case PlacementMode.MousePoint:
                case PlacementMode.AbsolutePoint:
                    axis = PopupPrimaryAxis.Horizontal;
                    if (dropFromRight)
                    {
                        if (i == 0) return new PointCombination(InterestPoint.TopLeft, InterestPoint.TopRight);
                        if (i == 1) return new PointCombination(InterestPoint.TopLeft, InterestPoint.TopLeft);
                        if (i == 2) return new PointCombination(InterestPoint.TopLeft, InterestPoint.BottomRight);
                        if (i == 3) return new PointCombination(InterestPoint.TopLeft, InterestPoint.BottomLeft);
                    }
                    else
                    {
                        if (i == 0) return new PointCombination(InterestPoint.TopLeft, InterestPoint.TopLeft);
                        if (i == 1) return new PointCombination(InterestPoint.TopLeft, InterestPoint.TopRight);
                        if (i == 2) return new PointCombination(InterestPoint.TopLeft, InterestPoint.BottomLeft);
                        if (i == 3) return new PointCombination(InterestPoint.TopLeft, InterestPoint.BottomRight);
                    }
                    break;

                case PlacementMode.Center:
                    axis = PopupPrimaryAxis.None;
                    return new PointCombination(InterestPoint.Center, InterestPoint.Center);

                case PlacementMode.Absolute:
                case PlacementMode.Custom:
                default:
                    axis = PopupPrimaryAxis.None;
                    return new PointCombination(InterestPoint.TopLeft, InterestPoint.TopLeft);
            }

            return new PointCombination(InterestPoint.TopLeft, InterestPoint.TopRight);
        }

        private int GetNumberOfCombinations(PlacementMode placement)
        {
            return PopupInternalMethods.GetNumberOfCombinations(popup, placement);
        }

        private Matrix SCGetTransformToDevice()
        {
            return PopupInternalMethods.SCGetTransformToDevice(popup);
        }

        private void SCSetPopupPos(bool position, int x, int y, bool size, int width, int height)
        {
            PopupInternalMethods.SCSetPopupPos(popup, position, x, y, size, width, height);
        }

        public static int DoubleToInt(double val)
        {
            return (0 < val) ? (int)(val + 0.5) : (int)(val - 0.5);
        }


        #endregion

        #region Private Classes & Enums

        private static class PopupInternalMethods
        {
            private static MethodInfo methodGetBounds;
            private static MethodInfo methodGetPlacementTargetInterestPoints;
            private static MethodInfo methodGetChildInterestPoints;
            private static MethodInfo methodGetNumberOfCombinations;
            //private static MethodInfo methodGetPointCombination;
            private static MethodInfo methodGetScreenBounds;
            private static MethodInfo methodSecHelperGetWindowRect;
            private static MethodInfo methodSecHelperGetTransformToDevice;
            private static MethodInfo methodSecHelperSetPopupPos;

            private static PropertyInfo propertyPlacementInternal;
            private static PropertyInfo propertyDropOpposition;

            private static FieldInfo _secHelper;

            private static BindingFlags DefaultBindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;

            static PopupInternalMethods()
            {
                Type popupType = typeof(Popup);

                methodGetBounds = popupType.GetMethod("GetBounds", DefaultBindingFlags);

                methodGetScreenBounds = popupType.GetMethod("GetScreenBounds", DefaultBindingFlags);

                methodGetPlacementTargetInterestPoints = popupType.GetMethod("GetPlacementTargetInterestPoints", DefaultBindingFlags);

                methodGetChildInterestPoints = popupType.GetMethod("GetChildInterestPoints", DefaultBindingFlags);

                methodGetNumberOfCombinations = popupType.GetMethod("GetNumberOfCombinations", DefaultBindingFlags | BindingFlags.Static);

                //methodGetPointCombination = popupType.GetMethod("GetPointCombination", DefaultBindingFlags);

                propertyPlacementInternal = popupType.GetProperty("PlacementInternal", DefaultBindingFlags);

                propertyDropOpposition = popupType.GetProperty("DropOpposite", DefaultBindingFlags);

                _secHelper = popupType.GetField("_secHelper", DefaultBindingFlags);

                Type secHelperType = _secHelper.FieldType;

                methodSecHelperGetWindowRect = secHelperType.GetMethod("GetParentWindowRect", DefaultBindingFlags);

                methodSecHelperGetTransformToDevice = secHelperType.GetMethod("GetTransformToDevice", DefaultBindingFlags);

                methodSecHelperSetPopupPos = secHelperType.GetMethod("SetPopupPos", DefaultBindingFlags);
            }

            public static PlacementMode GetPlacementInternal(Popup target)
            {
                return (PlacementMode)propertyPlacementInternal.GetValue(target);
            }

            public static bool GetDropOpposition(Popup target)
            {
                return (bool)propertyDropOpposition.GetValue(target);
            }

            public static Rect GetBounds(Popup target, Point[] placement)
            {
                return (Rect)methodGetBounds.Invoke(target, new object[] { placement });
            }

            public static Rect GetScreenBounds(Popup target, Rect targetBounds, Point point)
            {
                return (Rect)methodGetScreenBounds.Invoke(target, new object[] { targetBounds, point });
            }

            public static Point[] GetChildInterestPoints(Popup target, PlacementMode placement)
            {
                return (Point[])methodGetChildInterestPoints.Invoke(target, new object[] { placement });
            }

            public static Point[] GetPlacementTargetInterestPoints(Popup target, PlacementMode placement)
            {
                return (Point[])methodGetPlacementTargetInterestPoints.Invoke(target, new object[] { placement });
            }

            public static int GetNumberOfCombinations(Popup target, PlacementMode placement)
            {
                object result = methodGetNumberOfCombinations.Invoke(target, new object[] { placement });

                return result is int ? (int)result : 0;
            }

            public static Matrix SCGetTransformToDevice(Popup target)
            {
                object result = methodSecHelperGetTransformToDevice.Invoke(_secHelper.GetValue(target), null);

                return result is Matrix ? (Matrix)result : Matrix.Identity;
            }

            public static Rect SCGetWindowRect(Popup target)
            {
                return (Rect)methodSecHelperGetWindowRect.Invoke(_secHelper.GetValue(target), null);
            }

            public static void SCSetPopupPos(Popup target, bool position, int x, int y, bool size, int width, int height)
            {
                methodSecHelperSetPopupPos.Invoke(_secHelper.GetValue(target), new object[] { position, x, y, size, width, height });
            }
        }

        private class PositionInfo
        {
            // The position of the upper left corner of the popup after nudging
            public int X;
            public int Y;

            // The size of the popup
            public Size ChildSize;

            // The screen rect of the mouse
            public Rect MouseRect = Rect.Empty;
        }

        // This struct is returned by GetPointCombination to indicate
        // which points on the target can align with points on the child
        private struct PointCombination
        {
            public PointCombination(InterestPoint targetInterestPoint, InterestPoint childInterestPoint)
            {
                TargetInterestPoint = targetInterestPoint;
                ChildInterestPoint = childInterestPoint;
            }

            public InterestPoint TargetInterestPoint;
            public InterestPoint ChildInterestPoint;
        }

        // Indicies into InterestPoint point array
        private enum InterestPoint
        {
            TopLeft = 0,
            TopRight = 1,
            BottomLeft = 2,
            BottomRight = 3,
            Center = 4,
        }

        #endregion
    }
}
