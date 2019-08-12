 using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;


namespace EDS
{
    public class EllipticCurve_Point : IComparable
    {
        BigInteger x;
        BigInteger y;
        public bool IsNull;

        public BigInteger X { get { return x; } set { x = value; } }

        public BigInteger Y { get { return y; } set { y = value; } }

        public EllipticCurve_Point()
        {
            this.X = new BigInteger();
            this.Y = new BigInteger();
            this.IsNull = false;
        }

        public EllipticCurve_Point(BigInteger xx, BigInteger yy)
        {
            this.X = xx;
            this.Y = yy;
            this.IsNull = false;
        }

        public EllipticCurve_Point(EllipticCurve_Point p)
        {
            this.X = p.X;
            this.Y = p.Y;
            this.IsNull = p.IsNull;
        }

        public EllipticCurve_Point(bool o)
        {
            this.X = new BigInteger();
            this.Y = new BigInteger();
            this.IsNull = o;
        }

        public static bool operator ==(EllipticCurve_Point first, EllipticCurve_Point second)
        {
            return first.X == second.X && first.Y == second.Y;
        }

        public static bool operator !=(EllipticCurve_Point first, EllipticCurve_Point second)
        {
            return first.X != second.X && first.Y != second.Y;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public int CompareTo(object p2)
        {
            EllipticCurve_Point pp = p2 as EllipticCurve_Point;
            if (this.X < pp.X)
                return -1;
            if (this.X > pp.X)
                return 1;
            if (this.Y < pp.Y)
                return -1;
            if (this.Y > pp.Y)
                return 1;
            return 0;
        }
    }

}
