using System;

namespace Nodum.Node
{
    public class Position
    {
        private double _x;
        public double X
        {
            get => _x;
            set
            {
                _x = value;
                OnPositionChanged?.Invoke();
            }
        }
        private double _y;
        public double Y
        {
            get => _y;
            set
            {
                _y = value;
                OnPositionChanged?.Invoke();
            }
        }
        public Action OnPositionChanged { get; set; }
        public Action UpdatePosition { get; set; }
    }
}