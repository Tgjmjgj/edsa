using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Drawing;
using Microsoft.Win32;

namespace EDS
{
    /// <summary>
    /// Логика взаимодействия для ExplorerListBox.xaml
    /// </summary>
    public partial class ExplorerListBox : UserControl
    {
        public static DependencyProperty HeightNodeProperty;

        public int HeightNode
        {
            get { return (int)GetValue(HeightNodeProperty); }
            set { SetValue(HeightNodeProperty, value); }
        }

        private string CurrentDirectory;

        public ExplorerListBox()
        {
            InitializeComponent();
        }

        static ExplorerListBox()
        {
            HeightNodeProperty = DependencyProperty.Register("HeightNode", typeof(int), typeof(ExplorerListBox));
            
        }

        public event SelectedFileEventHandler ListboxSelectionChanged;

        private void OnListboxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IExplorerNode node = ((IExplorerNode)listbox.SelectedItem);
            if (listbox.SelectedItem != null && ListboxSelectionChanged != null)
                if (node.Type == explType.File)
                    ListboxSelectionChanged(sender, new SelectedFileEventArgs(new EDSFile(node.Path, ((ExplorerNode)node).EdsState)));
                else
                    ListboxSelectionChanged(sender, new SelectedFileEventArgs());
        }

        public void AddExplorerNode(string path, string name, DateTime chg, explType type, long size, bool haveEds = false)
        {
            ExplorerNode node;
            if (type == explType.Folder)
            {
                node = new ExplorerNode(path, name, chg, type, size);
                node.MouseDoubleClick += Node_MouseDoubleClick;
            }
            else
                node = new ExplorerNode(path, name, chg, type, size, Icon.ExtractAssociatedIcon(path));
            if (haveEds)
                node.EdsState = 1;
            else
                node.EdsState = 0;
            node.Height = this.HeightNode;
            Binding binding = new Binding();
            binding.Source = this.listbox;
            binding.Path = new PropertyPath("ActualWidth");
            binding.Mode = BindingMode.OneWay;
            node.SetBinding(WidthProperty, binding);
            this.listbox.Items.Add(node);
        }

        public void AddDriveNode(string name)
        {
            var node = new ExplorerDriveNode();
            node.drive_text.Text = name;
            node.Height = 48;
            node.MouseDoubleClick += Node_MouseDoubleClick;
            this.listbox.Items.Add(node);
        }

        public void AddKeysNode(string path)
        {
            var node = new ExplorerNode(path, explType.Keys);
            node.MouseDoubleClick += Node_MouseDoubleClick;
            node.EdsState = 0;
            node.Height = this.HeightNode;
            Binding binding = new Binding();
            binding.Source = this.listbox;
            binding.Path = new PropertyPath("ActualWidth");
            binding.Mode = BindingMode.OneWay;
            node.SetBinding(WidthProperty, binding);
            this.listbox.Items.Add(node);
        }

        private void Node_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            IExplorerNode node = (IExplorerNode)sender;
            if (node.Type != explType.Keys)
                SetDirectory(node.Path);
            else
                SupportEDS.GetMainWindow().GetKeyFile(node.Path);
        }

        public void SetCommonDrivesDirectory()
        {
            this.listbox.Items.Clear();
            string[] drives = Environment.GetLogicalDrives();
            foreach (string drive in drives)
                this.AddDriveNode(drive);
            this.CurrentDirectory = "";
        }
        
        public void SetDirectory(string path)
        {
            this.listbox.Items.Clear();
            try
            {
                if (path == SupportEDS.spec_folder)
                {
                    List<string> f = Directory.EnumerateFiles(path).ToList();
                    for (int i = 0; i < f.Count; i++)
                    {
                        f[i] = f[i].Remove(f[i].Length - 5, 5);
                        string[] mas = f[i].Split('\\');
                        f[i] = mas[mas.Length - 1];
                    }
                    f = SupportEDS.Distinct(f);
                    foreach (string str in f)
                        AddKeysNode(str);
                }
                else
                {
                    foreach (string sub in Directory.EnumerateDirectories(path))
                    {
                        DirectoryInfo dir = new DirectoryInfo(sub);
                        AddExplorerNode(sub, dir.Name, dir.LastWriteTime, explType.Folder, 0);
                    }
                    List<string> files = new List<string>(), eds = new List<string>(), all = Directory.EnumerateFiles(path).ToList();
                    for (int i = 0; i < all.Count; i++)
                    {
                        if (all[i].Substring(all[i].Length - 5, 5) == ".edsc")
                            eds.Add(all[i]);
                        else
                            files.Add(all[i]);
                    }
                    foreach (string sub in files)
                    {
                        FileInfo file = new FileInfo(sub);
                        if (eds.Contains(sub + ".edsc"))
                            AddExplorerNode(sub, file.Name, file.LastWriteTime, explType.File, file.Length, true);
                        else
                            AddExplorerNode(sub, file.Name, file.LastWriteTime, explType.File, file.Length);
                    }
                }
                this.CurrentDirectory = path;
                int indx = -1;
                for (int i = 0; i < this.path_combobox.Items.Count; i++)
                {
                    if ((string)(this.path_combobox.Items[i]) == path)
                        indx = i;
                }
                if (indx != -1)
                    this.path_combobox.SelectedIndex = indx;
                else
                {
                    this.path_combobox.Items.Add(path);
                    this.path_combobox.SelectedItem = this.path_combobox.Items[this.path_combobox.Items.Count - 1];
                    if (this.path_combobox.Items.Count > 5)
                        this.path_combobox.Items.RemoveAt(0);
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show(String.Format("Нет доступа к {0}", path), "Расположение недоступно", MessageBoxButton.OK, MessageBoxImage.Stop);
                BackButton_Click(this, null);
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message, "Ошибка");
                BackButton_Click(this, null);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.CurrentDirectory != "")
            {
                var dir = new DirectoryInfo(this.CurrentDirectory);
                if (dir.Parent == null)
                    SetCommonDrivesDirectory();
                else
                    SetDirectory(dir.Parent.FullName);
            }
        }

        private void KeysButton_Click(object sender, RoutedEventArgs e)
        {
            SetDirectory(SupportEDS.spec_folder);
        }

        private void LoadKeysButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog _OpenFile_dlg = new OpenFileDialog();
            if (CurrentDirectory != "")
                _OpenFile_dlg.InitialDirectory = (string)this.path_combobox.SelectedItem;
            _OpenFile_dlg.AddExtension = true;
            _OpenFile_dlg.Filter += "Файлы ключей EDSA (KEYS) (*.priv, *.publ)|*.priv;*.publ" +
                                        "|Секретный ключ (*.priv)|*.priv" +
                                        "|Открытый ключ (*.publ)|*.publ";
            _OpenFile_dlg.Multiselect = true;
            _OpenFile_dlg.Title = "Загрузить";
            if (_OpenFile_dlg.ShowDialog() == true)
            {
                try
                {
                    foreach (string name in _OpenFile_dlg.FileNames)
                    {
                        string[] subs = name.Split('\\');
                        File.Copy(name, SupportEDS.spec_folder + "\\" + subs[subs.Length - 1]);
                    }
                }
                catch (Exception excp)
                {
                    MessageBox.Show(excp.Message);
                }
            }
        }

        private void path_combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetDirectory((string)e.AddedItems[0]);
        }

        public void UpdateFileInfo(EDSFile f)
        {
            foreach(IExplorerNode n in this.listbox.Items)
                if (n.Path == f.File.FullName && n.Type == explType.File)
                {
                    ExplorerNode node = (ExplorerNode)n;
                    node.EdsState = f.EdsState;
                    break;
                }
        }
    }
}
