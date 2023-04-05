using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Windows.Forms;
using System.Numerics;
using System.Drawing.Drawing2D;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        int size;
        double[,] mainmat;
        double[,] mainmat2;
        double[] rest;
        double[] sec;
        double[] sec2;
        public Form1()
        {
            InitializeComponent();
        }
        void input()
        {
            size = int.Parse(textBox1.Text);
            mainmat = new double[size, size];
            sec = new double[size];
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                {
                    mainmat[i,j] = Convert.ToDouble(dataGridView1.Rows[i].Cells[j].Value);
                }
            for (int i = 0; i < size; i++)
                sec[i] = Convert.ToDouble(dataGridView3.Rows[i].Cells[0].Value);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            int n = 2;
            if (!int.TryParse(textBox1.Text, out n) || n < 2)
            {
                MessageBox.Show("Будь ласка, введіть число не менше 2.\nУ комірці буде встановлено значення 2.\n");
                textBox1.Text = "2";
                n = int.Parse(this.textBox1.Text);
            }
            dataGridView3.RowCount = n;
            dataGridView2.RowCount = 2;
            dataGridView2.ColumnCount = n;
            dataGridView1.RowCount = n;
            dataGridView1.ColumnCount = n;
        }
        double[,] newmatrix(double[,] m,int x)
        {
        int size = m.GetLength(0);
        double[,]result=new double[size-1,size-1];
        int z = 0;

        for (int i = 0; i < size-1; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (j != x)
                {
                    result[i, z] = m[i + 1, j];
                    z++;
                }
            }
            z = 0;
        }
        return result;
        }
        double determinant(double[,] m)
        { 
        var size=m.GetLength(0);
            double result=0;
            if(size==2)return m[0,0]*m[1,1]-m[0,1]*m[1,0];
            else 
            {
                for(int i=0;i<size;i++)
                {
                if(i%2==0)result+=m[0,i]*determinant(newmatrix(m,i));
                else result-=m[0,i]*determinant(newmatrix(m,i));
                }
                return result;
                }
        }
        void cramermethod()
        {
            double maindeterm;
            maindeterm = determinant(mainmat);
            this.dataGridView2.RowCount = 2;
            double[] a = new double[size];
            double[,] t = new double[size, size];
            bool flag = true;
            for (int i = 0; i < size; i++)
            {
                Array.Copy(mainmat, t, mainmat.Length);
                for (int j = 0; j < size; j++)
                    t[j, i] = sec[j];
                a[i] = determinant(t);
                if (a[i] != 0) flag = false;
            }
            if (maindeterm != 0)
            {
                for (int i = 0; i < size; i++)
                {
                    double s = a[i] / maindeterm;
                    dataGridView2.Rows[0].Cells[i].Value = "x" + (i + 1).ToString();
                    dataGridView2.Rows[1].Cells[i].Value = s.ToString();
                }
            }
            else
            {
                if (!flag) MessageBox.Show("Вибачте, але головний визначник дорівнює 0.\nСистема не має розв`язків.\n\n");
                else MessageBox.Show("Вибачте, але усі визначники дорінюють 0.\nСистема не має єдиного розв`язку.\n\n");
            }
        }
        void gausmethod()
        {
            int[] index = new int[size];
            //for(int i=0;i<size;i++)index[i]=i;
            for (int i = 0; i < size; i++)
            {
                int x;
                for (x = i; x < size; x++)
                    if (mainmat[i, x] != 0) break;
                if (x == size)
                {
                    if (sec[i] == 0)
                    { MessageBox.Show("Система не має єдиного розвязку"); return; }
                    else { MessageBox.Show("Система не має розвязку"); return; }
                }
                if (i != x)
                {
                    int y = index[i]; index[i] = index[x]; index[x] = y;
                    for (int j = 0; j < size; j++)
                    {
                        double r = mainmat[j, i]; mainmat[j, i] = mainmat[j, x]; mainmat[j, x] = r;
                    }
                }
                sec[i] /= mainmat[i, i];
                double z = mainmat[i, i];
                for (int j = 0; j < size; j++)
                    mainmat[i, j] /= z;
                for (int l = 0; l < size; l++)
                {
                    if (l != i)
                    {
                        double y = mainmat[l, i];
                        for (int j = 0; j < size; j++)
                            mainmat[l, j] -= y * mainmat[i, j];
                        sec[l] -= y * sec[i];
                    }
                }
            }
            for(int i=0;i<size;i++)
                for(int j=i+1;j<size;j++)
                    if (index[i] > index[j])
                    {
                        int y = index[i]; index[i] = index[j]; index[j] = y;
                        double r = sec[i]; sec[i] = sec[j]; sec[j] = r;
                    }
            for (int i = 0; i < size; i++)
            {
                dataGridView2.Rows[0].Cells[i].Value = "x" + (i+1).ToString();
                dataGridView2.Rows[1].Cells[i].Value = sec[i].ToString();
            }
        }
   
        void itermethod()
        {
            double e;
            if (!double.TryParse(textBox2.Text, out e))
            {
                MessageBox.Show("Ви не вказали точність обчислень.\nБуде встановлено значення 0.1.\n");
                textBox2.Text = "0.1";
                e = double.Parse(textBox2.Text);
            }

            double[] x0 = new double[size]; // Initial guess
            double[] x1 = new double[size]; // Next guess

            for(int i = 0; i < size; i ++)
            {
                x0[i] = sec[i] / mainmat[i,i];
            }

            // Iteration loop
            while (true)
            {
                // Calculate the next guess
                for (int i = 0; i < size; i++)
                {
                    x1[i] = sec[i];
                    for (int j = 0; j < size; j++)
                    {
                        if (i != j)
                        {
                            x1[i] -= mainmat[i, j] * x0[j];
                        }
                    }
                    x1[i] /= mainmat[i, i];
                }

                // Check for convergence
                double maxDiff = 0.0;
                for (int i = 0; i < size; i++)
                {
                    double diff = Math.Abs(x1[i] - x0[i]);
                    if (diff > maxDiff)
                    {
                        maxDiff = diff;
                    }
                }

                Console.WriteLine("{0}, {1}",e, maxDiff);

                if (maxDiff <= e)
                {
                    break;
                }

                for (int i = 0; i < size; i++)
                {
                    Console.WriteLine("x[{0}] = {1}", i, x1[i]);
                }
                Console.WriteLine("---------------");

                // Update the guess
                for (int i = 0; i < size; i++)
                {
                    x0[i] = x1[i];
                }
            }

            for (int i = 0; i < size; i++)
            {
                dataGridView2.Rows[0].Cells[i].Value = "x" + (i + 1).ToString();
                dataGridView2.Rows[1].Cells[i].Value = x1[i].ToString();
            } // Found solution
        }


        void yakobimethod()
        {
            double e;
            if (!double.TryParse(textBox2.Text, out e))
            {
                MessageBox.Show("Ви не вказали точність обчислень.\nБуде встановлено значення 0.1.\n");
                textBox2.Text = "0.1";
                e = double.Parse(textBox2.Text);
            }
            
            double[] x0 = new double[size]; // initial guess
            double[] x1 = new double[size]; // updated guess
            double error = double.MaxValue;

            while (error > e)
            {
                for (int i = 0; i < size; i++)

                {
                    double sum = sec[i];
                    for (int j = 0; j < size; j++)
                    {
                        if (i != j)
                        {
                            sum -= mainmat[i, j] * x0[j];
                        }
                    }
                    x1[i] = sum / mainmat[i, i];
                }

                error = 0.0;
                for (int i = 0; i < size; i++)
                {
                    double diff = Math.Abs(x1[i] - x0[i]);
                    if (diff > error)
                    {
                        error = diff;
                    }
                    x0[i] = x1[i];
                }
                for (int i = 0; i < size; i++)
                {
                    Console.WriteLine("x[{0}] = {1}", i, x1[i]);
                }
                Console.WriteLine("---------------");
            }

            Console.WriteLine("Solution:");
            for (int i = 0; i < size; i++)
            {
                Console.WriteLine($"x{i + 1} = {x1[i]}");
            }

            for (int i = 0; i < size; i++)
            {
                dataGridView2.Rows[0].Cells[i].Value = "x" + (i + 1).ToString();
                dataGridView2.Rows[1].Cells[i].Value = x1[i].ToString();
            }
        }

        double[,] transpose(double[,] T, int size)
        {
            var TT = new double[size, size];
            for (int i = 0; i < size; i++)
            {
                for(int j = 0; j < size; j++)
                {
                    TT[i,j] = T[j,i];
                }
            }
            return TT;
        }


        void squarerootmethod()
        {
           

            var T = new double[size, size];
            var TT = new double[size, size];
            var D = new double[size];
            var X = new double[size];
            var Y = new double[size];


            // Calculate mat S
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (i == j)
                    {
                        double sum = 0;
                        for (int k = 0; k < i; k++)
                        {
                            sum += Math.Abs(T[k, i]) * Math.Abs(T[k, i]) * D[k];
                        }

                        D[i] = Math.Sign(mainmat[i, i] - sum);

                        T[i, j] = Math.Sqrt(Math.Abs(mainmat[i, j] - sum));
                    }
                    if (i < j)
                    {
                        double sum = 0;
                        for (int k = 0; k < i; k++)
                        {
                            sum += T[k, i] * T[k, j] * D[k];
                        }
                        T[i, j] = (mainmat[i, j] - sum) / (T[i, i] * D[i]);
                    }
                    if (i > j)
                    {
                        T[i, j] = 0;
                    }
                    Console.WriteLine("i:{0} j:{1} D[i,i]:{3} T[i,j]:{2}", i, j, T[i, j], D[i]);
                }

            }
            T = transpose(T, size);

            Y[0] = sec[0] * D[0] / T[0,0];
            Console.WriteLine("Y[{0}] is: {1}", 0, Y[0]);
            for (int i = 1; i < size; i++)
            {
                double sum = 0;
                for(int k = 0; k < i; k++)
                {
                    sum += T[i, k] * Y[k] * D[k];
                }
                Y[i] = (sec[i] - sum) / T[i, i] * D[i];
                Console.WriteLine("Y[{0}] is: {1}", i, Y[i]);
            }

            T = transpose(T, size);

            X[size-1 ] = Y[size-1] / T[size-1, size-1];
            Console.WriteLine("X[{0}] is: {1}", size-1, X[size-1]);
            for ( int i = size - 2; i >= 0; i--)
            {
                double sum = 0;
                for(int k = i; k < size; k++)
                {
                    sum += T[i, k] * X[k];
                }
                X[i] = (Y[i] - sum) / T[i, i];
                Console.WriteLine("X[{0}] is: {1}", i, X[i]);
            }

            for (int i = 0; i < size; i++)
            {
                dataGridView2.Rows[0].Cells[i].Value = "x" + (i + 1).ToString();
                dataGridView2.Rows[1].Cells[i].Value = X[i].ToString();
            }

            return;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            input();
            if (radioButton1.Checked) cramermethod();
            if (radioButton3.Checked) gausmethod();
            if (radioButton2.Checked) itermethod();
            if (radioButton4.Checked) yakobimethod();
            if (radioButton5.Checked) squarerootmethod();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            dataGridView3.Rows.Clear();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged_1(object sender, EventArgs e)
        {

        }
    }
}
