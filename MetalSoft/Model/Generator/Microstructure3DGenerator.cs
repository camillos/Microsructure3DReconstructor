using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MetalSoft.Model.Generator
{
    public class Connection
    {
        public int Index1 { get; set; }
        public int Index2 { get; set; }

        public Connection(int i1, int i2)
        {
            Index1 = i1;
            Index2 = i2;
        }

    }

    public class PolarPoint
    {
        double fi;
        double r;

        public double Fi { get { return fi; } }
        public double R { get { return r; } }


        public PolarPoint(double r, double fi)
        {
            this.r = r;
            this.fi = fi;
        }

        public static PolarPoint CreateFromCartesian(double x, double y)
        {
            double R = 0;
            double Fi = 0;

            R = Math.Sqrt((x * x) + (y * y));

            if (R == 0) { return new PolarPoint(R, Fi); }

            if (x > 0 && y >= 0) Fi = Math.Atan(y / x) * (180 / Math.PI);
            else if (x > 0 && y < 0) Fi = Math.Atan(y / x) * (180 / Math.PI) + 360;
            else if (x < 0) Fi = Math.Atan(y / x) * (180 / Math.PI) + 180;
            else if (x == 0 && y > 0) Fi = 90;
            else if (x == 0 && y < 0) Fi = 3 * 180 / 2;

            return new PolarPoint(R, Fi);

        }
    }

    public class Point3D
    {
        public int X, Y, Z;
        public int R, G, B;

        public int GlobalID { get; set; }


        public Point3D(Point3D p)
        {
            this.X = p.X;
            this.Y = p.Y;
            this.Z = p.Z;

            this.R = p.R;
            this.G = p.G;
            this.B = p.B;

            this.GlobalID = p.GlobalID;

        }


        public Point3D()
        {

            R = 0;
            G = 0;
            B = 0;
        }

        public Point3D(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;

            R = 0;
            G = 0;
            B = 0;
        }

        public Point3D(int x, int y, int z, int r, int g, int b)
        {
            X = x;
            Y = y;
            Z = z;

            R = r;
            G = g;
            B = b;
        }

        public Point3D(int x, int y, int z, int id, int r, int g, int b)
        {
            X = x;
            Y = y;
            Z = z;

            GlobalID = id;

            R = r;
            G = g;
            B = b;
        }


    }



    public class Microstructure3DGenerator
    {
        SeedSection top;
        SeedSection bottom;

        public List<Point3D> vertex;
        public List<Connection> connections;

        private int lastSectionStart = 0;

        public Microstructure3DGenerator()
        {
            vertex = new List<Point3D>();
            connections = new List<Connection>();
        }

        public void CreateLinearConnection3(SeedSection s1, SeedSection s2)
        {

            top = s1;
            bottom = s2;

            PointF topConetroid, bottomCentroid;
            Point centroid = new Point(0, 0);
            List<Point> topCircuit, bottomCircuit;
            List<Point> topPoints, bottomPoints;

            ShapeFactor sf = new ShapeFactor(top);
            topConetroid = sf.Centroid();

            sf = new ShapeFactor(bottom);
            bottomCentroid = sf.Centroid();


            // przesuwamy współrzedne do wspólnego środka (0,0)
            topCircuit = new List<Point>();
            for (int i = 0; i < top.CircuitCount; i++)
            {
                Point temp = top.GetCircuitPoint(i);
                topCircuit.Add(new Point((int)(temp.X - topConetroid.X + 0.5), (int)(temp.Y - topConetroid.Y + 0.5)));
            }

            bottomCircuit = new List<Point>();
            for (int i = 0; i < bottom.CircuitCount; i++)
            {
                Point temp = bottom.GetCircuitPoint(i);
                bottomCircuit.Add(new Point((int)(temp.X - bottomCentroid.X + 0.5), (int)(temp.Y - bottomCentroid.Y + 0.5)));
            }

            int startTop = GetStartIndex(topCircuit);
            int startBottom = GetStartIndex(bottomCircuit);

            topPoints = new List<Point>();
            bottomPoints = new List<Point>();

            for (int i = startTop; i < topCircuit.Count; i++)
                topPoints.Add(topCircuit[i]);
            for (int i = 0; i < startTop; i++)
                topPoints.Add(topCircuit[i]);

            for (int i = startBottom; i < bottomCircuit.Count; i++)
                bottomPoints.Add(bottomCircuit[i]);
            for (int i = 0; i < startBottom; i++)
                bottomPoints.Add(bottomCircuit[i]);

            double threshold = 0;
            int connectionCount = 0;

            List<Point> less, more;

            if (topPoints.Count > bottomPoints.Count)
            {
                threshold = (double)topCircuit.Count / bottomPoints.Count;
                connectionCount = topPoints.Count;
                more = topPoints;
                less = bottomPoints;
            }
            else
            {
                threshold = (double)bottomPoints.Count / topPoints.Count;
                connectionCount = bottomPoints.Count;
                more = bottomPoints;
                less = topPoints;
            }

            List<Connection> conn = new List<Connection>();
            int[,] connection = new int[2, connectionCount];

            //bool createConn = true;
            int connIndex = 0;
            int lessIndex = 0;
            int morIndex = 0;

            int vertexCount = vertex.Count;

            double sumLess = 0;
            double sumMore = 0;

            while (lessIndex < less.Count && morIndex < more.Count)
            {
                if (sumMore <= sumLess + 1)
                {

                    conn.Add(new Connection(vertexCount + lessIndex, vertexCount + less.Count + morIndex));
                    lessIndex++;
                    morIndex++;

                    sumLess += 1;
                    sumMore += threshold;

                }
                else
                {
                    conn.Add(new Connection(vertexCount + lessIndex, vertexCount + less.Count + morIndex));
                    morIndex++;

                    sumLess += 1;
                    sumMore += threshold;
                    sumMore -= 1;
                }

            }

            while (morIndex < more.Count)
            {
                conn.Add(new Connection(vertexCount + less.Count - 1, vertexCount + less.Count + morIndex));
                morIndex++;
            }

            while (lessIndex < less.Count)
            {
                conn.Add(new Connection(vertexCount + lessIndex, vertexCount + less.Count + more.Count - 1));
                lessIndex++;
            }
        }

        public void CreateLinearConnection4(List<SeedSection> seeds)
        {
            if (seeds.Count == 1)
            {
                AddToVertex(seeds[0].Circuit, 0);
                return;
            }

            for (int s = 0; s < seeds.Count - 1; s++)
            {
                top = seeds[s];
                bottom = seeds[s + 1];

                PointF topConetroid, bottomCentroid;
                //Point centroid = new Point(0, 0);
                List<Point> topCircuit, bottomCircuit;
                List<Point> topPoints, bottomPoints;

                ShapeFactor sf = new ShapeFactor(top);
                topConetroid = sf.Centroid();

                sf = new ShapeFactor(bottom);
                bottomCentroid = sf.Centroid();


                // przesuwamy współrzedne do wspólnego środka (0,0)
                topCircuit = new List<Point>();
                for (int i = 0; i < top.CircuitCount; i++)
                {
                    Point temp = top.GetCircuitPoint(i);
                    topCircuit.Add(new Point((int)(temp.X - topConetroid.X + 0.5), (int)(temp.Y - topConetroid.Y + 0.5)));
                }

                bottomCircuit = new List<Point>();
                for (int i = 0; i < bottom.CircuitCount; i++)
                {
                    Point temp = bottom.GetCircuitPoint(i);
                    bottomCircuit.Add(new Point((int)(temp.X - bottomCentroid.X + 0.5), (int)(temp.Y - bottomCentroid.Y + 0.5)));
                }

                // kolejkujemy punkty na obwodzie
                int startTop = GetStartIndex(topCircuit);
                int startBottom = GetStartIndex(bottomCircuit);

                topPoints = new List<Point>();
                bottomPoints = new List<Point>();

                for (int i = startTop; i < topCircuit.Count; i++)
                    topPoints.Add(top.GetCircuitPoint(i));
                for (int i = 0; i < startTop; i++)
                    topPoints.Add(top.GetCircuitPoint(i));

                for (int i = startBottom; i < bottomCircuit.Count; i++)
                    bottomPoints.Add(bottom.GetCircuitPoint(i));
                for (int i = 0; i < startBottom; i++)
                    bottomPoints.Add(bottom.GetCircuitPoint(i));


                top.OrderCircuit = topPoints;
                bottom.OrderCircuit = bottomPoints;


                // wyliczamy progi indexów dla top i botton
                double thresholdTop = 0;
                double thresholdBottom = 0;

                //List<Point> less, more;

                if (topPoints.Count > bottomPoints.Count)
                {
                    thresholdTop = (double)topCircuit.Count / bottomPoints.Count;
                    thresholdBottom = 1;
                }
                else
                {
                    thresholdBottom = (double)bottomPoints.Count / topPoints.Count;
                    thresholdTop = 1;
                }


                List<Connection> conn = new List<Connection>();
                int topIndex = 0;
                int bottomIndex = 0;

                double topSum = 0;
                double bottomSum = 0;


                while (topIndex < topPoints.Count && bottomIndex < bottomPoints.Count)
                {
                    if (topPoints.Count > bottomPoints.Count)
                    {
                        if (topSum <= bottomSum + 1)
                        {
                            conn.Add(new Connection(lastSectionStart + topIndex, lastSectionStart + topPoints.Count + bottomIndex));
                            topIndex++;
                            bottomIndex++;

                            topSum += thresholdTop;
                            bottomSum += thresholdBottom;
                        }
                        else
                        {
                            conn.Add(new Connection(lastSectionStart + topIndex, lastSectionStart + topPoints.Count + bottomIndex));
                            topIndex++;

                            topSum += thresholdTop;
                            topSum -= (int)thresholdTop;   // -= 1
                            bottomSum += thresholdBottom;
                        }
                    }
                    else
                    {
                        if (bottomSum <= topSum + 1)
                        {
                            conn.Add(new Connection(lastSectionStart + topIndex, lastSectionStart + topPoints.Count + bottomIndex));
                            topIndex++;
                            bottomIndex++;

                            topSum += thresholdTop;
                            bottomSum += thresholdBottom;
                        }
                        else
                        {
                            conn.Add(new Connection(lastSectionStart + topIndex, lastSectionStart + topPoints.Count + bottomIndex));
                            bottomIndex++;

                            topSum += thresholdTop;

                            bottomSum += thresholdBottom;
                            bottomSum -= (int)thresholdBottom; // -=1
                        }

                    }
                }

                while (topIndex < topPoints.Count)
                {
                    conn.Add(new Connection(lastSectionStart + topIndex, lastSectionStart + topPoints.Count + bottomPoints.Count - 1));
                    topIndex++;
                }

                while (bottomIndex < bottomPoints.Count)
                {
                    conn.Add(new Connection(lastSectionStart + topPoints.Count - 1, lastSectionStart + topPoints.Count + bottomIndex));
                    bottomIndex++;
                }



                if (s == 0)
                    AddToVertex(topPoints, s);

                lastSectionStart = vertex.Count;
                AddToVertex(bottomPoints, s + 1);

                connections.AddRange(conn);
            }
        }

        public void CreateLinearConnection4(SeedSection seedTop, SeedSection seedBottom)
        {
            top = seedTop;
            bottom = seedBottom;

            PointF topConetroid, bottomCentroid;
            //Point centroid = new Point(0, 0);
            List<Point> topCircuit, bottomCircuit;
            List<Point> topPoints, bottomPoints;

            ShapeFactor sf = new ShapeFactor(top);
            topConetroid = sf.Centroid();

            sf = new ShapeFactor(bottom);
            bottomCentroid = sf.Centroid();


            // przesuwamy współrzedne do wspólnego środka (0,0)
            topCircuit = new List<Point>();
            for (int i = 0; i < top.CircuitCount; i++)
            {
                Point temp = top.GetCircuitPoint(i);
                topCircuit.Add(new Point((int)(temp.X - topConetroid.X + 0.5), (int)(temp.Y - topConetroid.Y + 0.5)));
            }

            bottomCircuit = new List<Point>();
            for (int i = 0; i < bottom.CircuitCount; i++)
            {
                Point temp = bottom.GetCircuitPoint(i);
                bottomCircuit.Add(new Point((int)(temp.X - bottomCentroid.X + 0.5), (int)(temp.Y - bottomCentroid.Y + 0.5)));
            }

            // kolejkujemy punkty na obwodzie
            int startTop = GetStartIndex(topCircuit);
            int startBottom = GetStartIndex(bottomCircuit);

            topPoints = new List<Point>();
            bottomPoints = new List<Point>();

            for (int i = startTop; i < topCircuit.Count; i++)
                topPoints.Add(top.GetCircuitPoint(i));
            for (int i = 0; i < startTop; i++)
                topPoints.Add(top.GetCircuitPoint(i));

            for (int i = startBottom; i < bottomCircuit.Count; i++)
                bottomPoints.Add(bottom.GetCircuitPoint(i));
            for (int i = 0; i < startBottom; i++)
                bottomPoints.Add(bottom.GetCircuitPoint(i));


            top.OrderCircuit = topPoints;
            bottom.OrderCircuit = bottomPoints;


            // wyliczamy progi indexów dla top i botton
            double thresholdTop = 0;
            double thresholdBottom = 0;


            if (topPoints.Count > bottomPoints.Count)
            {
                thresholdTop = (double)topCircuit.Count / bottomPoints.Count;
                thresholdBottom = 1;

            }
            else
            {
                thresholdBottom = (double)bottomPoints.Count / topPoints.Count;
                thresholdTop = 1;

            }


            List<Connection> conn = new List<Connection>();
            int topIndex = 0;
            int bottomIndex = 0;

            double topSum = 0;
            double bottomSum = 0;

            lastSectionStart = 0;


            while (topIndex < topPoints.Count && bottomIndex < bottomPoints.Count)
            {
                if (topPoints.Count > bottomPoints.Count)
                {
                    if (topSum <= bottomSum + 1)
                    {
                        conn.Add(new Connection(lastSectionStart + topIndex, lastSectionStart + topPoints.Count + bottomIndex));
                        topIndex++;
                        bottomIndex++;

                        topSum += thresholdTop;
                        bottomSum += thresholdBottom;
                    }
                    else
                    {
                        conn.Add(new Connection(lastSectionStart + topIndex, lastSectionStart + topPoints.Count + bottomIndex));
                        topIndex++;

                        topSum += thresholdTop;
                        topSum -= (int)thresholdTop;   // -= 1
                        bottomSum += thresholdBottom;
                    }
                }
                else
                {
                    if (bottomSum <= topSum + 1)
                    {
                        conn.Add(new Connection(lastSectionStart + topIndex, lastSectionStart + topPoints.Count + bottomIndex));
                        topIndex++;
                        bottomIndex++;

                        topSum += thresholdTop;
                        bottomSum += thresholdBottom;
                    }
                    else
                    {
                        conn.Add(new Connection(lastSectionStart + topIndex, lastSectionStart + topPoints.Count + bottomIndex));
                        bottomIndex++;

                        topSum += thresholdTop;

                        bottomSum += thresholdBottom;
                        bottomSum -= (int)thresholdBottom; // -=1
                    }

                }
            }

            while (topIndex < topPoints.Count)
            {
                conn.Add(new Connection(lastSectionStart + topIndex, lastSectionStart + topPoints.Count + bottomPoints.Count - 1));
                topIndex++;
            }

            while (bottomIndex < bottomPoints.Count)
            {
                conn.Add(new Connection(lastSectionStart + topPoints.Count - 1, lastSectionStart + topPoints.Count + bottomIndex));
                bottomIndex++;
            }


            AddToVertex(topPoints, 0);
            lastSectionStart = vertex.Count;
            AddToVertex(bottomPoints, 1);

            connections.AddRange(conn);

        }



        private void AddToVertex(List<Point> v, int sliceIndex)
        {
            foreach (Point p in v)
            {
                vertex.Add(new Point3D(p.X, p.Y, sliceIndex * 10));
            }
        }




        private int GetStartIndex(List<Point> seed)
        {
            int index = 0;
            int y = int.MinValue;

            for (int i = 0; i < seed.Count; i++)
            {
                if (seed[i].X != 0) continue;

                if (seed[i].Y > y)
                {
                    index = i;
                    y = seed[i].Y;
                }

            }

            return index;
        }

        private double Length(Point a, Point b)
        {
            return Math.Sqrt(Math.Pow(b.X - a.X, 2) + Math.Pow(b.Y - a.Y, 2));
        }

        public List<Point3D> fillPoints = new List<Point3D>();
        Color color;

        public void Fill(List<SeedSection> seeds, int z, int dz)
        {
            color = seeds[0].SeedColor;

            fillPoints.Clear();

            int currentZ = z;

            // naprawienie bledu zwiazanego ze znikaniem ziaren jesli znaleziony byl tylko na jednym przekroju

            if (seeds.Count == 1)
            {
                FillSection(seeds[0], currentZ);
            }



            for (int i = 0; i < seeds.Count - 1; i++)
            {
                if (i == 0)
                    // wypełniamy przekrój na wysokosci z
                    FillSection(seeds[i], currentZ);

                vertex.Clear();
                connections.Clear();

                CreateLinearConnection4(seeds[i], seeds[i + 1]);

                // wypełniamy wszystkie przekroje do wysokosci < z + dz
                for (int j = currentZ + 1; j < currentZ + dz; j++)
                {
                    // tworzymy punkty konturu
                    List<Point> circuit = new List<Point>();

                    foreach (Connection con in connections)
                    {
                        Point3D topVertex = vertex[con.Index1];
                        Point3D bottomVertex = vertex[con.Index2];

                        topVertex.Z = currentZ;
                        bottomVertex.Z = currentZ + dz;

                        Point3D circuitVertex = GetPointFromLine(topVertex, bottomVertex, j);

                        if (circuitVertex != null)
                            circuit.Add(new Point(circuitVertex.X, circuitVertex.Y));
                    }

                    /////////////////////////////////////////////////////

                    //brakuje sprawdzenia spujnosci konturu

                    List<Point> filledCircuit = new List<Point>();
                    for (int c = 0; c < circuit.Count - 1; c++)
                    {
                        List<Point> newPoints = FillCircuitByBresenham(circuit[c], circuit[c + 1]);

                        if (newPoints.Count > 0)
                        {
                            if (filledCircuit.Count > 0)
                            {
                                if (filledCircuit.Last().X != circuit[c].X && filledCircuit.Last().Y != circuit[c].Y)
                                    filledCircuit.Add(circuit[c]);
                            }
                            else
                            {
                                filledCircuit.Add(circuit[c]);
                            }

                            filledCircuit.AddRange(newPoints);

                            filledCircuit.Add(circuit[c + 1]);
                        }
                        else
                        {
                            filledCircuit.Add(circuit[c]);
                            filledCircuit.Add(circuit[c + 1]);
                        }
                    }

                    Point first = circuit[0];
                    Point last = circuit[circuit.Count - 1];
                    List<Point> newPoints2 = FillCircuitByBresenham(first, last);

                    if (newPoints2.Count > 0)
                    {
                        if (filledCircuit.Count > 0)
                        {
                            if (filledCircuit.Last().X != last.X && filledCircuit.Last().Y != last.Y)
                                filledCircuit.Add(last);
                        }
                        else
                        {
                            filledCircuit.Add(last);
                        }

                        filledCircuit.AddRange(newPoints2);

                        if (filledCircuit.Count > 0)
                        {
                            if (filledCircuit.First().X != first.X && filledCircuit.First().Y != first.Y)
                                filledCircuit.Add(first);
                        }
                        else
                        {
                            filledCircuit.Add(first);
                        }
                    }
                    else
                    {
                        filledCircuit.Add(last);
                        filledCircuit.Add(first);
                    }

                    circuit = filledCircuit;


                    ////////////////////////////////////////////////////////////////////////////

                    SeedSection seed = new SeedSection();
                    seed.Circuit = circuit;
                    seed.MicrostructureContainer = seeds[i].MicrostructureContainer;

                    FillSection(seed, j);



                }

                // narysowanie ostatniego przekroju
                FillSection(seeds[i + 1], currentZ + dz);

                currentZ += dz;

            }





        }

        private List<Point> FillCircuitByBresenham(Point a, Point b)
        {
            List<Point> newPoint = new List<Point>();

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
            //if (x < bm.Width && x >= 0 && y < bm.Height && y >= 0)
            //    bm.SetPixel(x, y, c);

            if ((x != a.X && y != a.Y) || (x != b.X && y != b.Y))
                newPoint.Add(new Point(x, y));

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
                    //if (x < bm.Width && x >= 0 && y < bm.Height && y >= 0)
                    //    bm.SetPixel(x, y, c);

                    if ((x != a.X && y != a.Y) || (x != b.X && y != b.Y))
                        newPoint.Add(new Point(x, y));
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
                    //if (x < bm.Width && x >= 0 && y < bm.Height && y >= 0)
                    //    bm.SetPixel(x, y, c);

                    if ((x != a.X && y != a.Y) || (x != b.X && y != b.Y))
                        newPoint.Add(new Point(x, y));
                }
            }

            return newPoint;
        }

        private Point3D GetPointFromLine(Point3D a, Point3D b, double z)
        {

            Point3D r = new Point3D(b.X - a.X, b.Y - a.Y, b.Z - a.Z);
            if (r.Z == 0) return null;

            double m = (double)(z - a.Z) / r.Z;

            double x = a.X + m * r.X;
            double y = a.Y + m * r.Y;

            return new Point3D((int)(x + 0.5), (int)(y + 0.5), (int)z);


        }

        int fileName = 0;
        private void FillSection(SeedSection currentSection, int currentZ)
        {

            ShapeFactor sf = new ShapeFactor(currentSection);
            PointF centroid = sf.CentroidFromCircuit();

            int x = (int)(centroid.X + 0.5);
            int y = (int)(centroid.Y + 0.5);

            int _w = Helper.MaxX;
            int _h = Helper.MaxY;

            Bitmap _sliceBitmap = new Bitmap(_w, _h);

            using (Graphics g = Graphics.FromImage(_sliceBitmap))
            {
                g.Clear(Color.White);    
                g.FillPolygon(new SolidBrush(Color.Red), currentSection.Circuit.ToArray());
            }

           
            bool runPart = false;
            List<Point3D> tempFill = new List<Point3D>();


            int x_min = currentSection.Circuit.Min(p => p.X);
            int x_max = currentSection.Circuit.Max(p => p.X);

            int y_min = currentSection.Circuit.Min(p => p.Y);
            int y_max = currentSection.Circuit.Max(p => p.Y);


            for (int i = x_min; i < x_max; i++)
            {
                for (int j = y_min; j < y_max; j++)
                {
                    Color tempColor = _sliceBitmap.GetPixel(i, j);

                    if (tempColor.R == Color.Red.R &&
                        tempColor.G == Color.Red.G &&
                        tempColor.B == Color.Red.B)
                    {
                        tempFill.Add(new Point3D(i, j, currentZ, color.R, color.G, color.B));
                    }
                }
            }


            foreach (Point3D p in tempFill)
            {
                p.GlobalID = currentSection.MicrostructureContainer.ID;
            }


            fillPoints.AddRange(tempFill);

        }

        private Point GetPointInside(List<Point> circuit)
        {
            Point result = new Point(-1, -1);

            int x_min = circuit.Min(p => p.X);
            int x_max = circuit.Max(p => p.X);

            int y_min = circuit.Min(p => p.Y);
            int y_max = circuit.Max(p => p.Y);

            for (int point_x = x_min; point_x <= x_max; point_x++)
                for (int point_y = y_min; point_y <= y_max; point_y++)
                {


                    List<Point> crossXright = circuit.Where((p => p.Y == point_y && p.X > point_x))
                                                .DistinctBy(p => p.X)
                                                .Select(p => p)
                                                .ToList();

                    List<Point> crossXleft = circuit.Where((p => p.Y == point_y && p.X < point_x))
                                                .DistinctBy(p => p.X)
                                                .Select(p => p)
                                                .ToList();

                    List<Point> crossYup = circuit.Where((p => p.X == point_x && p.Y < point_y))
                                                .DistinctBy(p => p.Y)
                                                .Select(p => p)
                                                .ToList();

                    List<Point> crossYdown = circuit.Where((p => p.X == point_x && p.Y > point_y))
                                                .DistinctBy(p => p.Y)
                                                .Select(p => p)
                                                .ToList();

                    // jesli srodek ciezkosci znajduje sie w obiekcie
                    if ((crossXright.Count > 0 && crossXright.Count % 2 == 1)
                        && (crossXleft.Count > 0 && crossXleft.Count % 2 == 1))
                    {
                        if ((crossYup.Count > 0 && crossYup.Count % 2 == 1)
                            && (crossYdown.Count > 0 && crossYdown.Count % 2 == 1))
                        {
                            result = new Point(point_x, point_y);
                            return result;
                        }
                    }
                }

            return result;
        }

    }
}
