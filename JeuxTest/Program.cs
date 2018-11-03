using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input; // Nécéssite une référence dll
using static System.ConsoleColor;

namespace Game
{
    public class Program
    {





        public static string[] DefaultGraphics =
        {
            " ",
            " "
        };
        public static string[] PlayerGraphic = {
            " XXXXX ",
            "XxOxXXX",
            "0Xx   0",
            "00x0x00",
            " xXXXx ",
        };
        public static string[] AsteroidGraphic = {
            "  AAAAAA  ",
            " AAAAAAAAA",
            " AAAAAAAAA",
            "AAAAAAAAA ",
            "  AAAAAAAA"
        };





        public static ConsoleColor[,] PlayerGraphicColorMatrix = {
            { Gray, Blue, DarkRed , DarkRed, DarkRed, Blue, Gray },
            { Gray, Blue, Blue, Blue, Blue, Blue, Gray },
            { Gray, Gray, Blue, Blue, Blue, Blue, Gray },
            { Gray, Gray, DarkRed, DarkRed, Gray, Gray, Gray },
            { Gray, Gray, Blue, Blue, DarkRed, Gray, Gray }
        };






        // Generates temp files.
        public static int RenderMode = 1;
        public static Physics.Space Map = new Physics.Space(new Vector2(400, 400));
        public static Systems.Camera Camera = new Systems.Camera();
        public static Program Game = new Program();
        public static Systems.Display Display = new Systems.Display();
        public static Systems.ScreenRenderer ScreenRenderer = new Systems.ScreenRenderer();
        public static bool GameExit = false;
        public Player player = new Player(PlayerGraphic, PlayerGraphicColorMatrix);




        void GameInit()
        {

            PopulateAsteroid(new Vector2(20, 20));
            PopulateAsteroid(new Vector2(150, 30));
            PopulateAsteroid(new Vector2(35, 50));
            Console.SetBufferSize((int)Console.LargestWindowWidth, (int)Console.LargestWindowHeight);
            Console.SetWindowSize((int)(Console.LargestWindowWidth / 1.5), (int)(Console.LargestWindowHeight / 1.5));
            Console.CursorVisible = false;
            player.Position = new Vector2(10, 10);
            Camera.SetFocus(player);
            Camera.FitScreenSize();

        }



        // STAThread is only there to make the old System.Windows.Input work.
        [STAThread]
        static void Main(string[] args)
        {
            Game.GameProcesses();
        }




        public void GameProcesses()
        {
            GameInit();
            while (!GameExit)
            {
                DateTime time = DateTime.Now;
                Console.CursorVisible = false;
                Systems.Update.Process(time);
            }
        }




        void PopulateAsteroid(Vector2 position)
        {
            GameObject newAsteroid = new GameObject(AsteroidGraphic)
            {
                Position = position
            };
        }


    }
}
