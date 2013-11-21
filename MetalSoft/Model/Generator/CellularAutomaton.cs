using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetalSoft.Model;

namespace MetalSoft.Model.Generator
{
    public class CellularAutomaton
    {
        public List<Point3D> Vertex { get; set; }
        public int MaxX { get; set; }
        public int MaxY { get; set; }
        public int MaxZ { get; set; }

        public Point3D[, ,] Microstructure { get; set; }

        private Point3D[, ,] nextMs;


        /// <summary>
        /// Uruchamia automat komórkowy, wypełniając brakujące punkty
        /// Do działania wymaga zainicjowania listy vertex i maxx, maxy, maxz
        /// </summary>
        public void Run()
        {
            Microstructure = new Point3D[MaxX, MaxY, MaxZ];

            // przepisanie z podanej listy vereksow punktow do nowej struktury obliczen
            List<Point3D> toDel = new List<Point3D>();      // aby wstawic w MS puse miejsdca jesli wysepuja duplikaty w tym punkcie
            foreach (Point3D p in Vertex)
            {
                if (Microstructure[p.X, p.Y, p.Z] == null)
                    Microstructure[p.X, p.Y, p.Z] = p;//new Point3D(p);
                else
                    toDel.Add(Microstructure[p.X, p.Y, p.Z]);
            }

            foreach (Point3D p in toDel)
            {
                Microstructure[p.X, p.Y, p.Z] = null;
            }

            toDel.Clear();


            bool wasChange = true;

            // zaczyna prace automat

            while (wasChange)
            {
                wasChange = false;
                nextMs = new Point3D[MaxX, MaxY, MaxZ];
                for (int x = 0; x < MaxX; x++)
                    for (int y = 0; y < MaxY; y++)
                        for (int z = 0; z < MaxZ; z++)
                        {
                            // w przypdaku kiedy jest to punkt nalezacy do ziarna
                            if (Microstructure[x, y, z] != null)
                            {
                                nextMs[x, y, z] = Microstructure[x, y, z];
                                continue;
                            }

                            List<Point3D> neight = Moor(x, y, z);
                            // dana komurka nie ma sasiadow
                            if (neight.Count < 1) continue;



                            // zliczamy sasiadow i wybieramy najlepiej sasiadujacego
                            var counter = (from n in neight
                                           group n by new { n.R, n.G, n.B, n.GlobalID } into grp
                                           select new { grp.Key.R, grp.Key.G, grp.Key.B, grp.Key.GlobalID, count = grp.Count() }).ToList();

                            int maxCount = counter.Max(q => q.count);
                            var qmax = counter.Where(q => q.count == maxCount).Select(p => new { p.R, p.G, p.B, p.GlobalID }).ToList();

                            Random r = new Random();
                            var cell = qmax[r.Next(qmax.Count)];

                            nextMs[x, y, z] = new Point3D(x, y, z, cell.GlobalID, cell.R, cell.G, cell.B);

                        }

                // przepisujemy nextMs do MS oraz sprawdzamy czy nastapila zmiana
                for (int x = 0; x < MaxX; x++)
                    for (int y = 0; y < MaxY; y++)
                        for (int z = 0; z < MaxZ; z++)
                        {
                            
                            // jesli w nowej inetacji powsatl element to go przepisujemy
                            if (Microstructure[x, y, z] == null && nextMs[x, y, z] != null)
                            {
                                Microstructure[x, y, z] = nextMs[x, y, z];
                                wasChange = true;
                            }
                            else
                                // jesli oba sa puste to nic sie nie dzieje
                                if (Microstructure[x, y, z] == null || nextMs[x, y, z] == null)
                                {
                                    wasChange = true;
                                }
                      
                        }

                nextMs = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }


        /// <summary>
        /// Zwraca sąsiadów komórki wybranych na zasadzie sąsiedztwa Moora
        /// Zakomnetowane warunki periodyczne
        /// </summary>
        private List<Point3D> Moor(int x, int y, int z)
        {
            int left = x - 1;
            int right = x + 1;

            int front = y + 1;
            int back = y - 1;

            int down = z + 1;
            int up = z - 1;

            // dla periodycznych warunków brzegowych

            //if (left < 0) left = MaxX - 1;
            //if (right > MaxX - 1) right = 0;

            //if (front > MaxY - 1) front = 0;
            //if (back < 0) back = MaxY - 1;

            //if (down > MaxZ - 1) down = 0;
            //if (up < 0) up = MaxZ - 1;

            // bez periodycznych warónków brzegowych

            if (left < 0) left = 0;
            if (right > MaxX - 1) right = MaxX - 1;

            if (front > MaxY - 1) front = MaxY - 1;
            if (back < 0) back = 0;

            if (down > MaxZ - 1) down = MaxZ - 1;
            if (up < 0) up = 0;


            List<Point3D> neight = new List<Point3D>();

            if (Microstructure[left, y, z] != null)
                neight.Add(Microstructure[left, y, z]);

            if (Microstructure[right, y, z] != null)
                neight.Add(Microstructure[right, y, z]);

            if (Microstructure[x, front, z] != null)
                neight.Add(Microstructure[x, front, z]);

            if (Microstructure[x, back, z] != null)
                neight.Add(Microstructure[x, back, z]);

            if (Microstructure[x, y, down] != null)
                neight.Add(Microstructure[x, y, down]);

            if (Microstructure[x, y, up] != null)
                neight.Add(Microstructure[x, y, up]);

            return neight;
        }



    }
}
