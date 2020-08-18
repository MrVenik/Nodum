namespace NodumVisualCalculator.Data
{
    public class Line
    {
        public Position From { get; set; }
        public Position To { get; set; }

        public Line()
        {
            From = new Position();
            To = new Position();
        }

        public Line(Position from, Position to)
        {
            From = from;
            To = to;
        }

        public string Curve
        {
            get
            {
                if (From != null && To != null)
                {
                    int x0, y0, x1, y1, x2, y2, x3, y3;

                    x0 = (int)From.X;
                    y0 = (int)From.Y;

                    x3 = (int)To.X;
                    y3 = (int)To.Y;

                    x1 = (int)(x0 + ((x3 - x0) / 2.0));
                    y1 = y0;

                    x2 = (int)(x0 + ((x1 - x0) / 2.0));
                    y2 = y3;

                    return $"M {x0}, {y0} C {x1}, {y1} {x2}, {y2} {x3}, {y3}";
                }
                else return string.Empty;
            }

        }
    }
}
