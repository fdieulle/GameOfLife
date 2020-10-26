using System;
using System.Windows;
using System.Windows.Input;

namespace GameOfLife
{
    public interface IMouseListener
    {
        event Action<IInputElement, MouseEventArgs> Moved;
        event Action<IInputElement, MouseButtonEventArgs> LeftButtonDown;
        event Action<IInputElement, MouseButtonEventArgs> RightButtonDown;
        event Action<IInputElement, MouseWheelEventArgs> Wheel;
    }
}
