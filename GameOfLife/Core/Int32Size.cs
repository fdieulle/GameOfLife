namespace GameOfLife
{
    public struct Int32Size
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public Int32Size(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }
}
