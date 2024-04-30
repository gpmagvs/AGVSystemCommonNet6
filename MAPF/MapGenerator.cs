namespace AGVSystemCommonNet6.MAPF
{
    public class MapGenerator
    {
        private int width;
        private int height;
        private char[,] mapGrid;
        private List<Tuple<double, double>> coordinates;
        private List<Tuple<Tuple<double, double>, Tuple<double, double>>> connections;

        public MapGenerator(int width, int height)
        {
            this.width = width;
            this.height = height;
            mapGrid = new char[height, width];
            coordinates = new List<Tuple<double, double>>();
            connections = new List<Tuple<Tuple<double, double>, Tuple<double, double>>>();

            // Initialize the map with obstacles
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    mapGrid[i, j] = '@';
                }
            }
        }

        public void AddCoordinate(double x, double y)
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                coordinates.Add(new Tuple<double, double>(x, y));
                mapGrid[(int)y, (int)x] = '.';  // Set passable point
            }
        }

        public void AddConnection(Tuple<double, double> start, Tuple<double, double> end)
        {
            connections.Add(new Tuple<Tuple<double, double>, Tuple<double, double>>(start, end));
        }

        public void GenerateConnections()
        {
            foreach (var connection in connections)
            {
                ConnectPoints(connection.Item1, connection.Item2);
            }
        }

        private void ConnectPoints(Tuple<double, double> start, Tuple<double, double> end)
        {
            List<Tuple<double, double>> linePoints = GetLinePoints(start.Item1, start.Item2, end.Item1, end.Item2);
            foreach (var point in linePoints)
            {
                if (point.Item1 >= 0 && point.Item1 < width && point.Item2 >= 0 && point.Item2 < height)
                {
                    mapGrid[(int)point.Item2, (int)point.Item1] = '.';  // Set passable point along the line
                }
            }
        }

        private List<Tuple<double, double>> GetLinePoints(double x0, double y0, double x1, double y1)
        {
            List<Tuple<double, double>> points = new List<Tuple<double, double>>();

            // Bresenham's line algorithm with doubleing point coordinates
            bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep)
            {
                double temp = x0;
                x0 = y0;
                y0 = temp;

                temp = x1;
                x1 = y1;
                y1 = temp;
            }

            if (x0 > x1)
            {
                double temp = x0;
                x0 = x1;
                x1 = temp;

                temp = y0;
                y0 = y1;
                y1 = temp;
            }

            double dx = x1 - x0;
            double dy = Math.Abs(y1 - y0);

            double error = dx / 2.0f;
            int ystep = (y0 < y1) ? 1 : -1;
            double y = y0;

            for (double x = x0; x <= x1; x++)
            {
                points.Add(steep ? new Tuple<double, double>(y, x) : new Tuple<double, double>(x, y));

                error -= dy;
                if (error < 0)
                {
                    y += ystep;
                    error += dx;
                }
            }

            return points;
        }

        public override string ToString()
        {
            char[] row = new char[width];
            string result = "";
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    row[j] = mapGrid[i, j];
                }
                result += new string(row) + "\n";
            }
            return result;
        }
    }

}
