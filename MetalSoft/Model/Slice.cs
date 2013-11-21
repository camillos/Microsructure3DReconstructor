using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MetalSoft.Model
{
    public class Slice
    {
        private List<SeedSection> _seeds;
        private Bitmap _sliceBitmap;
        private int _w, _h;
        private int _minPoints = 5;
        private bool[,] _mask;

        public Slice CreateCopyToMatch()
        {
            Slice copy = new Slice();
            copy._seeds = new List<SeedSection>();
            for (int i = 0; i < this._seeds.Count; i++)
            {
                copy._seeds.Add(this._seeds[i]);
            }
            copy._sliceBitmap = this._sliceBitmap;
            copy._w = this._w;
            copy._h = this._h;
            copy._minPoints = this._minPoints;
            copy._mask = this._mask;

            return copy;
        }

        private Slice() { }


        public Slice(Bitmap bm)
        {
            _sliceBitmap = bm;
            _w = _sliceBitmap.Width;
            _h = _sliceBitmap.Height;
            _mask = new bool[_w, _h];
            _seeds = new List<SeedSection>();

            for (int i = 0; i < _w; i++)
                for (int j = 0; j < _h; j++)
                    _mask[i, j] = true;

            // tworzenie ziaren
            for (int i = 0; i < _w; i++)
                for (int j = 0; j < _h; j++)
                {
                    if (_mask[i, j])
                    {
                        SeedSection seed = CreateSeed(i, j);

                        // sprawdzamy czy ziarno nie jest za mala
                        if (seed.PointsCount > _minPoints)
                        {
                            CreateCircuit(seed);
                            CreateInterPoints(seed, i, j);
                            _seeds.Add(seed);

                        }
                    }
                }
            SortSeed();

        }

        public int SeedCount
        {
            get { return _seeds.Count; }
        }
        private SeedSection CreateSeed(int x, int y)
        {

            SeedSection seed = new SeedSection();
            seed.Owner = this;
            List<Point> seedPoints = new List<Point>();           // lista punktow ziaren

            Queue<Point> qe = new Queue<Point>();           // kolejka punktow do przetworzenia
            bool change = false;
            int w = _sliceBitmap.Width; int h = _sliceBitmap.Height;

            seed.Width = w; seed.Height = h;

            qe.Enqueue(new Point(x, y));
            //_points[x, y].Owner = seed;
            _mask[x, y] = false;
            Color colorKey = _sliceBitmap.GetPixel(x, y);
            seed.SeedColor = colorKey;

            Point my = qe.Peek();

            int nx = my.X, ny = my.Y;

            while (qe.Count > 0)
            {
                change = false;

                // sasiad na lewo
                nx = my.X + 1; ny = my.Y; // dla lewego
                if (nx >= 0 && nx < w && ny >= 0 && ny < h && _sliceBitmap.GetPixel(my.X + 1, my.Y) == colorKey && _mask[my.X + 1, my.Y] == true)
                {

                    qe.Enqueue(new Point(my.X + 1, my.Y));
                    _mask[my.X + 1, my.Y] = false;
                    change = true;
                }
                else
                {
                    // sasiad na prawo
                    nx = my.X - 1; ny = my.Y; // dla prawego
                    if (nx >= 0 && nx < w && ny >= 0 && ny < h && _sliceBitmap.GetPixel(my.X - 1, my.Y) == colorKey && _mask[my.X - 1, my.Y] == true)
                    {

                        qe.Enqueue(new Point(my.X - 1, my.Y));
                        _mask[my.X - 1, my.Y] = false;
                        change = true;
                    }
                    else
                    {
                        // sasiad w dol
                        nx = my.X; ny = my.Y + 1; // dla dolnego
                        if (nx >= 0 && nx < w && ny >= 0 && ny < h && _sliceBitmap.GetPixel(my.X, my.Y + 1) == colorKey && _mask[my.X, my.Y + 1] == true)
                        {

                            qe.Enqueue(new Point(my.X, my.Y + 1));
                            _mask[my.X, my.Y + 1] = false;
                            change = true;
                        }
                        else
                        {
                            // sasiad w gore
                            nx = my.X; ny = my.Y - 1; // dla gornego
                            if (nx >= 0 && nx < w && ny >= 0 && ny < h && _sliceBitmap.GetPixel(my.X, my.Y - 1) == colorKey && _mask[my.X, my.Y - 1] == true)
                            {
                                qe.Enqueue(new Point(my.X, my.Y - 1));
                                _mask[my.X, my.Y - 1] = false;
                                change = true;
                            }
                        }
                    }
                }

                if (change == false)
                {
                    seedPoints.Add(qe.Dequeue());
                    if (qe.Count > 0)
                        my = qe.Peek();
                }
            }

            seed.SetPointsList(seedPoints);
            //seed.OnFinished();

            return seed;
        }

        #region OBOWOD - NIEUYWANE

        private void CreateCircuit(SeedSection seed)
        {
            // punkt startowy
            Turtle tt = new Turtle();
            tt.x = seed.GetFirstPoint.X;
            tt.y = seed.GetFirstPoint.Y;

            // dlugosc obwodu
            int edge = 0;

            //poczatek skierowany w lewo
            tt.dx = -1; tt.dy = 0;

            Color c = _sliceBitmap.GetPixel(tt.x, tt.y);
            List<Point> lp = new List<Point>();

            //przesówamy w lewo
            while (compareColor(tt.nextX(), tt.nextY(), c))
                tt.move();

            Point start = new Point(tt.x, tt.y);

            //obracamy w prawo
            tt.turnRight();
            lp.Add(new Point(tt.x, tt.y));



            do
            {
                dir direction = dir.straight;

                // kierunke prosto
                if (compareColor(tt.nextX(), tt.nextY(), c))
                {
                    direction = dir.straight;
                }

                // skret w prawo
                if ((!compareColor(tt.nextX(), tt.nextY(), c)) && (!compareColor(tt.leftSideX(), tt.leftSideY(), c)))
                {
                    direction = dir.right;
                }

                // skret w lewo
                if (compareColor(tt.leftSideX(), tt.leftSideY(), c))
                {
                    direction = dir.left;
                }

                switch (direction)
                {
                    case dir.straight:
                        {
                            tt.move();
                            lp.Add(new Point(tt.x, tt.y));
                            edge++;
                            break;
                        }
                    case dir.left:
                        {
                            tt.turnLeft();
                            //lp.Add(new Point(tt.x, tt.y));
                            tt.move();
                            break;
                        }
                    case dir.right:
                        {
                            tt.turnRight();
                            edge++;
                            break;
                        }
                }


            } while (!(start.X == tt.x && start.Y == tt.y && lp.Count > 1));
            seed.SetCircuitList(lp);
            seed.EdgeCount = edge;
        }
        private struct Turtle
        {
            public int x, y, dx, dy;
            public void move()
            {
                x = x + dx;
                y = y + dy;
            }
            public void turnRight()
            {
                if (dx == 1)
                {
                    dx = 0; dy = 1;
                    return;
                }
                if (dy == 1)
                {
                    dy = 0; dx = -1;
                    return;
                }
                if (dx == -1)
                {
                    dx = 0; dy = -1;
                    return;
                }
                if (dy == -1)
                {
                    dy = 0; dx = 1;
                    return;
                }
            }
            public void turnLeft()
            {
                if (dx == 1)
                {
                    dx = 0; dy = -1;
                    return;
                }
                if (dy == -1)
                {
                    dy = 0; dx = -1;
                    return;
                }
                if (dx == -1)
                {
                    dx = 0; dy = 1;
                    return;
                }
                if (dy == 1)
                {
                    dy = 0; dx = 1;
                    return;
                }
            }
            public int nextX()
            {
                return x + dx;
            }
            public int nextY()
            {
                return y + dy;
            }
            public int leftSideX()
            {
                // w prawo
                if (dx == 1 && dy == 0)
                    return x;
                // w lewo
                if (dx == -1 && dy == 0)
                    return x;
                // w dol
                if (dx == 0 && dy == 1)
                    return x + 1;
                // w gore
                //if (dx == 0 && dy == -1)
                return x - 1;
            }
            public int leftSideY()
            {
                // w prawo
                if (dx == 1 && dy == 0)
                    return y - 1;
                // w lewo
                if (dx == -1 && dy == 0)
                    return y + 1;
                // w dol
                if (dx == 0 && dy == 1)
                    return y;
                // w gore
                //if (dx == 0 && dy == -1)
                return y;
            }
            public int leftSideNextX()
            {
                // w prawo
                if (dx == 1 && dy == 0)
                    return x + 1;
                // w lewo
                if (dx == -1 && dy == 0)
                    return x - 1;
                // w dol
                if (dx == 0 && dy == 1)
                    return x + 1;
                // w gore
                //if (dx == 0 && dy == -1)
                return x - 1;
            }
            public int leftSideNextY()
            {
                // w prawo
                if (dx == 1 && dy == 0)
                    return y - 1;
                // w lewo
                if (dx == -1 && dy == 0)
                    return y + 1;
                // w dol
                if (dx == 0 && dy == 1)
                    return y + 1;
                // w gore
                //if (dx == 0 && dy == -1)
                return y - 1;
            }
            public int rightSideX()
            {
                if (dy == -1 && dx == 0)
                    return x + 1;
                if (dy == 1 && dx == 0)
                    return x - 1;
                if (dy == 0 && dx == 1)
                    return x;
                // if(dy = 0 && dx = -1)
                return x;

            }
            public int rightSideY()
            {
                if (dy == -1 && dx == 0)
                    return y;
                if (dy == 1 && dx == 0)
                    return y;
                if (dy == 0 && dx == 1)
                    return y + 1;
                // if(dy == 0 && dx == -1)
                return y - 1;
            }
            public int rightSideNextX()
            {
                if (dy == -1 && dx == 0)
                    return x + 1;
                if (dy == 1 && dx == 0)
                    return x - 1;
                if (dy == 0 && dx == 1)
                    return x + 1;
                // if(dy = 0 && dx = -1)
                return x - 1;

            }
            public int rightSideNextY()
            {
                if (dy == -1 && dx == 0)
                    return y - 1;
                if (dy == 1 && dx == 0)
                    return y + 1;
                if (dy == 0 && dx == 1)
                    return y + 1;
                // if(dy == 0 && dx == -1)
                return y - 1;
            }

        }
        private enum dir
        {
            left,
            right,
            straight
        }

        // nowa wersja checkPixel
        // 
        // true - te same kolory pikxeli 
        // false - rozne kolory
        private bool compareColor(int x, int y, Color expample)
        {
            if (x >= _w || x < 0)
                return false;
            if (y >= _h || y < 0)
                return false;

            if (_sliceBitmap.GetPixel(x, y) == expample)
                return true;
            return false;
        }

        #endregion

        public SeedSection GetSeed(int i)
        {
            return _seeds[i];
        }
        public void RemoveSeed(int i)
        {
            if (i < 0 || i > _seeds.Count) return;
            _seeds.RemoveAt(i);
        }
        public void SortSeed()
        {
            int count = _seeds.Count;
            bool p;

            for (int j = count - 1; j > 0; j--)
            {
                p = false;
                for (int i = 0; i < j; i++)
                    if (_seeds[i].PointsCount < _seeds[i + 1].PointsCount)
                    {
  
                        SeedSection seed = _seeds[i];
                        _seeds[i] = _seeds[i + 1];
                        _seeds[i + 1] = seed;

                        p = true;
                    }
                if (!p) break;
            }
        }

        public Bitmap SliceBitmap { get { return _sliceBitmap; } }

        public void CreateInterPoints(SeedSection seed, int x, int y)
        {
            ShapeFactor factor = new ShapeFactor(seed);
            PointF centroid = factor.Centroid();

            int n = (int)(360.0 / Config.InterpolationAngle);

            int x0 = (int)centroid.X;
            int y0 = (int)centroid.Y;

            int r = 50;

            string s = string.Empty;

            //Bitmap bm = new Bitmap(200, 200);
            //bm.SetPixel(x0, y0, Color.Red);

            List<Point> _interPoints = new List<Point>();
            List<double> segment = new List<double>();

            double segmentMax = 0;

            // wyznacza kolejne punkty
            for (int i = 0; i < n; i++)
            {
                int xi = (int)(x0 + (r * Math.Cos((2 * Math.PI * i) / n)));
                int yi = (int)(y0 + (r * Math.Sin((2 * Math.PI * i) / n)));

                _interPoints.Add(new Point(xi, yi));


                double seg = Lenght(new Point(x0, y0), new Point(xi, yi), _sliceBitmap, _sliceBitmap.GetPixel(x, y));
                segment.Add(seg);

                if (seg > segmentMax) segmentMax = seg;

            }


            // normalizacja segmentow

            if (segmentMax == 0) segmentMax = 1;

            for (int i = 0; i < segment.Count; i++)
                segment[i] = segment[i] / segmentMax;


            seed.MaxSegment = (int)segmentMax;

            seed.InterPoints = _interPoints;
            seed.Segments = segment;

        }

        private static int Lenght(Point a, Point b, Bitmap img, Color c)
        {
            int cout = 0;



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
            if (x >= 0 && x < img.Width && y >= 0 && y < img.Height && img.GetPixel(x, y) == c)
                cout++;
            else
                return cout;


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
                    if (x >= 0 && x < img.Width && y >= 0 && y < img.Height && img.GetPixel(x, y) == c)
                        cout++;
                    else
                        return cout;
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
                    if (x >= 0 && x < img.Width && y >= 0 && y < img.Height && img.GetPixel(x, y) == c)
                        cout++;
                    else
                        return cout;
                }
            }

            return cout;
        }
    }
}
