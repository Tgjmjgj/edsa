using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace EDS
{
    class Schoof
    {
        public struct ijPair
        {
            public int i, j;
        }

        private const int NumberPts = 1;
        private BigInteger p1, q, q2, q4;
        private BackgroundWorker bw;
        private EllipticCurve ecs;
        private HcsrWithSEED hcr;
        private PolynomialMethods pm;
        private Random random;
        private List<long> primes;
        private List<BigInteger> xl, yl;
        private List<BigInteger> X, Y, root;

        public Schoof(int seed, BigInteger a, BigInteger b, BigInteger p, BackgroundWorker bw)
        {
            this.ecs = new EllipticCurve(p, a, b, 0, 0);
            this.bw = bw;
            q = Maths.Sqrt(p);
            if (q * q < p)
                q++;
            p1 = p - 1;
            q2 = 2 * q;
            q4 = 4 * q;
            hcr = new HcsrWithSEED(seed);
            pm = new PolynomialMethods(hcr);
            random = new Random(seed);
        }

        private BigInteger Tmod2()
        {
            X = new List<BigInteger>();
            Y = new List<BigInteger>();
            X.Add(0);
            X.Add(1);
            Y.Add(ecs.B);
            Y.Add(ecs.A);
            Y.Add(0);
            Y.Add(1);
            root = new List<BigInteger>();
            pm.FindRootsModuloPrime(ecs.P, Y, ref root);
            return root.Count > 0 ? 0 : 1;
        }

        private void BuildPointLists()
        {
            bool stop = false;
            BigInteger x = 0, y = 0, y2 = 0, z = 0;
            xl = new List<BigInteger>();
            yl = new List<BigInteger>();
            while (!stop)
            {
                for (int i = 0; i < NumberPts; i++)
                {
                    if (bw.CancellationPending)
                        return;
                    while (!stop)
                    {
                        if (bw.CancellationPending)
                            return;
                        x = hcr.RandomRange(1, p1);
                        z = ecs.Weierstrass(x);
                        if (z != 0)
                        {
                            y = hcr.SquareRootModPrime(z, ecs.P);
                            BigInteger test = (y * y) % ecs.P;
                            y2 = (y * y) % ecs.P;
                            if (y != 0 && z == y2)
                                stop = true;
                        }
                    }
                    xl.Add(x);
                    yl.Add(y);
                }
            }
        }

        private List<long> GetPrimes()
        {
            int SieveSize = 1000000;
            bool[] sieve = new bool[SieveSize];
            List<long> primes = new List<long>();
            int limit = SieveSize - 1;
            int i, k, n, nn, sqrtLimit = (int)Math.Sqrt(limit);
            for (n = 2; n <= limit; n++)
                sieve[n] = true;
            for (n = 2; n <= sqrtLimit; n++)
                for (i = 2; i <= n - 1; i++)
                    sieve[i * n] = false;
            for (n = 2; n <= sqrtLimit; n++)
            {
                if (sieve[n])
                {
                    k = 0;
                    nn = n * n;
                    i = nn + k * n;
                    while (i <= limit)
                    {
                        sieve[i] = false;
                        i = nn + k * n;
                        k++;
                    }
                }
            }
            for (n = 2; n <= limit; n++)
                if (sieve[n])
                    primes.Add(n);
            return primes;
        }

        private BigInteger TestHelper(BigInteger h0, BigInteger h1, BigInteger p2, BigInteger t, EllipticCurve_Point P)
        {
            BigInteger n0 = p2 - t, n1 = p2 + t;
            EllipticCurve_Point Q = new EllipticCurve_Point();
            if (n0 >= h0 && n0 <= h1)
                if (ecs.Mult(n0, P, ref Q))
                    if (Q.IsNull)
                        return n0;
            if (n1 >= h0 && n1 <= h1)
                if (ecs.Mult(n1, P, ref Q))
                    if (Q.IsNull)
                        return n1;
            return -1;
        }

        private BigInteger Test(BigInteger h0, BigInteger h1, BigInteger p2, BigInteger x, BigInteger y, List<BigInteger> m, List<BigInteger> v, BigInteger N)
        {
            BigInteger m2 = 0, n = 0, pr = 0;
            BigInteger t, tModN = 0;
            EllipticCurve_Point P = new EllipticCurve_Point();
            P.X = x;
            P.Y = y;
            t = Maths.ChineseRemainderTheorem(m, v);
            tModN = (t % N) % q2;
            n = TestHelper(h0, h1, p2, tModN, P);
            if (n != -1)
                return n;
            return -1;
        }

        private BigInteger BabyStepGiantStep(long l, BigInteger t2, BigInteger x, BigInteger y)
        {
            long W = (long)(Math.Ceiling(Math.Sqrt(0.5 * l)));
            BigInteger beta = 0, gamma = 0, k = 0;
            BigInteger pmodl = ecs.P % l;
            BigInteger ps = ecs.P * ecs.P;
            BigInteger Ax = 0, Bx = 0, Ay = 0, By = 0, kmod2 = 0;
            BigInteger psil = Maths.Psi((int)l, x, y, ecs.A, ecs.B, ecs.P);
            List<ijPair> S = new List<ijPair>();
            if (psil == 0)
                return -3;
            EllipticCurve_Point P0 = new EllipticCurve_Point(), 
                P1 = new EllipticCurve_Point(),
                P2 = new EllipticCurve_Point(), 
                P3 = new EllipticCurve_Point(),
                P12 = new EllipticCurve_Point(),
                P = new EllipticCurve_Point();
            List<EllipticCurve_Point> A = new List<EllipticCurve_Point>();
            List<EllipticCurve_Point> B = new List<EllipticCurve_Point>();
            P.X = x;
            P.Y = y;
            P0.X = BigInteger.ModPow(x, ecs.P, psil);
            P0.Y = BigInteger.ModPow(y, ecs.P, psil);
            P1.X = BigInteger.ModPow(x, ps, psil);
            P1.Y = BigInteger.ModPow(y, ps, psil);
            ecs.Mult(pmodl, P, ref P2);
            if (!ecs.Sum(P1, P2, ref P12))
                return -1;
            if (P12.IsNull)
                return 0;
            for (beta = 0; beta < W; beta++)
            {
                if (!ecs.Mult(beta, P0, ref P3))
                    return -1;
                if (!ecs.Sum(P12, P3, ref P3))
                    return -1;
                bool found = false;
                for (int i = 0; !found && i < A.Count; i++)
                    found = A[i].X == P3.X && A[i].Y == P3.Y;
                if (!found && !P3.IsNull)
                    A.Add(P3);
            }
            for (gamma = 0; gamma <= W; gamma++)
            {
                if (!ecs.Mult(gamma * W, P0, ref P3))
                    return -1;
                bool found = false;
                for (int i = 0; !found && i < B.Count; i++)
                    found = B[i].X == P3.X && B[i].Y == P3.Y;
                if (!found && !P3.IsNull)
                    B.Add(P3);
            }
            if (A.Count != 0 && B.Count != 0)
            {
                A.Sort();
                B.Sort();
                for (int i = 0; i < A.Count; i++)
                {
                    Ax = A[i].X;
                    for (int j = 0; j < B.Count; j++)
                    {
                        Bx = B[j].X;
                        if (Ax == Bx)
                        {
                            ijPair ij = new ijPair();
                            ij.i = i;
                            ij.j = j;
                            S.Add(ij);
                        }
                    }
                }
                if (S.Count == 1)
                {
                    beta = S[0].i;
                    gamma = S[0].j;
                    k = beta + gamma * W;
                    kmod2 = k % 2;
                    if (t2 == kmod2)
                    {
                        Ay = A[(int)beta].Y;
                        By = B[(int)gamma].Y;
                        if (Ay == By)
                            return k;
                        else
                            return l - k;
                    }
                    k = beta - gamma * W;
                    kmod2 = k % 2;
                    if (kmod2 < 0)
                        kmod2 += 2;
                    if (t2 == kmod2)
                    {
                        Ay = A[(int)beta].Y;
                        By = B[(int)gamma].Y;
                        if (Ay == By)
                            return k;
                        else
                            return l - k;
                    }
                }
            }
            return -1;
        }

        private BigInteger SchoofAlgorithm(BigInteger t2)
        {
            BigInteger p2 = ecs.P + 1;
            BigInteger h0 = p2 - q2;
            BigInteger h1 = p2 + q2;
            BigInteger x = 0, y = 0, n = 0, N = 0, t = 0;
            EllipticCurve_Point P = new EllipticCurve_Point(), 
                Q = new EllipticCurve_Point();
            List<BigInteger> m = null, v = null;
            t = q2;
            P.X = xl[0];
            P.Y = yl[0];
            n = TestHelper(h0, h1, p2, t, P);
            if (n != -1)
                return n;
            if (bw.CancellationPending)
                return -1;
            x = xl[0];
            y = yl[0];
            m = new List<BigInteger>();
            v = new List<BigInteger>();
            m.Add(2);
            v.Add(t2);
            for (int i = 1; i < primes.Count; i++)
            {
                if (bw.CancellationPending)
                    return -1;
                if (ecs.P < 64 && i > 200)
                    return -1;
                if (ecs.P < 1024 && i > 2000)
                    return -1;
                if (ecs.P == 1301)
                    return -1;
                BigInteger f = primes[i];
                BigInteger u = BabyStepGiantStep(primes[i], t2, x, y);
                if (i > 10000)
                    return -1;
                if (u >= 0)
                {
                    m.Add(f);
                    v.Add(u);
                    if (m.Count >= 3)
                    {
                        N = 2;
                        for (int j = 1; j < m.Count; j++)
                            N *= m[j];
                        if (N > q4 && m.Count >= 3)
                        {
                            n = Test(h0, h1, p2, x, y, m, v, N);
                            if (n > 0)
                                return n;
                        }
                    }
                }
                else if (u == -3)
                {
                    for (int j = 1; ; j++)
                    {
                        if (bw.CancellationPending)
                            return -1;
                        t = j * f;
                        if (t % 2 == t2)
                        {
                            P.X = x;
                            P.Y = y;
                            if (t >= h0 && t <= h1)
                            {
                                if (ecs.Mult(t, P, ref Q))
                                    if (Q.IsNull)
                                        return t;
                            }
                        }
                    }
                }
            }
            return -1;
        }

        public BigInteger RunSchoof()
        {
            bool composite = hcr.Composite(ecs.P, 20);
            if (composite)
                return -2;
            BigInteger t2 = Tmod2();
            BuildPointLists();
            primes = GetPrimes();
            return SchoofAlgorithm(t2);
        }
    }
}