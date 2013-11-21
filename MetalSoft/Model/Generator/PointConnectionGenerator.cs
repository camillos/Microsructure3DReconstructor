using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MetalSoft.Model.Generator
{

    public class CirclePoint
    {
        public int Id { get; set; }
        public int X { get;set; }
        public int Y { get; set; }
        public int Quarter { get; set; }

        public bool dirChange;
        public bool matched;

        public CirclePoint(int id, int x, int y)
        {
            Id = id;
            X = x;
            Y = y;

            if (x >= 0 && y >= 0) Quarter = 1;
            else if (x <= 0 && y >= 0) Quarter = 2;
            else if (x <= 0 && y <= 0) Quarter = 3;
            else if (x >= 0 && y <= 0) Quarter = 4;

            dirChange = false;
            matched = false;
        }

    }

    public class PointConnectionGenerator
    {
        double r;
        int pointCount;

        CirclePoint[] topPoints;
        CirclePoint[] bottomPoints;

        public CirclePoint[] TopPoints { get { return topPoints; } }
        public CirclePoint[] BottomPoints { get { return bottomPoints; } }


        public PointConnectionGenerator(double r, int pointCount)
        {
            this.r = r;
            this.pointCount = pointCount;

            topPoints = new CirclePoint[pointCount];
            bottomPoints = new CirclePoint[pointCount];


            for (int i = 0; i < pointCount; i++)
            {
                double x = r * Math.Cos(2 * Math.PI * i / pointCount) + 0.5;
                double y = r * Math.Sin(2 * Math.PI * i / pointCount) + 0.5;

                topPoints[i] = new CirclePoint(i, (int)x, (int)y);
                bottomPoints[i] = new CirclePoint(i, (int)x, (int)y);


            }
        }

        public void MatchToSegments(List<Point> top, List<Point> bottom)
        {

            for (int i = 0; i < pointCount; i++)
            {
                Point[] points = PointPath(new Point(0, 0), new Point (topPoints[i].X, topPoints[i].Y));
                points = points.Reverse().ToArray();

                foreach(Point p in points)
                {
                    if (top.Contains(p))
                    {
                        topPoints[i].X = p.X;
                        topPoints[i].Y = p.Y;
                        topPoints[i].matched = true;
                        break;
                    }
                }

                points = PointPath(new Point(0, 0), new Point(bottomPoints[i].X, bottomPoints[i].Y));
                points = points.Reverse().ToArray();

                foreach (Point p in points)
                {
                    if (bottom.Contains(p))
                    {
                        bottomPoints[i].X = p.X;
                        bottomPoints[i].Y = p.Y;
                        break;
                    }
                }
            }

            // dla tych których sie nie udało dopasować
            for (int i = 0; i < pointCount; i++)
            {
                if (topPoints[i].matched == true) continue;

                Point[] points = PointPath(new Point(0, 0), new Point(topPoints[i].X, topPoints[i].Y));
                points = points.Reverse().ToArray();

                //CirclePoint p = topPoints[i];
                Point matched = FindMatched(points, top);

                topPoints[i].X = matched.X;
                topPoints[i].Y = matched.Y;
                topPoints[i].matched = true;
            }

            for (int i = 0; i < pointCount; i++)
            {
                if (bottomPoints[i].matched == true) continue;

                Point[] points = PointPath(new Point(0, 0), new Point(bottomPoints[i].X, bottomPoints[i].Y));
                points = points.Reverse().ToArray();

                //CirclePoint p = topPoints[i];
                Point matched = FindMatched(points, bottom);

                bottomPoints[i].X = matched.X;
                bottomPoints[i].Y = matched.Y;
                bottomPoints[i].matched = true;
            }
        }

        private Point[] PointPath(Point a, Point b)
        {
            List<Point> points = new List<Point>();
          

            // zmienne pomocnicze
            int d, dx, dy, ai, bi, xi, yi;
            int x = a.X, y = a.Y;
            // ustalenie kierunku rysowania
            if (a.X < b.X)
            {
                xi = 1;
                dx = b.X - a.X;
            }
            else
            {
                xi = -1;
                dx = a.X - b.X;
            }
            // ustalenie kierunku rysowania
            if (a.Y < b.Y)
            {
                yi = 1;
                dy = b.Y - a.Y;
            }
            else
            {
                yi = -1;
                dy = a.Y - b.Y;
            }

            // pierwszy piksel
            //cout++;
            points.Add(new Point(x, y));

            // oś wiodąca OX
            if (dx > dy)
            {
                ai = (dy - dx) * 2;
                bi = dy * 2;
                d = bi - dx;
                // pętla po kolejnych x
                while (x != b.X)
                {
                    // test współczynnika
                    if (d >= 0)
                    {
                        x += xi;
                        y += yi;
                        d += ai;
                    }
                    else
                    {
                        d += bi;
                        x += xi;
                    }

                    //cout++;
                    points.Add(new Point(x, y));

                }
            }
            // oś wiodąca OY
            else
            {
                ai = (dx - dy) * 2;
                bi = dx * 2;
                d = bi - dy;
                // pętla po kolejnych y
                while (y != b.Y)
                {
                    // test współczynnika
                    if (d >= 0)
                    {
                        x += xi;
                        y += yi;
                        d += ai;
                    }
                    else
                    {
                        d += bi;
                        y += yi;
                    }

                    //cout++;
                    points.Add(new Point(x, y));

                }
            }

            return points.ToArray();
        }


        private bool Contain(SeedSection seed, Point point)
        {
            return seed.Circuit.Contains(point);
        }

        private Point FindMatched(Point[] path, List<Point> circuit)
        {
            Point result = new Point(0, 0);
            bool matching = true;

            int r = 2;

            //while (matching)
            {
                foreach (Point p in path)
                {
                    Point[] tab = new Point[4];

                    tab[0] = new Point(p.X - 1, p.Y - 1);
                    tab[1] = new Point(p.X + 1, p.Y - 1);

                    tab[2] = new Point(p.X - 1, p.Y + 1);
                    tab[3] = new Point(p.X + 1, p.Y + 1);

                    for (int i = 0; i < 4; i++)
                    {
                        if (circuit.Contains(tab[i]))
                        {
                            matching = false;
                            result = tab[i];
                            break;
                        }
                    }

                    if (!matching) break;
                }
            }

            return result;
        }
    }
}
