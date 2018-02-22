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
            //var start = new[] { 6, .3 };

            //var xy = new[,]
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

            var start = new[] { 2.5, .25 };

            var points = new[,]
            {
                {1, 3.2939 },
                {2, 4.2699 },
                {4, 7.1749 },
                {5, 9.3008 },
                {8, 20.259 }
            };


            for (var z = 0; z < 1; z++)
            {

                var J = new double[points.GetLength(0), start.Length];
                var df = new double[start.Length];
                var dF = new double[start.Length, start.Length];
                var p = new double[start.Length];


                for (var j = 0; j < points.GetLength(0); j++)
                {
                    J[j, 0] = Math.Exp(start[1] * points[j, 0]);
                    J[j, 1] = points[j, 0] * start[0] * Math.Exp(start[1] * points[j, 0]);
                }

                for (var i = 0; i < start.Length; i++)
                {
                    for (var j = 0; j < points.GetLength(0); j++)
                    {
                        df[i] += (start[0] * Math.Exp(start[1] * points[j, 0]) - points[j, 1]) * J[j, i];  //r * J
                    }
                }


                for (var i = 0; i < points.GetLength(0); i++)
                {
                    dF[0, 0] += J[i, 0] * J[i, 0];
                    dF[0, 1] += J[i, 1] * J[i, 0];
                    dF[1, 0] += J[i, 0] * J[i, 1];
                    dF[1, 1] += J[i, 1] * J[i, 1];
                }


                var D = dF[0, 0] * dF[1, 1] - dF[1, 0] * dF[0, 1];

                for (var i = 0; i < 2; i++)
                {
                    for (var j = 0; j < 2; j++)
                    {
                        dF[i, j] = dF[i, j] / D;
                    }
                }

                var tmp = dF[1, 1];
                dF[1, 1] = dF[0, 0];
                dF[0, 0] = tmp;
                dF[0, 1] = -dF[0, 1];
                dF[1, 0] = -dF[1, 0];


                p[0] = -(df[0] * dF[0, 0] + df[1] * dF[0, 1]);
                p[1] = -(df[0] * dF[1, 0] + df[1] * dF[1, 1]);

                start[0] += p[0];
                start[1] += p[1];


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

                Console.WriteLine($"\np:\n{p[0]} \n{p[1]}");
            }

            Console.WriteLine($"\nx1: {start[0]}, x2: {start[1]}");

            Console.ReadKey();
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
