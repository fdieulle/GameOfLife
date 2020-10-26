namespace GameOfLife
{
    public interface IMouseInput
    {
        void Register(IMouseListener listener);
        void Unregister(IMouseListener listener);
    }
}
