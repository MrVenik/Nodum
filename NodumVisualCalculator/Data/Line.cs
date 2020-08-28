namespace NodumVisualCalculator.Data
{
    public class Line
    {
        public double FromX { get; set; }
        public double FromY { get; set; }
        public double ToX { get; set; }
        public double ToY { get; set; }

        public string Curve
        {
            get
            {
                int x0, y0, x1, y1, x2, y2, x3, y3;

                x0 = (int)FromX;
                y0 = (int)FromY;

                x3 = (int)ToX;
                y3 = (int)ToY;

                x1 = (int)(x0 + ((x3 - x0) / 2.0));
                y1 = y0;

                x2 = (int)(x0 + ((x1 - x0) / 2.0));
                y2 = y3;

                return $"M {x0}, {y0} C {x1}, {y1} {x2}, {y2} {x3}, {y3}";
            }

        }
    }
}
