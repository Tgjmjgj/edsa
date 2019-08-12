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

namespace EDS
{
    /// <summary>
    /// Логика взаимодействия для PageFileInfo.xaml
    /// </summary>
    public partial class PageFileInfo : Page
    {
        private EDSFile file;

        public EDSFile File
        {
            get
            {
                return file;
            }

            set
            {
                file = value;
            }
        }

        public PageFileInfo()
        {
            InitializeComponent();
        }

        public PageFileInfo(EDSFile f)
        {
            this.File = f;
            InitializeComponent();
        }
        

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (File != null)
            {
                this.file_name.Text = File.File.Name;
                this.file_big_image.Source = SupportEDS.BitmapToImageSource(Icon.ExtractAssociatedIcon(File.File.FullName).ToBitmap());
                this.file_extn_val.Text = File.File.Extension;
                this.file_desc_val.Text = File.File.Name;
                this.file_path_val.Text = File.File.DirectoryName;
                this.file_size_val.Text = SupportEDS.FileSizeFormat(File.File.Length, true);
                this.file_crat_val.Text = File.File.CreationTime.ToString("d MMMM yyyy г., h:mm:ss");
                this.file_edit_val.Text = File.File.LastWriteTime.ToString("d MMMM yyyy г., h:mm:ss");
                this.file_open_val.Text = File.File.LastAccessTime.ToString("d MMMM yyyy г., h:mm:ss");
                if (File.EdsState == 0)
                {
                    this.message_text.Text = "Цифровой подписи не обнаружено.";
                    this.big_button_text.Text = "СОЗДАТЬ ПОДПИСЬ";
                }
                else if (File.EdsState == 1)
                {
                    this.message_text.Text = "Цифровая подпись присутствует.";
                    this.big_button_text.Text = "ПРОВЕРИТЬ";
                }
                else if (File.EdsState == 2)
                {
                    this.message_text.Text = "Подлинность цифровой подписи подтверждена.";
                    this.big_button_text.Text = "ПРОВЕРИТЬ";
                }
                else if(File.EdsState == 3)
                {
                    this.message_text.Text = "Подпись не соответствует документу.";
                    this.big_button_text.Text = "ПРОВЕРИТЬ";
                }
            }
        }

        private void File_name_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.ani_flow_text.From = this.canvas_file_name.ActualWidth;
            this.ani_flow_text.To = 0 - this.file_name.ActualWidth;
        }

        private void Create_eds_but_Click(object sender, RoutedEventArgs e)
        {
            CreateEDS();
        }

        private void TextBlock_MouseUp(object sender, MouseButtonEventArgs e)
        {
            CreateEDS();
        }

        private void CreateEDS()
        {
            var main = SupportEDS.GetMainWindow();
            if (main.selectScheme != null)
            {
                if (this.big_button_text.Text == "СОЗДАТЬ ПОДПИСЬ")
                {
                    if (main.EDS.D != 0)
                    {
                        try
                        {
                            main.EDS.LoadScheme(main.selectScheme);
                            byte[] file = System.IO.File.ReadAllBytes(this.File.File.FullName);
                            byte[] H = main.EDS.Hash.GetHash(file);
                            string sign = main.EDS.SingGen(H, main.EDS.D);
                            System.IO.File.WriteAllText(this.File.File.FullName + ".edsc", sign, Encoding.Default);
                            this.File.EdsState = 1;
                            ((PageExplorer)main.frame_left.Content).explorer.UpdateFileInfo(this.File);
                            this.message_text.Text = "Цифровая подпись присутствует.";
                            this.big_button_text.Text = "ПРОВЕРИТЬ";
                        }
                        catch
                        {
                            MessageBox.Show("Подпись не соответствует выбранной схеме или ключу");
                        }
                    }
                    else
                        MessageBox.Show("Не выбран секретный ключ", "Внимание");
                }
                else if (this.big_button_text.Text == "ПРОВЕРИТЬ")
                {
                    if (!main.EDS.Q.IsNull)
                    {
                        try { 

                        main.EDS.LoadScheme(main.selectScheme);
                            if (System.IO.File.Exists(this.File.File.FullName + ".edsc"))
                            {
                                byte[] file = System.IO.File.ReadAllBytes(this.File.File.FullName);
                                byte[] H = main.EDS.Hash.GetHash(file);
                                string sign = System.IO.File.ReadAllLines(this.File.File.FullName + ".edsc", Encoding.Default)[0];
                                bool result = main.EDS.SingVer(H, sign, main.EDS.Q);
                                if (result)
                                {
                                    this.message_text.Text = "Подлинность цифровой подписи подтверждена.";
                                    this.File.EdsState = 2;
                                }
                                else
                                {
                                    this.message_text.Text = "Подпись не соответствует документу.";
                                    this.File.EdsState = 3;
                                }
                                ((PageExplorer)main.frame_left.Content).explorer.UpdateFileInfo(this.File);
                            }
                        }
                        catch
                        {
                            MessageBox.Show("Подпись не соответствует выбранной схеме или ключу");
                        }
                    }
                    else
                        MessageBox.Show("Не выбран открытый ключ", "Внимание");
                }
            }
        }
    }
}
