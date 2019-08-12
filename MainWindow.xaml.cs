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
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BigInteger a1 = Maths.JacobiRand(-3, 1);
        BigInteger a2 = Maths.Jacobi(-3, 1);
        BigInteger b1 = Maths.JacobiRand(7, 13);
        BigInteger b2 = Maths.Jacobi(7, 13);
        BigInteger c1 = Maths.JacobiRand(20, 19);
        BigInteger c2 = Maths.Jacobi(20, 19);
        BigInteger d1 = Maths.JacobiRand(0, 7);
        BigInteger d2 = Maths.Jacobi(0, 7);
        BigInteger e1 = Maths.JacobiRand(-3, -11);
        BigInteger e2 = Maths.Jacobi(-3, -11);

        public SchemeContext db;

        public Scheme selectScheme;
        public EllipticCurve_EDS EDS;

        public MainWindow()
        {
            string user_folder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var dir = new DirectoryInfo(user_folder + "/.edsa.keys");
            if (!dir.Exists)
            {
                dir.Create();
            }
            db = new SchemeContext();
            selectScheme = null;
            EDS = new EllipticCurve_EDS();
            InitializeComponent();
            this.Closing += MainWindow_Closing;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.frame_left.Source = new Uri("PageExplorer.xaml", UriKind.Relative);
            this.frame_right.Source = new Uri("PageEmpty.xaml", UriKind.Relative);
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            db.Dispose();
        }

        private void Back_nav_button_Click(object sender, RoutedEventArgs e)
        {
            this.back_column.Width = new GridLength(0);
            this.frame_left_column.Width = new GridLength(1, GridUnitType.Star);
            if (frame_right.NavigationService.CanGoBack)
                this.frame_right.NavigationService.GoBack();
        }
        
        public void GetKeyFile(string pth)
        {
            var frm = this.frame_left.Content as PageExplorer;
            frm.GetKeyFile(pth);
        }
    }
}
