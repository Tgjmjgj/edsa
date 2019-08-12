using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace EDS
{
    static class SupportEDS
    {
        public static string spec_folder =  Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\.edsa.keys";

        public static BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                return bitmapimage;
            }
        }
        
        public static MainWindow GetMainWindow()
        {
            return (MainWindow)App.Current.MainWindow;
        }

        public static string FileSizeFormat(long bt, bool sec = false)
        {
            string ret = "";
            double buf = bt;
            int i = 0;
            while (buf > 1024)
            {
                buf /= 1024;
                i++;
            }
            buf = Math.Round(buf, 1);
            ret = buf.ToString();
            switch (i)
            {
                case 0: ret += " Б"; break;
                case 1: ret += " Кб"; break;
                case 2: ret += " Мб"; break;
                case 3: ret += " Гб"; break;
            }
            if (sec)
                ret += " (" + bt.ToString() + " байт)";
            return ret;
        }

        public static bool IsDigit(Key k)
        {
            if (k >= Key.D0 && k <= Key.D9)
                return true;
            else
                return false;
        }

        public static bool IsSimpleKey(Key k)
        {
            if ((k >= Key.D0 && k <= Key.D9) || (k >= Key.A && k <= Key.Z) || k == (Key)'_')
                return true;
            else
                return false;
        }

        public static List<string> Distinct(List<string> l)
        {
            List<string> ret = new List<string>();
            for (int i = 0; i < l.Count; i++)
            {
                bool find = false;
                for (int j = 0; j < ret.Count; j++)
                    if (l[i] == ret[j])
                        find = true;
                if (!find)
                    ret.Add(l[i]);
            }
            return ret;
        }
        
        public static Scheme NewScheme(string nam, string pp, string aa, string bb, string gxx, string gyy, string nn, int hh)
        {
            Scheme scheme = new Scheme();
            scheme.Name = nam;
            scheme.Owner = "user";
            scheme.P = pp;
            scheme.A = aa;
            scheme.B = bb;
            scheme.Gx = gxx;
            scheme.Gy = gyy;
            scheme.N = nn;
            scheme.H = hh;
            return scheme;
        }
        
        public static uint[] BigIntegerToUintArray(BigInteger a)
        {
            bool negative = false;
            BigInteger bi;
            if (a < 0)
            {
                negative = true;
                bi = -a;
            }
            else
                bi = a;
            List<uint> buf = new List<uint>();
            do
            {
                buf.Add((uint)(bi & 0xffffffff));
                bi = bi >> 32;
            }
            while (bi != 0);
            if (negative)
                buf.Add(0x80000000);
            else
                buf.Add(0x0);
            return buf.ToArray();
        }

        public static BigInteger BigIntegerFromUintArray(uint[] arr)
        {
            BigInteger p = new BigInteger(0);
            for (int i = 0; i < arr.Length - 1; i++)
            {
                BigInteger buf = arr[i];
                p += (buf << (32 * i));
            }
            if ((arr[arr.Length - 1] & 0x80000000) != 0)
                p = -p;
            return p;
        }

        public static string DecStringFromHexString(string hex)
        {
            Dictionary<char, int> dic = new Dictionary<char, int>() { { '0', 0 }, { '1', 1 }, { '2', 2 },
                { '3', 3 }, { '4', 4 }, { '5', 5 }, { '6', 6 }, { '7', 7 }, { '8', 8 }, { '9', 9 },
                { 'A', 10 }, { 'B', 11 }, { 'C', 12 }, { 'D', 13 }, { 'E', 14 }, { 'F', 15 } };
            BigInteger buf = BigInteger.Zero;
            hex = hex.ToUpper();
            for (int i = 0; i < hex.Length; i++)
                buf += BigInteger.Multiply(BigInteger.Pow(16, i), dic[hex[hex.Length - 1 - i]]);
            return buf.ToString();
        }
        
        public static string DecStringFromByteArray(byte[] bts)
        {
            BigInteger buf = BigInteger.Zero;
            for (int i = 0; i < bts.Length; i++)
                buf += BigInteger.Multiply(BigInteger.Pow(256, i), bts[bts.Length - 1 - i]);
            return buf.ToString();
        }

        public static string Add0PaddingToString(string input, int size)
        {
            if (input.Length != size)
            {
                do
                    if (input.Length < size)
                        input = "0" + input;
                    else
                        input = input.Substring(1);
                while (input.Length != size);
            }
            return input;
        }
    }
}
