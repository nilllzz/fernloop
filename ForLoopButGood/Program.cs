using System;

namespace ForLoopButGood
{
    class Program
    {
        static void Main(string[] args)
        {
            Looper.F0r.Loop(0, 100, 1, i =>
            {
                Console.WriteLine(i.ToString());
            });
        }
    }
    
}
