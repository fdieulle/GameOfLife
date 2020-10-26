using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;

namespace GameOfLife
{
    public class PixelWorld
    {
        private readonly List<IFrameObject> _frameObjects = new List<IFrameObject>();
        private readonly List<IRenderObject> _renderObjects = new List<IRenderObject>();
        private bool _isStarted;

        public Int32Size Size { get; set; }

        public Int32Rect Viewport { get; set; }

        public void AddObject(object obj)
        {
            if (obj is IFrameObject frameObject)
            {
                if (_isStarted)
                    frameObject.Start(this);

                _frameObjects.Add(frameObject);
            }
            if (obj is IRenderObject renderObject)
                _renderObjects.Add(renderObject);
        }

        public void RemoveObject(object obj)
        {
            if (obj is IFrameObject frameObject)
                _frameObjects.Remove(frameObject);
            if (obj is IRenderObject renderObject)
                _renderObjects.Remove(renderObject);
        }

        public void ClearObjects()
        {
            _frameObjects.Clear();
            _renderObjects.Clear();
        }

        public void Start()
        {
            if (_isStarted) return;

            foreach (var obj in _frameObjects)
                obj.Start(this);
            _isStarted = true;
        }

        public void Update()
        {
            foreach (var obj in _frameObjects)
                obj.Update();
        }

        public unsafe void Render(WriteableBitmap pixels)
        {
            var backBuffer = (int*)pixels.BackBuffer;
            foreach (var obj in _renderObjects)
            {
                var rect = obj.Render(backBuffer, Size, Viewport);
                if (rect.HasArea)
                    pixels.AddDirtyRect(rect);
            }
        }
    }
}
