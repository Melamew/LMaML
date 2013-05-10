using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using iLynx.Common;
using iLynx.Common.Serialization;
// ReSharper disable LocalizableElement

namespace iLynx.TestBench
{
    class Program
    {
        private static Random Rand = new Random();
        public static void Main(string[] args)
        {
            WriteOptions();
            ConsoleKey key;
            RuntimeCommon.DefaultLogger = new ConsoleLogger("TestBed");
            while ((key = Console.ReadKey().Key) != ConsoleKey.Escape)
            {
                Console.Clear();
                switch (key)
                {
                    case ConsoleKey.D1:
                        RunSerializerTests();
                        break;
                    case ConsoleKey.Escape:
                        return;
                }
                WriteOptions();
            }
        }

        private static void WriteOptions()
        {
            Console.WriteLine("Press 1 to run some serializer tests");
            Console.WriteLine("Hit Escape to, well, escape from this place");
        }

        private static void RunSerializerTests()
        {
            var serializerService = new Serializer(new ConsoleLogger());
            const long count = (long) (int.MaxValue * 0.5);
            var lastUpdate = DateTime.Now - TimeSpan.FromSeconds(10);
            var serializeSw = new Stopwatch();
            var deserializeSw = new Stopwatch();
            long totalBytes = 0;
            var i = 0;
            var errors = 0;
            //var averageSerializationAverageSpeed = 0d;
            //var averageDeserializationAverageSpeed = 0d;
            var totalAverages = new double[2];
            var started = DateTime.Now;
            RuntimeCommon.DefaultLogger.Log(LoggingType.Information, null, string.Format("Started Test at {0}", started));
            foreach (var random in CreateSomething(count))
            {
                ++i;
                RandomClass1 other;
                using (var memoryStream = new MemoryStream())
                {
                    serializeSw.Start();
                    serializerService.Serialize(random, memoryStream);
                    serializeSw.Stop();
                    totalBytes += memoryStream.Length;
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    deserializeSw.Start();
                    other = serializerService.Deserialize<RandomClass1>(memoryStream);
                    deserializeSw.Stop();
                }
                if (!other.Compare(random, false))
                    errors++;
                if (DateTime.Now - lastUpdate <= TimeSpan.FromSeconds(1)) continue;
                Console.Clear();
                var avgSer = (totalBytes / 1024d / 1024) / serializeSw.Elapsed.TotalSeconds;
                var avgDes = (totalBytes / 1024d / 1024) / deserializeSw.Elapsed.TotalSeconds;
                WriteCenter(string.Format("{0}/{1}", i, count), 3);
                WriteCenter(string.Format("Average Serialize Speed  : {0:F2} MiB/s", avgSer), 2);
                WriteCenter(string.Format("Average Deserialize Speed: {0:F2} MiB/s", avgDes), 1);
                WriteCenter(string.Format("Remaining: {0}", count - i), 0);
                WriteCenter(string.Format("Errors: {0}", errors), -1);
                var itemsPerSecond = i / (DateTime.Now - started).TotalSeconds;
                WriteCenter(string.Format("~{0:F2} Items/Second", itemsPerSecond), -2);
                try { WriteCenter(string.Format("~{0} Time Remaining", TimeSpan.FromHours((count - i) / itemsPerSecond / 3600)), -3); }
                catch { WriteCenter(string.Format("TimeSpan Can't Handle The Truth"), -3); }
                totalAverages[0] += avgSer;
                totalAverages[1] += avgDes;
                deserializeSw.Reset();
                serializeSw.Reset();
                totalBytes = 0;
                lastUpdate = DateTime.Now;
            }
            RuntimeCommon.DefaultLogger.Log(LoggingType.Information, null, "Completed Test run at {0}");
            RuntimeCommon.DefaultLogger.Log(LoggingType.Information, null, string.Format("Average Serialize Speed:   {0} MiB/s", totalAverages[0] / count));
            RuntimeCommon.DefaultLogger.Log(LoggingType.Information, null, string.Format("Average Deserialize Speed: {0} MiB/s", totalAverages[1] / count));
            //for (var i = 0; i < 10; ++i)
            //    Console.WriteLine("Pass: {0}", pass);
        }

        private static void WriteCenter(string str, int top)
        {
            Console.SetCursorPosition(Console.WindowWidth / 2 - str.Length / 2, Console.WindowHeight / 2 - top);
            Console.Write(str);
        }

        private static IEnumerable<RandomClass1> CreateSomething(long count)
        {
            while (count-- > 0)
            {
                var largeBuffer = new byte[8];
                Rand.NextBytes(largeBuffer);
                yield return new RandomClass1
                                 {
                                     ThisIsaField = Rand.Next(int.MinValue, int.MaxValue),
                                     AndThisIsAProperty = Rand.Next(int.MinValue, int.MaxValue),
                                     AString = string.Format("{0}Blah{1}".Repeat(Rand.Next(1, 200)), count, Rand.Next(-5000, 5000)),
                                     AndALargeNumber = BitConverter.ToUInt64(largeBuffer, 0),
                                 };
            }
        }

        private class RandomClass1
        {
            public int ThisIsaField;
            public int AndThisIsAProperty { get; set; }

            public string AString { get; set; }
            public ulong AndALargeNumber { get; set; }

            public override string ToString()
            {
                return
                    string.Format(
                        "RandomClass1 Current Values:{0}ThisIsAField: {1}{0}AndThisIsAProperty: {2}{0}AString: {3}{0}AndALargeNumber: {4}",
                        Environment.NewLine, ThisIsaField, AndThisIsAProperty, AString, AndALargeNumber);
            }

            public bool Compare(RandomClass1 other, bool verbose)
            {
                var result = ThisIsaField == other.ThisIsaField;
                result &= AndThisIsAProperty == other.AndThisIsAProperty;
                result &= AString == other.AString;
                result &= AndALargeNumber == other.AndALargeNumber;
                if (verbose)
                {
                    Console.WriteLine("ThisIsAField:");
                    Console.WriteLine("Me: {0}, Them: {1}", ThisIsaField, other.ThisIsaField);
                    Console.WriteLine("AndThisIsAProperty:");
                    Console.WriteLine("Me: {0}, Them: {1}", AndThisIsAProperty, other.AndThisIsAProperty);
                    Console.WriteLine("AString:");
                    Console.WriteLine("Me: {0}, Them: {1}", AString, other.AString);
                    Console.WriteLine("AndALargeNumber:");
                    Console.WriteLine("Me: {0}, Them: {1}", AndALargeNumber, other.AndALargeNumber);
                }
                return result;
            }
        }
    }
}
// ReSharper restore LocalizableElement