using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace MetalSoft.Model
{
    public class SeedSection
    {
        public int ID { get; set; }


        private List<Point> _points; // punktu pola
        private List<Point> _interPoints; // punkty do interpolacji, wykreslana na kole
        private List<double> _segments; // kolejne odległości punktów interpolacji od środka cieżkości - służą do rysowania wykresy kształtu
        private List<Point> _circuit;
        private int _edgeCount;
        private Color _color;
        private Slice _owner;
        private SeedSectionContainer _container;
        private int _maxSegment;

        public int Width { get; set; }
        public int Height { get; set; }


        private System.Windows.Media.Imaging.BitmapImage bitmapImage;
        public System.Windows.Media.Imaging.BitmapImage BitmapSource 
        {
            get 
            {
                if (bitmapImage == null)
                {
                    OnFinished();
                }

                return bitmapImage;
            }
        }

        public GlobalContainer MicrostructureContainer { get; set; }

        public void OnFinished()
        {
            Bitmap bitmap = new Bitmap(Width, Height);
            for (int i = 0; i < _points.Count; i++)
            {
                bitmap.SetPixel(_points[i].X, _points[i].Y, _color);
            }

            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;
                bitmapImage = new System.Windows.Media.Imaging.BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

            }
        }


        public SeedSectionContainer Container { get { return _container; } set { _container = value; } }
        public int MaxSegment { get { return _maxSegment; } set { _maxSegment = value; } }

        public SeedSection()
        {
            _points = new List<Point>();
            ID = Helper.SeedSectionID++;
        }
        public void AddPoint(int x, int y)
        {
            _points.Add(new Point(x, y));
        }
        public void SetPointsList(List<Point> ls)
        {
            _points = ls;
        }
        public void SetCircuitList(List<Point> ls)
        {
            _circuit = ls;
        }

        public List<Point> Circuit { get { return _circuit; } set { _circuit = value; } }
        public List<Point> OrderCircuit { get; set; }


        public int PointsCount
        {
            get { return _points.Count; }
        }
        public int CircuitCount
        {
            get { return _circuit.Count; }
        }
        public Point GetFirstPoint
        {
            get { return _points[0]; }
        }
        public int EdgeCount
        {
            get { return _edgeCount; }
            set { _edgeCount = value; }
        }
        public Point GetPoint(int i)
        {
            return _points[i];
        }
        public Point GetCircuitPoint(int i)
        {
            return _circuit[i];
        }
        public Color SeedColor
        {
            get { return _color; }
            set { _color = value; }
        }
        public Slice Owner { get { return _owner; } set { _owner = value; } }

        public void SortPoints()
        {
            QuickSort(_points, 0, _points.Count - 1);  // sortuje po Y
            PartSort(_points);                       // sortuje po X
        }


        public List<Point> InterPoints { get { return _interPoints; } set { _interPoints = value; } }
        public List<double> Segments { get { return _segments; } set { _segments = value; } }











        private void QuickSort(List<Point> items, int left, int right)
        {
            int i = left;
            int j = right;
            Point x;

            x = items[(left + right) / 2];
            do
            {
                while (items[i].Y < x.Y) i++; //items[i] < x
                while (items[j].Y > x.Y) j--; //items[j] > x

                if (i <= j)
                {
                    Swap(items, i, j);
                    i++; j--;
                }
            }
            while (i < j);

            if (left < j) QuickSort(items, left, j);
            if (right > i) QuickSort(items, i, right);
        }
        private void Swap(List<Point> list, int left, int right)
        {
            Point temp = list[left];
            list[left] = list[right];
            list[right] = temp;
        }
        private void PartSort(List<Point> items)
        {
            int l = 0, r = 1;
            while (r < items.Count)
            {
                if (items[l].Y == items[r].Y)
                    r++;
                else
                {
                    QuickSortX(items, l, r - 1);
                    l = r;
                    r++;
                }


            }
        }
        private void QuickSortX(List<Point> items, int left, int right)
        {
            int i = left;
            int j = right;
            Point x;

            x = items[(left + right) / 2];
            do
            {
                while (items[i].X < x.X) i++; //items[i] < x
                while (items[j].X > x.X) j--; //items[j] > x

                if (i <= j)
                {
                    Swap(items, i, j);
                    i++; j--;
                }
            }
            while (i < j);

            if (left < j) QuickSortX(items, left, j);
            if (right > i) QuickSortX(items, i, right);
        }
    }
}
