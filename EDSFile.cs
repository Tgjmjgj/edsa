using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDS
{
    public class EDSFile
    {
        private int edsState;

        private FileInfo file;

        public EDSFile()
        {
            this.File = null;
            this.EdsState = 0;
        }

        public EDSFile(string pt, int st)
        {
            this.File = new FileInfo(pt);
            this.EdsState = st;
        }

        public int EdsState { get { return edsState; } set { edsState = value; } }

        public FileInfo File { get { return file; } set { file = value; } }
    }
}
