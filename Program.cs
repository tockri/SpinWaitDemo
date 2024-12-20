using System;
using System.Threading;

namespace SpinWaitDemo
{
    internal class Program
    {
        private enum Mode
        {
            Loop,
            SpinUntil,
            SpinOnce,
            NotDefined
        }


        static void Main(string[] args)
        {
            Mode mode = ReadModeFromConsole();
            int count = ReactCountFromConsole();
            Demo(mode, count);
        }


        static Mode ReadModeFromConsole()
        {
            Console.Write("Select the waiting function.\n1: while loop\n2: SpinWait.SpinUntil\n3: SpinWait.SpinOnce\n(default: 1) : ");
            while (true)
            {
                string mode = Console.ReadLine().Trim();
                switch (mode)
                {
                    case "":
                    case "1":
                        Console.WriteLine("Selected: using while loop");
                        return Mode.Loop;
                    case "2":
                        Console.WriteLine("Selected: using SpinWait.SpinUntil");
                        return Mode.SpinUntil;
                    case "3":
                        Console.WriteLine("Selected: using SpinWait.SpinOnce");
                        return Mode.SpinOnce;
                    default:
                        Console.Write("Invalid input. Please input 1, 2 or 3: ");
                        break;
                }
            }
        }

        /// <summary>
        /// カウントを入力させる
        /// </summary>
        static int ReactCountFromConsole()
        {
            int count = 0;
            Console.Write("Input count: ");
            while (count == 0)
            {
                if (int.TryParse(Console.ReadLine(), out count))
                {
                    if (count >= 100)
                    {
                        Console.WriteLine("Count is too large. Please input less than 100.");
                        count = 0;
                    }
                    break;
                }
                else
                {
                    Console.Write("Invalid input.");
                }
                Console.Write("Input count: ");
            }
            return count;
        }


        /// <summary>
        /// SpinWaitの動作を理解するための挙動本体
        /// </summary>
        static void Demo(Mode mode, int count)
        {
            var begin = DateTime.Now;
            for (int i = 1; i <= count; i++)
            {
                var until = begin.AddSeconds(i);
                var loops = mode == Mode.Loop ? WaitUntilWithEmptyLoop(until)
                    : mode == Mode.SpinOnce ? WaitUntilUsingSpinOnce(until)
                    : mode == Mode.SpinUntil ? WaitUntilUsingSpinUntil(until)
                    : throw new Exception($"Invalid mode: {mode}");
                Console.WriteLine($"{i} : {loops} loops");
            }
        }

        /// <summary>
        /// SpinWait.SpinUntilを使って指定時刻まで待機する
        /// </summary>
        /// <returns>ループしないので0</returns>
        static int WaitUntilUsingSpinUntil(DateTime time)
        {
            SpinWait.SpinUntil(() => DateTime.Now >= time);
            return 0;
        }

        /// <summary>
        /// SpinWait.SpinOnceを使って指定時刻まで待機する
        /// </summary>
        /// <returns>ループ回数</returns>
        static int WaitUntilUsingSpinOnce(DateTime time)
        {
            int count = 0;
            SpinWait sw = new SpinWait();
            while (DateTime.Now < time)
            {
                sw.SpinOnce();
                count++;
            }
            return count;
        }

        /// <summary>
        /// 空ループを使って指定時刻まで待機する
        /// </summary>
        /// <returns>ループ回数</returns>
        static int WaitUntilWithEmptyLoop(DateTime time)
        {
            int count = 0;
            while (DateTime.Now < time)
            {
                count++;
            }
            return count;
        }

    }
}
