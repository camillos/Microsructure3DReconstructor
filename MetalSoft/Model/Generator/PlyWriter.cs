using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MetalSoft.Model.Generator
{
    public class PlyWriter
    {
        public static void WriteVertax(string fileName, List<Point3D> vertax)
        {
            //string fileName = "plik.ply";

            string[] header = new string[] {
                                            "ply",
                                            "format ascii 1.0",
                                            "element vertex ",   //2
                                            "property float x",
                                            "property float y",
                                            "property float z",
                                            "property uchar red",
                                            "property uchar green",
                                            "property uchar blue",
                                            "element edge ",  //9
                                            "property int vertex1",
                                            "property int vertex2",
                                            "property uchar red",
                                            "property uchar green",
                                            "property uchar blue",
                                            "end_header"
                                             };
            header[2] += vertax.Count;
            header[9] += 0;



            using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileName))
            {
                foreach (string s in header)
                    file.WriteLine(s);

                string line = string.Empty;
                foreach (Point3D p in vertax)
                {
                    line = p.X.ToString() + " " + p.Y.ToString() + " " + p.Z.ToString() + " " + p.R.ToString() + " " + p.G.ToString() + " " + p.B.ToString();
                    file.WriteLine(line);

                    line = string.Empty;
                }
            }
        }

        public static void WriteVertax2(string fileName, Point3D[,,] vertax, int maxX, int maxY, int maxZ)
        {
            //string fileName = "plik.ply";

            string[] header = new string[] {
                                            "ply",
                                            "format ascii 1.0",
                                            "element vertex ",   //2
                                            "property float x",
                                            "property float y",
                                            "property float z",
                                            "property uchar red",
                                            "property uchar green",
                                            "property uchar blue",
                                            "element edge ",  //9
                                            "property int vertex1",
                                            "property int vertex2",
                                            "property uchar red",
                                            "property uchar green",
                                            "property uchar blue",
                                            "end_header"
                                             };
            header[2] += (maxX * maxY * maxZ).ToString();
            header[9] += 0;



            using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileName))
            {
                foreach (string s in header)
                    file.WriteLine(s);

                string line = string.Empty;
                for (int z = 0; z < maxZ; z++)
                    for (int x = 0; x < maxX; x++)
                        for (int y = 0; y < maxY; y++)
                        {
                            Point3D p = vertax[x, y, z];

                            if (p == null) continue;

                            line = p.X.ToString() + " " + p.Y.ToString() + " " + p.Z.ToString() + " " + p.R.ToString() + " " + p.G.ToString() + " " + p.B.ToString();
                            file.WriteLine(line);

                            line = string.Empty;
                        }
            }
        }


        public static void WriteVertaxToTxtWithColor(string fileName, Point3D[, ,] vertax, int maxX, int maxY, int maxZ)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileName))
            {
                string line = string.Empty;
                for (int z = 0; z < maxZ; z++)
                    for (int x = 0; x < maxX; x++)
                        for (int y = 0; y < maxY; y++)
                        {
                            Point3D p = vertax[x, y, z];

                            if (p == null) continue;

                            line = p.X.ToString() + " " + p.Y.ToString() + " " + p.Z.ToString() + " " + p.R.ToString() + " " + p.G.ToString() + " " + p.B.ToString();
                            file.WriteLine(line);

                            line = string.Empty;
                        }
            }
        }


        public static void WriteVertaxToTxtWithID(string fileName, Point3D[, ,] vertax, int maxX, int maxY, int maxZ)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileName))
            {
                string line = string.Empty;
                for (int z = 0; z < maxZ; z++)
                    for (int x = 0; x < maxX; x++)
                        for (int y = 0; y < maxY; y++)
                        {
                            Point3D p = vertax[x, y, z];

                            if (p == null) continue;

                            line = p.X.ToString() + " " + p.Y.ToString() + " " + p.Z.ToString() + " " + p.GlobalID.ToString();
                            file.WriteLine(line);

                            line = string.Empty;
                        }
            }
        }































        public static void WriteVertax(string fileName, List<Point> vertax)
        {
            //string fileName = "plik.ply";

            string[] header = new string[] {
                                            "ply",
                                            "format ascii 1.0",
                                            "element vertex ",   //2
                                            "property float x",
                                            "property float y",
                                            "property float z",
                                            "property uchar red",
                                            "property uchar green",
                                            "property uchar blue",
                                            "element edge ",  //9
                                            "property int vertex1",
                                            "property int vertex2",
                                            "property uchar red",
                                            "property uchar green",
                                            "property uchar blue",
                                            "end_header"
                                             };
            header[2] += vertax.Count;
            header[9] += 0;



            using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileName))
            {
                foreach (string s in header)
                    file.WriteLine(s);
                
                string line = string.Empty;
                foreach (Point p in vertax)
                {
                    line = p.X.ToString() + " " + p.Y.ToString() + " 0 255 0 0";
                    file.WriteLine(line);
           
                    line = string.Empty;
                }
            }
        }

        public static void WriteConnection(List<Point> vertax1, List<Point> vertax2, int[,] connection)
        {
            string fileName = "plik.ply";

            string[] header = new string[] {
                                            "ply",
                                            "format ascii 1.0",
                                            "element vertex ",   //2
                                            "property float x",
                                            "property float y",
                                            "property float z",
                                            "property uchar red",
                                            "property uchar green",
                                            "property uchar blue",
                                            "element edge ",  //9
                                            "property int vertex1",
                                            "property int vertex2",
                                            "property uchar red",
                                            "property uchar green",
                                            "property uchar blue",
                                            "end_header"
                                             };
            header[2] += (vertax1.Count + vertax2.Count);
            header[9] += (int)(connection.Length / 2);



            using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileName))
            {
                foreach (string s in header)
                    file.WriteLine(s);

                string line;

                foreach (Point p in vertax1)
                {
                    line = string.Empty;
                    line = p.X.ToString() + " " + p.Y.ToString() + " 0 255 0 0";
                    file.WriteLine(line);
                }

                foreach (Point p in vertax2)
                {
                    line = string.Empty;
                    line = p.X.ToString() + " " + p.Y.ToString() + " 10 0 255 0";
                    file.WriteLine(line);
                }

                int offset = vertax1.Count;

                for (int i = 0; i < connection.Length / 2; i++)
                {
                    line = string.Empty;
                    line = connection[0, i] + " " + (connection[1, i] + offset) + " 0 0 255";
                    file.WriteLine(line);
                }
            }
        }

        public static void WriteConnection(string fileName, List<Point3D> vertex, List<Connection> connection)
        {
            //string fileName = "plik.ply";

            string[] header = new string[] {
                                            "ply",
                                            "format ascii 1.0",
                                            "element vertex ",   //2
                                            "property float x",
                                            "property float y",
                                            "property float z",
                                            "property uchar red",
                                            "property uchar green",
                                            "property uchar blue",
                                            "element edge ",  //9
                                            "property int vertex1",
                                            "property int vertex2",
                                            "property uchar red",
                                            "property uchar green",
                                            "property uchar blue",
                                            "end_header"
                                             };
            header[2] += vertex.Count;
            header[9] += connection.Count;



            using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileName))
            {
                foreach (string s in header)
                    file.WriteLine(s);

                string line;

                foreach (Point3D p in vertex)
                {
                    line = string.Empty;
                    line = p.X.ToString() + " " + p.Y.ToString() + " "+p.Z.ToString() + " 255 0 0";
                    file.WriteLine(line);
                }


                for (int i = 0; i < connection.Count; i++)
                {
                    line = string.Empty;
                    line = connection[i].Index1 + " " + connection[i].Index2 + " 0 0 255";
                    file.WriteLine(line);
                }
            }
        }



        public static void WriteConnection(CirclePoint[] point1, CirclePoint[] point2)
        {
            string fileName = "plik.ply";

            string[] header = new string[] {
                                            "ply",
                                            "format ascii 1.0",
                                            "element vertex ",   //2
                                            "property float x",
                                            "property float y",
                                            "property float z",
                                            "property uchar red",
                                            "property uchar green",
                                            "property uchar blue",
                                            "element edge ",  //9
                                            "property int vertex1",
                                            "property int vertex2",
                                            "property uchar red",
                                            "property uchar green",
                                            "property uchar blue",
                                            "end_header"
                                             };
            header[2] += (point1.Length + point2.Length);
            header[9] += (int)(point1.Length);



            using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileName))
            {
                foreach (string s in header)
                    file.WriteLine(s);

                string line;

                foreach (CirclePoint p in point1)
                {
                    line = string.Empty;
                    line = p.X.ToString() + " " + p.Y.ToString() + " 0 255 0 0";
                    file.WriteLine(line);
                }

                foreach (CirclePoint p in point2)
                {
                    line = string.Empty;
                    line = p.X.ToString() + " " + p.Y.ToString() + " 10 0 255 0";
                    file.WriteLine(line);
                }

                int offset = point1.Length;

                for (int i = 0; i < point1.Length; i++)
                {
                    line = string.Empty;
                    line = i + " " + (i + offset) + " 0 0 255";
                    file.WriteLine(line);
                }
            }
        }

        public static void WriteConnection(SeedSection s1, SeedSection s2)
        {
            string fileName = "plik.ply";

            string[] header = new string[] {
                                            "ply",
                                            "format ascii 1.0",
                                            "element vertex ",   //2
                                            "property float x",
                                            "property float y",
                                            "property float z",
                                            "property uchar red",
                                            "property uchar green",
                                            "property uchar blue",
                                            "element edge ",  //9
                                            "property int vertex1",
                                            "property int vertex2",
                                            "property uchar red",
                                            "property uchar green",
                                            "property uchar blue",
                                            "end_header"
                                             };
            header[2] += (s1.Circuit.Count + s2.Circuit.Count);

            int connectionCount = s1.CircuitCount;
            if (s2.CircuitCount < connectionCount) connectionCount = s2.CircuitCount;

            header[9] += connectionCount;



            using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileName))
            {
                foreach (string s in header)
                    file.WriteLine(s);

                string line;

                foreach (Point p in s1.Circuit)
                {
                    line = string.Empty;
                    line = p.X.ToString() + " " + p.Y.ToString() + " 0 255 0 0";
                    file.WriteLine(line);
                }

                foreach (Point p in s2.Circuit)
                {
                    line = string.Empty;
                    line = p.X.ToString() + " " + p.Y.ToString() + " 10 0 255 0";
                    file.WriteLine(line);
                }

                int offset = s1.Circuit.Count;

                for (int i = 0; i < s1.CircuitCount && i < s2.Circuit.Count; i++)
                {
                    line = string.Empty;
                    line = i + " " + (i + offset) + " 0 0 255";
                    file.WriteLine(line);
                }
            }
        }















    }
}
