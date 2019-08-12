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
    /// Логика взаимодействия для TextBoxGenerate.xaml
    /// </summary>
    public partial class TextBoxGenerate : UserControl
    {
        
        public event RoutedEventHandler GenButtonClick;
        private void OnGenButtonClick(object sender, RoutedEventArgs e)
        {
            if (GenButtonClick != null)
                GenButtonClick(sender, e);
        }

        public TextBoxGenerate()
        {
            InitializeComponent();
        }

        private void Down_button_Click(object sender, RoutedEventArgs e)
        {
            if (hidden_down_grid.Visibility == Visibility.Visible)
                hidden_down_grid.Visibility = Visibility.Hidden;
            else
                hidden_down_grid.Visibility = Visibility.Visible;
        }

    }
}
