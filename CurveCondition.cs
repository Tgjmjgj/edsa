using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace EDS
{
    static class CurveCondition
    {
        public static bool AnomalousCondition(BigInteger p, BigInteger n)
        {
            return !(p == n);
        }

        public static bool MOVCondition(int B, BigInteger p, BigInteger n)
        {
            BigInteger t = 0;
            for (int i = 0; i < B; i++)
            {
                t = (t * p) % n;
                if (t == 1)
                    return false;
            }
            return true;
        }

        public static int ProtectionLevel(BigInteger n)
        {
            return Maths.Length(n) / 8;
        }

        public static bool NLengthCondition(BigInteger p, BigInteger n)
        {
            if (n * n > 16 * p)
                return true;
            else
                return false;
        }

        public static bool SingularCondition(BigInteger a, BigInteger b, BigInteger p)
        {
            BigInteger alpha = 4 * (a * a * a) + 27 * (b * b);
            if (alpha % p == 0)
                return false;
            else
                return true;
        }
    }
}
