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
            Mode mode = GetMode(args);
            if (mode == Mode.NotDefined)
            {
                Console.WriteLine("Usage: SpinWaitDemo.exe (loop | once | until)");
                return;
            }
            int count = GetCount();
            Demo(mode, count);
        }

        /// <summary>
        /// 引数からモードを取得する
        /// </summary>
        static Mode GetMode(string[] args)
        {
            if (args.Length == 0)
            {
                return Mode.NotDefined;
            }
            switch (args[0].ToLower())
            {
                case "loop":
                    return Mode.Loop;
                case "until":
                    return Mode.SpinUntil;
                case "once":
                    return Mode.SpinOnce;
                default:
                    return Mode.NotDefined;
            }
        }

        /// <summary>
        /// カウントを入力させる
        /// </summary>
        static int GetCount()
        {
            int count = 0;
            Console.Write("Input count: ");
            while (count == 0)
            {
                if (int.TryParse(Console.ReadLine(), out count))
                {
                    break;
                }
                else
                {
                    Console.Write("Invalid input. Input count: ");
                }
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
