using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MetalSoft.Model
{
    public class ShapeFactor
    {
        private SeedSection _seed;
        private int _pointsCount;
        private int _circuitCount;

        public ShapeFactor(SeedSection seed)
        {
            _seed = seed;
            _pointsCount = seed.PointsCount;
            _circuitCount = seed.CircuitCount;
        }

        public PointF Centroid()
        {
            PointF centroid = new PointF();
            int sum_x = 0; int sum_y = 0;

            for (int i = 0; i < _pointsCount; i++)
            {
                sum_x += _seed.GetPoint(i).X;
                sum_y += _seed.GetPoint(i).Y;
            }

            centroid.X = (float)sum_x / _pointsCount;
            centroid.Y = (float)sum_y / _pointsCount;

            return centroid;
        }

        public PointF CentroidFromCircuit()
        {
            PointF centroid = new PointF();
            int sum_x = 0; int sum_y = 0;

            for (int i = 0; i < _circuitCount; i++)
            {
                sum_x += _seed.GetCircuitPoint(i).X;
                sum_y += _seed.GetCircuitPoint(i).Y;
            }

            centroid.X = (float)sum_x / _circuitCount;
            centroid.Y = (float)sum_y / _circuitCount;

            return centroid;
        }


        public double Area()
        {
            return _pointsCount;
        }
        public double MalinowskiejFactor()
        {
            return ((double)_seed.EdgeCount / (2 * Math.Sqrt(Math.PI * _seed.PointsCount))) - 1.0;
        }
        public double Cyrkularnosci1Factor()
        {
            return 2.0 * Math.Sqrt((double)_seed.PointsCount / Math.PI);
        }
        public double Cyrkularnosci2Factor()
        {
            return (double)_seed.EdgeCount / Math.PI;
        }
        public double BlairaBlissaFactor()
        {
            PointF centroid = Centroid();
            double sumLength = 0;
            for (int i = 0; i < _pointsCount; i++)
                sumLength += Math.Pow(SegmentLenght(_seed.GetPoint(i), centroid), 2);
            return (double)_pointsCount / Math.Sqrt(2.0 * Math.PI * sumLength);
        }
        public double HaralickaFactror()
        {
            double sum = 0; double sum2 = 0;
            PointF centroid = Centroid();

            for (int i = 0; i < _circuitCount; i++)
            {
                sum += SegmentLenght(_seed.GetCircuitPoint(i), centroid);
                sum2 += Math.Pow(SegmentLenght(_seed.GetCircuitPoint(i), centroid), 2);
            }

            double r = (Math.Sqrt(Math.Pow(sum, 2) / (double)_circuitCount * sum2 - 1.0)) / 10000.0;
            if (double.IsNaN(r))
                return 0;
            return r;

        }
        public double Lp1Factor()
        {
            PointF centroid = Centroid();
            double r_min = SegmentLenght(_seed.GetCircuitPoint(0), centroid);
            double r_max = r_min;

            for (int i = 0; i < _circuitCount; i++)
            {
                double temp = SegmentLenght(_seed.GetCircuitPoint(i), centroid);
                if (temp < r_min) r_min = temp;
                if (temp > r_max) r_max = temp;
            }

            if (r_max == 0) return 0;

            return r_min / r_max;
        }
        public double Lp2Factor()
        {
            double max = 0;
            for (int i = 0; i < _circuitCount; i++)
                for (int j = 0; j < _circuitCount; j++)
                    if (SegmentLenght(_seed.GetCircuitPoint(i), _seed.GetCircuitPoint(j)) > max)
                        max = SegmentLenght(_seed.GetCircuitPoint(i), _seed.GetCircuitPoint(j));

            return (double)max / _circuitCount;
        }
        public double MzFactor()
        {
            return (double)2.0 * Math.Sqrt(Math.PI * _pointsCount) / _seed.EdgeCount;
        }

        private double SegmentLenght(Point a, PointF b)
        {
            return Math.Sqrt(Math.Pow(b.X - a.X, 2) + Math.Pow(b.Y - b.Y, 2));
        }
        private double SegmentLenght(Point a, Point b)
        {
            return Math.Sqrt(Math.Pow(b.X - a.X, 2) + Math.Pow(b.Y - b.Y, 2));
        }
    }
}
