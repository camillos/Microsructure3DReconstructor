using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetalSoft.Model.Generator;

namespace MetalSoft.Model
{
    public class GlobalContainer
    {
        public int ID { get; set; }


        private List<SeedSection> seedSections;
        public int startLayer;

        public List<SeedSection> SeedSections { get { return seedSections; } }

        public GlobalContainer(int startLayer)
        {
            this.startLayer = startLayer;

            seedSections = new List<SeedSection>();

            ID = Helper.ID++;

        }

        public void AddSeedSection(SeedSection seed)
        {
            if (seedSections.Where(ss => ss.ID == seed.ID).Count() == 0)
            {
                seed.MicrostructureContainer = this;
                seedSections.Add(seed);
            }
            
        }
    }


    public class Microstructure3D
    {
        private List<GlobalContainer> globalContainers;

        public int MaxX { get; set; }
        public int MaxY { get; set; }
        public int MaxZ { get; set; }

        public bool Generated { get; set; }

        public int DeltaZ { get; set; }

        public List<Point3D> Vertex { get; set; }

        private CellularAutomaton cellular;




        public Microstructure3D()
        {
            globalContainers = new List<GlobalContainer>();
            Generated = false;
        }

        public void Add(SeedSectionContainer container, int layer)
        {
            Generated = false;
            ////////////////////////////////////////////
            // jesli juz taki contener został dodany, to go aktualizujemy tylko
            //if (globalContainers.Where(gc => gc.ID == container.ID).Count() > 0)
            //{

            //    return;
            //}



            SeedSection pattern = container.Pattern;
            SeedSection matched = container.Matched;

            if (pattern == null && matched == null) 
                return;

            if (pattern == null && matched != null)
            {
                if (matched.MicrostructureContainer == null)
                {
                    System.Diagnostics.Debug.WriteLine("Dodaje mached do layer={0}", layer + 1);
                    GlobalContainer gc = new GlobalContainer(layer + 1);
                    gc.AddSeedSection(matched);

                    globalContainers.Add(gc);
                }
                //else
                //{
                //    GlobalContainer gc = matched.MicrostructureContainer;
                //}


            }
            else
            {
                if (pattern.MicrostructureContainer == null)
                {
                    GlobalContainer gc = new GlobalContainer(layer);
                    gc.AddSeedSection(pattern);

                    if (matched != null) gc.AddSeedSection(matched);

                    globalContainers.Add(gc);

                }
                else
                {
                    GlobalContainer gc = pattern.MicrostructureContainer;
                    if (matched != null) gc.AddSeedSection(matched);
                }
            }
        }

        public void AddEmptyContainer(int layer)
        {
            Generated = false;
            GlobalContainer gc = new GlobalContainer(layer + 1);
            globalContainers.Add(gc);
        }

        public void Generate()
        {
            if (Vertex == null)
            {
                Vertex = new List<Point3D>();
            }
            else
            {
                Vertex.Clear();
            }


            for (int i = 0; i < globalContainers.Count; i++)
            {
                //if (i == 2)
                //{
                GlobalContainer gc = globalContainers[i];
                gc.ID = globalContainers[i].ID;
                Microstructure3DGenerator gen = new Microstructure3DGenerator();
                //gen.CreateLinearConnection4(gc.SeedSections);
                //PlyWriter.WriteConnection(i.ToString() + ".ply", gen.vertex, gen.connections);

                //if (gc.startLayer > 1) System.Diagnostics.Debug.WriteLine("gcID={0}, startLayer={1}", gc.ID, gc.startLayer);

                gen.Fill(gc.SeedSections, gc.startLayer * Helper.DeltaZ, DeltaZ);

                Vertex.AddRange(gen.fillPoints);
                //PlyWriter.WriteVertax(i.ToString() + "vertex.ply", gen.fillPoints);
                //PlyWriter.WriteVertax("fill.ply", gen.fillPoints);
                //}

            }

            globalContainers.Clear();

            // automat komurkowy

            cellular = new CellularAutomaton() { Vertex = this.Vertex, MaxX = this.MaxX, MaxY = this.MaxY, MaxZ = this.MaxZ };
            cellular.Run();

            Generated = true;

        }

        public void SaveVertexAsPly(string filename)
        {
            PlyWriter.WriteVertax2(filename, cellular.Microstructure, MaxX, MaxY, MaxZ);
        }

        public void SaveVertexAsTxtWithColor(string filename)
        {
            PlyWriter.WriteVertaxToTxtWithColor(filename, cellular.Microstructure, MaxX, MaxY, MaxZ);
        }

        public void SaveVertexAsTxtWithID(string filename)
        {
            PlyWriter.WriteVertaxToTxtWithID(filename, cellular.Microstructure, MaxX, MaxY, MaxZ);
        }



        //public void SaveSeedToOtherFile()
        //{
        //    PlyWriter.WriteVertax("fill.ply", vertex);
            
        //    PlyWriter.WriteVertax2("fill_ca.ply", ca.Microstructure, MaxX, MaxY, MaxZ);
        //    PlyWriter.WriteVertaxToTxtWithID("fill_id.txt", ca.Microstructure, MaxX, MaxY, MaxZ);
        //    PlyWriter.WriteVertaxToTxtWithColor("fill_color.txt", ca.Microstructure, MaxX, MaxY, MaxZ);
        //    //Helper.SaveLayerAsBmp(ca.Microstructure);
        //    //Microstructure3DGenerator.SaveLayerAsBmp();
        //    //Microstructure3DGenerator.SaveCircuitLayerAsBmp();
        //}
    }
}
