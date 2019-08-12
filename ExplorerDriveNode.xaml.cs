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

namespace EDS
{
    /// <summary>
    /// Логика взаимодействия для ExplorerDriveNode.xaml
    /// </summary>
    public partial class ExplorerDriveNode : UserControl, IExplorerNode
    {

        public string Path
        {
            get { return this.drive_text.Text; }
            set { this.drive_text.Text = value; }
        }

        public explType Type
        {
            get { return explType.Drive; }
            set { }
        }

        public ExplorerDriveNode()
        {
            InitializeComponent();
        }
    }
}
