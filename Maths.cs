using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace EDS
{
    static class Maths
    {
        public static BigInteger AddMod(BigInteger a, BigInteger b, BigInteger p)
        {
            BigInteger x = (a + b) % p;
            if (x < 0)
                x += p;
            return x;
        }

        public static BigInteger MulMod(BigInteger a, BigInteger b, BigInteger p)
        {
            BigInteger x = (a * b) % p;
            if (x < 0)
                x += p;
            return x;
        }

        public static BigInteger SubMod(BigInteger a, BigInteger b, BigInteger p)
        {
            BigInteger x = (a - b) % p;
            if (x < 0)
                x += p;
            return x;
        }

        public static BigInteger RandBigInteger(int len, Random rnd)
        {
            int b = len;
            int ost = b % 8;
            int b_count = (b / 8) + (ost == 0 ? 0 : 1);
            if (b_count == 0)
                return BigInteger.Zero;
            byte ml = (byte)Math.Pow(2, (b - 1) % 8);
            for (int i = ((b - 1) % 8) - 1; i >= 0; i--)
                ml += (byte)(Math.Pow(2, i) * rnd.Next(0, 2));
            byte[] arr = new byte[b_count];
            arr[0] = ml;
            for (int i = 1; i < b_count; i++)
                arr[i] = (byte)rnd.Next(0, 256);
            return BigInteger.Parse(SupportEDS.DecStringFromByteArray(arr));
        }

        public static BigInteger Sqrt(BigInteger n)
        {
            if (n == 0)
                return 0;
            BigInteger r = Sqrt(n >> 2);
            BigInteger r2 = r << 1;
            BigInteger s = r2 + 1;
            if (n < s * s)
                return r2;
            else
                return s;
        }
        
        public static BigInteger GetInverse(BigInteger a, BigInteger m)
        {
            BigInteger x, y;
            BigInteger g = GCD(a, m, out x, out y);
            BigInteger ret = new BigInteger();
            if (g == 1)
                ret = Maths.AddMod(x % m, m, m);
            else
                ret = 0;
            return ret;
        }

        private static BigInteger GCD(BigInteger a, BigInteger b, out BigInteger x, out BigInteger y)
        {
            if (a == 0)
            {
                x = 0;
                y = 1;
                return b;
            }
            BigInteger x1, y1;
            BigInteger d = GCD(b % a, a, out x1, out y1);
            x = y1 - (b / a) * x1;
            y = x1;
            return d;
        }

        public static BigInteger ModSqrt(BigInteger a, BigInteger q)
        {
            BigInteger generRnd = new BigInteger();
            do
                generRnd = RandBigInteger(255, new Random());
            while (BigInteger.ModPow(generRnd, (q - 1) / 2, q) == 1);
            BigInteger s = 0;
            BigInteger t = q - 1;
            while ((t & 1) != 1)
            {
                s++;
                t = t >> 1;
            }
            BigInteger InvA = GetInverse(a, q);
            BigInteger c = BigInteger.ModPow(generRnd, t, q);
            BigInteger r = BigInteger.ModPow(a, ((t + 1) / 2), q);
            BigInteger d = new BigInteger();
            for (int i = 1; i < s; i++)
            {
                BigInteger temp = 2;
                temp = BigInteger.ModPow(temp, (s - i - 1), q);
                d = BigInteger.ModPow(BigInteger.ModPow(r, 2, q) * InvA, temp, q);
                if (d == (q - 1))
                    r = (r * c) % q;
                c = BigInteger.ModPow(c, 2, q);
            }
            return r;
        }
        
        public static BigInteger RandInRange(BigInteger lower, BigInteger upper)
        {
            Random random = new Random();
            if (lower <= long.MaxValue && upper <= long.MaxValue && lower < upper)
            {
                BigInteger r;
                while (true)
                {
                    double rd = random.NextDouble();
                    r = lower + (long)(((long)upper - (long)lower) * rd);
                    if (r >= lower && r <= upper)
                        return r;
                }
            }
            BigInteger delta = upper - lower;
            byte[] deltaBytes = delta.ToByteArray();
            byte[] buffer = new byte[deltaBytes.Length];
            while (true)
            {
                random.NextBytes(buffer);

                BigInteger r = new BigInteger(buffer) + lower;

                if (r >= lower && r <= upper)
                    return r;
            }
        }

        public static int Length(BigInteger a)
        {
            byte[] arr = a.ToByteArray();
            int count = (arr.Length - 1) * 8;
            while (arr[arr.Length - 1] != 0)
            {
                count++;
                arr[arr.Length - 1] /= 2;
            }
            return count;
        }
        
        public static BigInteger CheckingForNearPrimality(BigInteger n, BigInteger r_min, BigInteger l_max)
        {
            BigInteger u = n;
            if (isProbablePrime(u))
                return u;
            BigInteger h = 1;
            for (BigInteger l = 2; l <= l_max; l++)
            {
                if (!isProbablePrime(l))
                    continue;
                while (u % l == 0)
                {
                    u = u / l;
                    h *= l;
                    if (u < r_min)
                        return -1;
                }
            }
            if (isProbablePrime(u))
                return u;
            else
                return -1;
        }

        public static int JacobiRand(BigInteger a, BigInteger n)
        {
            if (a == 0)
                return 0;
            if (a == 1)
                return 1;
            int e = 0;
            BigInteger a1 = a;
            BigInteger quotient = a1 / 2;
            BigInteger remainder = a1 % 2;
            while (remainder == 0)
            {
                e++;
                a1 = quotient;
                quotient = a1 / 2;
                remainder = a1 % 2;
            }
            int s = 0;
            if (e % 2 == 0)
                s = 1;
            else
            {
                BigInteger mod8 = n % 8;
                if (mod8 == 1 || mod8 == 7)
                    s = +1;
                if (mod8 == 3 || mod8 == 5)
                    s = -1;
            }
            BigInteger mod4 = n % 4;
            BigInteger a14 = a1 % 4;
            if (mod4 == 3 && a14 == 3)
                s = -s;
            BigInteger n1 = n % a1;
            return s * JacobiRand(n1, a1);
        }
        
        public static long[] GetPrimes(int max)
        {
            int SieveSize = max;
            bool[] sieve = new bool[SieveSize];
            List<long> primes = new List<long>();
            // "сито Эратосфена"
            int limit = SieveSize - 1;
            int i, k, n, nn, sqrtLimit = (int)Math.Sqrt(limit);
            // инициализируем простыми значениями
            for (n = 2; n <= limit; n++)
                sieve[n] = true;
            // удаляем делимые на n
            for (n = 2; n <= sqrtLimit; n++)
                for (i = 2; i <= n - 1; i++)
                    sieve[i * n] = false;
            // удаляем квадраты
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
            return primes.ToArray();
        }

        public static int Jacobi(BigInteger a, BigInteger b)
        {
            if ((b & 0x1) == 0)
                throw (new ArgumentException("Число должно быть нечетным."));

            if (a >= b)
                a %= b;
            if (a == 0)
                return 0;
            if (a == 1)
                return 1;
            uint[] a_data = SupportEDS.BigIntegerToUintArray(a);
            uint[] b_data = SupportEDS.BigIntegerToUintArray(b);
            if (a < 0)
            {
                int kr = 0;
                if (b_data.Length == 2)
                    kr = 1;
                if (((b_data[0] - kr) & 0x2) == 0) 
                    return Jacobi(-a, b);
                else
                    return -Jacobi(-a, b);
            }
            int e = 0;
            for (int index = 0; index < a_data.Length - 1; index++)
            {
                uint mask = 0x01;
                for (int i = 0; i < 32; i++)
                {
                    if ((a_data[index] & mask) != 0)
                    {
                        index = a_data.Length;
                        break;
                    }
                    mask <<= 1;
                    e++;
                }
            }
            BigInteger a1 = a >> e;
            uint[] a1_data = SupportEDS.BigIntegerToUintArray(a1);
            int s = 1;
            if ((e & 0x1) != 0 && ((b_data[0] & 0x7) == 3 || (b_data[0] & 0x7) == 5))
                s = -1;
            if ((b_data[0] & 0x3) == 3 && (a1_data[0] & 0x3) == 3)
                s = -s;
            if (a1 == 1)
                return s;
            else
                return (s * Jacobi(b % a1, a1));
        }

        public static BigInteger BarrettReduction(BigInteger x, BigInteger n, BigInteger constant)
        {
            uint[] n_data = SupportEDS.BigIntegerToUintArray(n);
            int k = Length(n_data.Length - 1),
                kPlusOne = k + 1,
                kMinusOne = k - 1;
            BigInteger q1 = new BigInteger();
            // q1 = x / b^(k-1)
            uint[] x_data = SupportEDS.BigIntegerToUintArray(x);
            int new_lng = x_data.Length - kMinusOne;
            uint[] q1_data;
            if (new_lng > 1)
                q1_data = new uint[new_lng];
            else
                q1_data = new uint[2];
            q1_data[q1_data.Length - 1] = 0;
            for (int i = kMinusOne, j = 0; i < x_data.Length - 1; i++, j++)
                q1_data[j] = x_data[i];
            q1 = SupportEDS.BigIntegerFromUintArray(q1_data);
            BigInteger q2 = q1 * constant;
            // q3 = q2 / b^(k+1)
            uint[] q2_data = SupportEDS.BigIntegerToUintArray(q2);
            int new_lng2 = q2_data.Length - kPlusOne;
            uint[] q3_data;
            if (new_lng2 > 1)
                q3_data = new uint[new_lng2];
            else
                q3_data = new uint[2];
            q3_data[q3_data.Length - 1] = 0;
            for (int i = kPlusOne, j = 0; i < q2_data.Length - 1; i++, j++)
                q3_data[j] = q2_data[i];
            // r1 = x mod b^(k+1)
            // i.e. keep the lowest (k+1) words
            BigInteger r1 = new BigInteger();
            int lengthToCopy = ((x_data.Length - 1) > kPlusOne) ? kPlusOne : (x_data.Length - 1);
            uint[] r1_data = new uint[lengthToCopy + 1];
            for (int i = 0; i < lengthToCopy; i++)
                r1_data[i] = x_data[i];
            r1_data[r1_data.Length - 1] = 0;
            // r2 = (q3 * n) mod b^(k+1)
            // partial multiplication of q3 and n
            BigInteger r2 = new BigInteger();
            uint[] r2_data = new uint[kPlusOne + 1];
            for (int i = 0; i < q3_data.Length - 1; i++)
            {
                if (q3_data[i] == 0) continue;
                ulong mcarry = 0;
                int t = i;
                for (int j = 0; j < n_data.Length - 1 && t < kPlusOne; j++, t++)
                {
                    // t = i + j
                    ulong val = ((ulong)q3_data[i] * (ulong)n_data[j]) +
                                 (ulong)r2_data[t] + mcarry;
                    r2_data[t] = (uint)(val & 0xFFFFFFFF);
                    mcarry = (val >> 32);
                }
                if (t < kPlusOne)
                    r2_data[t] = (uint)mcarry;
            }
            r2_data[r2_data.Length - 1] = 0;
            r1 = SupportEDS.BigIntegerFromUintArray(r1_data);
            r2 = SupportEDS.BigIntegerFromUintArray(r2_data);
            r1 -= r2;
            if (r1 < 0)
            {
                r1 = -r1;
            }
            while (r1 >= n)
                r1 -= n;
            return r1;
        }

        public static bool isProbablePrime(BigInteger bi)
        {
            BigInteger thisVal;
            if (bi < 0)
                thisVal = -bi;
            else
                thisVal = bi;
            if (bi < UInt32.MaxValue)
            {
                // проверка малых значений
                if (thisVal == 0 || thisVal == 1)
                    return false;
                else if (thisVal == 2 || thisVal == 3)
                    return true;
            }
            // четные числа
            if ((thisVal & 0x1) == 0)
                return false;
            // тест на делимость на простые < 2000
            long[] primes = GetPrimes(2000);
            for (int p = 0; p < primes.Length; p++)
            {
                BigInteger divisor = primes[p];
                if (divisor >= thisVal)
                    break;
                BigInteger resultNum = thisVal % divisor;
                if ((int)resultNum == 0)
                    return false;
            }
            // Выполнения теста Рабина-Миллера р.2
            BigInteger p_sub1 = thisVal - 1;
            int s = 0;
            uint[] data = SupportEDS.BigIntegerToUintArray(p_sub1);
            for (int i = 0; i < data.Length - 1; i++)
            {
                uint mask = 0x01;
                for (int j = 0; j < 32; j++)
                {
                    if ((data[i] & mask) != 0)
                    {
                        i = data.Length;
                        break;
                    }
                    mask <<= 1;
                    s++;
                }
            }
            BigInteger t = p_sub1 >> s;
            int bits = Length(thisVal);
            // b = 2^t mod p
            BigInteger b = BigInteger.ModPow(2, t, thisVal);
            bool result = false;
            if (b == 1)         // a^t mod p = 1
                result = true;
            for (int j = 0; result == false && j < s; j++)
            {
                if (b == p_sub1)         // a^((2^j)*t) mod p = p-1 для любого 0 <= j <= s-1
                {
                    result = true;
                    break;
                }

                b = (b * b) % thisVal;
            }
            // если число является сложным псевдопростым по основанию 2, переходим к тяжелому тесту Лукаса
            if (result)
                result = LucasStrongTestHelper(thisVal);

            return result;
        }

        private static bool LucasStrongTestHelper(BigInteger thisVal)
        {
            long D = 5, sign = -1, dCount = 0;
            bool done = false;
            while (!done)
            {
                int Jresult = Jacobi(D, thisVal);
                if (Jresult == -1)
                    done = true;    // J(D, this) = 1
                else
                {
                    if (Jresult == 0 && Math.Abs(D) < thisVal) // найден делитель
                        return false;
                    if (dCount == 20)
                    {
                        BigInteger root = Sqrt(thisVal);
                        if (root * root == thisVal)
                            return false;
                    }
                    D = (Math.Abs(D) + 2) * sign;
                    sign = -sign;
                }
                dCount++;
            }
            long Q = (1 - D) >> 2;
            BigInteger p_add1 = thisVal + 1;
            uint[] data = SupportEDS.BigIntegerToUintArray(p_add1);
            int s = 0;
            for (int i = 0; i < data.Length - 1; i++)
            {
                uint mask = 0x01;
                for (int j = 0; j < 32; j++)
                {
                    if ((data[i] & mask) != 0)
                    {
                        i = data.Length;
                        break;
                    }
                    mask <<= 1;
                    s++;
                }
            }
            BigInteger t = p_add1 >> s;
            // вычисление константы для Редукции Баррета = b^(2k) / m
            BigInteger constant = new BigInteger();
            uint[] thisVal_data = SupportEDS.BigIntegerToUintArray(thisVal);
            int nLen = (thisVal_data.Length - 1) << 1;
            uint[] const_data = new uint[nLen + 2];
            for (int i = 0; i < const_data.Length; i++)
                if (i == nLen)
                    const_data[i] = 0x00000001;
                else
                    const_data[i] = 0x0;
            constant = SupportEDS.BigIntegerFromUintArray(const_data);
            constant = constant / thisVal;
            BigInteger[] lucas = LucasSequenceHelper(1, Q, t, thisVal, constant, 0);
            bool isPrime = false;
            if ((lucas[0] == 0) || lucas[1] == 0)
            {
                // u(t) = 0 либо V(t) = 0
                isPrime = true;
            }
            for (int i = 1; i < s; i++)
            {
                if (!isPrime)
                {
                    lucas[1] = BarrettReduction(lucas[1] * lucas[1], thisVal, constant);
                    lucas[1] = (lucas[1] - (lucas[2] << 1)) % thisVal;
                    //lucas[1] = ((lucas[1] * lucas[1]) - (lucas[2] << 1)) % thisVal;
                    if ((lucas[1] == 0))
                        isPrime = true;
                }
                lucas[2] = BarrettReduction(lucas[2] * lucas[2], thisVal, constant);     //Q^k
            }
            if (isPrime)     // дополнительная проверка на составные числа
            {
                BigInteger g = BigInteger.GreatestCommonDivisor(thisVal, Q);
                if (g == 1)
                {
                    uint[] lucas_data = SupportEDS.BigIntegerToUintArray(lucas[2]);
                    if (lucas[2] < 0)
                        lucas[2] += thisVal;
                    BigInteger temp = (Q * Jacobi(Q, thisVal)) % thisVal;
                    if (temp < 0)
                        temp += thisVal;
                    if (lucas[2] != temp)
                        isPrime = false;
                }
            }
            return isPrime;
        }

        private static BigInteger[] LucasSequenceHelper(BigInteger P, BigInteger Q, BigInteger k, BigInteger n, BigInteger constant, int s)
        {
            BigInteger[] result = new BigInteger[3];
            if ((k & 0x01) == 0)
                throw (new ArgumentException("Значение аргумента k должно быть четным"));
            int numbits = Length(k);
            uint mask = (uint)0x1 << ((numbits & 0x1F) - 1);
            // v = v0, v1 = v1, u1 = u1, Q_k = Q^0
            BigInteger v = 2 % n, Q_k = 1 % n, v1 = P % n, u1 = Q_k;
            bool flag = true;
            uint[] k_data = SupportEDS.BigIntegerToUintArray(k);
            for (int i = k_data.Length - 2; i >= 0; i--)
            {
                while (mask != 0)
                {
                    if (i == 0 && mask == 0x00000001)
                        break;
                    if ((k_data[i] & mask) != 0)
                    {
                        u1 = (u1 * v1) % n;
                        v = ((v * v1) - (P * Q_k)) % n;
                        v1 = BarrettReduction(v1 * v1, n, constant);
                        v1 = (v1 - ((Q_k * Q) << 1)) % n;
                        if (flag)
                            flag = false;
                        else
                            Q_k = BarrettReduction(Q_k * Q_k, n, constant);
                        Q_k = (Q_k * Q) % n;
                    }
                    else
                    {
                        u1 = ((u1 * v) - Q_k) % n;
                        v1 = ((v * v1) - (P * Q_k)) % n;
                        v = BarrettReduction(v * v, n, constant);
                        v = (v - (Q_k << 1)) % n;
                        if (flag)
                        {
                            Q_k = Q % n;
                            flag = false;
                        }
                        else
                            Q_k = BarrettReduction(Q_k * Q_k, n, constant);
                    }
                    mask >>= 1;
                }
                mask = 0x80000000;
            }
            u1 = ((u1 * v) - Q_k) % n;
            v = ((v * v1) - (P * Q_k)) % n;
            if (flag)
                flag = false;
            else
                Q_k = BarrettReduction(Q_k * Q_k, n, constant);
            Q_k = (Q_k * Q) % n;
            for (int i = 0; i < s; i++)
            {
                u1 = (u1 * v) % n;
                v = ((v * v) - (Q_k << 1)) % n;
                if (flag)
                {
                    Q_k = Q % n;
                    flag = false;
                }
                else
                    Q_k = BarrettReduction(Q_k * Q_k, n, constant);
            }
            result[0] = u1;
            result[1] = v;
            result[2] = Q_k;
            return result;
        }
        
        public static BigInteger genPseudoPrime(int bits, Random rnd)
        {
            BigInteger result = new BigInteger();
            bool done = false;

            while (!done)
            {
                result = RandBigInteger(bits, rnd);
                result |= 0x01;
                done = isProbablePrime(result);
            }
            return result;
        }

        public static BigInteger ChineseRemainderTheorem(List<BigInteger> m, List<BigInteger> v)
        {
            int t = m.Count;
            BigInteger ci = 0, u = 0, x = 0, ux = 0, prod = 0;
            List<BigInteger> C = new List<BigInteger>();
            C.Add(1);
            for (int i = 1; i < t; i++)
            {
                ci = 1;
                for (int j = 0; j <= i - 1; j++)
                {
                    u = GetInverse(m[j], m[i]);
                    ci = MulMod(u, ci, m[i]);
                }
                C.Add(ci);
            }
            u = v[0];
            x = u;
            for (int i = 1; i < C.Count; i++)
            {
                ux = SubMod(v[i], x, m[i]);
                u = MulMod(ux, C[i], m[i]);
                prod = 1;
                for (int j = 0; j <= i - 1; j++)
                    prod = prod * m[j];
                x += u * prod;
            }
            return x;
        }
        
        public static BigInteger Psi(int n, BigInteger x, BigInteger y, BigInteger a, BigInteger b, BigInteger p)
        {
            BigInteger y2 = MulMod(2, y, p);
            BigInteger y2inv = GetInverse(y2, p);
            List<BigInteger> psi = new List<BigInteger>();
            if (n == -1)
                return -1;
            else if (n == 0)
                return 0;
            else if (n == 1)
                return 1;
            else if (n == 2)
                return y2;
            else
            {
                BigInteger x2 = MulMod(x, x, p);
                BigInteger x3 = MulMod(x, x2, p);
                BigInteger x4 = MulMod(x2, x2, p);
                BigInteger x6 = MulMod(x3, x3, p);
                BigInteger a2 = MulMod(a, a, p);
                BigInteger a3 = MulMod(a, a2, p);
                BigInteger b2 = MulMod(b, b, p);
                BigInteger b3 = MulMod(b, b2, p);
                BigInteger bx = MulMod(b, x, p);
                BigInteger t1 = MulMod(3, x4, p);
                BigInteger t2 = MulMod(6, MulMod(a, x2, p), p);
                BigInteger t3 = MulMod(12, MulMod(b, x, p), p);
                BigInteger psi3 = AddMod(t1, t2, p);
                psi3 = AddMod(psi3, t3, p);
                psi3 = SubMod(psi3, a2, p);
                if (n == 3)
                    return psi3;
                if (n > 3)
                {
                    BigInteger t4 = 0, t5 = 0, t6 = 0;
                    t1 = x6;
                    t2 = MulMod(5, MulMod(a, x4, p), p);
                    t3 = MulMod(20, MulMod(b, x3, p), p);
                    t4 = MulMod(5, MulMod(a2, x2, p), p);
                    t5 = MulMod(4, MulMod(MulMod(a, b, p), x, p), p);
                    t6 = MulMod(8, b2, p);
                    BigInteger psi4 = AddMod(t1, t2, p);
                    psi4 = AddMod(psi4, t3, p);
                    psi4 = SubMod(psi4, t4, p);
                    psi4 = SubMod(psi4, t5, p);
                    psi4 = SubMod(psi4, t6, p);
                    psi4 = SubMod(psi4, a3, p);
                    psi4 = MulMod(4, MulMod(y, psi4, p), p);
                    if (n == 4)
                        return psi4;
                    if (n > 4)
                    {
                        psi.Add(0);
                        psi.Add(1);
                        psi.Add(y2);
                        psi.Add(psi3);
                        psi.Add(psi4);
                        for (int i = 5; i <= n; i++)
                        {
                            if (i % 2 == 1)
                            {
                                int j = (i - 1) / 2;
                                BigInteger psinp0 = psi[j + 0];
                                BigInteger psinp2 = psi[j + 2];
                                BigInteger psinp1 = psi[j + 1];
                                BigInteger psinm1 = psi[j - 1];
                                BigInteger psinp03 = BigInteger.ModPow(psinp0, 3, p);
                                BigInteger psinp13 = BigInteger.ModPow(psinp1, 3, p);
                                BigInteger term1 = MulMod(psinp2, psinp03, p);
                                BigInteger term2 = MulMod(psinp13, psinm1, p);
                                psi.Add(SubMod(term1, term2, p));
                            }
                            else
                            {
                                int j = i / 2;
                                BigInteger psinp0 = psi[j + 0];
                                BigInteger psinp2 = psi[j + 2];
                                BigInteger psinp1 = psi[j + 1];
                                BigInteger psinm1 = psi[j - 1];
                                BigInteger psinm2 = psi[j - 2];
                                BigInteger psinm12 = BigInteger.ModPow(psinm1, 2, p);
                                BigInteger psinp12 = BigInteger.ModPow(psinp1, 2, p);
                                BigInteger factor1 = MulMod(psinp2, psinm12, p);
                                BigInteger factor2 = MulMod(psinm2, psinp12, p);
                                BigInteger factor3 = SubMod(factor1, factor2, p);
                                BigInteger factor4 = MulMod(factor3, y2inv, p);
                                psi.Add(MulMod(psinp0, factor4, p));
                            }
                        }
                    }
                }
            }
            return psi[n];
        }


    }
}
