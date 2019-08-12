﻿using System.Collections.Generic;
using System.Numerics;

namespace EDS
{
    class PolynomialMethods
    {
        private HcsrWithSEED hcr;

        public PolynomialMethods(HcsrWithSEED hcr)
        {
            this.hcr = hcr;
        }
        
        private List<BigInteger> PolyCopy(List<BigInteger> a)
        {
            List<BigInteger> b = new List<BigInteger>();
            for (int i = 0; i < a.Count; i++)
                b.Add(a[i]);
            return b;
        }

        private List<BigInteger> PolyRev(List<BigInteger> a)
        {
            List<BigInteger> b = new List<BigInteger>();
            for (int i = a.Count - 1; i >= 0; i--)
                b.Add(a[i]);
            return b;
        }

        public List<BigInteger> PolyMulMod(BigInteger p, List<BigInteger> a, List<BigInteger> b)
        {
            int i, j, k, m = a.Count - 1, n = b.Count - 1, q = m + n;
            BigInteger ai = 0, bj = 0, sum = 0;
            List<BigInteger> c = new List<BigInteger>();
            for (k = 0; k <= q; k++)
            {
                sum = 0;
                for (i = 0; i <= k; i++)
                {
                    j = k - i;
                    if (i > m)
                        ai = 0;
                    else
                        ai = a[i];
                    if (j > n)
                        bj = 0;
                    else
                        bj = b[j];
                    sum = Maths.AddMod(sum, Maths.MulMod(ai, bj, p), p);
                }
                c.Add(sum);
            }
            return c;
        }

        public void PolyDivMod(BigInteger p, List<BigInteger> u, List<BigInteger> v, ref List<BigInteger> q, ref List<BigInteger> r)
        {
            int j, jk, k, nk, m = u.Count - 1, n = v.Count - 1, t, s;
            BigInteger a = 0, b = 0, vn = 0, qk = 0;
            List<BigInteger> tr = new List<BigInteger>();
            r = new List<BigInteger>();
            vn = v[n];
            if (n == 0)
            {
                r.Add(0);
                q = new List<BigInteger>();
                for (int i = 0; i < u.Count; i++)
                    q.Add(Maths.MulMod(u[i], vn, p));
                return;
            }
            for (j = 0; j <= m; j++)
                r.Add(u[j]);
            if (m < n)
            {
                q = new List<BigInteger>();
                q.Add(0);
            }
            else
            {
                t = m - n;
                s = n - 1;
                q = new List<BigInteger>();
                for (k = t; k >= 0; k--)
                    q.Add(0);
                for (k = t; k >= 0; k--)
                {
                    nk = n + k;
                    if (k != 0)
                        a = BigInteger.ModPow(vn, k, p);
                    else
                        a = 1;
                    qk = Maths.MulMod(r[nk], a, p);
                    q[k] = qk;
                    for (j = nk - 1; j >= 0; j--)
                    {
                        jk = j - k;
                        if (jk >= 0)
                        {
                            a = Maths.MulMod(vn, r[j], p);
                            b = Maths.MulMod(r[nk], v[jk], p);
                            r[j] = Maths.SubMod(a, b, p);
                        }
                        else
                            r[j] = Maths.MulMod(vn, r[j], p);
                    }
                }
                for (int i = s; i > 0; i--)
                {
                    if (r[i] == 0)
                        s--;
                    else
                        break;
                }
                for (int i = s; i >= 0; i--)
                    tr.Add(r[i]);
                r = PolyRev(tr);
            }
        }

        public void PolyPowMod(BigInteger p, BigInteger n, List<BigInteger> A, List<BigInteger> m, ref List<BigInteger> s)
        {
            BigInteger a = n, m2 = 0;
            List<BigInteger> P = new List<BigInteger>(), q = new List<BigInteger>();
            List<BigInteger> x = null, y = new List<BigInteger>();
            s = new List<BigInteger>();
            s.Add(1);
            x = PolyCopy(A);
            while (a != 0)
            {
                m2 = a % 2;
                if (m2 == 1)
                {
                    P = PolyMulMod(p, s, x);
                    PolyDivMod(p, P, m, ref q, ref s);
                }
                a = a / 2;
                if (a != 0)
                {
                    P = PolyMulMod(p, x, x);
                    PolyDivMod(p, P, m, ref q, ref x);
                }
            }
        }

        public List<BigInteger> PolySubMod(BigInteger p, List<BigInteger> a, List<BigInteger> b)
        {
            int da = a.Count - 1, db = b.Count - 1;
            BigInteger ci = 0, z = 0;
            List<BigInteger> c = new List<BigInteger>();
            if (da >= db)
            {
                for (int i = 0; i <= db; i++)
                {
                    ci = Maths.SubMod(a[i], b[i], p);
                    c.Add(ci);
                }
                for (int i = db + 1; i <= da; i++)
                    c.Add(a[i]);
            }
            else
            {
                for (int i = 0; i <= da; i++)
                {
                    ci = Maths.SubMod(a[i], b[i], p);
                    c.Add(ci);
                }
                for (int i = da + 1; i <= db; i++)
                {
                    ci = Maths.SubMod(z, b[i], p);
                    c.Add(ci);
                }
            }
            int s = c.Count - 1;
            List<BigInteger> tc = new List<BigInteger>();
            for (int i = s; i >= 0; i--)
            {
                if (c[i] == 0)
                    s--;
                else
                    break;
            }
            for (int i = s; i >= 0; i--)
                tc.Add(c[i]);
            c = PolyCopy(tc);
            return c;
        }

        public List<BigInteger> PolyGCDMod(BigInteger p, List<BigInteger> A, List<BigInteger> B)
        {
            bool nonzero = false;
            int degreeA = A.Count - 1, degreeB = B.Count - 1;
            List<BigInteger> a = null, b = null, q = null, r = null;
            if (degreeA > degreeB)
            {
                a = PolyCopy(A);
                b = PolyCopy(B);
            }
            else
            {
                a = PolyCopy(B);
                b = PolyCopy(A);
            }
            for (int i = 0; i <= b.Count - 1 && !nonzero; i++)
                nonzero = b[i] != 0;
            while (nonzero)
            {
                PolyDivMod(p, a, b, ref q, ref r);
                a = PolyCopy(b);
                b = PolyCopy(r);
                nonzero = false;
                for (int i = 0; i <= b.Count - 1 && !nonzero; i++)
                    nonzero = b[i] != 0;
            }
            return a;
        }

        private void Recurse(BigInteger p, List<BigInteger> A, ref List<BigInteger> root)
        {
            int count = 0, degreeA = A.Count - 1, degreeB = 0;
            BigInteger exp = 0, p1 = p - 1, D = 0, a = 0, b = 0, c = 0, e = 0;
            List<BigInteger> B = null, d = null;
            List<BigInteger> q = null, r = null, u = null;
            exp = p1 / 2;
            if (degreeA != 0)
            {
                if (degreeA == 1)
                {
                    if (A[0] != 0)
                    {
                        a = Maths.GetInverse(A[1], p);
                        b = A[0];
                        b = -b;
                        b = Maths.MulMod(b, a, p);
                        root.Add(b);
                    }
                }
                else if (degreeA == 2)
                {
                    a = Maths.MulMod(A[1], A[1], p);
                    b = Maths.MulMod(A[0], A[2], p);
                    c = Maths.MulMod(b, 4, p);
                    D = Maths.SubMod(a, c, p);
                    e = hcr.SquareRootModPrime(D, p);
                    BigInteger test = (e * e) % p;
                    if (e == 1)
                        return;
                    a = Maths.MulMod(A[2], 2, p);
                    D = Maths.GetInverse(a, p);
                    if (D == 0)
                    {
                        a = -a;
                        a = Maths.AddMod(a, p, p);
                        D = Maths.GetInverse(a, p);
                    }
                    a = Maths.SubMod(e, A[1], p);
                    root.Add(Maths.MulMod(a, D, p));
                    A[1] = -A[1];
                    e = -e;
                    a = Maths.AddMod(A[1], e, p);
                    root.Add(Maths.MulMod(a, D, p));
                }
                else
                {
                    do
                    {
                        count++;
                        a = hcr.RandomRange(0, p1);
                        u = new List<BigInteger>();
                        u.Add(a);
                        u.Add(1);
                        PolyPowMod(p, exp, u, A, ref d);
                        if (d.Count - 1 != 0)
                        {
                            d[0] = Maths.SubMod(d[0], 1, p);
                            B = PolyGCDMod(p, d, A);
                            if (B.Count == 1 && B[0] == 1)
                                return;
                            degreeB = B.Count - 1;
                        }
                    }
                    while (count < 16 && degreeB == 0 || degreeB == degreeA);
                    if (count == 16)
                        return;
                    Recurse(p, B, ref root);
                    PolyDivMod(p, A, B, ref q, ref r);
                    Recurse(p, q, ref root);
                }
            }
        }

        private BigInteger Horner(BigInteger p, BigInteger x, List<BigInteger> P)
        {
            BigInteger s = P[P.Count - 1], t = 0;
            for (int i = P.Count - 2; i >= 0; i--)
            {
                t = Maths.MulMod(s, x, p);
                s = Maths.AddMod(t, P[i], p);
            }
            return s;
        }

        public void FindRootsModuloPrime(BigInteger p, List<BigInteger> P, ref List<BigInteger> root)
        {
            BigInteger r = 0, s = 0, t = 0;
            List<BigInteger> A = null, B = null;
            if (p <= P.Count - 1)
            {
                s = 0;
                root = new List<BigInteger>();
                for (int i = 0; i < p; i++)
                {
                    s = i;
                    r = Horner(p, s, P);
                    if (r == 0)
                        root.Add(s);
                }
            }
            else
            {
                A = PolyCopy(P);
                if (root == null)
                    root = new List<BigInteger>();
                if (A[0] == 0)
                {
                    root.Add(0);
                    B = new List<BigInteger>();
                    for (int i = 0; i < A.Count - 1; i++)
                        B.Add(A[i + 1]);
                    A = PolyCopy(B);
                }
                Recurse(p, A, ref root);
                if (root != null)
                    root.Sort();
            }
        }
    }
}
