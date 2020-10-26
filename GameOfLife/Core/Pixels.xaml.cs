using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace GameOfLife
{
    /// <summary>
    /// Interaction logic for Pixels.xaml
    /// </summary>
    public partial class Pixels : UserControl, IMouseListener
    {
        private readonly PixelWorld _world = new PixelWorld();
        private readonly DispatcherTimer _timer = new DispatcherTimer();
        private readonly List<object> _objects = new List<object>();
        private WriteableBitmap _pixels;

        #region Properties

        public double FramesPerSecond
        {
            get { return (double)GetValue(FramesPerSecondProperty); }
            set { SetValue(FramesPerSecondProperty, value); }
        }

        public static readonly DependencyProperty FramesPerSecondProperty =
            DependencyProperty.Register("FramesPerSecond", typeof(double), typeof(Pixels), new PropertyMetadata(4.0, OnFramesPerSecondPropertyChanged));

        private static void OnFramesPerSecondPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
            => ((Pixels)d).OnFramesPerSecondChanged((double)e.OldValue, (double)e.NewValue);

        protected virtual void OnFramesPerSecondChanged(double oldValue, double newValue) 
            => _timer.Interval = TimeSpan.FromMilliseconds(1e3 / newValue);

        public IEnumerable Objects  
        {
            get { return (IEnumerable)GetValue(ObjectsProperty); }
            set { SetValue(ObjectsProperty, value); }
        }

        public static readonly DependencyProperty ObjectsProperty =
            DependencyProperty.Register("Objects", typeof(IEnumerable), typeof(Pixels), new PropertyMetadata(null, OnObjectsPropertyChanged));

        private static void OnObjectsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((Pixels)d).OnObjectsChanged((IEnumerable)e.OldValue, (IEnumerable)e.NewValue);

        #endregion

        public Pixels()
        {
            InitializeComponent();
            InitializeMouseListener();

            RenderOptions.SetBitmapScalingMode(Image, BitmapScalingMode.NearestNeighbor);
            RenderOptions.SetEdgeMode(Image, EdgeMode.Aliased);

            _timer.Interval = TimeSpan.FromMilliseconds(1e3 / FramesPerSecond);
            _timer.Tick += RenderFrame;

            Start();
        }
               
        public void Start()
        {
            _world.Start();
            _timer.Start();
        }

        public void Stop() => _timer.Stop();

        private void RenderFrame(object sender, EventArgs e)
        {
            try
            {
                _world.Update();

                // Resize world
                if (_pixels == null || (int)_pixels.Width != _world.Size.Width || (int)_pixels.Height != _world.Size.Height)
                    ResizeWorld(_world.Size);

                _pixels.Lock();
                _world.Render(_pixels);
            }
            finally
            {
                // Release the back buffer and make it available for display.
                _pixels.Unlock();
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            AdjustCamera(constraint);
            return base.MeasureOverride(constraint);
        }

        private void ResizeWorld(Int32Size size)
        {
            if (size.Width <= 0 || size.Height <= 0) return;

            _pixels = new WriteableBitmap(
                    size.Width, size.Height,
                    96, 96, PixelFormats.Bgr32, null);
            Image.Source = _pixels;

            if (ActualWidth > 0 && !double.IsNaN(ActualWidth) && ActualHeight > 0 && !double.IsNaN(ActualHeight))
                AdjustCamera(new Size(ActualWidth, ActualHeight));
        }

        private void AdjustCamera(Size windowSize)
        {
            var m = new Matrix();
            var viewport = _world.Viewport;
            m.Translate(viewport.X, viewport.Y);
            var scaleX = windowSize.Width / viewport.Width;
            var scaleY = windowSize.Height / viewport.Height;
            var scale = Math.Min(scaleX, scaleY);
            m.ScaleAt(scale, scale, viewport.X + viewport.Width / 2.0, viewport.Y + viewport.Height / 2.0);

            Image.RenderTransform = new MatrixTransform(m);
        }

        #region Manage Objects

        public void AddObject(object obj)
        {
            _world.AddObject(obj);

            if (obj is IMouseInput mi)
                mi.Register(this);
        }

        public void RemoveObject(object obj)
        {
            _world.RemoveObject(obj);
            ReleaseObject(obj);
        }

        private void ReleaseObject(object obj)
        {
            if (obj is IMouseInput mi)
                mi.Unregister(this);
        }

        public void ClearObjects()
        {
            _world.ClearObjects();
            foreach (var obj in _objects)
                ReleaseObject(obj);
            _objects.Clear();
        }

        protected virtual void OnObjectsChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            if (oldValue is INotifyCollectionChanged oncc)
                oncc.CollectionChanged -= OnObjectsCollectionChanged;

            ClearObjects();

            if (newValue != null)
            {
                foreach (var obj in newValue)
                    AddObject(obj);
            }

            if (newValue is INotifyCollectionChanged nncc)
                nncc.CollectionChanged += OnObjectsCollectionChanged;
        }

        private void OnObjectsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
                ClearObjects();

            if (e.OldItems != null)
                foreach (var obj in e.OldItems)
                    RemoveObject(obj);

            if (e.NewItems != null)
                foreach (var obj in e.NewItems)
                    AddObject(obj);
        }

        #endregion

        #region IMouseListener

        public event Action<IInputElement, MouseEventArgs> Moved;
        public event Action<IInputElement, MouseButtonEventArgs> LeftButtonDown;
        public event Action<IInputElement, MouseButtonEventArgs> RightButtonDown;
        public event Action<IInputElement, MouseWheelEventArgs> Wheel;

        private void InitializeMouseListener()
        {
            Image.MouseLeftButtonDown += OnMouseLeftButtonDown;
            Image.MouseRightButtonDown += OnMouseRightButtonDown;
            Image.MouseMove += OnMouseMove;
            Image.MouseWheel += OnMouseWheel;
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => LeftButtonDown?.Invoke(Image, e);

        private void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e) => RightButtonDown?.Invoke(Image, e);

        private void OnMouseMove(object sender, MouseEventArgs e) => Moved?.Invoke(Image, e);

        private void OnMouseWheel(object sender, MouseWheelEventArgs e) => Wheel?.Invoke(Image, e);

        #endregion
    }
}
