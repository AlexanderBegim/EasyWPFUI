using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media.Animation;
using System.Windows.Input;
using EasyWPFUI;
using EasyWPFUI.Controls;
using EasyWPFUI.Common;
using EasyWPFUI.Extensions;

namespace EasyWPFUI.Controls.Primitives
{
    public class FlyoutBase : DependencyObject
    {
        private Popup m_popup;
        private Window m_parentWindow;
        private Thickness m_parentWindowBorder;
        private Rect m_currentTargetBoundsInCoreWindowSpace = new Rect(0, 0, 0, 0);

        protected Control m_presenter;


        private Storyboard m_openAnimationStoryboard;
        private Storyboard m_closeAnimationStoryboard;
        private TimeSpan m_expandAnimationDuration = TimeSpan.FromMilliseconds(100);
        private Thickness m_popupOffset = new Thickness(5);

        #region Events

        public event TypedEventHandler<FlyoutBase, object> Closed;
        public event TypedEventHandler<FlyoutBase,FlyoutBaseClosingEventArgs> Closing;
        public event TypedEventHandler<FlyoutBase, object> Opened;
        public event TypedEventHandler<FlyoutBase, object> Opening;

        #endregion

        #region AttachedFlyout Property

        public static readonly DependencyProperty AttachedFlyoutProperty = DependencyProperty.RegisterAttached("AttachedFlyout", typeof(FlyoutBase), typeof(FlyoutBase), new FrameworkPropertyMetadata(null));

        public static FlyoutBase GetAttachedFlyout(FrameworkElement ui)
        {
            if (ui == null)
                throw new ArgumentNullException("ui");

            return (FlyoutBase)ui.GetValue(AttachedFlyoutProperty);
        }

        public static void SetAttachedFlyout(FrameworkElement ui, FlyoutBase value)
        {
            ui.SetValue(AttachedFlyoutProperty, value);
        }

        #endregion

        #region IsOpen Property

        public static readonly DependencyPropertyKey IsOpenPropertyKey = DependencyProperty.RegisterReadOnly("IsOpen", typeof(bool), typeof(FlyoutBase), new FrameworkPropertyMetadata(false, OnIsOpenPropertyChanged));

        public static readonly DependencyProperty IsOpenProperty = IsOpenPropertyKey.DependencyProperty;

        public bool IsOpen
        {
            get
            {
                return (bool)GetValue(IsOpenProperty);
            }
            protected set
            {
                SetValue(IsOpenPropertyKey, value);
            }
        }

        private static void OnIsOpenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FlyoutBase flyoutBase = (FlyoutBase)d;

            flyoutBase.OnDependencyPropertyChanged(e);
        }

        #endregion

        #region Placement Property

        public static readonly DependencyProperty PlacementProperty = DependencyProperty.Register("Placement", typeof(FlyoutPlacementMode), typeof(FlyoutBase), new FrameworkPropertyMetadata(FlyoutPlacementMode.Bottom, OnPlacementPropertyChanged));

        public FlyoutPlacementMode Placement
        {
            get
            {
                return (FlyoutPlacementMode)GetValue(PlacementProperty);
            }
            set
            {
                SetValue(PlacementProperty, value);
            }
        }

        private static void OnPlacementPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FlyoutBase flyoutBase = (FlyoutBase)d;

            flyoutBase.OnDependencyPropertyChanged(e);
        }

        #endregion

        #region Target Property

        public static readonly DependencyPropertyKey TargetPropertyKey = DependencyProperty.RegisterReadOnly("Target", typeof(FrameworkElement), typeof(FlyoutBase), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty TargetProperty = TargetPropertyKey.DependencyProperty;

        public FrameworkElement Target
        {
            get
            {
                return (FrameworkElement)GetValue(TargetProperty);
            }
            private set
            {
                SetValue(TargetPropertyKey, value);
            }
        }

        #endregion

        #region AreOpenCloseAnimationsEnabled Property

        public static readonly DependencyProperty AreOpenCloseAnimationsEnabledProperty = DependencyProperty.Register("AreOpenCloseAnimationsEnabled", typeof(bool), typeof(FlyoutBase), new FrameworkPropertyMetadata(true, OnAreOpenCloseAnimationsEnabledPropertyChanged));

        public bool AreOpenCloseAnimationsEnabled
        {
            get
            {
                return (bool)GetValue(AreOpenCloseAnimationsEnabledProperty);
            }
            set
            {
                SetValue(AreOpenCloseAnimationsEnabledProperty, value);
            }
        }

        private static void OnAreOpenCloseAnimationsEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FlyoutBase flyoutBase = (FlyoutBase)d;

            flyoutBase.OnDependencyPropertyChanged(e);
        }

        #endregion

        #region ShouldConstrainToRootBounds Property

        public static readonly DependencyProperty ShouldConstrainToRootBoundsProperty = DependencyProperty.Register("ShouldConstrainToRootBounds", typeof(bool), typeof(FlyoutBase), new FrameworkPropertyMetadata(true, OnShouldConstrainToRootBoundsPropertyChanged));

        public bool ShouldConstrainToRootBounds
        {
            get
            {
                return (bool)GetValue(ShouldConstrainToRootBoundsProperty);
            }
            set
            {
                SetValue(ShouldConstrainToRootBoundsProperty, value);
            }
        }

        private static void OnShouldConstrainToRootBoundsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FlyoutBase flyoutBase = (FlyoutBase)d;

            flyoutBase.OnDependencyPropertyChanged(e);
        }

        #endregion

        #region CornerRadius Property

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(FlyoutBase), new FrameworkPropertyMetadata());

        public CornerRadius CornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(CornerRadiusProperty);
            }
            set
            {
                SetValue(CornerRadiusProperty, value);
            }
        }

        #endregion


        #region Methods

        public FlyoutBase()
        {
            ThemeManager.ApplicationThemeChanged += ThemeManagerApplicationThemeChanged;
        }

        ~FlyoutBase()
        {
            ThemeManager.ApplicationThemeChanged -= ThemeManagerApplicationThemeChanged;
        }

        protected virtual Control CreatePresenter()
        {
            throw new NotImplementedException();
        }

        public static void ShowAttachedFlyout(FrameworkElement element)
        {
            FlyoutBase flyoutBase = GetAttachedFlyout(element);

            if (flyoutBase == null)
            {
                return; // TODO ?
            }

            flyoutBase.ShowAt(element);
        }

        public void ShowAt(FrameworkElement placementTarget)
        {
            ShowAtCore(placementTarget);
        }

        public void ShowAt(FrameworkElement placementTarget, FlyoutShowOptions showOptions)
        {
            if (placementTarget is FrameworkElement target)
            {
                if (showOptions != null)
                {
                    Placement = showOptions.Placement;
                }

                ShowAtCore(target);
            }
        }

        public void Hide()
        {
            IsOpen = false;
        }

        /* Private Methods */

        private void ShowAtCore(FrameworkElement placementTarget)
        {
            if (placementTarget == null)
            {
                throw new ArgumentNullException("placementTarget");
            }

            if (IsOpen)
            {
                IsOpen = false;
            }

            Target = placementTarget;

            m_parentWindow = Window.GetWindow(placementTarget) ?? Application.Current.MainWindow;
            m_parentWindowBorder = GetWindowPadding();

            IsOpen = true;
        }

        private void CreateOpenAnimation()
        {
            m_openAnimationStoryboard = new Storyboard();

            DoubleAnimation openOpacityAnimation = new DoubleAnimation(0, 1, m_expandAnimationDuration);

            m_openAnimationStoryboard.Children.Add(openOpacityAnimation);

            if (m_presenter != null)
            {
                Storyboard.SetTargetProperty(openOpacityAnimation, new PropertyPath(Grid.OpacityProperty));
            }
        }

        private void CreateCloseAnimation()
        {
            m_closeAnimationStoryboard = new Storyboard();

            DoubleAnimation closeOpacityAnimation = new DoubleAnimation(1, 0, m_expandAnimationDuration);

            m_closeAnimationStoryboard.Children.Add(closeOpacityAnimation);

            if (m_presenter != null)
            {
                Storyboard.SetTargetProperty(closeOpacityAnimation, new PropertyPath(Grid.OpacityProperty));
            }
        }

        private void CreateNewPopup()
        {
            m_popup = new Popup();

            m_popup.Child = m_presenter;
            m_popup.AllowsTransparency = true;
            m_popup.StaysOpen = true;

            m_popup.Opened += OnPopupOpened;
            m_popup.Closed += OnPopupClosed;
            m_popup.PreviewMouseDown += OnPopupPreviewMouseDown;
        }


        /* Property Changed Methods */

        private void OnDependencyPropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            DependencyProperty property = args.Property;

            if (property == IsOpenProperty)
            {
                OnIsOpenChanged();
            }
            else if (property == PlacementProperty)
            {
                OnPlacementChanged();
            }
            else if (property == AreOpenCloseAnimationsEnabledProperty)
            {
                OnAreOpenCloseAnimationsEnabledChanged();
            }
            else if (property == ShouldConstrainToRootBoundsProperty)
            {
                OnShouldConstrainToRootBoundsChanged();
            }
        }

        private void OnIsOpenChanged()
        {
            if(IsOpen)
            {
                IsOpenChangedToOpen();
            }
            else
            {
                IsOpenChangedToClose();
            }
        }

        private void OnPlacementChanged()
        {
            SetOrClearPopupFullPlacement();

            PositionPopup();
        }

        private void OnAreOpenCloseAnimationsEnabledChanged()
        {

        }

        private void OnShouldConstrainToRootBoundsChanged()
        {

        }

        /* */

        private void IsOpenChangedToOpen()
        {
            if (m_presenter == null)
            {
                m_presenter = CreatePresenter();

                if (AreOpenCloseAnimationsEnabled)
                {
                    if (m_openAnimationStoryboard == null)
                    {
                        CreateOpenAnimation();
                    }

                    if (m_closeAnimationStoryboard == null)
                    {
                        CreateCloseAnimation();
                    }
                }
            }

            if(m_popup == null)
            {
                CreateNewPopup();
            }

            if(!m_popup.IsOpen)
            {
                Opening?.Invoke(this, EventArgs.Empty);
            }

            if(m_popup != null && !m_popup.IsOpen)
            {
                m_popup.IsOpen = true;
            }
        }

        private void IsOpenChangedToClose()
        {
            if(m_popup != null && m_popup.IsOpen)
            {
                RaiseClosingEvent();
            }
        }

        private void RaiseClosingEvent()
        {
            FlyoutBaseClosingEventArgs args = new FlyoutBaseClosingEventArgs();

            Closing?.Invoke(this, args);

            if(args.Cancel)
            {
                IsOpen = true;
            }
            else
            {
                StartContractToClose();
            }
        }

        private void StartOpenAnimation()
        {
            if (m_openAnimationStoryboard != null)
            {
                m_openAnimationStoryboard.Begin(m_presenter);
            }
        }

        private void StartContractToClose()
        {
            if (AreOpenCloseAnimationsEnabled && m_closeAnimationStoryboard != null)
            {
                m_closeAnimationStoryboard.Completed += OnStoryBoardCompleted;

                m_closeAnimationStoryboard.Begin(m_presenter);
            }
            else
            {
                ClosePopup();
            }
        }

        private void OnStoryBoardCompleted(object sender, EventArgs e)
        {
            m_closeAnimationStoryboard.Completed -= OnStoryBoardCompleted;

            ClosePopup();
        }

        private void ClosePopup()
        {
            if(m_popup != null)
            {
                m_popup.IsOpen = false;
            }
        }

        private void SetMouseCapture()
        {
            if(m_presenter != null)
            {
                Mouse.Capture(m_presenter, CaptureMode.SubTree);
            }
        }

        private bool ClickedOnContentArea(MouseEventArgs args)
        {
            if (m_presenter != null)
            {
                Point point = args.GetPosition(m_presenter);

                if ((point.X >= 0 && point.X < m_presenter.ActualWidth) && (point.Y >= 0 && point.Y < m_presenter.ActualHeight))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private void SetOrClearPopupFullPlacement()
        {
            if (m_presenter != null && Placement == FlyoutPlacementMode.Full)
            {
                Binding width = new Binding()
                {
                    Source = m_parentWindow,
                    Path = new PropertyPath(FrameworkElement.ActualWidthProperty)
                };

                Binding height = new Binding()
                {
                    Source = m_parentWindow,
                    Path = new PropertyPath(FrameworkElement.ActualHeightProperty)
                };

                m_presenter.SetBinding(FrameworkElement.WidthProperty, width);
                m_presenter.SetBinding(FrameworkElement.HeightProperty, height);
            }
            else if(m_presenter != null)
            {
                m_presenter.ClearValue(FrameworkElement.WidthProperty);
                m_presenter.ClearValue(FrameworkElement.HeightProperty);
            }
        }

        private void PositionPopup()
        {
            if(m_presenter == null)
            {
                return;
            }

            if (Target == null)
            {
                throw new Exception("Target is null");
            }

            m_currentTargetBoundsInCoreWindowSpace = Target.TransformToVisual(m_parentWindow).TransformBounds(new Rect(0, 0, Target.ActualWidth, Target.ActualHeight));

            double windowTop = (m_parentWindow.WindowState == WindowState.Normal) ? m_parentWindow.Top : 0 - m_parentWindowBorder.Top;
            double windowLeft = (m_parentWindow.WindowState == WindowState.Normal) ? m_parentWindow.Left : 0 - m_parentWindowBorder.Left;

            if(m_popup != null)
            {
                // TODO Get Alternative Placement

                double verticalOffset = windowTop;
                double horizontalOffset = windowLeft;

                switch (Placement)
                {
                    case FlyoutPlacementMode.Top:
                        {
                            verticalOffset += m_currentTargetBoundsInCoreWindowSpace.Y + -(m_presenter.ActualHeight + m_popupOffset.Top);
                            horizontalOffset += m_currentTargetBoundsInCoreWindowSpace.X - (m_presenter.ActualWidth - m_currentTargetBoundsInCoreWindowSpace.Width) / 2;
                            break;
                        }

                    case FlyoutPlacementMode.Bottom:
                        {
                            verticalOffset += m_currentTargetBoundsInCoreWindowSpace.Y + m_currentTargetBoundsInCoreWindowSpace.Height + m_popupOffset.Bottom;
                            horizontalOffset += m_currentTargetBoundsInCoreWindowSpace.X - (m_presenter.ActualWidth - m_currentTargetBoundsInCoreWindowSpace.Width) / 2;
                            break;
                        }

                    case FlyoutPlacementMode.Left:
                        {
                            verticalOffset += m_currentTargetBoundsInCoreWindowSpace.Y - (m_presenter.ActualHeight - m_currentTargetBoundsInCoreWindowSpace.Height) / 2;
                            horizontalOffset += m_currentTargetBoundsInCoreWindowSpace.X - m_presenter.ActualWidth - m_popupOffset.Left;
                            break;
                        }

                    case FlyoutPlacementMode.Right:
                        {
                            verticalOffset += m_currentTargetBoundsInCoreWindowSpace.Y - (m_presenter.ActualHeight - m_currentTargetBoundsInCoreWindowSpace.Height) / 2;
                            horizontalOffset += m_currentTargetBoundsInCoreWindowSpace.X + m_currentTargetBoundsInCoreWindowSpace.Width + m_popupOffset.Right;
                            break;
                        }

                    case FlyoutPlacementMode.TopEdgeAlignedRight:
                        {
                            verticalOffset += m_currentTargetBoundsInCoreWindowSpace.Y + -(m_presenter.ActualHeight + m_popupOffset.Top);
                            horizontalOffset += m_currentTargetBoundsInCoreWindowSpace.X - m_presenter.ActualWidth + m_currentTargetBoundsInCoreWindowSpace.Width;
                            break;
                        }

                    case FlyoutPlacementMode.TopEdgeAlignedLeft:
                        {
                            verticalOffset += m_currentTargetBoundsInCoreWindowSpace.Y + -(m_presenter.ActualHeight + m_popupOffset.Top);
                            horizontalOffset += m_currentTargetBoundsInCoreWindowSpace.X;
                            break;
                        }

                    case FlyoutPlacementMode.BottomEdgeAlignedRight:
                        {
                            verticalOffset += m_currentTargetBoundsInCoreWindowSpace.Y + m_currentTargetBoundsInCoreWindowSpace.Height + m_popupOffset.Bottom;
                            horizontalOffset += m_currentTargetBoundsInCoreWindowSpace.X - m_presenter.ActualWidth + m_currentTargetBoundsInCoreWindowSpace.Width;
                            break;
                        }

                    case FlyoutPlacementMode.BottomEdgeAlignedLeft:
                        {
                            verticalOffset += m_currentTargetBoundsInCoreWindowSpace.Y + m_currentTargetBoundsInCoreWindowSpace.Height + m_popupOffset.Bottom;
                            horizontalOffset += m_currentTargetBoundsInCoreWindowSpace.X;
                            break;
                        }

                    case FlyoutPlacementMode.LeftEdgeAlignedTop:
                        {
                            verticalOffset += m_currentTargetBoundsInCoreWindowSpace.Y;
                            horizontalOffset += m_currentTargetBoundsInCoreWindowSpace.X - m_presenter.ActualWidth - m_popupOffset.Left;
                            break;
                        }

                    case FlyoutPlacementMode.LeftEdgeAlignedBottom:
                        {
                            verticalOffset += m_currentTargetBoundsInCoreWindowSpace.Y - m_presenter.ActualHeight + m_currentTargetBoundsInCoreWindowSpace.Height;
                            horizontalOffset += m_currentTargetBoundsInCoreWindowSpace.X - m_presenter.ActualWidth - m_popupOffset.Left;
                            break;
                        }

                    case FlyoutPlacementMode.RightEdgeAlignedTop:
                        {
                            verticalOffset += m_currentTargetBoundsInCoreWindowSpace.Y;
                            horizontalOffset += m_currentTargetBoundsInCoreWindowSpace.X + m_currentTargetBoundsInCoreWindowSpace.Width + m_popupOffset.Right;
                            break;
                        }

                    case FlyoutPlacementMode.RightEdgeAlignedBottom:
                        {
                            verticalOffset += m_currentTargetBoundsInCoreWindowSpace.Y - m_presenter.ActualHeight + m_currentTargetBoundsInCoreWindowSpace.Height;
                            horizontalOffset += m_currentTargetBoundsInCoreWindowSpace.X + m_currentTargetBoundsInCoreWindowSpace.Width + m_popupOffset.Right;
                            break;
                        }

                    case FlyoutPlacementMode.Full:
                        {
                            verticalOffset += (m_parentWindow.ActualHeight / 2) - (m_presenter.ActualHeight / 2);
                            horizontalOffset += (m_parentWindow.ActualWidth / 2) - (m_presenter.ActualWidth / 2);
                            break;
                        }

                    default:
                        {
                            break;
                        }
                }

                m_popup.VerticalOffset = verticalOffset;
                m_popup.HorizontalOffset = horizontalOffset;
            }
        }

        /* */

        private Thickness GetWindowPadding()
        {
            int borderPadding = NativeMethods.GetSystemMetrics(NativeMethods.SM_CXPADDEDBORDER);
            Thickness curr = SystemParameters.WindowResizeBorderThickness;
            return new Thickness(curr.Left + borderPadding, curr.Top + borderPadding, curr.Right + borderPadding, curr.Top + borderPadding);
        }

        /* Event Handlers */

        private void ThemeManagerApplicationThemeChanged(object sender, ApplicationThemeChangedEventArgs e)
        {
            if (m_presenter != null)
            {
                ElementTheme theme = (ThemeManager.ApplicationTheme == ApplicationTheme.Dark) ? ElementTheme.Dark : ElementTheme.Light;

                m_presenter.SetValue(ThemeManager.ElementThemeProperty, theme);
            }
        }

        private void OnPopupOpened(object sender, EventArgs e)
        {
            SetOrClearPopupFullPlacement();

            PositionPopup();

            if (m_presenter != null)
            {
                m_presenter.PreviewMouseDown += OnContentPresenterMouseDown;
                m_presenter.MouseLeave += OnContentPresenterMouseLeave;
                m_presenter.LostMouseCapture += OnContentPresenterLostMouseCapture;
            }

            if (m_parentWindow != null)
            {
                m_parentWindow.SizeChanged += OnWindowSizeChanged;
                m_parentWindow.LocationChanged += OnWindowLocationChanged;
                m_parentWindow.Deactivated += OnWindowDeactivated;
            }

            if (Target != null)
            {
                Target.LayoutUpdated += OnTargetLayoutUpdated;
            }

            if (AreOpenCloseAnimationsEnabled)
            {
                StartOpenAnimation();
            }

            SetMouseCapture();

            Opened?.Invoke(this, EventArgs.Empty);
        }

        private void OnPopupClosed(object sender, EventArgs e)
        {
            if (m_presenter != null)
            {
                m_presenter.PreviewMouseDown -= OnContentPresenterMouseDown;
                m_presenter.MouseLeave -= OnContentPresenterMouseLeave;
                m_presenter.LostMouseCapture -= OnContentPresenterLostMouseCapture;
            }

            if (m_parentWindow != null)
            {
                m_parentWindow.SizeChanged -= OnWindowSizeChanged;
                m_parentWindow.LocationChanged -= OnWindowLocationChanged;
                m_parentWindow.Deactivated -= OnWindowDeactivated;
            }

            if (Target != null)
            {
                Target.LayoutUpdated -= OnTargetLayoutUpdated;
            }

            Closed?.Invoke(this, EventArgs.Empty);
        }

        private void OnPopupPreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!m_popup.IsOpen)
            {
                e.Handled = true;
            }
        }

        private void OnContentPresenterLostMouseCapture(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!ClickedOnContentArea(e))
            {
                IsOpen = false;
            }
        }

        private void OnContentPresenterMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (Mouse.Captured != m_presenter)
            {
                SetMouseCapture();
            }
        }

        private void OnContentPresenterMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!ClickedOnContentArea(e))
            {
                IsOpen = false;
            }
        }

        private void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            PositionPopup();
        }

        private void OnWindowLocationChanged(object sender, EventArgs e)
        {
            PositionPopup();
        }

        private void OnWindowDeactivated(object sender, EventArgs e)
        {
            if (IsOpen)
            {
                IsOpen = false;
            }
        }

        private void OnTargetLayoutUpdated(object sender, EventArgs e)
        {
            PositionPopup();
        }

        #endregion
    }
}
