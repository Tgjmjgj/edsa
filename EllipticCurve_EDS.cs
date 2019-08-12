using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;


namespace EDS
{
    public class EllipticCurve_EDS
    {
        private int key_length = 224;
        private EllipticCurve_Point q = new EllipticCurve_Point();
        private BigInteger d = new BigInteger();
        
        private STRIBOG hash = new STRIBOG(256);
        public EllipticCurve curv;

        public int Key_length { get { return key_length; } set { key_length = value; } }

        public EllipticCurve_Point Q { get { return q; } set { q = value; } }

        public BigInteger D { get { return d; } set { d = value; } }

        internal STRIBOG Hash { get { return hash; } set { hash = value; } }

        public EllipticCurve_EDS()
        {
            this.curv = null;
        }

        public EllipticCurve_EDS(BigInteger p, BigInteger a, BigInteger b, BigInteger n, BigInteger gx, BigInteger gy)
        {
            this.curv = new EllipticCurve(p, a, b, n, new EllipticCurve_Point(gx, gy));
        }

        public void LoadScheme(Scheme sch)
        {
            this.curv = new EllipticCurve(BigInteger.Parse(sch.P), BigInteger.Parse(sch.A), BigInteger.Parse(sch.B),
                            BigInteger.Parse(sch.N), new EllipticCurve_Point(BigInteger.Parse(sch.Gx), BigInteger.Parse(sch.Gy)));
        }
        
        public BigInteger GenPrivateKey(int BitSize)
        {
            BigInteger d = new BigInteger();
            Random rnd = new Random();
            do
            {
                d = Maths.RandBigInteger(BitSize, rnd);
            }
            while ((d < 0) || (d > this.curv.N));
            return d;
        }
        
        public EllipticCurve_Point GenPublicKey(BigInteger d)
        {
            EllipticCurve_Point Q = new EllipticCurve_Point();
            curv.Mult(d, this.curv.G, ref Q);
            return Q;
        }
        
        public bool CreateKeys()
        {
            try
            {
                this.D = this.GenPrivateKey(Key_length);
                this.Q = this.GenPublicKey(D);
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        public string SingGen(byte[] h, BigInteger d)
        {
            BigInteger alpha = BigInteger.Parse(SupportEDS.DecStringFromByteArray(h));
            BigInteger e = alpha % this.curv.N;
            if (e == 0)
                e = 1;
            BigInteger k = new BigInteger();
            EllipticCurve_Point C = new EllipticCurve_Point();
            BigInteger r = new BigInteger();
            BigInteger s = new BigInteger();
            do
            {
                Random rnd = new Random();
                do
                {
                    k = Maths.RandBigInteger(Maths.Length(this.curv.N), rnd);
                }
                while ((k < 0) || (k > this.curv.N));
                curv.Mult(k, this.curv.G, ref C);
                r = C.X % this.curv.N;
                s = ((r * d) + (k * e)) % this.curv.N;
            }
            while ((r == 0) || (s == 0));
            int midl = Maths.Length(this.curv.N) / 4;
            string Rvector = SupportEDS.Add0PaddingToString(r.ToString("X"), midl);
            string Svector = SupportEDS.Add0PaddingToString(s.ToString("X"), midl);
            return Rvector + Svector;
        }

        public bool SingVer(byte[] H, string sing, EllipticCurve_Point Q)
        {
            int midl = Maths.Length(this.curv.N) / 4;
            string Rvector = sing.Substring(0, midl);
            string Svector = sing.Substring(midl, midl);
            BigInteger r = BigInteger.Parse(SupportEDS.DecStringFromHexString(Rvector));
            BigInteger s = BigInteger.Parse(SupportEDS.DecStringFromHexString(Svector));
            if ((r < 1) || (r > (this.curv.N - 1)) || (s < 1) || (s > (this.curv.N - 1)))
                return false;
            BigInteger alpha = BigInteger.Parse(SupportEDS.DecStringFromByteArray(H));
            BigInteger e = alpha % this.curv.N;
            if (e == 0)
                e = 1;
            BigInteger v = Maths.GetInverse(e, this.curv.N);
            BigInteger z1 = (s * v) % this.curv.N;
            BigInteger z2 = this.curv.N + ((-(r * v)) % this.curv.N);
            EllipticCurve_Point A = new EllipticCurve_Point();
            EllipticCurve_Point B = new EllipticCurve_Point();
            EllipticCurve_Point C = new EllipticCurve_Point();
            curv.Mult(z1, this.curv.G, ref A);
            curv.Mult(z2, Q, ref B);
            curv.Sum(A, B, ref C);
            BigInteger R = C.X % this.curv.N;
            if (R == r)
                return true;
            else
                return false;
        }
        
        public bool VerifyKeys()
        {
            try
            {
                byte[] H = this.Hash.GetHash(Encoding.Default.GetBytes("Message"));
                string sign = this.SingGen(H, this.D);
                bool result = this.SingVer(H, sign, this.Q);
                return result;
            }
            catch
            {
                return false;
            }
        }

    }
}
