using System.Windows;

namespace GameOfLife
{
    public unsafe interface IRenderObject
    {
        Int32Rect Render(int* pixels, Int32Size worldSize, Int32Rect viewportSize);
    }
}
