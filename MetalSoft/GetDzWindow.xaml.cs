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

namespace MetalSoft
{
    /// <summary>
    /// Interaction logic for GetDzWindow.xaml
    /// </summary>
    public partial class GetDzWindow : Window
    {
        public int DeltaZ { get; set; }
        public bool WasSet { get; set; }
        
        public GetDzWindow()
        {
            InitializeComponent();
            WasSet = false;
        }

        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try 
            {
                DeltaZ = int.Parse(tbox.Text);
                WasSet = true;
                this.Close();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
