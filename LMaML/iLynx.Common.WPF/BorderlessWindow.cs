using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Screen = System.Windows.Forms.Screen;

namespace iLynx.Common.WPF
{
    /// <summary>
    /// DockingWindow
    /// </summary>
    public class BorderlessWindow : Window
    {
        /// <summary>
        /// The header size property
        /// </summary>
        public static readonly DependencyProperty HeaderSizeProperty =
            DependencyProperty.Register("HeaderSize", typeof(GridLength), typeof(BorderlessWindow), new PropertyMetadata(default(GridLength)));

        /// <summary>
        /// Gets or sets the size of the header.
        /// </summary>
        /// <value>
        /// The size of the header.
        /// </value>
        public GridLength HeaderSize
        {
            get { return (GridLength)GetValue(HeaderSizeProperty); }
            set { SetValue(HeaderSizeProperty, value); }
        }

        /// <summary>
        /// The header property
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(BorderlessWindow), new PropertyMetadata(default(object)));

        /// <summary>
        /// The title font size property
        /// </summary>
        public static readonly DependencyProperty TitleFontSizeProperty =
            DependencyProperty.Register("TitleFontSize", typeof(double), typeof(BorderlessWindow), new PropertyMetadata(default(double)));

        /// <summary>
        /// The title font weight property
        /// </summary>
        public static readonly DependencyProperty TitleFontWeightProperty =
            DependencyProperty.Register("TitleFontWeight", typeof(FontWeight), typeof(BorderlessWindow), new PropertyMetadata(default(FontWeight)));

        /// <summary>
        /// The title font style property
        /// </summary>
        public static readonly DependencyProperty TitleFontStyleProperty =
            DependencyProperty.Register("TitleFontStyle", typeof(FontStyle), typeof(BorderlessWindow), new PropertyMetadata(default(FontStyle)));

        /// <summary>
        /// The title font family property
        /// </summary>
        public static readonly DependencyProperty TitleFontFamilyProperty =
            DependencyProperty.Register("TitleFontFamily", typeof(FontFamily), typeof(BorderlessWindow), new PropertyMetadata(default(FontFamily)));

        /// <summary>
        /// The title font stretch property
        /// </summary>
        public static readonly DependencyProperty TitleFontStretchProperty =
            DependencyProperty.Register("TitleFontStretch", typeof(FontStretch), typeof(BorderlessWindow), new PropertyMetadata(default(FontStretch)));

        /// <summary>
        /// The is collapsible property
        /// </summary>
        public static readonly DependencyProperty IsCollapsibleProperty =
            DependencyProperty.Register("IsCollapsible", typeof(bool), typeof(BorderlessWindow), new PropertyMetadata(default(bool)));

        /// <summary>
        /// The toggle collapsed command property
        /// </summary>
        public static readonly DependencyProperty ToggleCollapsedCommandProperty =
            DependencyProperty.Register("ToggleCollapsedCommand", typeof(ICommand), typeof(BorderlessWindow), new PropertyMetadata(new DelegateCommand<BorderlessWindow>(OnToggleCollapsed)));

        private double storedHeight;
        private double storedWidth;

        /// <summary>
        /// Called when [toggle collapsed].
        /// </summary>
        /// <param name="window">The window.</param>
        private static void OnToggleCollapsed(BorderlessWindow window)
        {
            window.IsCollapsed = !window.IsCollapsed;
            if (window.IsCollapsed)
            {
                window.storedHeight = window.Height;
                window.storedWidth = window.Width;
                window.Height = window.CollapsedHeight;
                window.Width = window.CollapsedWidth;
                window.OnCollapsed();
            }
            else
            {
                window.Height = window.storedHeight;
                window.Width = window.storedWidth;
                window.OnExpanded();
            }
        }

        /// <summary>
        /// The expanded event
        /// </summary>
        public static readonly RoutedEvent ExpandedEvent = EventManager.RegisterRoutedEvent("Expanded",
                                                                                            RoutingStrategy.Direct,
                                                                                            typeof(RoutedEventHandler),
                                                                                            typeof(BorderlessWindow));

        /// <summary>
        /// The collapsed event
        /// </summary>
        public static readonly RoutedEvent CollapsedEvent = EventManager.RegisterRoutedEvent("Collapsed",
                                                                                             RoutingStrategy.Direct,
                                                                                             typeof(RoutedEventHandler),
                                                                                             typeof(BorderlessWindow));

        /// <summary>
        /// Occurs when [expanded].
        /// </summary>
        public event RoutedEventHandler Expanded
        {
            add { AddHandler(ExpandedEvent, value); }
            remove { RemoveHandler(ExpandedEvent, value); }
        }


        /// <summary>
        /// Occurs when [collapsed].
        /// </summary>
        public event RoutedEventHandler Collapsed
        {
            add { AddHandler(CollapsedEvent, value); }
            remove { RemoveHandler(CollapsedEvent, value); }
        }

        /// <summary>
        /// Called when [expanded].
        /// </summary>
        private void OnExpanded()
        {
            RaiseEvent(new RoutedEventArgs(ExpandedEvent));
        }

        /// <summary>
        /// Called when [collapsed].
        /// </summary>
        private void OnCollapsed()
        {
            RaiseEvent(new RoutedEventArgs(CollapsedEvent));
        }

        /// <summary>
        /// The is collapsed property
        /// </summary>
        public static readonly DependencyProperty IsCollapsedProperty =
            DependencyProperty.Register("IsCollapsed", typeof(bool), typeof(BorderlessWindow), new PropertyMetadata(default(bool)));

        /// <summary>
        /// The collapsed header property
        /// </summary>
        public static readonly DependencyProperty CollapsedHeaderProperty =
            DependencyProperty.Register("CollapsedHeader", typeof(object), typeof(BorderlessWindow), new PropertyMetadata(default(object)));

        /// <summary>
        /// The collapsed height property
        /// </summary>
        public static readonly DependencyProperty CollapsedHeightProperty =
            DependencyProperty.Register("CollapsedHeight", typeof(double), typeof(BorderlessWindow), new PropertyMetadata(64d));

        /// <summary>
        /// The collapsed width property
        /// </summary>
        public static readonly DependencyProperty CollapsedWidthProperty =
            DependencyProperty.Register("CollapsedWidth", typeof(double), typeof(BorderlessWindow), new PropertyMetadata(200d));

        /// <summary>
        /// Gets or sets the width of the collapsed.
        /// </summary>
        /// <value>
        /// The width of the collapsed.
        /// </value>
        public double CollapsedWidth
        {
            get { return (double)GetValue(CollapsedWidthProperty); }
            set { SetValue(CollapsedWidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the height of the collapsed.
        /// </summary>
        /// <value>
        /// The height of the collapsed.
        /// </value>
        public double CollapsedHeight
        {
            get { return (double)GetValue(CollapsedHeightProperty); }
            set { SetValue(CollapsedHeightProperty, value); }
        }

        /// <summary>
        /// Gets or sets the collapsed header.
        /// </summary>
        /// <value>
        /// The collapsed header.
        /// </value>
        public object CollapsedHeader
        {
            get { return GetValue(CollapsedHeaderProperty); }
            set { SetValue(CollapsedHeaderProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is collapsed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is collapsed; otherwise, <c>false</c>.
        /// </value>
        public bool IsCollapsed
        {
            get { return (bool)GetValue(IsCollapsedProperty); }
            set { SetValue(IsCollapsedProperty, value); }
        }

        /// <summary>
        /// Gets or sets the toggle collapsed command.
        /// </summary>
        /// <value>
        /// The toggle collapsed command.
        /// </value>
        public ICommand ToggleCollapsedCommand
        {
            get { return (ICommand)GetValue(ToggleCollapsedCommandProperty); }
            set { SetValue(ToggleCollapsedCommandProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is collapsible.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is collapsible; otherwise, <c>false</c>.
        /// </value>
        public bool IsCollapsible
        {
            get { return (bool)GetValue(IsCollapsibleProperty); }
            set { SetValue(IsCollapsibleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the title font stretch.
        /// </summary>
        /// <value>
        /// The title font stretch.
        /// </value>
        public FontStretch TitleFontStretch
        {
            get { return (FontStretch)GetValue(TitleFontStretchProperty); }
            set { SetValue(TitleFontStretchProperty, value); }
        }

        /// <summary>
        /// Gets or sets the title font family.
        /// </summary>
        /// <value>
        /// The title font family.
        /// </value>
        public FontFamily TitleFontFamily
        {
            get { return (FontFamily)GetValue(TitleFontFamilyProperty); }
            set { SetValue(TitleFontFamilyProperty, value); }
        }

        /// <summary>
        /// Gets or sets the title font style.
        /// </summary>
        /// <value>
        /// The title font style.
        /// </value>
        public FontStyle TitleFontStyle
        {
            get { return (FontStyle)GetValue(TitleFontStyleProperty); }
            set { SetValue(TitleFontStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the title font weight.
        /// </summary>
        /// <value>
        /// The title font weight.
        /// </value>
        public FontWeight TitleFontWeight
        {
            get { return (FontWeight)GetValue(TitleFontWeightProperty); }
            set { SetValue(TitleFontWeightProperty, value); }
        }

        /// <summary>
        /// Gets or sets the size of the title font.
        /// </summary>
        /// <value>
        /// The size of the title font.
        /// </value>
        public double TitleFontSize
        {
            get { return (double)GetValue(TitleFontSizeProperty); }
            set { SetValue(TitleFontSizeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>
        /// The header.
        /// </value>
        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// The border size property
        /// </summary>
        public static readonly DependencyProperty BorderSizeProperty =
            DependencyProperty.Register("BorderSize", typeof(GridLength), typeof(BorderlessWindow), new PropertyMetadata(default(GridLength)));

        /// <summary>
        /// Gets or sets the size of the border.
        /// </summary>
        /// <value>
        /// The size of the border.
        /// </value>
        public GridLength BorderSize
        {
            get { return (GridLength)GetValue(BorderSizeProperty); }
            set { SetValue(BorderSizeProperty, value); }
        }

        /// <summary>
        /// The can maximize property
        /// </summary>
        public static readonly DependencyProperty CanMaximizeProperty =
            DependencyProperty.Register("CanMaximize", typeof(bool), typeof(BorderlessWindow), new PropertyMetadata(default(bool)));

        /// <summary>
        /// The close command property
        /// </summary>
        public static readonly DependencyPropertyKey CloseCommandPropertyKey =
            DependencyProperty.RegisterReadOnly("CloseCommand", typeof(ICommand), typeof(BorderlessWindow), new PropertyMetadata(new DelegateCommand<BorderlessWindow>(OnCloseCommand)));

        /// <summary>
        /// The close command property
        /// </summary>
        public static readonly DependencyProperty CloseCommandProperty = CloseCommandPropertyKey.DependencyProperty;

        /// <summary>
        /// The minimize command property key
        /// </summary>
        public static readonly DependencyPropertyKey MinimizeCommandPropertyKey =
            DependencyProperty.RegisterReadOnly("MinimizeCommand", typeof(ICommand), typeof(BorderlessWindow), new PropertyMetadata(new DelegateCommand<BorderlessWindow>(OnMinimizeCommand)));

        /// <summary>
        /// The minimize command property
        /// </summary>
        public static readonly DependencyProperty MinimizeCommandProperty =
            MinimizeCommandPropertyKey.DependencyProperty;

        /// <summary>
        /// The toggle maximized command property key
        /// </summary>
        public static readonly DependencyPropertyKey ToggleMaximizedCommandPropertyKey =
            DependencyProperty.RegisterReadOnly("ToggleMaximizedCommand", typeof(ICommand), typeof(BorderlessWindow), new PropertyMetadata(new DelegateCommand<BorderlessWindow>(OnToggleMaximized)));

        /// <summary>
        /// The toggle maximized command property
        /// </summary>
        public static readonly DependencyProperty ToggleMaximizedCommandProperty =
            ToggleMaximizedCommandPropertyKey.DependencyProperty;

        /// <summary>
        /// The header border thickness property
        /// </summary>
        public static readonly DependencyProperty HeaderBorderThicknessProperty =
            DependencyProperty.Register("HeaderBorderThickness", typeof(Thickness), typeof(BorderlessWindow), new PropertyMetadata(default(Thickness)));

        /// <summary>
        /// Gets or sets the header border thickness.
        /// </summary>
        /// <value>
        /// The header border thickness.
        /// </value>
        public Thickness HeaderBorderThickness
        {
            get { return (Thickness)GetValue(HeaderBorderThicknessProperty); }
            set { SetValue(HeaderBorderThicknessProperty, value); }
        }

        private static readonly Dictionary<ResizeDirection, Cursor> ResizeCursors =
            new Dictionary<ResizeDirection, Cursor>
            {
                { ResizeDirection.Top, Cursors.SizeNS},
                { ResizeDirection.Bottom, Cursors.SizeNS},
                { ResizeDirection.Left, Cursors.SizeWE},
                { ResizeDirection.Right, Cursors.SizeWE},
                { ResizeDirection.TopLeft, Cursors.SizeNWSE},
                { ResizeDirection.TopRight, Cursors.SizeNESW},
                { ResizeDirection.BottomLeft, Cursors.SizeNESW},
                { ResizeDirection.BottomRight, Cursors.SizeNWSE},
            };

        private bool isMaximized;
        private Rect previousBounds;
        private Button maximizeButton;
        private Rectangle headerRect;
        private Border mainBorder;
        private ResizeDirection resizeDirection;
        private Rect startBounds;
        private Point startPoint;

        /// <summary>
        /// Gets the header rectangle.
        /// </summary>
        /// <value>
        /// The header rectangle.
        /// </value>
        protected Rectangle HeaderRectangle { get { return headerRect; } }

        /// <summary>
        /// Called when [toggle maximized].
        /// </summary>
        /// <param name="borderlessWindow">The borderless window.</param>
        private static void OnToggleMaximized(BorderlessWindow borderlessWindow)
        {
            if (borderlessWindow.isMaximized)
                borderlessWindow.Restore();
            else
                borderlessWindow.Maximize();
        }

        private void Restore()
        {
            Left = previousBounds.Left;
            Top = previousBounds.Top;
            Width = previousBounds.Width;
            Height = previousBounds.Height;
            if (null != maximizeButton)
                maximizeButton.Content = "1";
            isMaximized = false;
        }

        private void Maximize()
        {
            var area = Screen.GetWorkingArea(new System.Drawing.Rectangle((int)Left, (int)Top, (int)ActualWidth, (int)ActualHeight));
            previousBounds = new Rect(Left, Top, ActualWidth, ActualHeight);
            Left = area.Left;
            Top = area.Top;
            Width = area.Width;
            Height = area.Height;
            if (null != maximizeButton)
                maximizeButton.Content = "2";
            isMaximized = true;
        }

        /// <summary>
        /// Called when [minimize command].
        /// </summary>
        /// <param name="borderlessWindow">The borderless window.</param>
        private static void OnMinimizeCommand(BorderlessWindow borderlessWindow)
        {
            borderlessWindow.WindowState = WindowState.Minimized;
        }

        private FrameworkElement contentHost;

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            contentHost = Template.FindName("PART_Content", this) as FrameworkElement;
            maximizeButton = Template.FindName("maximizeButton", this) as Button;
            if (null != maximizeButton)
                maximizeButton.Content = "1";

            mainBorder = Template.FindName("PART_Main", this) as Border;
            if (null != mainBorder)
            {
                mainBorder.PreviewMouseMove += MainBorderOnPreviewMouseMove;
                mainBorder.MouseDown += MainBorderOnMouseDown;
                mainBorder.MouseUp += MainBorderOnMouseUp;
            }

            headerRect = Template.FindName("DragGrip", this) as Rectangle;
            if (null == headerRect) return;
            headerRect.PreviewMouseDown += HeaderRectOnPreviewMouseDown;
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);
            if (lastState != WindowState.Maximized) return;
            Restore();
            WindowState = WindowState.Normal;
            lastState = WindowState.Normal;
        }

        private bool isMoving;

        private void HeaderRectOnPreviewMouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            isMoving = true;
            DragMove();
        }

        private WindowState lastState;

        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);
            if (WindowState == WindowState.Maximized && lastState != WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                Maximize();
                lastState = WindowState;
                return;
            }
            if (WindowState != WindowState.Normal || lastState == WindowState.Normal) return;
            Restore();
            lastState = WindowState;
        }

        private ResizeDirection GetDirection(Point p)
        {
            var borderThickness = mainBorder.BorderThickness;
            var result = ResizeDirection.None;
            if (p.X >= 0 && p.X <= borderThickness.Left)
            {
                if (p.Y >= 0 && p.Y <= borderThickness.Top)
                    result = ResizeDirection.TopLeft;
                else if (p.Y >= borderThickness.Top && p.Y <= mainBorder.ActualHeight - borderThickness.Bottom)
                    result = ResizeDirection.Left;
                else
                    result = ResizeDirection.BottomLeft;
            }
            else if (p.X >= borderThickness.Left && p.X <= mainBorder.ActualWidth - borderThickness.Right)
            {
                if (p.Y >= 0 && p.Y <= borderThickness.Top)
                    result = ResizeDirection.Top;
                else if (p.Y >= mainBorder.ActualHeight - borderThickness.Bottom && p.Y <= mainBorder.ActualHeight)
                    result = ResizeDirection.Bottom;
            }
            else if (p.X >= mainBorder.ActualWidth - borderThickness.Right && p.X <= mainBorder.ActualWidth)
            {
                if (p.Y >= 0 && p.Y <= borderThickness.Top)
                    result = ResizeDirection.TopRight;
                else if (p.Y >= borderThickness.Top && p.Y <= mainBorder.ActualHeight - borderThickness.Bottom)
                    result = ResizeDirection.Right;
                else
                    result = ResizeDirection.BottomRight;
            }
            return result;
        }

        private ResizeDirection previousDirection;

        private void SetCursor(ResizeDirection direction)
        {
            if (ResizeDirection.None == direction)
            {
                Mouse.OverrideCursor = previousOverrideCursor;
                return;
            }
            Cursor cursor;
            if (!ResizeCursors.TryGetValue(direction, out cursor)) return;
            if (cursor == Mouse.OverrideCursor) return;
            previousOverrideCursor = Mouse.OverrideCursor;
            Mouse.OverrideCursor = cursor;
        }

        private void MainBorderOnMouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            resizeDirection = ResizeDirection.None;
            SetCursor(resizeDirection);
            mainBorder.ReleaseMouseCapture();
        }

        private void MainBorderOnMouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (isMoving)
            {
                isMoving = false;
                return;
            }
            var point = mouseButtonEventArgs.GetPosition(mainBorder);
            var direction = GetDirection(point);
            if (direction == ResizeDirection.None) return;
            mainBorder.CaptureMouse();
            SetStartBounds();
            resizeDirection = direction;
            startPoint = point;
            SetCursor(resizeDirection);
        }

        private void SetStartBounds()
        {
            startBounds = new Rect(Left, Top, ActualWidth, ActualHeight);
        }

        private void MainBorderOnPreviewMouseMove(object sender, MouseEventArgs mouseEventArgs)
        {
            if (ResizeDirection.None == resizeDirection)
            {
                if (previousDirection != ResizeDirection.None)
                    SetCursor(ResizeDirection.None);
                var direction = GetDirection(mouseEventArgs.GetPosition(mainBorder));
                previousDirection = direction;
                SetCursor(direction);
            }
            var point = mouseEventArgs.GetPosition(mainBorder);
            DoResize(point);
            startPoint = mouseEventArgs.GetPosition(mainBorder);
            SetStartBounds();
        }

        private void DoResize(Point mouse)
        {
            var delta = new Point(startPoint.X - mouse.X, startPoint.Y - mouse.Y);
            if (ResizeDirection.Top == (resizeDirection & ResizeDirection.Top) && Height - mouse.Y > MinHeight)
            {
                Height -= mouse.Y;
                Top += mouse.Y;
            }
            if (ResizeDirection.Left == (resizeDirection & ResizeDirection.Left) && Width - mouse.X > MinWidth)
            {
                Width -= mouse.X;
                Left += mouse.X;
            }
            if (ResizeDirection.Bottom == (resizeDirection & ResizeDirection.Bottom) && Height - delta.Y > MinHeight)
                Height = startBounds.Height - delta.Y;
            if (ResizeDirection.Right == (resizeDirection & ResizeDirection.Right) && Width - delta.X > MinWidth)
                Width = startBounds.Width - delta.X;
        }

        private Cursor previousOverrideCursor;

        /// <summary>
        /// Called when [close command].
        /// </summary>
        /// <param name="borderlessWindow">The borderless window.</param>
        private static void OnCloseCommand(BorderlessWindow borderlessWindow)
        {
            if (null == borderlessWindow) return;
            borderlessWindow.Close();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can maximize.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance can maximize; otherwise, <c>false</c>.
        /// </value>
        public bool CanMaximize
        {
            get { return (bool)GetValue(CanMaximizeProperty); }
            set { SetValue(CanMaximizeProperty, value); }
        }

        /// <summary>
        /// Initializes the <see cref="BorderlessWindow" /> class.
        /// </summary>
        static BorderlessWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BorderlessWindow), new FrameworkPropertyMetadata(typeof(BorderlessWindow)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BorderlessWindow" /> class.
        /// </summary>
        public BorderlessWindow()
        {
            WindowStyle = WindowStyle.None;
            AllowsTransparency = true;
        }

        /// <summary>
        /// Gets the double click time.
        /// </summary>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern uint GetDoubleClickTime();

        /// <summary>
        /// ResizeDirection
        /// </summary>
        [Flags]
        public enum ResizeDirection
        {
            None = 0,
            Bottom = 1,
            Left = 2,
            Right = 4,
            Top = 8,
            TopLeft = Top | Left,
            TopRight = Top | Right,
            BottomLeft = Bottom | Left,
            BottomRight = Bottom | Right,
        }
    }
}
