using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MetalSoft.Model
{
    public class SeedSectionConnector
    {
        private Microstructure ms;
        private List<SeedSectionContainer> seedContainer;

        
        public int ContainerCount { get { return seedContainer.Count; } }
        public SeedSectionContainer GetContainer(int i)
        {
            return seedContainer[i];
        }

        // nie uzywana jeszcze, nie sprawdzona
        public SeedSectionContainer AddEmptyContainer()
        {
            SeedSectionContainer ssc = new SeedSectionContainer();
            seedContainer.Add(ssc);

            return ssc;
        }

        public SeedSectionConnector(Microstructure micro)
        {
            ms = micro;
        }

        public void ConnectSeedSection(Slice pattern, Slice matched)
        {
            seedContainer = new List<SeedSectionContainer>();

            Slice top, bottom;
            List<double> diff = new List<double>(); // aktualne różnice pomiędzy segmentami wykresu
            List<double> stdDeviation = new List<double>(); // zbiór odchyleń standardowych

            top = pattern;
            // nie pobieramy bezposrednio z mikrostruktury, gdyz bedziemy usywac ziarna juz dospasowane
            // co spowodowałoby wyczyszczenie tej warstwy
            //bottom = new Slice(matched.SliceBitmap);
            bottom = matched.CreateCopyToMatch();


            // ziarena z górnej warstwy
            for (int j = 0; j < top.SeedCount; j++)
            {
                // porównujemy z kazdym który pozostał na dolenej warstwie
                for (int k = 0; k < bottom.SeedCount; k++)
                {
                    //wyliczamy różnice między segmentami
                    for (int s = 0; s < top.GetSeed(j).Segments.Count && s < bottom.GetSeed(k).Segments.Count; s++)
                        diff.Add(Math.Abs(top.GetSeed(j).Segments[s] - bottom.GetSeed(k).Segments[s]));

                    // obliczamy odchylenie standardowe różnic

                    double average = diff.Sum() / diff.Count;
                    double variation = 0;

                    for (int d = 0; d < diff.Count; d++)
                    {
                        variation += Math.Pow(diff[d] - average, 2);
                    }

                    variation /= (diff.Count - 1);

                    stdDeviation.Add(Math.Sqrt(variation));

                    diff.Clear();
                }

                SeedSectionContainer container = new SeedSectionContainer();
                container.Pattern = top.GetSeed(j);   //Add(top.GetSeed(j));
                seedContainer.Add(container);
                

                // nie zostało dodane zadne odchylenie, poniewaz wszystkie ziarna z warstwy dolnej zostaly juz dopasowane
                // dlatego nalezy przejsc do nastepnego - co powinno sie powtarzac dla wszystkich pozostalych zianrach na
                // warstwie gornej
                if (stdDeviation.Count == 0)
                    continue;

                // wybieramy najmniejsze odchylenie standardowe
                double minStdDev = stdDeviation[0];
                int indexMinStdDec = 0;
                bool swaped = false;

                ShapeFactor f1 = new ShapeFactor(top.GetSeed(j));
                PointF a = f1.Centroid();
                PointF b = new PointF();
                int maxSegmentTop = top.GetSeed(j).MaxSegment;
                int maxSegmentBottomLast = 0;

                double discreteLast = 0;

                for (int sd = 0; sd < stdDeviation.Count; sd++)
                {
                    ShapeFactor f2 = new ShapeFactor(bottom.GetSeed(sd));
                    b = f2.Centroid();
                    int maxSegmentBottom = bottom.GetSeed(sd).MaxSegment;

                    double discreteLenght = DiscreteLenght(new Point((int)(a.X + 0.5), (int)(a.Y + 0.5)), new Point((int)(b.X + 0.5), (int)(b.Y + 0.5)));

                    //if (discreteLenght > ((maxSegmentBottom + maxSegmentTop) / 2) && stdDeviation[sd] < minStdDev)
                    //{
                    //}

                    if (stdDeviation[sd] <= minStdDev && discreteLenght <= ((maxSegmentTop + maxSegmentBottom) / 2))
                    {
                        minStdDev = stdDeviation[sd];
                        indexMinStdDec = sd;
                        discreteLast = discreteLenght;
                        maxSegmentBottomLast = maxSegmentBottom;
                        swaped = true;
                    }
                }


                // łączymy ziarno z górnej warstwy z ziarnem z dolnej

                if (swaped)
                {
                    //cont.Add(bottom.GetSeed(indexMinStdDec));
                    container.Matched = bottom.GetSeed(indexMinStdDec);
                    container.a = a;
                    container.b = b;
                    container.discreteLenght = discreteLast;
                    container.maxTop = maxSegmentTop;
                    container.maxBottom = maxSegmentBottomLast;

                    // usuwamy ziarno z dolnej warstwy
                    bottom.RemoveSeed(indexMinStdDec);
                }


               


                // czyscimy listy do analizy kolejnej pary
                diff.Clear();
                stdDeviation.Clear();

            }

            // jesli zostały jakieś na dolenj warstwie tworzymy dla nich kontenery

            for (int k = 0; k < bottom.SeedCount; k++)
            {
                SeedSectionContainer con = new SeedSectionContainer();
                //con.Add(bottom.GetSeed(i));
                con.Pattern = null;
                con.Matched = bottom.GetSeed(k);

                seedContainer.Add(con);
            }
        }


        private int DiscreteLenght(Point a, Point b)
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
            cout++;

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

                    cout++;

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

                    cout++;

                }
            }

            return cout;
        }
    }
}
