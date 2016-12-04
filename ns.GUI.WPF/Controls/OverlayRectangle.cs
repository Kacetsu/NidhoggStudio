using ns.Base.Plugins.Properties;
using ns.Communication.Client;
using ns.Communication.Models.Properties;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ns.GUI.WPF.Controls {

    public class OverlayRectangle {
        private const double MINRECTANGLESIZE = 20;
        private const double SIZECHANGER_OFFSET = 6;
        private Brush _borderBrush = Brushes.LightBlue;
        private Rectangle _bottomLeftSizeChanger;
        private Rectangle _bottomRightSizeChanger;
        private Point _endPosition;
        private bool _isDrag = false;
        private RectangleProperty _property;
        private Rectangle _rectangle;
        private Point _startPosition;
        private Rectangle _topLeftSizeChanger;
        private Rectangle _topRightSizeChanger;

        /// <summary>
        /// Initializes a new instance of the <see cref="OverlayRectangle"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="parent">The parent.</param>
        public OverlayRectangle(RectangleProperty property, Canvas parent) {
            _rectangle = new Rectangle();
            _rectangle.Stroke = _borderBrush;
            _rectangle.Fill = new SolidColorBrush(Color.FromArgb(40, 175, 238, 238));
            _rectangle.StrokeThickness = 2.0;
            _property = property;
            Canvas.SetTop(_rectangle, property.Y);
            Canvas.SetLeft(_rectangle, property.X);
            _rectangle.Width = _property.Width;
            _rectangle.Height = _property.Height;

            property.PropertyChanged += Property_PropertyChanged;
            _rectangle.MouseLeftButtonDown += Rectangle_MouseLeftButtonDown;
            _rectangle.MouseLeftButtonUp += Rectangle_MouseLeftButtonUp;
            _rectangle.MouseMove += Rectangle_MouseMove;
            _rectangle.MouseEnter += Rectangle_MouseEnter;
            _rectangle.MouseLeave += Rectangle_MouseLeave;

            if (property.Direction == PropertyDirection.In) {
                _topLeftSizeChanger = CreateSizeChanger(_property.X, _property.Y);
                _topRightSizeChanger = CreateSizeChanger(_property.X + _rectangle.Width, _property.Y);
                _topRightSizeChanger.Visibility = Visibility.Collapsed;
                _bottomLeftSizeChanger = CreateSizeChanger(_property.X, _property.Y + _rectangle.Height);
                _bottomLeftSizeChanger.Visibility = Visibility.Collapsed;
                _bottomRightSizeChanger = CreateSizeChanger(_property.X + _rectangle.Width, _property.Y + _rectangle.Height);
                _bottomRightSizeChanger.Visibility = Visibility.Collapsed;
                parent.Children.Add(_topLeftSizeChanger);
                parent.Children.Add(_topRightSizeChanger);
                parent.Children.Add(_bottomLeftSizeChanger);
                parent.Children.Add(_bottomRightSizeChanger);
                UpdateSizeChanger();
            }

            ClientCommunicationManager.ProjectService.Callback.PropertyChanged += Callback_PropertyChanged;
        }

        /// <summary>
        /// Gets the rectangle.
        /// </summary>
        /// <value>
        /// The rectangle.
        /// </value>
        public Rectangle Rectangle {
            get { return _rectangle; }
        }

        private void Callback_PropertyChanged(object sender, Communication.Events.PropertyChangedEventArgs e) {
            if (!e.Id.Equals(_property?.Id)) {
                return;
            }

            PropertyModel model = ClientCommunicationManager.ProjectService.GetProperty(_property.Id);
            RectangleProperty rectangleProperty = model?.Property as RectangleProperty;

            if (rectangleProperty == null) {
                return;
            }

            Canvas.SetTop(_rectangle, rectangleProperty.Y);
            Canvas.SetLeft(_rectangle, rectangleProperty.X);
            _rectangle.Width = rectangleProperty.Width;
            _rectangle.Height = rectangleProperty.Height;
            UpdateSizeChanger();

            // Update property because it could be a other client.
            // Only update value if they don't are equal.
            if (_property.X != rectangleProperty.X) {
                _property.X = (double)_rectangle.GetValue(Canvas.LeftProperty);
            }

            if (_property.Y != rectangleProperty.Y) {
                _property.Y = (double)_rectangle.GetValue(Canvas.TopProperty);
            }

            if (_property.Width != rectangleProperty.Width) {
                _property.Width = _rectangle.Width;
            }

            if (_property.Height != rectangleProperty.Height) {
                _property.Height = _rectangle.Height;
            }
        }

        private Rectangle CreateSizeChanger(double x, double y) {
            Rectangle rectangle = new Rectangle();
            rectangle.Stroke = _borderBrush;
            rectangle.Fill = _borderBrush;
            rectangle.StrokeThickness = 0;
            Canvas.SetTop(rectangle, y - SIZECHANGER_OFFSET);
            Canvas.SetLeft(rectangle, x - SIZECHANGER_OFFSET);
            rectangle.Width = 12.0;
            rectangle.Height = 12.0;
            rectangle.MouseLeftButtonDown += Rectangle_MouseLeftButtonDown;
            rectangle.MouseLeftButtonUp += Rectangle_MouseLeftButtonUp;
            rectangle.MouseMove += Rectangle_MouseMove;
            rectangle.MouseEnter += Rectangle_MouseEnter;
            rectangle.MouseLeave += Rectangle_MouseLeave;
            return rectangle;
        }

        private void Property_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            RectangleProperty property = sender as RectangleProperty;
            if (e.PropertyName == "X") {
                Canvas.SetLeft(_rectangle, property.X);
            } else if (e.PropertyName == "Y") {
                Canvas.SetTop(_rectangle, property.Y);
            } else if (e.PropertyName == "Width") {
                _rectangle.Width = property.Width;
            } else if (e.PropertyName == "Height") {
                _rectangle.Height = property.Height;
            }
            UpdateSizeChanger();
        }

        private void Rectangle_MouseEnter(object sender, MouseEventArgs e) {
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void Rectangle_MouseLeave(object sender, MouseEventArgs e) {
            Mouse.OverrideCursor = null;
        }

        private void Rectangle_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            _isDrag = true;
            Mouse.OverrideCursor = Cursors.Hand;
            _startPosition = e.GetPosition(_rectangle.Parent as Canvas);

            Rectangle rect = null;

            if (sender == _rectangle) {
                rect = _rectangle;
            } else if (sender == _topLeftSizeChanger) {
                rect = _topLeftSizeChanger;
            } else if (sender == _topRightSizeChanger) {
                rect = _topRightSizeChanger;
            } else if (sender == _bottomLeftSizeChanger) {
                rect = _bottomLeftSizeChanger;
            } else if (sender == _bottomRightSizeChanger) {
                rect = _bottomRightSizeChanger;
            }

            if (rect != null)
                _endPosition = new Point(Canvas.GetLeft(rect), Canvas.GetTop(rect));

            Mouse.Capture((IInputElement)sender);
        }

        private void Rectangle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            _isDrag = false;
            Mouse.OverrideCursor = null;
            Mouse.Capture(null);
            ClientCommunicationManager.ProjectService.ChangePropertyValue(_property.Value, _property.Id);
        }

        private void Rectangle_MouseMove(object sender, MouseEventArgs e) {
            if (_isDrag) {
                Canvas parent = _rectangle.Parent as Canvas;
                Rectangle rect = sender as Rectangle;
                double newX = _startPosition.X + (e.GetPosition(parent).X - _startPosition.X);
                double newY = _startPosition.Y + (e.GetPosition(parent).Y - _startPosition.Y);
                Point offset = new Point((_startPosition.X - _endPosition.X), (_startPosition.Y - _endPosition.Y));
                double top = newY - offset.Y;
                double left = newX - offset.X;

                double oldTop = (double)rect.GetValue(Canvas.TopProperty);
                double oldLeft = (double)rect.GetValue(Canvas.LeftProperty);

                if (sender == _rectangle) {
                    if (top + _rectangle.Height > parent.ActualHeight) top = parent.ActualHeight - _rectangle.Height;
                    if (left + _rectangle.Width > parent.ActualWidth) left = parent.ActualWidth - _rectangle.Width;
                    if (top < 0) top = 0;
                    if (left < 0) left = 0;
                    _rectangle.SetValue(Canvas.TopProperty, top);
                    _rectangle.SetValue(Canvas.LeftProperty, left);
                } else if (sender == _topLeftSizeChanger) {
                    oldTop = (double)_rectangle.GetValue(Canvas.TopProperty);
                    oldLeft = (double)_rectangle.GetValue(Canvas.LeftProperty);
                    double newHeight = _rectangle.Height + (oldTop - top);
                    double newWidth = _rectangle.Width + (oldLeft - left);
                    _rectangle.Height = newHeight < MINRECTANGLESIZE ? MINRECTANGLESIZE : newHeight;
                    _rectangle.Width = newWidth < MINRECTANGLESIZE ? MINRECTANGLESIZE : newWidth;
                    _rectangle.SetValue(Canvas.TopProperty, top);
                    _rectangle.SetValue(Canvas.LeftProperty, left);
                } else if (sender == _topRightSizeChanger) {
                    double newHeight = _rectangle.Height + (oldTop - top);
                    double newWidth = _rectangle.Width - (oldLeft - left);
                    _rectangle.Height = newHeight < MINRECTANGLESIZE ? MINRECTANGLESIZE : newHeight;
                    _rectangle.Width = newWidth < MINRECTANGLESIZE ? MINRECTANGLESIZE : newWidth;
                    _rectangle.SetValue(Canvas.TopProperty, top);
                    _rectangle.SetValue(Canvas.LeftProperty, _rectangle.GetValue(Canvas.LeftProperty));
                }

                _property.X = (double)_rectangle.GetValue(Canvas.LeftProperty);
                _property.Y = (double)_rectangle.GetValue(Canvas.TopProperty);
                _property.Width = _rectangle.Width;
                _property.Height = _rectangle.Height;

                if (_property.Direction == PropertyDirection.In)
                    UpdateSizeChanger();
            }
        }

        private void UpdateSizeChanger() {
            _topLeftSizeChanger.SetValue(Canvas.TopProperty, ((double)_rectangle.GetValue(Canvas.TopProperty)) - SIZECHANGER_OFFSET + 1);
            _topLeftSizeChanger.SetValue(Canvas.LeftProperty, ((double)_rectangle.GetValue(Canvas.LeftProperty)) - SIZECHANGER_OFFSET + 1);

            _topRightSizeChanger.SetValue(Canvas.TopProperty, ((double)_rectangle.GetValue(Canvas.TopProperty)) - SIZECHANGER_OFFSET + 1);
            _topRightSizeChanger.SetValue(Canvas.LeftProperty, ((double)_rectangle.GetValue(Canvas.LeftProperty) + _rectangle.Width) - SIZECHANGER_OFFSET - 1);

            _bottomLeftSizeChanger.SetValue(Canvas.TopProperty, ((double)_rectangle.GetValue(Canvas.TopProperty) + _rectangle.Height) - SIZECHANGER_OFFSET - 1);
            _bottomLeftSizeChanger.SetValue(Canvas.LeftProperty, ((double)_rectangle.GetValue(Canvas.LeftProperty)) - SIZECHANGER_OFFSET + 1);

            _bottomRightSizeChanger.SetValue(Canvas.TopProperty, ((double)_rectangle.GetValue(Canvas.TopProperty) + _rectangle.Height) - SIZECHANGER_OFFSET - 1);
            _bottomRightSizeChanger.SetValue(Canvas.LeftProperty, ((double)_rectangle.GetValue(Canvas.LeftProperty) + _rectangle.Width) - SIZECHANGER_OFFSET - 1);
        }
    }
}