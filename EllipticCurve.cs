using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace EDS
{
    public class EllipticCurve
    {
        private BigInteger p = new BigInteger();
        private BigInteger a = new BigInteger();
        private BigInteger b = new BigInteger();
        private BigInteger n = new BigInteger();
        private EllipticCurve_Point g = new EllipticCurve_Point();

        public static EllipticCurve_Point O = new EllipticCurve_Point(true);
        private BigInteger power = 0;

        public BigInteger P { get { return p; } set { p = value; } }

        public BigInteger A { get { return a; } set { a = value; } }

        public BigInteger B { get { return b; } set { b = value; } }

        public BigInteger N { get { return n; } set { n = value; } }

        public EllipticCurve_Point G { get { return g; } set { g = value; } }

        public EllipticCurve(BigInteger p, BigInteger a, BigInteger b, BigInteger n, EllipticCurve_Point g)
        {
            this.P = p;
            this.A = a;
            this.B = b;
            this.N = n;
            this.G = g;
        }

        public EllipticCurve(BigInteger p, BigInteger a, BigInteger b, BigInteger n, BigInteger u)
        {
            this.P = p;
            this.A = a;
            this.B = b;
            this.N = n;
            this.power = u;
        }
        public bool Sum(EllipticCurve_Point p1, EllipticCurve_Point p2, ref EllipticCurve_Point res)
        {
            BigInteger lambda = -1;
            if (p1.IsNull && p2.IsNull)
            {
                res = EllipticCurve.O;
                return true;
            }
            BigInteger ps = (p - p2.Y) % p;
            if (p1.X == p2.X && p1.Y == ps)
            {
                res = EllipticCurve.O;
                return true;
            }
            if (p1.IsNull)
            {
                res = new EllipticCurve_Point(p2);
                return true;
            }
            if (p2.IsNull)
            {
                res = new EllipticCurve_Point(p1);
                return true;
            }
            if (p1.X != p2.X)
            {
                BigInteger i = (p2.X - p1.X) % p;
                BigInteger y = (p2.Y - p1.Y) % p;
                if (i < 0)
                    i += p;
                i = Maths.GetInverse(i, p);
                if (i == 0)
                    return false;
                lambda = (i * y) % p;
            }
            else
            {
                BigInteger y2 = (2 * p1.Y) % p;
                BigInteger i = Maths.GetInverse(y2, p);
                if (i < 0)
                    i += p;
                if (i == 0)
                    return false;
                BigInteger x3 = (3 * p1.X) % p;
                x3 = (x3 * p1.X) % p;
                x3 = (x3 + a) % p;
                lambda = (x3 * i) % p;
            }
            res = new EllipticCurve_Point();
            BigInteger lambda2 = (lambda * lambda) % p;
            BigInteger delta1 = (p1.X + p2.X) % p;
            res.X = (lambda2 - delta1) % p;
            BigInteger delta2 = (p1.X - res.X) % p;
            delta2 = (lambda * delta2) % p;
            res.Y = (delta2 - p1.Y) % p;
            if (res.X < 0)
                res.X += p;
            if (res.Y < 0)
                res.Y += p;
            return true;
        }

        public bool Mult(BigInteger x, EllipticCurve_Point p1, ref EllipticCurve_Point res)
        {
            if (x < 0)
            {
                res = EllipticCurve.O;
                return false;
            }
            BigInteger k = x;
            EllipticCurve_Point S;
            res = new EllipticCurve_Point(true);
            S = new EllipticCurve_Point(p1);
            while (k != 0)
            {
                BigInteger m = k % 2;
                if (m == 1)
                    if (!Sum(res, S, ref res))
                        return false;
                k = k / 2;
                if (k != 0)
                    if (!Sum(S, S, ref S))
                        return false;
            }
            return true;
        }

        public EllipticCurve_Point GenerateRandomPoint()
        {
            EllipticCurve_Point ret = new EllipticCurve_Point();
            BigInteger x = new BigInteger();
            BigInteger sqr = new BigInteger();
            BigInteger pp = BigInteger.Parse(P.ToString());
            do
            {
                x = Maths.RandInRange(0, P);
                BigInteger alpha = ((x * x * x) + A * x + B) % P;
                if (alpha == 0)
                {
                    ret.X = x;
                    ret.Y = 0;
                    return ret;
                }
                BigInteger bi = BigInteger.Parse(alpha.ToString());
                sqr = Maths.ModSqrt(bi, pp);
                if ((sqr * sqr) % pp != bi)
                    sqr = 0;
            }
            while (sqr == 0);
            ret.X = x;
            ret.Y = sqr;
            return ret;
        }

        public EllipticCurve_Point PointFinding(BigInteger u, BigInteger h, BigInteger n)
        {
            EllipticCurve_Point point;
            EllipticCurve_Point G = new EllipticCurve_Point();
            EllipticCurve_Point Q = new EllipticCurve_Point();
            do
            {
                do
                {
                    point = GenerateRandomPoint();
                }
                while (point.IsNull);
                Mult(h, point, ref G);
            }
            while (G.IsNull);
            Mult(n, G, ref Q);
            if (Q.IsNull)
                return G;
            else
                return EllipticCurve.O;
        }
        
        public EllipticCurve_Point Negate(EllipticCurve_Point p1)
        {
            EllipticCurve_Point Q = new EllipticCurve_Point();
            Q.X = p1.X;
            Q.Y = (p - p1.Y) % p;
            if (Q.Y < 0)
                Q.Y += p;
            return Q;
        }

        public BigInteger Weierstrass(BigInteger x)
        {
            BigInteger t3 = BigInteger.ModPow(x, 3, p);
            BigInteger t1 = Maths.MulMod(a, x, p);
            BigInteger t2 = Maths.AddMod(t3, t1, p);
            return Maths.AddMod(t2, b, p);
        }

    }
}
