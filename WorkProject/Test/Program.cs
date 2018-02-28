using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Test
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var points = new[,]
            {
                //{20.9655, 58.4901  },
                //{21.7087, 69.5809  },
                //{22.0723, 67.222  },
                //{22.3847, 77.83952 },
                //{22.641 , 78.66343 },
                //{23.1175, 99.37954 }
                {41, 57},
                {41.9, 66},
                {42.5, 76},
                {45.7, 92},
                {47.2, 88},
                {48, 79}
            };

            //var a = points[0, 1] / Math.Exp(-Math.Pow(points[0, 0] - start[1], 2) / (2 * Math.Pow(start[2], 2));
            //var b = 

            var start = new double[] { 2.5, .3, 2.5 };


            //var start = new[] { 6, .3 };

            //var points = new[,]
            //{
            //    {1, 8.3  }, //1815
            //    {2, 11.0 }, //1825
            //    {3, 14.7 }, //1835
            //    {4, 19.7 }, //1845
            //    {5, 26.7 }, //1855
            //    {6, 35.2 }, //1865
            //    {7, 44.4 }, //1875
            //    {8, 55.9 }  //1885
            //};

            //var start = new[] { 2.5, .25 };

            //var points = new[,]
            //{
            //    {1, 3.2939 },
            //    {2, 4.2699 },
            //    {4, 7.1749 },
            //    {5, 9.3008 },
            //    {8, 20.259 }
            //};


            for (var z = 0; z < 10; z++)
            {

                var J = new double[points.GetLength(0), start.Length];
                var df = new double[start.Length];
                var dF = new double[start.Length, start.Length];


                for (var j = 0; j < points.GetLength(0); j++)
                {
                    //    J[j, 0] = Math.Exp(start[1] * points[j, 0]);
                    //    J[j, 1] = points[j, 0] * start[0] * Math.Exp(start[1] * points[j, 0]);
                    J[j, 0] = Math.Exp(-Math.Pow(points[j, 0] - start[1], 2) / (2 * Math.Pow(start[2], 2)));
                    J[j, 1] = start[0] * (points[j, 0] - start[1]) * Math.Exp(-Math.Pow(points[j, 0] - start[1], 2) / (2 * Math.Pow(start[2], 2))) / Math.Pow(start[2], 2);
                    J[j, 2] = start[0] * Math.Pow(points[j, 0] - start[1], 2) * Math.Exp(-Math.Pow(points[j, 0] - start[1], 2) / (2 * Math.Pow(start[2], 2))) / Math.Pow(start[2], 3);
                }


                //r * J
                for (var i = 0; i < start.Length; i++)
                {
                    for (var j = 0; j < points.GetLength(0); j++)
                    {
                        //df[i] += (start[0] * Math.Exp(start[1] * points[j, 0]) - points[j, 1]) * J[j, i];
                        df[i] += (start[0] * Math.Exp(-Math.Pow(points[j, 0] - start[1], 2) / (2 * Math.Pow(start[2], 2))) - points[j, 1]) * J[j, i];
                    }
                }

                //J*Jt
                for (var i = 0; i < start.Length; i++)
                {
                    for (var j = 0; j < start.Length; j++)
                    {
                        for (var k = 0; k < points.GetLength(0); k++)
                        {
                            dF[i, j] += J[k, i] * J[k, j];
                        }
                    }
                }


                var ndF = Inverse(dF);

                var ndf = new double[1, df.Length];
                for (var i = 0; i < df.Length; i++)
                    ndf[0, i] = df[i];



                var p = MultiplyTwoMatrices(ndf, ndF);

                for (var i = 0; i < p.GetLength(0); i++)
                {
                    for (var j = 0; j < p.GetLength(1); j++)
                    {
                        p[i, j] = p[i, j] * -1;
                    }
                }

                start[0] += p[0, 0];
                start[1] += p[0, 1];
                start[2] += p[0, 2];


                //Console.WriteLine("r:");
                //foreach (var e in r)
                //    Console.WriteLine(e);

                //Console.WriteLine("\nJ:");
                //for (int i = 0; i < xy.GetLength(0); i++)
                //    Console.WriteLine($"{J[i, 0]} {J[i, 1]}");

                //Console.WriteLine($"\ndf:\n{df[0]} \n{df[1]}");

                //Console.WriteLine($"\nDf:\n{dF[0, 0]} {dF[0, 1]} \n{dF[1, 0]} {dF[1, 1]}");

                //Console.WriteLine($"\n{D}");

                //Console.WriteLine($"\nDf -1:\n{dF[0, 0]} {dF[0, 1]} \n{dF[1, 0]} {dF[1, 1]}");

                Console.WriteLine($"\np:\n{p[0, 0]} \n{p[0, 1]} \n{p[0, 2]}");
            }

            Console.WriteLine($"\nx1: {start[0]}, x2: {start[1]}, x3: {start[2]}");

            Console.ReadKey();
        }

        static double[,] MultiplyTwoMatrices(double[,] left, double[,] right)
        {
            //test
            //var test1 = new double[,]
            //{
            //    {1, 2, 3},
            //    {1, 3, 2}
            //};

            //var test2 = new double[,]
            //{
            //    {1, 2},
            //    {1, 3},
            //    {1, 2}
            //};

            //var res = MultiplyTwoMatrices(test1, test2);




            var elements = new double[left.GetLength(0), right.GetLength(1)];

            for (var i = 0; i < left.GetLength(0); i++)
            {
                for (var j = 0; j < right.GetLength(1); j++)
                {
                    elements[i, j] = ComputeOneElementForMatrixProduct(left, right, i, j);
                }
            }

            return elements;
        }

        static double ComputeOneElementForMatrixProduct(double[,] left, double[,] right, int i, int j)
        {
            var element = left[i, 0] * right[0, j];

            for (var index = 1; index < left.GetLength(1); index++)
            {
                element += left[i, index] * right[index, j];
            }

            return element;
        }

        static double[,] Inverse(double[,] mA, uint round = 0)
        {
            //var test = new double[,]
            //{
            //    {1, 2, 3},
            //    {1, 3, 2},
            //    {1, 2, 2}
            //};

            //var t = Inverse(test);
            //for (int i = 0; i < t.GetLength(0); i++)
            //    Console.WriteLine($"{t[i, 0]} {t[i, 1]} {t[i, 2]}");

            if (mA.GetLength(0) != mA.GetLength(1)) throw new ArgumentException("Обратная матрица существует только для квадратных, невырожденных, матриц.");

            var matrix = mA.Clone() as double[,]; //Делаем копию исходной матрицы
            var determinant = Determinant(mA); //Находим детерминант

            if (determinant == 0) return matrix; //Если определитель == 0 - матрица вырожденная

            for (int i = 0; i < mA.GetLength(0); i++)
            {
                for (int t = 0; t < mA.GetLength(0); t++)
                {
                    var tmp = mA.Exclude(i, t);  //получаем матрицу без строки i и столбца t
                    //(1 / determinant) * Determinant(tmp) - формула поределения элемента обратной матрицы
                    matrix[t, i] = round == 0 ? (1 / determinant) * Math.Pow(-1, i + t + 2) * Determinant(tmp) : Math.Round(((1 / determinant) * Determinant(tmp)), (int)round, MidpointRounding.ToEven);
                }
            }
            return matrix;
        }

        static double[,] Exclude(this double[,] value, int row, int column)
        {
            //var tmp = value.Clone() as double[,];
            var result = new double[value.GetLength(0) - 1, value.GetLength(1) - 1];

            for (int i = 0, j = 0; i < value.GetLength(0); i++)
            {
                if (i == row)
                    continue;

                for (int k = 0, u = 0; k < value.GetLength(1); k++)
                {
                    if (k == column)
                        continue;

                    result[j, u] = value[i, k];
                    u++;
                }
                j++;
            }
            return result;
        }

        static double Determinant(double[,] matrix)
        {
            double det = 1;
            //определяем переменную EPS
            const double EPS = 1E-9;

            var n = matrix.GetLength(0);
            //определяем массив размером nxn
            var a = new double[n][];

            for (var i = 0; i < n; i++)
            {
                a[i] = new double[n];
                for (var j = 0; j < n; j++)
                    a[i][j] = matrix[i, j];
            }
            //проходим по строкам
            for (var i = 0; i < n; ++i)
            {
                //присваиваем k номер строки
                var k = i;
                //идем по строке от i+1 до конца
                for (var j = i + 1; j < n; ++j)
                    //проверяем если равенство выполняется то k присваиваем j
                    if (Math.Abs(a[j][i]) > Math.Abs(a[k][i]))
                        k = j;
                //если равенство выполняется то определитель приравниваем 0 и выходим из программы
                if (Math.Abs(a[k][i]) < EPS)
                {
                    det = 0;
                    break;
                }
                //меняем местами a[i] и a[k]
                var tmp = a[i];
                a[i] = a[k];
                a[k] = tmp;
                //если i не равно k то меняем знак определителя
                if (i != k)
                    det = -det;
                //умножаем det на элемент a[i][i]
                det *= a[i][i];
                //идем по строке от i+1 до конца каждый элемент делим на a[i][i]
                for (var j = i + 1; j < n; ++j)
                    a[i][j] /= a[i][i];
                //идем по столбцам
                for (int j = 0; j < n; ++j)
                    //проверяем
                    if ((j != i) && (Math.Abs(a[j][i]) > EPS))
                        //если да, то идем по k от i+1
                        for (k = i + 1; k < n; ++k)
                            a[j][k] -= a[i][k] * a[j][i];
            }
            return det;
        }

        static void GaussNewton()
        {
            //var start = new[] { 6, .3 };

            //var xy = new[,]
            //{
            //    {1815, 8.3  },
            //    {1825, 11.0 },
            //    {1835, 14.7 },
            //    {1845, 19.7 },
            //    {1855, 26.7 },
            //    {1865, 35.2 },
            //    {1875, 44.4 },
            //    {1885, 55.9 }
            //};

            var start = new[] { 2.5, .25 };

            var xy = new[,]
            {
                {1, 3.2939 },
                {2, 4.2699 },
                {3, 7.1749 },
                {4, 9.3008 },
                {8, 20.259 }
            };


            var r = new double[xy.GetLength(0)];
            var J = new double[xy.GetLength(0), xy.GetLength(1)];

            for (var i = 0; i < xy.GetLength(0); i++)
                r[i] = start[0] * Math.Exp(start[1] * (i + 1)) - xy[i, 1];

            for (var i = 0; i < xy.GetLength(0); i++)
            {
                J[i, 0] = Math.Exp(start[1] * (i + 1));
                J[i, 1] = (i + 1) * start[0] * Math.Exp(start[1] * (i + 1));

            }



            Console.WriteLine("r:");

            foreach (var e in r)
                Console.WriteLine(e);

            Console.WriteLine("\np:");

            for (int i = 0; i < xy.GetLength(0); i++)
                Console.WriteLine("{0} {1}", J[i, 0], J[i, 1]);
        }

        static void NelderMead(double[] xs, double[] ys)
        {

            //double[] wd1 = { 20.9655, 21.7087, 22.0723, 22.3847, 22.641, 23.1175 };
            //double[] pc1 = { 58.4901, 69.5809, 67.222, 77.83952, 78.66343, 99.37954 };

            //var init = new[] { pc1.Sum(), 10, 50 };

            //var count = 0;


            //Func<double[], double> function = x =>
            //{
            //    var INTA = new double[6];
            //    var SE = new double[6];


            //    for (int i = 0; i < 6; i++)
            //    {
            //        INTA[i] = x[0] / (Math.Sqrt(2 * Math.PI) * x[1]) * Math.Exp(-.5 * (Math.Pow(x[2] - wd1[i], 2)) / Math.Pow((x[1]), 2));
            //        SE[i] = Math.Pow(INTA[i] - pc1[i], 2);
            //    }
            //    count++;
            //    return SE.Sum();
            //};
            ////10.0 * Math.Pow(x[0] + 1.0, 2.0) + Math.Pow(x[1], 2.0);

            //int start = 500;

            //for (int i = 0; i < 3000; i++)
            //{
            //    count = 0;

            //    // We can do so using the NelderMead class:
            //    var solver = new NelderMead(numberOfVariables: 3)
            //    {
            //        Convergence = new GeneralConvergence(3)
            //        {//
            //            Evaluations = start,//500
            //            RelativeParameterTolerance = 1e-8,

            //            MaximumEvaluations = 500 + i//1350
            //        },

            //        Function = function, // f(x) = 10 * (x+1)^2 + y^2
            //                             // DiameterTolerance = 1e-8,

            //    };

            //    //start = i;
            //    // Now, we can minimize it with:
            //    bool success = solver.Minimize(init);

            //    // And get the solution vector using
            //    double[] solution = solver.Solution; // should be (-1, 1)

            //    // The minimum at this location would be:
            //    //double minimum = solver.Value; // should be 0

            //    if (solution[0] < 39000 && solution[0] > 36000)
            //    {
            //        foreach (var s in solution)
            //            Console.WriteLine(s);

            //        Console.WriteLine(count);
            //        Console.WriteLine();

            //    }
            //}



            //Console.ForegroundColor = ConsoleColor.Green;

            //Console.WriteLine("Expected results: \n37483,00782 \n10,02566533 \n46,70633539");


        }

        static void Write()
        {
            using (var writer = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.txt"), true))
            {
                writer.WriteLine(DateTime.Now);
            }
        }

        //var res = FindAngle(x1, y1, x2, y2, x3, y3) * 180 / Math.PI; //(x2,y2) center point
        static double FindAngle(double x1, double y1, double x2, double y2, double x3, double y3)
        {
            return Math.Atan2(y1 - y2, x1 - x2) - Math.Atan2(y3 - y2, x3 - x2);
        }

        static double FindLength(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
        }
    }
}
