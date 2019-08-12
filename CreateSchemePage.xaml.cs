using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Threading;


namespace EDS
{
    /// <summary>
    /// Логика взаимодействия для CreateSchemePage.xaml
    /// </summary>
    public partial class CreateSchemePage : Page
    {
        public EllipticCurve_EDS EDS;

        private SchemeContext db;

        private BigInteger n;
        private Schoof algo;
        private BackgroundWorker bw;
        DispatcherTimer timer;
        DateTime dt;

        public CreateSchemePage()
        {
            this.EDS = new EllipticCurve_EDS();
            this.db = SupportEDS.GetMainWindow().db;

            InitializeComponent();

            this.gen_textbox_p.GenButtonClick += Gen_textbox_p_GenButtonClick;
            this.gen_textbox_p.value_textbox.KeyDown += Textbox_KeyDigitDown;
            this.gen_textbox_p.value_textbox.KeyDown += Textbox_P_KeyDown;
            this.gen_textbox_p.down_textbox.KeyDown += Textbox_KeyDigitDown;
            this.gen_textbox_p.gen_button.Click += Textbox_P_KeyDown;

            this.gen_textbox_a.GenButtonClick += Gen_textbox_a_GenButtonClick;
            this.gen_textbox_a.value_textbox.KeyDown += Textbox_KeyDigitAndSubDown;
            this.gen_textbox_p.value_textbox.KeyDown += Textbox_A_KeyDown;
            this.gen_textbox_a.down_textbox.KeyDown += Textbox_KeyDigitDown;
            this.gen_textbox_a.gen_button.Click += Textbox_A_KeyDown;
            this.gen_textbox_a.value_textbox.Text = "-3";

            this.gen_textbox_b.GenButtonClick += Gen_textbox_b_GenButtonClick;
            this.gen_textbox_b.value_textbox.KeyDown += Textbox_KeyDigitAndSubDown;
            this.gen_textbox_p.value_textbox.KeyDown += Textbox_B_KeyDown;
            this.gen_textbox_b.down_textbox.KeyDown += Textbox_KeyDigitDown;
            this.gen_textbox_b.gen_button.Click += Textbox_B_KeyDown;
        }
        
        private void Gen_textbox_p_GenButtonClick(object sender, RoutedEventArgs e)
        {
            if (gen_textbox_p.down_textbox.Text != "" && gen_textbox_p.down_textbox.Text != "0" && gen_textbox_p.down_textbox.Text != "1")
            {
                int bits = Convert.ToInt32(gen_textbox_p.down_textbox.Text);
                BigInteger prime_p = Maths.genPseudoPrime(bits, new Random());
                gen_textbox_p.value_textbox.Text = prime_p.ToString();
            }
        }

        private void Gen_textbox_a_GenButtonClick(object sender, RoutedEventArgs e)
        {
            if (gen_textbox_a.down_textbox.Text != "" && gen_textbox_a.down_textbox.Text != "0")
            {
                int bits = Convert.ToInt32(gen_textbox_a.down_textbox.Text);
                BigInteger rand_a = Maths.RandBigInteger(bits, new Random());
                gen_textbox_a.value_textbox.Text = rand_a.ToString();
            }
        }

        private void Gen_textbox_b_GenButtonClick(object sender, RoutedEventArgs e)
        {
            if (gen_textbox_b.down_textbox.Text != "" && gen_textbox_b.down_textbox.Text != "0")
            {
                int bits = Convert.ToInt32(gen_textbox_b.down_textbox.Text);
                BigInteger rand_b = Maths.RandBigInteger(bits, new Random());
                gen_textbox_b.value_textbox.Text = rand_b.ToString();
            }
        }
        
        private void Textbox_KeyDigitDown(object sender, KeyEventArgs e)
        {
            if (!SupportEDS.IsDigit(e.Key))
                e.Handled = true;
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void Scheme_name_textbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (!SupportEDS.IsSimpleKey(e.Key))
                e.Handled = true;
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void Textbox_KeyDigitAndSubDown(object sender, KeyEventArgs e)
        {
            if (!SupportEDS.IsDigit(e.Key))
                e.Handled = true;
            if (e.Key == Key.Space)
                e.Handled = true;
            if ((e.Key == Key.Subtract || e.Key == Key.OemMinus) && ((TextBox)sender).SelectionStart == 0)
                if (((TextBox)sender).Text != "")
                {
                    if (((TextBox)sender).Text[0] == '-')
                        e.Handled = true;
                    else
                        e.Handled = false;
                }
                else
                    e.Handled = false; ;
        }

        private void Schoof_button_Click(object sender, RoutedEventArgs e)
        {
            //new Tests().SchoofStatistics(20);
            bool leav = false;
            if (!CheckP())
                leav = true;
            if (!CheckAandB())
                leav = true;
            if (leav)
                return;
            this.warn_text_n.Visibility = Visibility.Hidden;
            BigInteger a = BigInteger.Parse(gen_textbox_a.value_textbox.Text);
            BigInteger b = BigInteger.Parse(gen_textbox_b.value_textbox.Text);
            BigInteger p = BigInteger.Parse(gen_textbox_p.value_textbox.Text);
            bw = new BackgroundWorker();
            algo = new Schoof(1, a, b, p, bw);
            dt = DateTime.Now;
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += new DoWorkEventHandler(Bw_DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Bw_RunWorkerCompleted);
            StartTimer();
            this.schoof_value.Text = "";
            this.schoof_stop_button.Visibility = Visibility.Visible;
            bw.RunWorkerAsync();
        }

        private void Bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            timer.Stop();
            if (n == -1)
                MessageBox.Show("Не удается вычислить порядок - измените конфигурацию", "Внимание");
            else if (n > 0)
                this.schoof_value.Text = n.ToString();
            this.schoof_stop_button.Visibility = Visibility.Hidden;
        }

        private void Schoof_stop_button_Click(object sender, RoutedEventArgs e)
        {
            bw.CancelAsync();
            timer.Stop();
            this.timer_textblock.Visibility = Visibility.Hidden;
            this.schoof_stop_button.Visibility = Visibility.Hidden;
        }

        private void Bw_DoWork(object sender, DoWorkEventArgs e)
        {
            n = algo.RunSchoof();
        }

        private void StartTimer()
        {
            this.timer_textblock.Text = "00:00:00.000";
            this.timer_textblock.Visibility = Visibility.Visible;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += OnTimerTick;
            timer.Start();
        }

        private void OnTimerTick(object sender, object e)
        {
            TimeSpan ts = DateTime.Now - dt;
            this.timer_textblock.Text = ts.ToString(@"hh\:mm\:ss\.fff");
        }

        private void Find_point_button_Click(object sender, RoutedEventArgs e)
        {
            this.warn_text_g.Visibility = Visibility.Hidden;
            if (!CheckN())
                return;
            BigInteger p = BigInteger.Parse(this.gen_textbox_p.value_textbox.Text);
            BigInteger a = BigInteger.Parse(this.gen_textbox_a.value_textbox.Text);
            BigInteger b = BigInteger.Parse(this.gen_textbox_b.value_textbox.Text);
            BigInteger u = BigInteger.Parse(this.schoof_value.Text);
            BigInteger n = BigInteger.Parse(this.n_value.Text);
            BigInteger h = u / n;
            EllipticCurve curv = new EllipticCurve(p, a, b, n, u);
            EllipticCurve_Point point;
            try
            {
                int k = 0;
                do
                {
                    k++;
                    point = curv.PointFinding(u, h, n);
                    if (!point.IsNull)
                        break;
                }
                while (k < 4);
                this.Gx_textbox.Text = point.X.ToString();
                this.Gy_textbox.Text = point.Y.ToString();
                CheckG();
            }
            catch
            {
                MessageBox.Show("Неверный порядок группы точек, проверьте параметры");
            }

        }

        private void N_h_button_Click(object sender, RoutedEventArgs e)
        {
            if (this.r_min_textbox.Text == "" || this.l_max_textbox.Text == "")
                return;
            if (!CheckShoof())
                return;
            BigInteger u = BigInteger.Parse(this.schoof_value.Text);
            BigInteger r_min = BigInteger.Parse(this.r_min_textbox.Text);
            BigInteger l_max = BigInteger.Parse(this.l_max_textbox.Text);
            if (l_max < 3)
                return;
            BigInteger n = Maths.CheckingForNearPrimality(u, r_min, l_max);
            if (n == -1)
            {
                this.n_value.Text = "";
                this.h_value.Text = "";
                MessageBox.Show("Необходимо изменить набор параметров a, b и p");
            }
            else
            {
                BigInteger h = u / n;
                this.n_value.Text = n.ToString();
                this.h_value.Text = h.ToString();
                ConditionsN();
            }
        }

        private void Random_button_Click(object sender, RoutedEventArgs e)
        {
            if (this.complexity_rand_scheme.Text == "")
                return;
            dt = DateTime.Now;
            StartTimer();
            BeginRandomGeneration();
        }

        private void BeginRandomGeneration()
        {
            int bits = Convert.ToInt32(this.complexity_rand_scheme.Text);
            Random rnd = new Random();
            BigInteger p = Maths.genPseudoPrime(bits, rnd);
            gen_textbox_p.value_textbox.Text = p.ToString();
            BigInteger a = Maths.RandBigInteger(bits, rnd);
            gen_textbox_a.value_textbox.Text = a.ToString();
            BigInteger b = Maths.RandBigInteger(bits, rnd);
            gen_textbox_b.value_textbox.Text = b.ToString();
            bw = new BackgroundWorker();
            algo = new Schoof(1, a, b, p, bw);
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += new DoWorkEventHandler(Bw_DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Bw_RunWorkerCompletedRand);
            this.schoof_stop_button.Visibility = Visibility.Visible;
            this.schoof_value.Text = "";
            bw.RunWorkerAsync();
        }

        private void Bw_RunWorkerCompletedRand(object sender, RunWorkerCompletedEventArgs e)
        {
            if (n == -1)
                BeginRandomGeneration();
            else if (n > 0)
            {
                this.schoof_value.Text = n.ToString();
                BigInteger u = BigInteger.Parse(this.schoof_value.Text);
                if (this.r_min_textbox.Text == "" || this.l_max_textbox.Text == "")
                {
                    this.r_min_textbox.Text = "5";
                    this.l_max_textbox.Text = "3";
                }
                BigInteger r_min = BigInteger.Parse(this.r_min_textbox.Text);
                BigInteger l_max = BigInteger.Parse(this.l_max_textbox.Text);
                BigInteger nn = Maths.CheckingForNearPrimality(u, r_min, l_max);
                if (nn == -1)
                    BeginRandomGeneration();
                else
                {
                    BigInteger h = u / nn;
                    this.n_value.Text = nn.ToString();
                    this.h_value.Text = h.ToString();

                    BigInteger p = BigInteger.Parse(this.gen_textbox_p.value_textbox.Text);
                    BigInteger a = BigInteger.Parse(this.gen_textbox_a.value_textbox.Text);
                    BigInteger b = BigInteger.Parse(this.gen_textbox_b.value_textbox.Text);
                    EllipticCurve curv = new EllipticCurve(p, a, b, nn, u);
                    EllipticCurve_Point point;
                    int k = 0;
                    do
                    {
                        k++;
                        point = curv.PointFinding(u, h, nn);
                        if (!point.IsNull)
                            break;
                    }
                    while (k < 3);
                    if (point.IsNull)
                        BeginRandomGeneration();
                    else
                    {
                        this.Gx_textbox.Text = point.X.ToString();
                        this.Gy_textbox.Text = point.Y.ToString();
                        timer.Stop();
                        this.schoof_stop_button.Visibility = Visibility.Hidden;
                        MessageBox.Show(String.Format("Найдена схема:\n p = {0},\n a = {1},\n b = {2},\n n = {3},\n Gx = {4},\n Gy = {5}.",
                            this.gen_textbox_p.value_textbox.Text, this.gen_textbox_a.value_textbox.Text, this.gen_textbox_b.value_textbox.Text,
                            this.n_value.Text, this.Gx_textbox.Text, this.Gy_textbox.Text));
                    }
                }
            }
        }
        
        private bool EverythingValuesIsFine()
        {
            if (this.gen_textbox_p.value_textbox.Text == "")
                return false;
            if (this.gen_textbox_a.value_textbox.Text == "" || this.gen_textbox_a.value_textbox.Text == "-")
                return false;
            if (this.gen_textbox_b.value_textbox.Text == "" || this.gen_textbox_b.value_textbox.Text == "-")
                return false;
            if (this.schoof_value.Text == "" || this.schoof_stop_button.Visibility == Visibility.Visible)
                return false;
            if (this.n_value.Text == "" || this.h_value.Text == "")
                return false;
            if (this.Gx_textbox.Text == "" || this.Gy_textbox.Text == "" || (this.Gx_textbox.Text == "0" && this.Gy_textbox.Text == "0"))
                return false;
            return true;
        }

        private void Save_scheme_button_Click(object sender, RoutedEventArgs e)
        {
            if (EverythingValuesIsFine())
            {
                string new_name = this.scheme_name_textbox.Text;
                if (new_name == "")
                {
                    MessageBox.Show("Не указано имя сохраняемой схемы.", "Внимание");
                    return;
                }
                if (db.Schemes.Where(sh => sh.Name == new_name).Count() != 0)
                    MessageBox.Show("Схема с данным наименованием уже присутствует в коллекции.", "Внимание");
                else
                {
                    string p = this.gen_textbox_p.value_textbox.Text;
                    string a = this.gen_textbox_a.value_textbox.Text;
                    string b = this.gen_textbox_b.value_textbox.Text;
                    string n = this.n_value.Text;
                    string h = this.h_value.Text;
                    string gx = this.Gx_textbox.Text;
                    string gy = this.Gy_textbox.Text;
                    Scheme newScheme = SupportEDS.NewScheme(new_name, p, a, b, gx, gy, n, Convert.ToInt32(h));
                    try
                    {
                        this.db.Schemes.Add(newScheme);
                        this.db.SaveChanges();
                        MessageBox.Show(String.Format("Схема {0} успешно сохранена.", new_name), "Выполнено");
                    }
                    catch (Exception excp)
                    {
                        MessageBox.Show(excp.Message);
                    }
                }
            }
        }

        private bool CheckAandB()
        {
            if (this.gen_textbox_a.value_textbox.Text == "" || this.gen_textbox_a.value_textbox.Text == "-")
            {
                this.warn_text_a.Text = "Поле не может быть пустым";
                this.warn_text_a.Visibility = Visibility.Visible;
                return false;
            }
            if (this.gen_textbox_b.value_textbox.Text == "" || this.gen_textbox_b.value_textbox.Text == "-")
            {
                this.warn_text_b.Text = "Поле не может быть пустым";
                this.warn_text_b.Visibility = Visibility.Visible;
                return false;
            }
            BigInteger p = BigInteger.Parse(this.gen_textbox_p.value_textbox.Text);
            BigInteger a = BigInteger.Parse(this.gen_textbox_a.value_textbox.Text);
            BigInteger b = BigInteger.Parse(this.gen_textbox_b.value_textbox.Text);
            if (!CurveCondition.SingularCondition(a, b, p))
            {
                this.warn_text_a.Text = "Дискриминант равен нулю";
                this.warn_text_a.Visibility = Visibility.Visible;
                this.warn_text_b.Text = "Дискриминант равен нулю";
                this.warn_text_b.Visibility = Visibility.Visible;
                return false;
            }
            return true;
        }

        private bool CheckP()
        {
            if (this.gen_textbox_p.value_textbox.Text == "")
            {
                this.warn_text_p.Text = "Поле не может быть пустым";
                this.warn_text_p.Visibility = Visibility.Visible;
                return false;
            }
            BigInteger p = BigInteger.Parse(this.gen_textbox_p.value_textbox.Text);
            if (!Maths.isProbablePrime(p) || p == 2)
            {
                this.warn_text_p.Text = "Число должно быть простым";
                this.warn_text_p.Visibility = Visibility.Visible;
                return false;
            }
            return true;
        }

        private bool CheckShoof()
        {
            if (this.schoof_value.Text == "")
            {
                this.warn_text_n.Text = "Необходимо вычислить порядок эллиптической кривой";
                this.warn_text_n.Visibility = Visibility.Visible;
                return false;
            }
            return true;
        }

        private bool CheckN()
        {
            if (this.n_value.Text == "")
                return false;
            else
                return true;
        }

        private void ConditionsN()
        {
            BigInteger p = BigInteger.Parse(this.gen_textbox_p.value_textbox.Text);
            BigInteger n = BigInteger.Parse(this.schoof_value.Text);
            if (!CurveCondition.MOVCondition(20, p, n))
            {
                this.warn_text_n.Text = "Не выполняются требования МОВ";
                this.warn_text_n.Visibility = Visibility.Visible;
            }
            if (!CurveCondition.AnomalousCondition(p, n))
            {
                this.warn_text_n.Text = "Аномальная кривая";
                this.warn_text_n.Visibility = Visibility.Visible;
            }

        }
        
        private bool CheckG()
        {
            if (this.Gx_textbox.Text == "" || this.Gy_textbox.Text == "")
                return false;
            return true;
        }

        private void StackPanel_p_GotFocus(object sender, RoutedEventArgs e)
        {
            this.textBlock_info.Text = "Параметр p - порядок или же размер поля Fp, определяет количество элементов в поле дискретных чисел, " +
                            "на которое проецируется эллиптическая кривая.\n Должен быть простым целым числом большой разрядности (" +
                            "для использования в реальных схемах длина p устанавливается не меньше, чем 256 бит).\n" +
                            "Для увеличения быстродействия алгоритмов рекомендуется выбирать p из обобщенных чисел Мерсенна (" +
                            "числа вида 2^m - 1).";
            this.image_info.Source = null;
        }

        private void StackPanel_ab_GotFocus(object sender, RoutedEventArgs e)
        {
            this.textBlock_info.Text = "а и b - коэффициенты уравнения" +
                            " задающие вид эллиптической кривой. Рекомендуется выбор значения a = -3," +
                            " что ускоряет операцию сложения в координатах Якоби.";
            this.image_info.Source = new BitmapImage(new Uri("Images/ec.png", UriKind.Relative));
        }

        private void N_GotFocus(object sender, RoutedEventArgs e)
        {
            this.textBlock_info.Text = "#E(Fp) - порядок эллиптической кривой, показатель, отражающий количество точек" +
                            " на эллиптической кривой над полем Fp.\b Наиболее распространенный метод вычисления количества" +
                            " точек алгоритм Шуфа имеет достаточно большую вычислительную сложность O(log^6(p)) и является узким местом" +
                            " в генерации параметров схем электронной цифровой подписи.";
            this.image_info.Source = null;
        }

        private void Rmin_Lmax_GotFocus(object sender, RoutedEventArgs e)
        {
            this.textBlock_info.Text = "R_min и L - параметры для алгоритма поиска ближайших простых делителей к заданному числу.\n" +
                            " R_min обозначает нижнюю границу искомого числа.\n" +
                            " L_max является показателем максимального учитываемого делителя. Минимальное значение: 3.";
            this.image_info.Source = null;
        }

        private void N_h_GotFocus(object sender, RoutedEventArgs e)
        {
            this.textBlock_info.Text = " Параметр n - порядок аддитивной абелевой группы точек эллиптической кривой, ключевая характеристика" +
                            " криптостойкости выбранной схемы. Указывает на число элементов в подмножестве точек эллиптической кривой.\n" +
                            " h - параметр, называемый кофактор. Определяется отношением общего числа точек на эллиптической кривой #E(Fp) к" +
                            " порядку группы точек n. Данное число должно быть как можно меньше, в идеале равно 1 - в случае совпадения порядка" +
                            " группы точек с порядком самой эллиптической кривой.";
            this.image_info.Source = null;
        }

        private void G_GotFocus(object sender, RoutedEventArgs e)
        {
            this.textBlock_info.Text = " Генерирующая (базисная) точка эллиптической кривой G – точка на эллиптической кривой," +
                            " входящай в состав аддитивной абелевой группы точек, позволяющая путем умножения на саму себя" +
                            " получить полный перечень точек группы.\n Для этой точки выполняется равенство  n*G = O.";
            this.image_info.Source = null;
        }
        
        private void Textbox_A_KeyDown(object sender, EventArgs e)
        {
            this.warn_text_a.Visibility = Visibility.Hidden;
        }

        private void Textbox_B_KeyDown(object sender, EventArgs e)
        {
            this.warn_text_b.Visibility = Visibility.Hidden;
        }

        private void Textbox_P_KeyDown(object sender, EventArgs e)
        {
            this.warn_text_p.Visibility = Visibility.Hidden;
        }

    }
}
