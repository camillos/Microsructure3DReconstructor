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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using MetalSoft.Model;

namespace MetalSoft
{
    /// <summary>
    /// Interaction logic for MatchingControl.xaml
    /// </summary>
    public partial  class MatchingControl : UserControl
    {
        private int imageWidth, imageHeigh;
        private Brush borderBrushBlack, borderBrushRed;
        Thickness thicknessBlack, thicknessRed;
        private bool selected;

        public SeedSectionContainer Container { get; set; }


        public MatchingControl(SeedSectionContainer container)
        {   
            InitializeComponent();
            
            this.Container = container;
            
            int width = 0;
            int height = 0;

            if (container.Pattern != null)
            {
                width = container.Pattern.Width;
                height = container.Pattern.Height;
            }
            else if (container.Matched != null)
            {
                width = container.Matched.Width;
                height = container.Matched.Height;
            }else 
            {
                width = Helper.MaxX;
                height = Helper.MaxY;
            }

            this.imageWidth = width;
            this.imageHeigh = height;

            iPattern.Width = imageWidth;
            iPattern.Height = imageHeigh;

            iMatched.Width = imageWidth;
            iMatched.Height = imageHeigh;

            borderBrushBlack = new SolidColorBrush(Colors.Black);
            borderBrushRed = new SolidColorBrush(Colors.Red);

            thicknessBlack = new Thickness(1);
            thicknessRed = new Thickness(3);

            selected = false;

            if (container.Pattern != null)
                PatternImage.Source = container.Pattern.BitmapSource;
            if (container.Matched != null)
                MatchedImage.Source = container.Matched.BitmapSource;



        }

        public MatchingControl(int imageWidth, int imageHeigh)
        {
            this.imageWidth = imageWidth;
            this.imageHeigh = imageHeigh;

            InitializeComponent();

            iPattern.Width = imageWidth;
            iPattern.Height = imageHeigh;

            iMatched.Width = imageWidth;
            iMatched.Height = imageHeigh;

            borderBrushBlack = new SolidColorBrush(Colors.Black);
            borderBrushRed = new SolidColorBrush(Colors.Red);

            thicknessBlack = new Thickness(1);
            thicknessRed = new Thickness(3);

            selected = false;
        }

        public Image PatternImage { get { return iPattern; } }
        public Image MatchedImage { get { return iMatched; } }

        public void UnSelect() 
        { 
            selected = false;
            MainWindow.SelectedMatching = null;
            bFrame.BorderBrush = borderBrushBlack;
            bFrame.BorderThickness = thicknessBlack;

        }

        public void Swap(MatchingControl source)
        {
            SeedSection matchedSource = source.Container.Matched;
            source.Container.Matched = this.Container.Matched;

            this.Container.Matched = matchedSource;

            source.UpdateImageSource();
            this.UpdateImageSource();

        }

        public void UpdateImageSource()
        {
            if (Container.Matched != null)
            {
                MatchedImage.Source = Container.Matched.BitmapSource;
            }
            else
                MatchedImage.Source = null;
        }

        private void onMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            e.Handled = true;

            if (MainWindow.SelectedMatching == null || MainWindow.SelectedMatching == this)
            {
                selected = !selected;

                if (selected)
                {
                    bFrame.BorderBrush = borderBrushRed;
                    bFrame.BorderThickness = thicknessRed;
                    MainWindow.SelectedMatching = this;
                }
                else
                {
                    bFrame.BorderBrush = borderBrushBlack;
                    bFrame.BorderThickness = thicknessBlack;
                    MainWindow.SelectedMatching = null;
                }
            }
            else
            {
                // nastepuje zamiana 
                MainWindow.SelectedMatching.Swap(this);
                MainWindow.SelectedMatching.UnSelect();
                
            }   
        }
    }
}
