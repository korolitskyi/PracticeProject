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
            var convertor = new HumanReadableByteSizeConvertor();
            convertor.Test();


            Console.ReadKey();
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
