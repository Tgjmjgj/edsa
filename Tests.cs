using ExcelLibrary.SpreadSheet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace EDS
{
    class Tests
    {

        DataTable data;
        BackgroundWorker bw;
        Schoof algo;
        long[] primes;
        int a, b, index, bits;

        public Tests()
        {
            this.a = -3;
            this.b = 7;
        }

        public void SchoofStatistics(int bit)
        {
            if (bit < 5)
                return;
            bits = bit;
            primes = Maths.GetPrimes((int)Math.Pow(2, bit));
            index = 6;
            data = new DataTable("results");
            data.Columns.Add("prime");
            data.Columns.Add("time");
            bw = new BackgroundWorker();
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += new DoWorkEventHandler(Bw_DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Bw_RunWorkerCompleted);
            Iteration();
        }

        private void Iteration()
        {
            algo = new Schoof(1, a, b, primes[index], bw);
            bw.RunWorkerAsync();
        }

        private void Bw_DoWork(object sender, DoWorkEventArgs e)
        {
            DateTime dt = DateTime.Now;
            BigInteger n = algo.RunSchoof();
            TimeSpan ts = DateTime.Now - dt;
            data.Rows.Add(new object[] { primes[index], ts.Ticks});
        }

        private void Bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            index++;
            if (index == 3910)
            {
                Workbook workbook = new Workbook();
                Worksheet worksheet = new Worksheet("NewWorksheet");
                for (int i = 0; i < data.Columns.Count; i++)
                {
                        worksheet.Cells[0, i] = new Cell(data.Columns[i].ColumnName);
                        for (int j = 0; j < data.Rows.Count; j++)
                            worksheet.Cells[j + 1, i] = new Cell(data.Rows[j][i] == DBNull.Value ? "" : data.Rows[j][i]);
                }
                workbook.Worksheets.Add(worksheet);
                workbook.Save("D:\\results.xls");
                MessageBox.Show("Выполнено");
            }
            else
                Iteration();
        }
    }
}
