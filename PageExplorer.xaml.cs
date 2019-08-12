using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
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

namespace EDS
{
    /// <summary>
    /// Логика взаимодействия для Page_explorer.xaml
    /// </summary>
    public partial class PageExplorer : Page
    {

        public PageExplorer()
        {
            InitializeComponent();
            this.keys_grid.Visibility = Visibility.Hidden;
            var main = SupportEDS.GetMainWindow();
            if (main.selectScheme != null)
            {
                this.scheme_status_textblock.Text = "Схема " + main.selectScheme.Name;
                this.keys_grid.Visibility = Visibility.Visible;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.explorer.SetCommonDrivesDirectory();
            this.explorer.ListboxSelectionChanged += Explorer_ListboxSelectionChanged;
        }

        private void Explorer_ListboxSelectionChanged(object sender, SelectedFileEventArgs e)
        {
            var main = SupportEDS.GetMainWindow();
            if (e.FileInfo == null)
                main.frame_right.Source = new Uri("PageEmpty.xaml", UriKind.Relative);
            else
                main.frame_right.Content = new PageFileInfo(e.FileInfo);
        }

        private void Select_scheme_button_Click(object sender, RoutedEventArgs e)
        {
            SupportEDS.GetMainWindow().frame_right.Source = new Uri("ConfigurateEDSPage.xaml", UriKind.Relative);
        }

        public void GetKeyFile(string pth)
        {
            this.filename_textblock.Text = pth;
            string fullpath = SupportEDS.spec_folder + "\\" + pth;
            var main = SupportEDS.GetMainWindow();
            if (File.Exists(fullpath + ".publ"))
            {
                string[] q = File.ReadAllLines(fullpath + ".publ", Encoding.Default)[0].Split('\0');
                main.EDS.Q = new EllipticCurve_Point(BigInteger.Parse(q[0]), BigInteger.Parse(q[1]));
                this.public_key_image.Source = new BitmapImage(new Uri("Images/ExplorerNode/Yes.png", UriKind.Relative));
            }
            else
                this.public_key_image.Source = new BitmapImage(new Uri("Images/ExplorerNode/No.png", UriKind.Relative));
            if (File.Exists(fullpath + ".priv"))
            {
                string d = File.ReadAllLines(fullpath + ".priv", Encoding.Default)[0];
                main.EDS.D = BigInteger.Parse(d);
                this.private_key_image.Source = new BitmapImage(new Uri("Images/ExplorerNode/Yes.png", UriKind.Relative));
            }
            else
                this.private_key_image.Source = new BitmapImage(new Uri("Images/ExplorerNode/No.png", UriKind.Relative));
        }
    }
}
