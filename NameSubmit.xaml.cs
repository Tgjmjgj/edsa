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
using System.Windows.Shapes;

namespace EDS
{
    /// <summary>
    /// Логика взаимодействия для NameSubmit.xaml
    /// </summary>
    public partial class NameSubmit : Window
    {
        private BigInteger d;
        private EllipticCurve_Point Q;

        public NameSubmit(BigInteger d, EllipticCurve_Point Q)
        {
            this.d = d;
            this.Q = Q;
            InitializeComponent();
        }

        private void Filename_textbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (!SupportEDS.IsSimpleKey(e.Key))
                e.Handled = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.filename_textbox.Text != "")
            {
                File.WriteAllText(SupportEDS.spec_folder + "\\" + this.filename_textbox.Text + ".priv", this.d.ToString(), Encoding.Default);
                File.WriteAllText(SupportEDS.spec_folder + "\\" + this.filename_textbox.Text + ".publ", this.Q.X.ToString() + '\0' + this.Q.Y.ToString(), Encoding.Default);
                this.Close();
            }

        }
    }
}
