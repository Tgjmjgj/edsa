using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDS
{
    public enum explType { File, Folder, Drive, Keys, EDS };

    interface IExplorerNode
    {
        string Path { get; set; }

        explType Type { get; set; }
    }
}
