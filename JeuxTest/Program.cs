using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsciiEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            //Core Engine = new Core();
            //Engine.Update();


            Console.SetBufferSize((Console.LargestWindowWidth), (Console.LargestWindowHeight));
            Console.CursorVisible = false;
            Console.BackgroundColor = ConsoleColor.Black;
            new Engine().Initialize();
        }
    }
}
