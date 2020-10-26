using System;
using System.Collections.ObjectModel;
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
