using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDS
{
    public class Data
    {

        public static void doubleCenter(ref double[,] matrix)
        {
            int n = matrix.GetLength(1);
            int k = matrix.GetLength(0);

            for (int j = 0; j < k; j++)
            {
                double avg = 0.0D;
                for (int i = 0; i < n; i++) avg += matrix[j, i];
                avg /= n;
                for (int i = 0; i < n; i++) matrix[j, i] -= avg;
            }
            for (int i = 0; i < n; i++)
            {
                double avg = 0.0D;
                for (int j = 0; j < k; j++) avg += matrix[j, i];
                avg /= matrix.Length;
                for (int j = 0; j < k; j++) matrix[j, i] -= avg;
            }
        }

        public static void multiply(ref double[,] matrix, double factor)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    matrix[i, j] *= factor;
                }
            }
        }

        public static void squareEntries(ref double[,] matrix)
        {
            int n = matrix.GetLength(1);
            int k = matrix.GetLength(0);
            for (int i = 0; i < k; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    matrix[i, j] = Math.Pow(matrix[i, j], 2.0D);
                }
            }
        }


        public static double normalize1(ref double[] x)
        {
            double norm = Math.Sqrt(prod(x, x));
            for (int i = 0; i < x.Length; i++) x[i] /= norm;
            return norm;
        }

        public static double prod(double[] x, double[] y)
        {
            double result = 0.0D;
            int Length = Math.Min(x.Length, y.Length);
            for (int i = 0; i < Length; i++) result += x[i] * y[i];
            return result;
        }

        public static void eigen(ref double[,] matrix,ref double[,] evecs,ref double[] evals)
        {
            int d = evals.Length;
            int k = matrix.GetLength(0);
            //double d1 = 1.E-05D;
            double r = 0.0D;
            for (int m = 0; m < d; m++)
            {
                //double eps;
                double[] temp = new double[evecs.GetLength(1)];
                for (int i = 0; i < evecs.GetLength(1); i++)
                {
                    temp[i] = evecs[m, i];          
                }
                double norm = Math.Sqrt(prod(temp, temp));
             
                for (int i = 0; i < evecs.GetLength(1); i++)
                {
                    evecs[m, i] /= norm;
                }
                evals[m] = norm;
            }
            int iterations = 0;
            while (r < 0.9999900000000001D)
            {
                double[,] tempOld = new double[d, k];

                for (int m = 0; m < d; m++)
                {
                    for (int i = 0; i < k; i++)
                    {
                        tempOld[m, i] = evecs[m, i];
                        evecs[m, i] = 0.0D;
                    }
                }

                for (int m = 0; m < d; m++)
                {
                    for (int i = 0; i < k; i++)
                        for (int j = 0; j < k; j++)
                            evecs[m, j] += matrix[i, j] * tempOld[m, i];
                }
                for (int m = 0; m < d; m++)
                {
                    double[] temp = new double[evecs.GetLength(1)];
                    for (int i = 0; i < evecs.GetLength(1); i++)
                    {
                        temp[i] = evecs[m, i];
                    }
                    for (int p = 0; p < m; p++)
                    {
                        double[] tempY = new double[evecs.GetLength(1)];
                        for (int i = 0; i < evecs.GetLength(1); i++)
                        {
                            tempY[i] = evecs[p, i];
                        }
                        double fac = Data.prod(tempY, temp) / Data.prod(tempY, tempY);
                        for (int i = 0; i < k; i++) evecs[m, i] -= fac * evecs[p, i];
                    }
                }

                for (int m = 0; m < d; m++)
                {
                    double[] temp = new double[evecs.GetLength(1)];
                    for (int i = 0; i < evecs.GetLength(1); i++)
                    {
                        temp[i] = evecs[m, i];
                    }
                    double norm = Math.Sqrt(prod(temp, temp));

                    for (int i = 0; i < evecs.GetLength(1); i++)
                    {
                        evecs[m, i] /= norm;
                    }
                    evals[m] = norm;
                }
                r = 1.0D;
                for (int m = 0; m < d; m++)
                {
                    double[] temp = new double[evecs.GetLength(1)];
                    double[] temp1 = new double[tempOld.GetLength(1)];
                    for (int i = 0; i < evecs.GetLength(1); i++)
                    {
                        temp[i] = evecs[m, i];
                    }
                    for (int i = 0; i < tempOld.GetLength(1); i++)
                    {
                        temp1[i] = tempOld[m, i];
                    }
                    r = Math.Min(Math.Abs(Data.prod(temp, temp1)), r);
                }
                iterations++;
            }
        }

        public static void randomize(ref double[,] matrix)
        {
            Random random = new Random();
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    matrix[i, j] = random.NextDouble();
                }
            }
        }
        public static int[] landmarkIndices(double[,] matrix)
        {
            int k = matrix.GetLength(0);
            int n = matrix.GetLength(1);
            int[] result = new int[k];
            for (int i = 0; i < k; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (matrix[i, j] == 0.0D)
                    {
                        result[i] = j;
                    }
                }
            }
            return result;
        }
        public static double[,] copyMatrix(double[,] matrix)
        {
            
            double[,] copy = new double[matrix.GetLength(0), matrix.GetLength(1)];
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    copy[i, j] = matrix[i, j];
                }
            }
            return copy;
        }
    }

}
