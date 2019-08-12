using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace EDS
{
    public class SelectedFileEventArgs : EventArgs
    {
        private EDSFile fileInfo;

        public SelectedFileEventArgs()
        {
            this.FileInfo = null;
        }

        public SelectedFileEventArgs(EDSFile f)
        {
            this.FileInfo = f;
        }

        public EDSFile FileInfo { get { return fileInfo; } set { fileInfo = value; } }
    }

    public delegate void SelectedFileEventHandler(object sender, SelectedFileEventArgs e);

}
