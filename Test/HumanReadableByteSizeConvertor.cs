using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Test
{
    public class HumanReadableByteSizeConvertor
    {
        //Problem of ByteConverters
        //The solution works all the way up until it approaches 1 MB. When given 999,999 bytes as input, the result(in SI mode) is "1000.0 kB". While it is true that 999,999 is closer to 1,000 × 1000^1 than it is to 999.9 × 1000^1, the 1,000 “significand” is out of range according to spec.The correct result is "1.0 MB".
        //FWIW, all 22 answers posted, including the ones using Apache Commons and Android libraries, had this bug (or a variation of it) at the time of writing this article.
        //So how do we fix this? First of all, we note that the exponent (exp) should change from ‘k’ to ‘M’ as soon as the number of bytes is closer to 1 × 1,000^2 (1 MB) than it is to 999.9 × 1000^1 (999.9 k). This happens at 999,950. Similarly, we should switch from ‘M’ to ‘G’ when we pass 999,950,000 and so on.


        /// <summary>
        /// Right method from stackoverflow with right convertion
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public string CorrectHumanReadableByteConverter(long bytes)
        {
            var unit = 1024;
            var absBytes = bytes == long.MinValue ? long.MaxValue : Math.Abs(bytes);
            if (absBytes < unit) return bytes + " B";
            var exp = (int)(Math.Log(absBytes) / Math.Log(unit));
            var th = (long)(Math.Pow(unit, exp) * (unit - 0.05));
            if (exp < 6 && absBytes >= th - ((th & 0xfff) == 0xd00 ? 52 : 0)) exp++;
            var pre = "KMGTPE"[exp - 1].ToString();
            if (exp > 4)
            {
                bytes /= unit;
                exp -= 1;
            }
            return String.Format("{0:0.##} {1}B", bytes / Math.Pow(unit, exp), pre);
        }

        /// <summary>
        /// Method wrote by me (that doesnt solve the problem)
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="decimals"></param>
        /// <param name="baseNumber"></param>
        /// <returns></returns>
        public string MyHumanReadableByteConverter(long bytes, int decimals = 2, int baseNumber = 1024)
        {
            string[] prefixes = new string[] { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

            if (bytes == 0)
                return "0 " + prefixes[0];

            double prevResult = 0;
            double mantissa = Math.Pow(10, decimals);


            for (int i = 0; i < prefixes.Length; i++)
            {
                double multiplier = Math.Pow(baseNumber, i);
                double result = bytes / multiplier;
                result = Math.Truncate(result * mantissa) / mantissa;

                if (Math.Abs(result) < 1)
                    return string.Format("{0} {1}", prevResult, prefixes[i - 1]);

                prevResult = result;
            }

            return bytes.ToString();
        }

        /// <summary>
        /// Another example from stackoverflow but as mine it has same issue 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="decimals"></param>
        /// <param name="base"></param>
        /// <returns></returns>
        public string SlowHumanReadableByteConverter(long bytes, int decimals = 2, int @base = 1024)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (bytes == 0)
                return "0 " + suf[0];
            long absBytes = Math.Abs(bytes);
            int place = Convert.ToInt32(Math.Floor(Math.Log(absBytes, @base)));


            double mantissa = Math.Pow(10, decimals);
            double num = Math.Truncate(absBytes / Math.Pow(@base, place) * mantissa) / mantissa;

            //double num = Math.Round(bytes / Math.Pow(1024, place), decimals);
            return (Math.Sign(bytes) * num).ToString() + " " + suf[place];
        }



        private void CompareByteSizeMethods(params Func<long, string>[] methods)
        {
            var random = new Random();
            var stopWatch = new Stopwatch();

            var testNumbers = new int[100000];

            for (int i = 0; i < testNumbers.Length; i++)
            {
                testNumbers[i] = random.Next(int.MinValue, int.MaxValue);
            }

            stopWatch.Start();

            for (var z = methods.Length - 2; z >= 0; z--)
            {
                for (var i = 0; i < testNumbers.Length; i++)
                {
                    var firstVariant = methods[z + 1].Invoke(testNumbers[i]);
                    var secondVariant = methods[z].Invoke(testNumbers[i]);

                    if (firstVariant != secondVariant)
                        Console.WriteLine("{0} and {1} are not equal for {2} bytes", firstVariant, secondVariant, testNumbers[i]);
                }
            }

            stopWatch.Stop();
            Console.WriteLine("Init duration = " + stopWatch.ElapsedTicks);

            for (var z = methods.Length - 1; z >= 0; z--)
            {
                stopWatch.Restart();

                for (int i = 0; i < testNumbers.Length; i++)
                {
                    var result = methods[z].Invoke(testNumbers[i]);
                }

                stopWatch.Stop();
                Console.WriteLine("{0} variant duration = {1}", methods[z].Method.Name, stopWatch.ElapsedTicks);
            }
        }

        private void TestByteSizeMethod()
        {
            while (true)
            {
                Console.WriteLine("Enter the value: ");
                var isSuccess = int.TryParse(Console.ReadLine(), out int result);
                if (isSuccess)
                {
                    Console.WriteLine("CorrectHumanReadableByteConverter Result: " + CorrectHumanReadableByteConverter(result));
                    Console.WriteLine("MyHumanReadableByteConverter Result: " + MyHumanReadableByteConverter(result));
                    Console.WriteLine("SlowHumanReadableByteConverter Result: " + SlowHumanReadableByteConverter(result));
                }
            }
        }

        public void Test()
        {
            CompareByteSizeMethods(
                e => CorrectHumanReadableByteConverter(e),
                e => MyHumanReadableByteConverter(e),
                e => SlowHumanReadableByteConverter(e)
            );
            TestByteSizeMethod();
        }
    }
}
