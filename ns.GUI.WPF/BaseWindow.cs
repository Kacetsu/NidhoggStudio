using ns.Base.Log;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ns.GUI.WPF {
    public class BaseWindow : System.Windows.Window {
        private const Int32 WM_SYSCOMMAND = 0x112;
        private HwndSource m_hwndSource;
        private DateTime _lastTitlebarClick;
        private TimeSpan _doubleClick = TimeSpan.FromMilliseconds(500);

        private Window _glowWindowTop;
        private Window _glowWinowLeft;
        private Window _glowWindowBottom;
        private Window _glowWindowRight;
        private const Int32 _edgeWindowSize = 23;

        private enum ResizeDirection {
            Left = 1,
            Right = 2,
            Top = 3,
            TopLeft = 4,
            TopRight = 5,
            Bottom = 6,
            BottomLeft = 7,
            BottomRight = 8
        }

        public static readonly DependencyProperty MaximizedProperty = DependencyProperty.Register("Maximized",
            typeof(bool),
            typeof(Window),
            new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty ShowMinimizeButtonProperty = DependencyProperty.Register("ShowMinimizeButton",
            typeof(bool),
            typeof(Window),
            new FrameworkPropertyMetadata(true));

        public static readonly DependencyProperty ShowMaximizeButtonProperty = DependencyProperty.Register("ShowMaximizeButton",
            typeof(bool),
            typeof(Window),
            new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Gets or sets if the Window is maximized.
        /// </summary>
        public bool Maximized {
            get { return (bool)GetValue(MaximizedProperty); }
            set { SetValue(MaximizedProperty, value); }
        }

        public bool ShowMinimizeButton {
            get { return (bool)GetValue(ShowMinimizeButtonProperty); }
            set { SetValue(ShowMinimizeButtonProperty, value); }
        }

        public bool ShowMaximizeButton {
            get { return (bool)GetValue(ShowMaximizeButtonProperty); }
            set { SetValue(ShowMaximizeButtonProperty, value); }
        }

        private void HandleLoaded(object sender, RoutedEventArgs e) {
            try {
                Button closeButton = (Button)this.Template.FindName("PART_CLOSE", this);
                Button maximizeRestoreButton = (Button)this.Template.FindName("PART_MAXIMIZE_RESTORE", this);
                Button minimizeButton = (Button)this.Template.FindName("PART_MINIMIZE", this);
                DockPanel titlebarDockPanel = (DockPanel)this.Template.FindName("Titlebar", this);
                Rectangle leftBorderRectangle = (Rectangle)this.Template.FindName("PART_LEFT_BORDER", this);
                Rectangle rightBorderRectangle = (Rectangle)this.Template.FindName("PART_RIGHT_BORDER", this);
                Rectangle topBorderRectangle = (Rectangle)this.Template.FindName("PART_TOP_BORDER", this);
                Rectangle bottomBorderRectangle = (Rectangle)this.Template.FindName("PART_BOTTOM_BORDER", this);
                closeButton.PreviewMouseLeftButtonDown += closeButton_PreviewMouseLeftButtonDown;
                maximizeRestoreButton.PreviewMouseLeftButtonDown += maximizeRestoreButton_PreviewMouseLeftButtonDown;
                minimizeButton.PreviewMouseLeftButtonDown += minimizeButton_PreviewMouseLeftButtonDown;
                titlebarDockPanel.PreviewMouseLeftButtonDown += titlebarDockPanel_PreviewMouseLeftButtonDown;
                leftBorderRectangle.PreviewMouseLeftButtonDown += leftBorderRectangle_PreviewMouseLeftButtonDown;
                rightBorderRectangle.PreviewMouseLeftButtonDown += rightBorderRectangle_PreviewMouseLeftButtonDown;
                topBorderRectangle.PreviewMouseLeftButtonDown += topBorderRectangle_PreviewMouseLeftButtonDown;
                bottomBorderRectangle.PreviewMouseLeftButtonDown += bottomBorderRectangle_PreviewMouseLeftButtonDown;
            } catch (Exception ex) {
                ns.Base.Log.Trace.WriteLine(ex.Message, ex.StackTrace, LogCategory.Error);
            }
        }

        private void minimizeButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        private void maximizeRestoreButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (this.WindowState == System.Windows.WindowState.Maximized) {
                this.WindowState = System.Windows.WindowState.Normal;
                Maximized = false;
            } else {
                this.WindowState = System.Windows.WindowState.Maximized;
                Maximized = true;
            }
        }

        private void closeButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            this.Close();
            base.OnClosed(e);
        }

        private void ResizeWindow(ResizeDirection direction) {
            m_hwndSource = (HwndSource)PresentationSource.FromVisual(this);
            NativeMethods.SendMessage(m_hwndSource.Handle, WM_SYSCOMMAND,
                (IntPtr)(61440 + direction), IntPtr.Zero);
        }

        private void bottomBorderRectangle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            ResizeWindow(ResizeDirection.Bottom);
        }

        private void topBorderRectangle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            ResizeWindow(ResizeDirection.Top);
        }

        private void rightBorderRectangle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            ResizeWindow(ResizeDirection.Right);
        }

        private void leftBorderRectangle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            ResizeWindow(ResizeDirection.Left);
        }

        private void titlebarDockPanel_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (DateTime.Now.Subtract(_lastTitlebarClick) <= _doubleClick) {
                if (Maximized == true) {
                    this.WindowState = System.Windows.WindowState.Normal;
                    Maximized = false;
                } else {
                    this.WindowState = System.Windows.WindowState.Maximized;
                    Maximized = true;
                }

            } else {
                DragMove();
            }

            _lastTitlebarClick = DateTime.Now;
        }

        protected override void OnInitialized(EventArgs e) {
            AllowsTransparency = true;
            //Height = 480;
            //Width = 852;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            WindowStyle = WindowStyle.None;

            Loaded += HandleLoaded;
            LocationChanged += HandleLocationChanged;
            SizeChanged += HandleLocationChanged;
            StateChanged += HandleWndStateChanged;

            GotKeyboardFocus += HandleGotKeyboardFocus;
            LostKeyboardFocus += HandleLostKeyboardFocus;
            Closing += HandleClosing;

            InitializeSurrounds();
            ShowSurrounds();

            base.OnInitialized(e);
        }

        private void HandleClosing(object sender, System.ComponentModel.CancelEventArgs e) {
            CloseSurrounds();
        }

        public void HandleGotKeyboardFocus(Object sender, KeyboardFocusChangedEventArgs e) {
            SetSurroundShadows(true);
        }

        public void HandleLostKeyboardFocus(Object sender, KeyboardFocusChangedEventArgs e) {
            SetSurroundShadows(false);
        }

        private void HandleLocationChanged(Object sender, EventArgs e) {
            _glowWindowTop.Left = Left - _edgeWindowSize;
            _glowWindowTop.Top = Top - _glowWindowTop.Height;
            _glowWindowTop.Width = Width + _edgeWindowSize * 2;
            _glowWindowTop.Height = _edgeWindowSize;

            _glowWinowLeft.Left = Left - _glowWinowLeft.Width;
            _glowWinowLeft.Top = Top;
            _glowWinowLeft.Width = _edgeWindowSize;
            _glowWinowLeft.Height = Height;

            _glowWindowBottom.Left = Left - _edgeWindowSize;
            _glowWindowBottom.Top = Top + Height;
            _glowWindowBottom.Width = Width + _edgeWindowSize * 2;
            _glowWindowBottom.Height = _edgeWindowSize;

            _glowWindowRight.Left = Left + Width;
            _glowWindowRight.Top = Top;
            _glowWindowRight.Width = _edgeWindowSize;
            _glowWindowRight.Height = Height;
        }

        private void HandleWndStateChanged(Object sender, EventArgs e) {
            if (WindowState == WindowState.Normal) {
                ShowSurrounds();
            } else {
                HideSurrounds();
            }
        }

        private static Window CreateTransparentWindow() {
            Window wnd = new Window();
            wnd.AllowsTransparency = true;
            wnd.ShowInTaskbar = false;
            wnd.WindowStyle = WindowStyle.None;
            wnd.Background = null;

            return wnd;
        }

        private void InitializeSurrounds() {
            // Top.
            _glowWindowTop = CreateTransparentWindow();

            // Left.
            _glowWinowLeft = CreateTransparentWindow();

            // Bottom.
            _glowWindowBottom = CreateTransparentWindow();

            // Right.
            _glowWindowRight = CreateTransparentWindow();

            SetSurroundShadows();
        }


        private void SetSurroundShadows(Boolean active = true) {
            if (active) {
                Double cornerRadius = 0.75;

                _glowWindowTop.Content = GetDecorator("Images/window_active_shadow_top.PNG");
                _glowWinowLeft.Content = GetDecorator("Images/window_active_shadow_left.PNG", cornerRadius);
                _glowWindowBottom.Content = GetDecorator("Images/window_active_shadow_bottom.PNG");
                _glowWindowRight.Content = GetDecorator("Images/window_active_shadow_right.PNG", cornerRadius);
            } else {
                _glowWindowTop.Content = GetDecorator("Images/window_inactive_shadow_top.PNG");
                _glowWinowLeft.Content = GetDecorator("Images/window_inactive_shadow_left.PNG");
                _glowWindowBottom.Content = GetDecorator("Images/window_inactive_shadow_bottom.PNG");
                _glowWindowRight.Content = GetDecorator("Images/window_inactive_shadow_right.PNG");
            }
        }

        [DebuggerStepThrough]
        private Decorator GetDecorator(String imageUri, Double radius = 0) {
            Border border = new Border();
            border.CornerRadius = new CornerRadius(radius);
            Uri baseUri = BaseUriHelper.GetBaseUri(this);
            Uri uri = new Uri("pack://application:,,,/ns.GUI.WPF;component/");
            border.Background = new ImageBrush(
                new BitmapImage(
                    new Uri(uri,
                        imageUri)));

            return border;
        }

        /// <summary>
        /// Shows the surrounding windows.
        /// </summary>
        private void ShowSurrounds() {
            _glowWindowTop.Show();
            _glowWinowLeft.Show();
            _glowWindowBottom.Show();
            _glowWindowRight.Show();
        }

        /// <summary>
        /// Hides the surrounding windows.
        /// </summary>
        private void HideSurrounds() {
            _glowWindowTop.Hide();
            _glowWinowLeft.Hide();
            _glowWindowBottom.Hide();
            _glowWindowRight.Hide();
        }
        /// <summary>
        /// Closes the surrounding windows.
        /// </summary>
        private void CloseSurrounds() {
            _glowWindowTop.Close();
            _glowWinowLeft.Close();
            _glowWindowBottom.Close();
            _glowWindowRight.Close();
        }

    }
}
