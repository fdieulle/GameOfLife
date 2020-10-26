using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace GameOfLife
{
    public class Cell : IRenderObject
    {
        private bool _nextState;
        private bool _isFirstRender = true;

        public int Row { get; set; }
        public int Column { get; set; }

        public bool IsAlive { get; private set; }

        public List<Cell> Neighbhours { get; } = new List<Cell>();

        public unsafe Int32Rect Render(int* pixels, Int32Size worldSize, Int32Rect viewport)
        {
            if (_isFirstRender)
            {
                IsAlive = *(pixels + (Row * worldSize.Width + Column)) != 0;
                _isFirstRender = false;
            }

            var color = IsAlive ? Colors.Black : Colors.White;
            *(pixels + (Row * worldSize.Width + Column)) = (color.R << 16) | (color.G << 8) | (color.B << 0);

            return new Int32Rect(Column, Row, 1, 1);
        }

        public void ComputeNextState()
        {
            var nbLiveNeighbours = Neighbhours.Count(p => p.IsAlive);
            if (IsAlive)
            {
                if (nbLiveNeighbours < 2) _nextState = false;
                else if (nbLiveNeighbours > 3) _nextState = false;
            }
            else if (nbLiveNeighbours == 3)
                _nextState = true;
        }

        public void Update()
        {
            IsAlive = _nextState;
        }

        public void SetAlive(bool isAlive) => _nextState = IsAlive = isAlive;
    }

}
