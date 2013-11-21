using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Drawing;
using System.IO;

namespace MetalSoft
{
    /// <summary>
    /// Interaction logic for SliceWindow.xaml
    /// </summary>
    public partial class SliceWindow : Window
    {
        public SliceWindow(string fileName)
        {
            InitializeComponent();

            Bitmap bm = new Bitmap(fileName);
            BitmapImage bitmapImage;

            using (MemoryStream memory = new MemoryStream())
            {
                bm.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                bitmapImage = new System.Windows.Media.Imaging.BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

            }

            image.Source = bitmapImage;

        }
    }
}
