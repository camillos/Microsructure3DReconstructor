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
using Microsoft.Win32;
using MetalSoft.Model;
using MetalSoft.Model.Generator;

namespace MetalSoft
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MatchingControl SelectedMatching { get; set; }

        private Microstructure ms;
        private Microstructure3D ms3d;
        private int layerIndex = 0;
        private int maxLayer = 0;
        bool[] layerAccept;
        private List<string> filesName = new List<string>();
        private List<string> fullFilesName = new List<string>();

        private SeedSectionConnector connector;

        public MainWindow()
        {
            InitializeComponent();
            lbFiles.ItemsSource = filesName;
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
           
        }

        private void miOpen_Click_1(object sender, RoutedEventArgs e)
        {
            List<string> ls = new List<string>();

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = true;

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                foreach (string s in dlg.FileNames)
                {
                    ls.Add(s);
                }
            }

            if (ls.Count > 0)
            {
                GetDzWindow dzWin = new GetDzWindow();
                dzWin.ShowDialog();

                wPanel.Children.Clear();
                wPanel.Children.Add(new Label() { Content = "Proszę czekać, trwa przetwarzanie..." });

                Task.Factory.StartNew(() =>
                    {
                        LoadSlice(ls, dzWin.DeltaZ);
                    });
                
            }

            
        }

        private void LoadSlice(List<string> ls,int deltaZ)
        {

            filesName.Clear();
            fullFilesName.Clear();

            for (int i = 0; i < ls.Count; i++)
            {
                int li = ls[i].LastIndexOf('\\');
                filesName.Add(ls[i].Substring(li + 1));

                fullFilesName.Add(ls[i]);
            }

            
            LoadMs(ls, deltaZ + 1);

            layerIndex = 0;
            maxLayer = ls.Count;

            layerAccept = new bool[maxLayer - 1];

            this.Dispatcher.Invoke((Action)delegate
            {
                lbFiles.Items.Refresh();
                ShowLayer(layerIndex);
            }
            );

            
        }



        private void LoadMs(List<string> ls, int dz)
        {
            ms = new Microstructure();
            ms3d = new Microstructure3D();
            ms.Generate(ls, ms3d, dz);
        }

        private void ShowLayer(int layerIndex)
        {
            if (ms == null) return;

            if (layerIndex >= ms.LayersCount) return;

            if (layerIndex >= 0 && layerIndex < ms.LayersCount - 1)
            {

                UpdateFotter(filesName[layerIndex], filesName[layerIndex + 1]);

                Slice pattern = ms.GetLayer(layerIndex);
                Slice matched = ms.GetLayer(layerIndex + 1);

                connector = new SeedSectionConnector(ms);
                connector.ConnectSeedSection(pattern, matched);

                wPanel.Children.Clear();

                for (int i = 0; i < connector.ContainerCount; i++)
                {
                    SeedSectionContainer container = connector.GetContainer(i);
                    MatchingControl mc = new MatchingControl(container);
                    wPanel.Children.Add(mc);
                }
            }
        }



        private void miNextLayer_Click_1(object sender, RoutedEventArgs e)
        {
            if (ms == null) return;
            if (layerIndex + 1 < maxLayer - 1)
            {
                Accept();
                layerIndex++;
                ShowLayer(layerIndex);
            }
            else
            {
                MessageBox.Show("Nie ma już więcej warstw.");
            }
            
        }

        //zatwierdz
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (connector == null || ms3d == null) return;

            System.Diagnostics.Debug.WriteLine("layerIndex={0}", layerIndex);
            for (int i = 0; i < connector.ContainerCount; i++)
            {
                ms3d.Add(connector.GetContainer(i), layerIndex);
            }

            //connector = null;

        }


        private void Accept()
        {

            if (connector == null || ms3d == null) return;

            //System.Diagnostics.Debug.WriteLine("layerIndex={0}", layerIndex);
            for (int i = 0; i < connector.ContainerCount; i++)
            {
                ms3d.Add(connector.GetContainer(i), layerIndex);
            }

            layerAccept[layerIndex] = true;

        }

        private void UpdateFotter(string pattern, string matched)
        {
            string s = "Wzorzec: " + pattern + "\t Dopasowywany: " + matched;
            lFooter.Content = s;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (ms == null) return;
            if (layerIndex + 1 < maxLayer - 1)
            {
                Accept();
                layerIndex++;
                ShowLayer(layerIndex);

            }
            else
            {
                MessageBox.Show("Nie ma już więcej warstw.");
            }

            
        }

        private void miPlySav1_Click_1(object sender, RoutedEventArgs e)
        {
            if (ms3d == null) return;
            Accept();

            IEnumerable<bool> s = layerAccept.Where(a => a == false);
            if (s.Count() > 0)
            {
                MessageBox.Show("Przed wygenerowaniem mikrostruktury nalezy sprawdzić wszystkie dopasowania!");
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PLY File|*.ply";
            saveFileDialog.Title = "Save as Ply File";
            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName == null) return;
            if (saveFileDialog.FileName == string.Empty || saveFileDialog.FileName == "") return;

            LoadWindow lw = new LoadWindow("Trwa zapisywanie...");
            lw.Show();

            Task.Factory.StartNew(() =>
                {
                    if (!ms3d.Generated) ms3d.Generate();

                    ms3d.SaveVertexAsPly(saveFileDialog.FileName);

                    this.Dispatcher.Invoke((Action)delegate
                    {
                        lw.Close();

                    });
                    MessageBox.Show("Zapisano");


                });
        }

        private void miTxtCol_Click_1(object sender, RoutedEventArgs e)
        {
            if (ms3d == null) return;
            Accept();

            IEnumerable<bool> s = layerAccept.Where(a => a == false);
            if (s.Count() > 0)
            {
                MessageBox.Show("Przed wygenerowaniem mikrostruktury nalezy sprawdzić wszystkie dopasowania!");
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "TXT File|*.txt";
            saveFileDialog.Title = "Save as TXT File with color";
            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName == null) return;
            if (saveFileDialog.FileName == string.Empty || saveFileDialog.FileName == "") return;

            LoadWindow lw = new LoadWindow("Trwa zapisywanie...");
            lw.Show();

            Task.Factory.StartNew(() =>
            {
                if (!ms3d.Generated) ms3d.Generate();

                ms3d.SaveVertexAsTxtWithColor(saveFileDialog.FileName);

                this.Dispatcher.Invoke((Action)delegate
                {
                    lw.Close();

                });
                MessageBox.Show("Zapisano");


            });

           
        }

        private void miTxtId_Click_1(object sender, RoutedEventArgs e)
        {
            if (ms3d == null) return;
            Accept();

            IEnumerable<bool> s = layerAccept.Where(a => a == false);
            if (s.Count() > 0)
            {
                MessageBox.Show("Przed wygenerowaniem mikrostruktury nalezy sprawdzić wszystkie dopasowania!");
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "TXT File|*.txt";
            saveFileDialog.Title = "Save as TXT File with ID";
            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName == null) return;
            if (saveFileDialog.FileName == string.Empty || saveFileDialog.FileName == "") return;


            LoadWindow lw = new LoadWindow("Trwa zapisywanie...");
            lw.Show();

            Task.Factory.StartNew(() =>
            {
                if (!ms3d.Generated) ms3d.Generate();

                ms3d.SaveVertexAsTxtWithID(saveFileDialog.FileName);

                this.Dispatcher.Invoke((Action)delegate
                {
                    lw.Close();

                });
                MessageBox.Show("Zapisano");


            });
        }



        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            //ms3d.AddEmptyContainer(layerIndex);
            if (connector == null) return;

            SeedSectionContainer con = connector.AddEmptyContainer();

            MatchingControl mc = new MatchingControl(con);
            wPanel.Children.Add(mc);

        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (ms == null) return;
            if (layerIndex - 1 >= 0)
            {
                Accept();
                layerIndex--;
                ShowLayer(layerIndex);
            }
            else
            {
                MessageBox.Show("To jest pierwsza warstwa");
            }
            
        }

        private void lbFiles_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lbFiles.SelectedIndex > -1)
            {
                SliceWindow sw = new SliceWindow(fullFilesName[lbFiles.SelectedIndex]);

                sw.Show();
            }
        }
    }
}
