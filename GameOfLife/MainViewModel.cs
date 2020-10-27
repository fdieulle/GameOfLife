using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Documents;
using XShell.Core;

namespace GameOfLife
{
    public class MainViewModel : AbstractNpc
    {
        private readonly Grid _grid = new Grid();

        public ObservableCollection<object> Objects { get; } = new ObservableCollection<object>();

        private double _framePerSecond = 4;
        public double FramesPerSecond
        {
            get => _framePerSecond;
            set
            {
                if (Math.Abs(_framePerSecond - value) < 1e-5) return;

                _framePerSecond = value;
                RaisePropertyChanged(nameof(FramesPerSecond));
            }
        }

        public List<Shape> Shapes { get; } = Enum.GetValues(typeof(Shape)).OfType<Shape>().ToList();

        private Shape _selectedShape = Shape.None;
        public Shape SelectedShape
        {
            get => _selectedShape;
            set
            {
                _selectedShape = value;
                _grid.InitializeShape(value);
            }
        }

        public RelayCommand StartSimulationCommand { get; }

        public RelayCommand StopSimulationCommand { get; }

        public RelayCommand ClearCommand { get; }

        public MainViewModel()
        {
            Objects.Add(_grid);

            StartSimulationCommand = new RelayCommand(p => _grid.StartSimulation());
            StopSimulationCommand = new RelayCommand(p => _grid.StopSimulation());
            ClearCommand = new RelayCommand(p => _grid.SetAllCellsAsDead());
        }
    }
}
