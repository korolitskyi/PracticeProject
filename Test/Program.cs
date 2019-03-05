using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Test
{
    internal static class Program
    {

        private static void Main(string[] args)
        {
            //NelderMead();
            //GaussNewton();


            //TestMethods();

            while (true)
            {
                int result = 0;
                var isSuccess = int.TryParse(Console.ReadLine(), out result);
                var t = OurGetByteSize(result);
                if(isSuccess)
                    Console.WriteLine(t);
            }

            

            Console.ReadKey();
        }

        static void TestMethods()
        {
            var random = new Random();
            var stopWatch = new Stopwatch();

            var testNumbers = new int[100000];

            for (int i = 0; i < testNumbers.Length; i++)
            {
                testNumbers[i] = random.Next(int.MinValue, int.MaxValue);
            }

            stopWatch.Start();

            for (int i = 0; i < testNumbers.Length; i++)
            {
                var firstVal = GetByteSize(testNumbers[i]);
                var secondVal = OurGetByteSize(testNumbers[i]);

                if(firstVal != secondVal)
                    Console.WriteLine("{0} and {1} are not equal for {2} butes", firstVal, secondVal, testNumbers[i]);
            }

            stopWatch.Stop();
            Console.WriteLine("Init duration = " + stopWatch.Elapsed);


            stopWatch.Restart();

            for (int i = 0; i < testNumbers.Length; i++)
            {
                GetByteSize(testNumbers[i]);
            }

            stopWatch.Stop();
            Console.WriteLine("first variant duration = " + stopWatch.Elapsed);

            stopWatch.Restart();

            for (int i = 0; i < testNumbers.Length; i++)
            {
                GetByteSize(testNumbers[i]);
            }

            stopWatch.Stop();
            Console.WriteLine("second variant duration = " + stopWatch.Elapsed);
        }

        static string GetByteSize(int byteCount, int decimals = 2, int @base = 1024)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, @base)));


            double mantissa = Math.Pow(10, decimals);
            double num = Math.Truncate(bytes / Math.Pow(1024, place) * mantissa) / mantissa;

            //double num = Math.Round(bytes / Math.Pow(1024, place), decimals);
            return (Math.Sign(byteCount) * num).ToString() + " " + suf[place];
        }

        static string OurGetByteSize(long fileSize, int decimals = 2, int @base = 1024)
        {
            string[] prefixes = new string[] { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

            if (fileSize == 0)
                return "0 " + prefixes[0];

            double prevResult = 0;
            double mantissa = Math.Pow(10, decimals);

            
            for (int i = 0; i < prefixes.Length; i++)
            {
                double multiplier = Math.Pow(@base, i);
                double result = fileSize / multiplier;
                result = Math.Truncate(result * mantissa) / mantissa;
                 
                if (Math.Abs(result) < 1)
                    return string.Format("{0} {1}", prevResult, prefixes[i - 1]);

                prevResult = result;
            }

            return fileSize.ToString();
        }




        static double[] RayToRayIntersection(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
        {
            double[] result = null;
            //Make sure the lines aren't parallel
            if ((y2 - y1) / (x2 - x1) != (y4 - y3) / (x4 - x3))
            {
                //check if one ray contains another one
                var d = (x2 - x1) * (y4 - y3) - (y2 - y1) * (x4 - x3);
                if (d != 0)
                {
                    var r = ((y1 - y3) * (x4 - x3) - (x1 - x3) * (y4 - y3)) / d;
                    var s = ((y1 - y3) * (x2 - x1) - (x1 - x3) * (y2 - y1)) / d;
                    if (r >= 0)
                    {
                        if (s >= 0)
                        {
                            result = new[] { x1 + r * (x2 - x1), y1 + r * (y2 - y1) };
                        }
                    }
                }
            }
            return result;
        }



        #region Aproximation

        #region GaussNewton
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
            var points = new[,]
            {
                //{20.9655, 58.4901  },
                //{21.7087, 69.5809  },
                //{22.0723, 67.222  },
                //{22.3847, 77.83952 },
                //{22.641 , 78.66343 },
                //{23.1175, 99.37954 }
                {20.736 , 58.4901},
                {21.4792, 69.5809},
                {21.8428, 67.222 },
                {22.1552, 77.8395},
                {22.4115, 78.6634},
                {22.888 , 99.3795}








            };

            //var a = points[0, 1] / Math.Exp(-Math.Pow(points[0, 0] - start[1], 2) / (2 * Math.Pow(start[2], 2));
            //var b = 

            var start = new double[] { 130, 10, 50 };

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
        }
        #endregion

        #region NelderMead

        static void NelderMead()
        {
            var xArr = new[]
            {
                20.736,
                21.4792,
                21.8428,
                22.1552,
                22.4115,
                22.888
            };

            var yArr = new[]
            {
                58.4901,
                69.5809,
                67.222,
                77.8395,
                78.6634,
                99.3795
            };

            //var xArr = new[]
            //{
            //    20.447033333333355, 20.58946666666668, 20.7040166666667, 20.73911666666669, 20.74963333333336, 20.764100000000013, 20.78283333333336, 20.896600000000017, 20.951166666666683, 21.021033333333367, 21.231250000000035, 21.37080000000003, 21.491583333333363, 21.60235000000004, 21.624016666666694, 21.648000000000014, 21.66145000000002, 21.66238333333335, 21.701683333333364, 21.738666666666678, 21.788983333333363, 21.78998333333335, 21.86003333333336, 21.864400000000035, 21.88915000000002, 21.969516666666703, 21.982566666666685, 22.022350000000035, 22.092783333333358, 22.10813333333336, 22.13315000000004, 22.148666666666685, 22.19646666666668, 22.209450000000015, 22.22238333333337, 22.263300000000026, 22.28383333333336, 22.332983333333363, 22.396016666666707, 22.410283333333364, 22.425400000000035, 22.429650000000038, 22.42996666666669, 22.492833333333362, 22.50273333333335, 22.531383333333377, 22.60768333333336, 22.636666666666695, 22.6410666666667, 22.669733333333358, 22.776500000000016, 23.01585000000003, 23.27570000000004, 23.2874166666667, 23.438050000000022

            //};

            //var yArr = new[]
            //{
            //    56.37502813833333, 53.979675568333334, 59.16982437833334, 69.07064508999999, 47.77055989166667, 63.859874311666665, 48.03523120833333, 68.94516784833333, 59.205059258333335, 66.58070750333333, 66.84528614833334, 71.55579227666665, 63.200036278333336, 69.34531049833335, 80.265006305, 79.19473872500001, 59.79527651666667, 69.445797715, 75.34006061833334, 68.78536942666666, 63.98488665333333, 81.63013178666667, 52.74558865833333, 80.39029655333333, 48.120545926666665, 64.13012582833333, 69.87078046166667, 85.13980555666666, 71.59532469166668, 59.225762929999995, 75.77495391, 99.03917272833333, 99.41423073833333, 82.43547798666667, 68.28980122666665, 59.64054013500001, 103.77423143833333, 74.54069882166668, 73.78552346, 85.85464785166666, 68.48005160999999, 69.56023144, 74.27061566500002, 69.75029025833334, 87.95458138500003, 88.53143736666668, 74.88534976833334, 102.369314035, 91.84477669666667, 116.77433659500001, 99.89053493833335, 100.50623381166666, 103.115894615, 108.44627956166669, 107.43088516166665

            //};

            var init = new double[] { yArr.Sum(), 10, 50 };

            var step = 0;

            Func<double[], double> function = constants =>
            {
                //Console.Write("Called with a={0} b={1} c={2}", constants[0], constants[1], constants[2]);

                double se = 0;

                for (int i = 0, len = xArr.Length; i < len; i++)
                {
                    var yRegress = constants[0] / (Math.Sqrt(2 * Math.PI) * constants[1]) * Math.Exp(-.5 * (Math.Pow(constants[2] - xArr[i], 2)) / Math.Pow((constants[1]), 2));
                    se += Math.Pow(yRegress - yArr[i], 2);
                }

                //Console.WriteLine("  Summ={0}", se);
                //Console.WriteLine(step++);
                return se;
            };

            var simplex = new Simplex
            {
                Arr = init,
                fx = function(init),
                id = 0
            };

            var date = DateTime.Now;

            var result = nelderMead(function, simplex);

            Console.WriteLine((DateTime.Now - date).Milliseconds);

            foreach (var e in result)
            {
                Console.WriteLine(e);
            }


            //Console.ForegroundColor = ConsoleColor.Green;

            //Console.WriteLine("Expected results: \n40838.5582 \n47.1331 \n10.1734");
        }



        public static void UpdateSimplex(Simplex[] simplex, Simplex value, int N)
        {
            for (var i = 0; i < value.Arr.Length; i++)
            {
                simplex[N].Arr[i] = value.Arr[i];
            }
            simplex[N].fx = value.fx;
        }

        public static void weightedSum(double[] ret, double w1, double[] v1, double w2, Simplex v2)
        {
            for (var j = 0; j < ret.Length; ++j)
            {
                ret[j] = w1 * v1[j] + w2 * v2.Arr[j];
            }
        }

        public class Simplex
        {
            public double[] Arr { get; set; }
            public double fx { get; set; }
            public double id { get; set; }
            public Simplex()
            { }

            public Simplex(Simplex s)
            {
                Arr = s.Arr.Clone() as double[];
                fx = s.fx;
                id = s.id;
            }
        }

        public static double[] nelderMead(Func<double[], double> f, Simplex x0)
        {
            const int maxIterations = 500;
            const double nonZeroDelta = 1.05;
            const double minErrorDelta = 1e-8;
            const double minTolerance = 1e-5;
            const int rho = 1;
            const int chi = 2;
            const double psi = -0.5;
            const double sigma = 0.5;

            // initialize simplex.
            var n = x0.Arr.Length;
            var simplex = new Simplex[n + 1];


            simplex[0] = new Simplex
            {
                Arr = x0.Arr,
                fx = f(x0.Arr),
                id = 0
            };

            for (var i = 0; i < n; ++i)
            {
                var point = x0.Arr.Clone() as double[];

                point[i] = point[i] * nonZeroDelta;

                simplex[i + 1] = new Simplex
                {
                    Arr = point,
                    fx = f(point),
                    id = i + 1
                };
            }

            var centroid = x0.Arr.Clone() as double[];
            var reflected = new Simplex(x0);
            var contracted = new Simplex(x0);
            var expanded = new Simplex(x0);

            for (var iteration = 0; iteration < maxIterations; ++iteration)
            {
                simplex = simplex.OrderBy(e => e.fx).ToArray();

                double maxDiff = 0;
                for (var i = 0; i < n; ++i)
                {
                    maxDiff = Math.Max(maxDiff, Math.Abs(simplex[0].Arr[i] - simplex[1].Arr[i]));
                }

                if ((Math.Abs(simplex[0].fx - simplex[n].fx) < minErrorDelta) &&
                    (maxDiff < minTolerance))
                {
                    break;
                }

                // compute the centroid of all but the worst point in the simplex
                for (var i = 0; i < n; ++i)
                {
                    centroid[i] = 0;
                    for (var j = 0; j < n; ++j)
                    {
                        centroid[i] += simplex[j].Arr[i];
                    }
                    centroid[i] /= n;
                }

                // reflect the worst point past the centroid  and compute loss at reflected
                // point
                var worst = new Simplex(simplex[n]);
                weightedSum(reflected.Arr, 1 + rho, centroid, -rho, worst);
                reflected.fx = f(reflected.Arr);

                // if the reflected point is the best seen, then possibly expand
                if (reflected.fx < simplex[0].fx)
                {
                    weightedSum(expanded.Arr, 1 + chi, centroid, -chi, worst);
                    expanded.fx = f(expanded.Arr);
                    if (expanded.fx < reflected.fx)
                    {
                        UpdateSimplex(simplex, expanded, n);
                    }
                    else
                    {
                        UpdateSimplex(simplex, reflected, n);
                    }
                }

                // if the reflected point is worse than the second worst, we need to
                // contract
                else if (reflected.fx >= simplex[n - 1].fx)
                {
                    var shouldReduce = false;

                    if (reflected.fx > worst.fx)
                    {
                        // do an inside contraction
                        weightedSum(contracted.Arr, 1 + psi, centroid, -psi, worst);
                        contracted.fx = f(contracted.Arr);
                        if (contracted.fx < worst.fx)
                        {
                            UpdateSimplex(simplex, contracted, n);
                        }
                        else
                        {
                            shouldReduce = true;
                        }
                    }
                    else
                    {
                        // do an outside contraction
                        weightedSum(contracted.Arr, 1 - psi * rho, centroid, psi * rho, worst);
                        contracted.fx = f(contracted.Arr);
                        if (contracted.fx < reflected.fx)
                        {
                            UpdateSimplex(simplex, contracted, n);
                        }
                        else
                        {
                            shouldReduce = true;
                        }
                    }

                    if (shouldReduce)
                    {
                        // if we don't contract here, we're done
                        if (sigma >= 1) break;

                        // do a reduction
                        for (var i = 1; i < simplex.Length; ++i)
                        {
                            weightedSum(simplex[i].Arr, 1 - sigma, simplex[0].Arr, sigma, simplex[i]);
                            simplex[i].fx = f(simplex[i].Arr);
                        }
                    }
                }
                else
                {
                    UpdateSimplex(simplex, reflected, n);
                }
            }

            simplex = simplex.OrderBy(e => e.fx).ToArray();
            return simplex[0].Arr;
        }

        #endregion

        #region Polyfit

        static void Polyfit()
        {
            var xyTable = new double[,]
            {
                {
                    20.376892592592615,
                    21.216142592592618,
                    22.0306277777778  ,
                    22.46246111111114 ,
                    22.955214814814845,
                    26.177565000000033
                },
                {
                    54.066358381111115,
                    59.87885624222221 ,
                    76.08806619444445 ,
                    74.76840910592594 ,
                    78.2198343725926  ,
                    96.16413338616665
                }
                //{
                //    20.736 ,
                //    21.4792,
                //    21.8428,
                //    22.1552,
                //    22.4115,
                //    22.888

                //},
                //{
                //    58.4901,
                //    69.5809,
                //    67.222 ,
                //    77.8395,
                //    78.6634,
                //    99.3795
                //}
            };

            for (var i = 0; i < xyTable.GetLength(1); i++)
            {
                xyTable[1, i] = Math.Log(xyTable[1, i]);
            }

            const int basis = 3; // power + 1
            var matrix = MakeSystem(xyTable, basis);

            for (int i = 0; i < basis; i++)
            {
                for (int j = 0; j < basis; j++)
                {
                    Console.Write(((matrix[i, j] > 0) ? "+" : "") +
                        Math.Round(matrix[i, j], 3) + "*c" + j + " ");
                }

                Console.Write(" = " + matrix[i, basis] + "\n");
            }

            var result = Gauss(matrix, basis, basis + 1);
            if (result == null)
            {
                Console.Write("Невозможно найти частное решение составленной системы уравнений\n");
                return;
            }

            Console.Write("\nResult:\n");
            for (var i = 0; i < basis; i++)
            {
                Console.Write("C" + i + " = " + result[i] + "\n");
            }

            var sigma = Math.Sqrt(-1 / (2 * result[2]));
            var mu = result[1] * Math.Pow(sigma, 2);
            var A = Math.Exp(result[0] + Math.Pow(mu, 2) / (2 * Math.Pow(sigma, 2)));

            Console.WriteLine($"\nsigma = {sigma}");
            Console.WriteLine($"\nmu = {mu}");
            Console.WriteLine($"\nA = {A}");

        }

        private static double[,] MakeSystem(double[,] xyTable, int basis)
        {
            var matrix = new double[basis, basis + 1];
            for (var i = 0; i < basis; i++)
            {
                for (var j = 0; j < basis; j++)
                {
                    matrix[i, j] = 0;
                }
            }
            for (var i = 0; i < basis; i++)
            {
                for (var j = 0; j < basis; j++)
                {
                    double sumA = 0, sumB = 0;
                    for (var k = 0; k < xyTable.GetLength(1); k++)
                    {
                        sumA += Math.Pow(xyTable[0, k], i) * Math.Pow(xyTable[0, k], j);
                        sumB += xyTable[1, k] * Math.Pow(xyTable[0, k], i);
                    }
                    matrix[i, j] = sumA;
                    matrix[i, basis] = sumB;
                }
            }
            return matrix;
        }

        private static double[] Gauss(double[,] matrix, int rowCount, int colCount)
        {
            int i;
            var mask = new int[colCount - 1];
            for (i = 0; i < colCount - 1; i++) mask[i] = i;
            if (GaussDirectPass(ref matrix, ref mask, colCount, rowCount))
            {
                double[] answer = GaussReversePass(ref matrix, mask, colCount, rowCount);
                return answer;
            }
            else return null;
        }
        private static bool GaussDirectPass(ref double[,] matrix, ref int[] mask,
            int colCount, int rowCount)
        {
            int i, j, k, maxId, tmpInt;
            double maxVal, tempDouble;
            for (i = 0; i < rowCount; i++)
            {
                maxId = i;
                maxVal = matrix[i, i];
                for (j = i + 1; j < colCount - 1; j++)
                    if (Math.Abs(maxVal) < Math.Abs(matrix[i, j]))
                    {
                        maxVal = matrix[i, j];
                        maxId = j;
                    }
                if (maxVal == 0) return false;
                if (i != maxId)
                {
                    for (j = 0; j < rowCount; j++)
                    {
                        tempDouble = matrix[j, i];
                        matrix[j, i] = matrix[j, maxId];
                        matrix[j, maxId] = tempDouble;
                    }
                    tmpInt = mask[i];
                    mask[i] = mask[maxId];
                    mask[maxId] = tmpInt;
                }
                for (j = 0; j < colCount; j++) matrix[i, j] /= maxVal;
                for (j = i + 1; j < rowCount; j++)
                {
                    double tempMn = matrix[j, i];
                    for (k = 0; k < colCount; k++)
                        matrix[j, k] -= matrix[i, k] * tempMn;
                }
            }
            return true;
        }

        private static double[] GaussReversePass(ref double[,] matrix, int[] mask,
            int colCount, int rowCount)
        {
            int i, j, k;
            for (i = rowCount - 1; i >= 0; i--)
                for (j = i - 1; j >= 0; j--)
                {
                    double tempMn = matrix[j, i];
                    for (k = 0; k < colCount; k++)
                        matrix[j, k] -= matrix[i, k] * tempMn;
                }
            double[] answer = new double[rowCount];
            for (i = 0; i < rowCount; i++) answer[mask[i]] = matrix[i, colCount - 1];
            return answer;
        }

        #endregion

        #endregion

        static void Write()
        {
            using (var writer = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.txt"), true))
            {
                writer.WriteLine(DateTime.Now);
            }
        }

        //var res = FindAngle(x1, y1, x2, y2, x3, y3) * 180 / Math.PI; //(x2,y2) vertex point
        static double FindAngle(double x1, double y1, double x2, double y2, double x3, double y3)
        {
            return Math.Atan2(y1 - y2, x1 - x2) - Math.Atan2(y3 - y2, x3 - x2);
        }

        static double FindLengthBtwPoints(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
        }
    }
}
