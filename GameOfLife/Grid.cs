using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace GameOfLife
{
    public class Grid : IFrameObject, IMouseInput
    {
        private const int size = 100;
        private bool _isSimulationStarted;

        private Cell[,] _cells;

        public void Start(PixelWorld world)
        {
            world.Size = new Int32Size(size, size);
            world.Viewport = new Int32Rect(0, 0, size, size);

            // Create the grid
            _cells = new Cell[size, size];
            for (var i = 0; i < size; i++)
                for (var j = 0; j < size; j++)
                    world.AddObject(_cells[i, j] = new Cell() { Row = i, Column = j });

            // Fill Cell neighbhours
            for (var i = 0; i < size; i++)
            {
                for (var j = 0; j < size; j++)
                {
                    var cell = _cells[i, j];
                    AddNeightbhour(cell, i + 0, j + 1);
                    AddNeightbhour(cell, i + 1, j + 1);
                    AddNeightbhour(cell, i + 1, j + 0);
                    AddNeightbhour(cell, i + 1, j - 1);
                    AddNeightbhour(cell, i + 0, j - 1);
                    AddNeightbhour(cell, i - 1, j - 1);
                    AddNeightbhour(cell, i - 1, j + 0);
                    AddNeightbhour(cell, i - 1, j + 1);

                    //if (i == 0 || i == 99 || j == 0 || j == 99)
                    //    cell.SetAlive(true);
                }
            }
        }

        public void SetAllCellsAsDead()
        {
            for (var i = 0; i < size; i++)
                for (var j = 0; j < size; j++)
                    _cells[i, j].SetAlive(false);
        }

        public void InitializeShape(Shape shape)
        {
            SetAllCellsAsDead();

            switch(shape)
            {
                case Shape.Block:
                    _cells[50, 50].SetAlive(true);
                    _cells[50, 51].SetAlive(true);
                    _cells[51, 50].SetAlive(true);
                    _cells[51, 51].SetAlive(true);
                    break;
                case Shape.Blinker:
                    _cells[50, 50].SetAlive(true);
                    _cells[50, 51].SetAlive(true);
                    _cells[50, 52].SetAlive(true);
                    break;
                case Shape.Toad:
                    _cells[50, 50].SetAlive(true);
                    _cells[50, 51].SetAlive(true);
                    _cells[50, 52].SetAlive(true);
                    _cells[51, 51].SetAlive(true);
                    _cells[51, 52].SetAlive(true);
                    _cells[51, 53].SetAlive(true);
                    break;
                case Shape.Pulsar:
                    _cells[50, 50].SetAlive(true);
                    _cells[50, 51].SetAlive(true);
                    _cells[50, 52].SetAlive(true);

                    _cells[51, 51].SetAlive(true);
                    _cells[52, 51].SetAlive(true);
                    _cells[53, 51].SetAlive(true);

                    _cells[54, 50].SetAlive(true);
                    _cells[54, 51].SetAlive(true);
                    _cells[54, 52].SetAlive(true);

                    _cells[51, 48].SetAlive(true);
                    _cells[52, 48].SetAlive(true);
                    _cells[53, 48].SetAlive(true);
                    break;
                case Shape.Glider:
                    _cells[50, 48].SetAlive(true);
                    _cells[50, 49].SetAlive(true);
                    _cells[51, 49].SetAlive(true);
                    _cells[51, 50].SetAlive(true);
                    _cells[52, 48].SetAlive(true);
                    break;
                case Shape.RPentomino:
                    _cells[50, 49].SetAlive(true);
                    _cells[50, 50].SetAlive(true);
                    _cells[51, 50].SetAlive(true);
                    _cells[49, 50].SetAlive(true);
                    _cells[49, 51].SetAlive(true);
                    break;
                case Shape.Custom1:
                    int index = 10;
                    for (var i = 0; i < 8; i++)
                        _cells[50, index++].SetAlive(true);
                    index++;
                    for (var i = 0; i < 5; i++)
                        _cells[50, index++].SetAlive(true);
                    index++; index++; index++;
                    for (var i = 0; i < 3; i++)
                        _cells[50, index++].SetAlive(true);
                    index++; index++; index++;
                    index++; index++; index++;
                    for (var i = 0; i < 7; i++)
                        _cells[50, index++].SetAlive(true);
                    index++;
                    for (var i = 0; i < 5; i++)
                        _cells[50, index++].SetAlive(true);
                    break;
            }
        }

        private void AddNeightbhour(Cell cell, int row, int column)
        {
            if (row >= 0 && row < _cells.GetLength(0) && column >= 0 && column < _cells.GetLength(1))
                cell.Neighbhours.Add(_cells[row, column]);
        }

        public void Update()
        {
            if (!_isSimulationStarted) return;

            for (var i = 0; i < size; i++)
                for (var j = 0; j < size; j++)
                    _cells[i, j].ComputeNextState();

            for (var i = 0; i < size; i++)
                for (var j = 0; j < size; j++)
                    _cells[i, j].Update();
        }

        public void StartSimulation()
        {
            _isSimulationStarted = true;
        }

        public void StopSimulation()
        {
            _isSimulationStarted = false;
        }

        #region IMouseInput

        public void Register(IMouseListener listener)
        {
            listener.Moved += OnMouseMoved;
            listener.LeftButtonDown += OnMouseLeftButtonDown;
            listener.LeftButtonDown += OnMouseRightButtonDown;
        }

        public void Unregister(IMouseListener listener)
        {
            listener.Moved -= OnMouseMoved;
            listener.LeftButtonDown -= OnMouseLeftButtonDown;
            listener.LeftButtonDown -= OnMouseRightButtonDown;
        }

        private void OnMouseRightButtonDown(IInputElement source, MouseButtonEventArgs e) 
            => SetIsAliveCell(e.GetPosition(source), true);

        private void OnMouseLeftButtonDown(IInputElement source, MouseButtonEventArgs e)
            => SetIsAliveCell(e.GetPosition(source), false);

        private void OnMouseMoved(IInputElement source, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                SetIsAliveCell(e.GetPosition(source), true);
            else if (e.RightButton == MouseButtonState.Pressed)
                SetIsAliveCell(e.GetPosition(source), false);
        }

        private void SetIsAliveCell(Point position, bool isAlive)
        {
            var row = Math.Max(0, Math.Min(size, (int)position.Y));
            var column = Math.Max(0, Math.Min(size, (int)position.X));
            _cells[row, column].SetAlive(isAlive);
        }

        #endregion
    }

    public enum Shape
    {
        None,
        // ---- Still lifes
        Block,
        //BeeHive,
        //Loaf,
        //Boat,
        //Tub,
        // ---- Oscillators
        Blinker,
        Toad,
        //Beacon,
        Pulsar,
        //PentaDecathlon,
        // ---- SpaceShips
        Glider,
        //LightWeightSpaceShip,
        //MiddleWeightSpaceShip,
        //HeavyWeightSpaceShip,
        // ---- Expandable
        RPentomino,
        //DieHard,
        //Acorn,
        //GosperGliderGun,
        //SimkinGliderGun,
        // ---- Customs
        Custom1
    }

}
