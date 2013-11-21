using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.ComponentModel;
using System.Diagnostics;

namespace MetalSoft.Model
{
    public class Microstructure
    {
        private List<Slice> _layers;
        //private System.Windows.Controls.ProgressBar pb;
        private List<string> filePath;
        

        //private BackgroundWorker worker;

        public bool Generated { get; set; }
             
        public void Generate(List<string> filePath,  Microstructure3D ms3d, int dz)
        {
            Generated = false;
            //this.pb = pb;
            this.filePath = filePath;

           // pb.Minimum = 0;
            //pb.Maximum = filePath.Count;

            //worker = new BackgroundWorker();
            //worker.DoWork += worker_DoWork;
            //worker.ProgressChanged += worker_ProgressChanged;

            _layers = new List<Slice>();
            for (int i = 0; i < filePath.Count; i++)
            {
                _layers.Add(new Slice(new Bitmap(Image.FromFile(filePath[i]))));
                //worker.ReportProgress(1);
               // pb.Value++;

            }

            if (_layers.Count > 0)
            {
                ms3d.MaxX = _layers[0].SliceBitmap.Width;
                ms3d.MaxY = _layers[0].SliceBitmap.Height;
                ms3d.MaxZ = ((_layers.Count - 1) * dz) + 1;           // po to aby mozna brac w warunkach z < maxz
                ms3d.DeltaZ = dz;

                Helper.MaxX = ms3d.MaxX;
                Helper.MaxY = ms3d.MaxY;
                Helper.MaxZ = ms3d.MaxZ;
                Helper.DeltaZ = ms3d.DeltaZ;

                Debug.WriteLine("maxX={0} maxY ={1} maxZ={2}", ms3d.MaxX, ms3d.MaxY, ms3d.MaxZ);

            }

            Generated = true;
            
            
        }

        //void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        //{
        //   // pb.Value = pb.Value + 1;
        //}

        //void worker_DoWork(object sender, DoWorkEventArgs e)
        //{
            
        //    _layers = new List<Slice>();
        //    for (int i = 0; i < filePath.Count; i++)
        //    {
        //        _layers.Add(new Slice(new Bitmap(Image.FromFile(filePath[i]))));
        //        worker.ReportProgress(1);
               
        //    }

        //    Generated = true;
        //}

        public int LayersCount
        {
            get { return _layers.Count; }
        }

        public Slice GetLayer(int i)
        {
            if (i < 0 || i > _layers.Count - 1)
                return null;
            return _layers[i];
        }
    }
}
