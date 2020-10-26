using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace GameOfLife
{
    public class InkPen : IRenderObject, IMouseInput
    {
        private readonly Queue<Pixel> _pixels = new Queue<Pixel>();

        public void Register(IMouseListener listener)
        {
            listener.Moved += OnMouseMoved;
            listener.LeftButtonDown += OnMouseLeftButtonDown;
            listener.LeftButtonDown += OnMouseRightButtonDown;
        }

        private void OnMouseRightButtonDown(IInputElement source, MouseButtonEventArgs e)
        {
            _pixels.Enqueue(new Pixel { Color = Colors.Black, Position = e.GetPosition(source) });
        }

        private void OnMouseLeftButtonDown(IInputElement source, MouseButtonEventArgs e)
        {
            _pixels.Enqueue(new Pixel { Color = Colors.Black, Position = e.GetPosition(source) });
        }

        private void OnMouseMoved(IInputElement source, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                _pixels.Enqueue(new Pixel { Color = Colors.Black, Position = e.GetPosition(source) });
            else if (e.RightButton == MouseButtonState.Pressed)
                _pixels.Enqueue(new Pixel { Color = Colors.Black, Position = e.GetPosition(source) });
        }

        public void Unregister(IMouseListener listener)
        {
            listener.Moved -= OnMouseMoved;
            listener.LeftButtonDown -= OnMouseLeftButtonDown;
            listener.LeftButtonDown -= OnMouseRightButtonDown;
        }

        public unsafe Int32Rect Render(int* pixels, Int32Size worldSize, Int32Rect viewportSize)
        {
            if (_pixels.Count == 0) return Int32Rect.Empty;

            var minX = int.MaxValue;
            var maxX = int.MinValue;
            var minY = int.MaxValue;
            var maxY = int.MinValue;
            while (_pixels.Count > 0)
            {
                var pixel = _pixels.Dequeue();
                var position = pixel.Position;
                var color = pixel.Color;

                var ptr = pixels + (int)position.X * worldSize.Width + (int)position.Y;
                *ptr = (color.R << 16) | (color.G << 8) | (color.B << 0);

                minX = Math.Min(minX, (int)position.X);
                maxX = Math.Max(maxX, (int)position.X + 1);
                minY = Math.Min(minY, (int)position.Y);
                maxY = Math.Max(maxY, (int)position.Y + 1);
            }

            return new Int32Rect(minX, minY, Math.Max(0, maxX - minX), Math.Max(0, maxY - minY));
        }

        private class Pixel
        {
            public Color Color { get; set; }
            public Point Position { get; set; }
        }
    }

}
