using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;

namespace QuicksortApp
{
    namespace SequentialApp
    {
        public class CQuickSort
        {
            public static void DoSort(int[] list)
            {

                QuickSort(list, 0, list.Length - 1, 0);
            }

            public static void QuickSort(int[] list, int left, int right, int depth)
            {
                if (left < right)
                {
                    int q = Partition(list, left, right);
                    QuickSort(list, left, q - 1, depth + 1);
                    QuickSort(list, q + 1, right, depth + 1);
                }
            }

            private static int Partition(int[] list, int left, int right)
            {
                int pivot = list[right];
                int temp;
                int i = left;

                for (int j = left; j < right; ++j)
                {
                    if (list[j] <= pivot)
                    {
                        temp = list[j];
                        list[j] = list[i];
                        list[i] = temp;
                        i++;
                    }
                }

                list[right] = list[i];
                list[i] = pivot;

                return i;
            }
        }
    }

    namespace ParallelApp
    {
        public class CQuickSort
        {
            private const int maxDepthValue = 3;

            public static void DoSort(int[] list)
            {

                QuickSort(list, 0, list.Length - 1, 0);
            }

            public static void QuickSort(int[] list, int left, int right, int depth)
            {
                if (left < right)
                {
                    int q = Partition(list, left, right);

                    if (depth < maxDepthValue)
                    {
                        Parallel.Invoke(
                            () => QuickSort(list, left, q - 1, depth + 1),
                            () => QuickSort(list, q + 1, right, depth + 1)
                            );
                    }
                    else
                    {
                        QuickSort(list, left, q - 1, depth + 1);
                        QuickSort(list, q + 1, right, depth + 1);
                    }
                }
            }

            private static int Partition(int[] list, int left, int right)
            {
                int pivot = list[right];
                int temp;
                int i = left;

                for (int j = left; j < right; ++j)
                {
                    if (list[j] <= pivot)
                    {
                        temp = list[j];
                        list[j] = list[i];
                        list[i] = temp;
                        i++;
                    }
                }

                list[right] = list[i];
                list[i] = pivot;

                return i;
            }
        }
    }

    class Program
    {
        public static void GenerateArray(out int[] arr, int len)
        {
            int Min = -1000;
            int Max = 1000;

            arr = new int[len];

            Random randNum = new Random();
            for (int i = 0; i < len; i++)
            {
                arr[i] = randNum.Next(Min, Max);
            }
        }

        static void Main(string[] args)
        {
            int experimentsNumber = 150;
            List<int> dimensions = new List<int>() { 1000, 2000, 5000, 10000, 20000, 50000, 75000, 100000 };

            Console.WriteLine("Start experiments");

            using (StreamWriter writetext = new StreamWriter("Res.txt"))
            {
                writetext.WriteLine("Experiments Number: " + experimentsNumber);

                foreach (int dim in dimensions)
                {
                    long averageSeqTime = 0;
                    long averageParTime = 0;

                    Console.WriteLine("Dimension " + dim);
                    writetext.WriteLine("Dimension: " + dim);
                    writetext.WriteLine("========Sequential========");

                    for (int experiment = 0; experiment < experimentsNumber; ++experiment)
                    {
                        int[] array;
                        GenerateArray(out array, dim);
                        int[] array1 = new int[array.Length];
                        Array.Copy(array, array1, array.Length);
                        //List<int> array2 = array.ToList();

                        Stopwatch stopWatch = new Stopwatch();
                        stopWatch.Start();
                        SequentialApp.CQuickSort.DoSort(array1);
                        stopWatch.Stop();
                        long durationS = stopWatch.ElapsedMilliseconds;

                        writetext.WriteLine(durationS);
                        averageSeqTime += durationS;
                    }

                    writetext.WriteLine("========Parallel========");
                    for (int experiment = 0; experiment < experimentsNumber; ++experiment)
                    {
                        int[] array;
                        GenerateArray(out array, dim);
                        //int[] array1 = new int[array.Length];
                        //Array.Copy(array, array1, array.Length);
                        int[] array2 = new int[array.Length];
                        Array.Copy(array, array2, array.Length);
                        //List<int> array2 = array.ToList();

                        Stopwatch stopWatch2 = new Stopwatch();
                        stopWatch2.Start();
                        ParallelApp.CQuickSort.DoSort(array2);
                        stopWatch2.Stop();
                        long durationP = stopWatch2.ElapsedMilliseconds;

                        writetext.WriteLine(durationP);
                        averageParTime += durationP;
                    }

                    Console.WriteLine("Average time of Sequential Quicksort (ms): " + (averageSeqTime / experimentsNumber));
                    Console.WriteLine("Average time of Parallel Quicksort (ms): " + (averageParTime / experimentsNumber));

                }
            }

            Console.WriteLine("All results have been written to Res.txt");

            Console.ReadKey();
        }
    }
}
