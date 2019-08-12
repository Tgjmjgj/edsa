using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace EDS
{
    /// <summary>
    /// Логика взаимодействия для ConfigurateEDSPage.xaml
    /// </summary>
    public partial class ConfigurateEDSPage : Page
    {
        public EllipticCurve_EDS EDS;

        private SchemeContext db;

        public ConfigurateEDSPage()
        {
            this.EDS = new EllipticCurve_EDS();
            this.db = SupportEDS.GetMainWindow().db;
            SupportEDS.GetMainWindow().frame_left_column.Width = new GridLength(0);
            SupportEDS.GetMainWindow().back_column.Width = new GridLength(56);
            InitializeComponent();
            List<Scheme> nist = this.db.Schemes.Where(sh => sh.Owner == "nist").ToList();

            foreach (Scheme s in nist)
                this.comboBox_nist.Items.Add(s.Name);

            List<Scheme> sec2 = this.db.Schemes.Where(sh => sh.Owner == "sec2").ToList();
            foreach (Scheme s in sec2)
                this.comboBox_sec2.Items.Add(s.Name);
            
            List<Scheme> user = this.db.Schemes.Where(sh => sh.Owner == "user").ToList();
            foreach (Scheme s in user)
                this.comboBox_custom.Items.Add(s.Name);
            this.textBlock_info.Text = "Параметры схемы электронной цифровой подписи на основе эллиптических кривых над конечным полем Fp:\n\n" +
                            "   1. Размер конечного поля p, где p > 3 - простое число;\n " +
                            "   2. Два элемента a и b, которые определяют уравнение эллиптической кривой Е;\n" +
                            "   3. Два элемента Gx и Gy, которые определяют базисную точку G = (Gx, Gy) простого порядка на E;\n" +
                            "   4. Порядок n точки G (рекомендовано n > 2^160 и n^2 > 16p);\n" +
                            "   5. Кофактор h = #E(Fp)/n.";
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox_nist_SelectionChanged(this, null);
        }

        private void ComboBox_nist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.listBox_nist != null)
            {
                string s = (string)this.comboBox_nist.SelectedItem;
                if (s == null)
                    return;
                Scheme scheme = this.db.Schemes.Where(sh => sh.Name == s).First();
                this.SchemeGrade(scheme);
                this.ui_p_nist.Text = scheme.P;
                this.ui_a_nist.Text = scheme.A;
                this.ui_b_nist.Text = scheme.B;
                this.ui_gx_nist.Text = scheme.Gx;
                this.ui_gy_nist.Text = scheme.Gy;
                this.ui_n_nist.Text = scheme.N;
                this.ui_f_nist.Text = scheme.H.ToString();
            }
        }

        private void ComboBox_sec2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.listBox_sec2 != null) 
            {
                string s = (string)this.comboBox_sec2.SelectedItem;
                if (s == null)
                    return;
                Scheme scheme = this.db.Schemes.Where(sh => sh.Name == s).First();
                this.SchemeGrade(scheme);
                this.ui_p_sec2.Text = scheme.P;
                this.ui_a_sec2.Text = scheme.A;
                this.ui_b_sec2.Text = scheme.B;
                this.ui_gx_sec2.Text = scheme.Gx;
                this.ui_gy_sec2.Text = scheme.Gy;
                this.ui_n_sec2.Text = scheme.N;
                this.ui_f_sec2.Text = scheme.H.ToString();
            }
        }

        private void ComboBox_custom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.listBox_custom != null)
            {
                string s = (string)this.comboBox_custom.SelectedItem;
                if (s == null)
                    return;
                Scheme scheme = this.db.Schemes.Where(sh => sh.Name == s).First();
                this.SchemeGrade(scheme);
                this.ui_p_custom.Text = scheme.P;
                this.ui_a_custom.Text = scheme.A;
                this.ui_b_custom.Text = scheme.B;
                this.ui_gx_custom.Text = scheme.Gx;
                this.ui_gy_custom.Text = scheme.Gy;
                this.ui_n_custom.Text = scheme.N;
                this.ui_f_custom.Text = scheme.H.ToString();
            }
        }
        
        private void Button_cr_keys_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EDS.LoadScheme(this.db.Schemes.Where(sh => sh.Name == this.select_scheme_textblock.Text).First());
                EDS.Key_length = Convert.ToInt32(this.key_lenght_textbox.Text);
                int bits = Maths.Length(EDS.curv.P);
                if (EDS.Key_length > bits)
                {
                    MessageBox.Show(String.Format("Разрядность ключа не должна превышать разрядность p ({0})!", bits), "Внимание");
                    return;
                }
                EDS.CreateKeys();
                if (!EDS.VerifyKeys())
                    MessageBox.Show("Ключи диссациативны, желательно их пересоздать.", "Внимание");
                this.public_key_d_textbox.Text = EDS.D.ToString();
                this.private_key_qx_textbox.Text = EDS.Q.X.ToString();
                this.private_key_qy_textbox.Text = EDS.Q.Y.ToString();
            }
            catch (Exception excp)
            {
                MessageBox.Show(excp.Message);
            }
        }

        private void Textbox_KeyDigitDown(object sender, KeyEventArgs e)
        {
            if (!SupportEDS.IsDigit(e.Key))
                e.Handled = true;
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void Create_scheme_button_Click(object sender, RoutedEventArgs e)
        {
            SupportEDS.GetMainWindow().frame_right.Source = new Uri("CreateSchemePage.xaml", UriKind.Relative);
        }

        private void Submit_button_Click(object sender, RoutedEventArgs e)
        {
            Scheme scheme = this.db.Schemes.Where(sh => sh.Name == this.select_scheme_textblock.Text).First();
            var main = SupportEDS.GetMainWindow();
            main.selectScheme = scheme;
            main.back_column.Width = new GridLength(0);
            main.frame_left_column.Width = new GridLength(1, GridUnitType.Star);
            main.frame_left.Content = new PageExplorer();
            main.frame_right.Source = new Uri("PageEmpty.xaml", UriKind.Relative);
        }

        private void Save_keys_button_Click(object sender, RoutedEventArgs e)
        {
            if (this.public_key_d_textbox.Text != "" && this.private_key_qx_textbox.Text != "" && this.private_key_qy_textbox.Text != "")
            {
                var wind = new NameSubmit(this.EDS.D, this.EDS.Q).ShowDialog();
            }
        }

        public void SchemeGrade(Scheme s)
        {
            BigInteger p = BigInteger.Parse(s.P);
            BigInteger a = BigInteger.Parse(s.A);
            BigInteger b = BigInteger.Parse(s.B);
            BigInteger n = BigInteger.Parse(s.N);
            Brush yellow = new SolidColorBrush(Color.FromArgb(255, 228, 149, 54));
            this.select_scheme_textblock.Text = s.Name;
            if (!CurveCondition.SingularCondition(a, b, p))
            {
                this.report_singular.Text = "Сингулярная кривая";
                this.report_singular.Foreground = Brushes.Red;
            }
            else
            {
                this.report_singular.Text = "Не сингулярная кривая";
                this.report_singular.Foreground = Brushes.Green;
            }
            if (!CurveCondition.AnomalousCondition(p, n))
            {
                this.report_anomal.Text = "Аномальная кривая";
                this.report_anomal.Foreground = Brushes.Red;
            }
            else
            {
                this.report_anomal.Text = "Не аномальная кривая";
                this.report_anomal.Foreground = Brushes.Green;
            }
            if (!CurveCondition.MOVCondition(20, p, n))
            {
                this.report_mov.Text = "Подвержена MOV атакам";
                this.report_mov.Foreground = yellow;
            }
            else
            {
                this.report_mov.Text = "Не подвержена MOV атакам";
                this.report_mov.Foreground = Brushes.Green;
            }
            if (!CurveCondition.NLengthCondition(p, n))
            {
                this.report_nlenght.Text = "Неприемлемый кофактор";
                this.report_nlenght.Foreground = Brushes.Red;
            }
            else
            {
                this.report_nlenght.Text = "Допустимый кофактор";
                this.report_nlenght.Foreground = Brushes.Green;
            }
            int prot = CurveCondition.ProtectionLevel(n);
            if (prot <= 16)
            {
                this.report_protect.Text = "Слабая защита";
                this.report_protect.Foreground = Brushes.Red;
            }
            else if (prot <= 29)
            {
                this.report_protect.Text = "Средняя защита";
                this.report_protect.Foreground = yellow;
            }
            else if (prot <= 39)
            {
                this.report_protect.Text = "Сильная защита";
                this.report_protect.Foreground = Brushes.Green;
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((TabItem)this.tabs.SelectedItem) == this.nist_tabitem)
                ComboBox_nist_SelectionChanged(this, null);
            else if (((TabItem)this.tabs.SelectedItem) == this.sec2_tabitem)
                ComboBox_sec2_SelectionChanged(this, null);
            else if (((TabItem)this.tabs.SelectedItem) == this.custom_tabitem)
                ComboBox_custom_SelectionChanged(this, null);
        }
    }
}
