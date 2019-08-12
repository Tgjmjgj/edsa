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
using System.Drawing;
using System.IO;

namespace EDS
{
    /// <summary>
    /// Логика взаимодействия для Explorer_node.xaml
    /// </summary>
    public partial class ExplorerNode : UserControl, IExplorerNode
    {
        public static DependencyProperty EdsStateProperty;

        private string fullpath;
        private explType type;

        public string Path
        {
            get { return fullpath; }
            set { fullpath = value; }
        }

        public explType Type
        {
            get { return type; }
            set { type = value; }
        }

        public int EdsState
        {
            get { return (int)GetValue(EdsStateProperty); }
            set { SetValue(EdsStateProperty, value); }
        }

        public ExplorerNode()
        {
            InitializeComponent();
        }

        public ExplorerNode(string fp, explType tp)
        {
            this.fullpath = fp;
            InitializeComponent();
            this.expl_name.Text = fp;
            this.expl_chng_date.Text = "";
            this.Type = tp;
            this.expl_type.Text = "Ключи";
            this.expl_size.Text = "";
            this.expl_ico.Source = new BitmapImage(new Uri("Images/ExplorerNode/Key.png", UriKind.Relative));
        }

        public ExplorerNode(string fp, string name, DateTime chg, explType tp, long sz, Icon ic = null)
        {
            this.fullpath = fp;
            this.Type = tp;
            this.EdsState = 0;
            InitializeComponent();
            this.expl_name.Text = name;
            this.expl_chng_date.Text = chg.ToLongTimeString();
            this.expl_type.Text = (tp == explType.File ? "Файл" : "Папка");
            if (sz < 1024)
                this.expl_size.Text = sz.ToString() + " Б";
            else
                this.expl_size.Text = (sz / 1024).ToString() + " Кб";
            if (ic == null)
                this.expl_ico.Source = new BitmapImage(new Uri("Images/ExplorerNode/Folder.png", UriKind.Relative));
            else
                this.expl_ico.Source = SupportEDS.BitmapToImageSource(ic.ToBitmap());
            if (tp == explType.File)
                this.expl_state.Visibility = Visibility.Visible;
        }

        static ExplorerNode()
        {
            EdsStateProperty = DependencyProperty.Register("EdsState", typeof(int), typeof(ExplorerNode));
        }
    }
}
