using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using MetalSoft.Model.Generator;


namespace MetalSoft.Model
{
    public class Helper
    {
        public static int MaxX { get; set; }
        public static int MaxY { get; set; }
        public static int MaxZ { get; set; }
        public static int DeltaZ { get; set; }

        public static int ID;
        public static int SeedSectionContainerID;
        public static int SeedSectionID;

        public static void SaveLayerAsBmp(Point3D[,,] points)
        {
            for(int z = 0; z < 11; z++)
            {
                Bitmap bm = new Bitmap(200, 200);

                using (Graphics g = Graphics.FromImage(bm))
                {
                    g.Clear(Color.White);
                }

                for (int x = 0; x < 200; x++)
                    for (int y = 0; y < 200; y++)
                    {
                        Point3D p = points[x, y, z];
                        bm.SetPixel(x, y, Color.FromArgb(p.R, p.G, p.B));
                    }

                bm.Save(z.ToString() + ".bmp");
            }
        }

    }
}
