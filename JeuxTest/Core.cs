using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input; // Nécéssite une référence dll
using static System.ConsoleColor;

namespace AsciiEngine
{
    public class Core
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
        public static int RenderMode = 2;
        public static Vector2 ScreenSize = new Vector2((int)(Console.LargestWindowWidth / 1.5), (int)(Console.LargestWindowHeight / 1.5));
        public static Physics.Space Map = new Physics.Space(new Vector2(400, 400));
        public static Camera Camera = new Camera();
        public static Core Engine = new Core();
        public static Systems.Display Display = new Systems.Display();
        //public static Systems.ScreenRenderer ScreenRenderer = new Systems.ScreenRenderer();
        public static Renderer Renderer = new Renderer();
        public static bool GameExit = false;

        public Vector2 RenderingSize = new Vector2(Console.LargestWindowWidth, Console.LargestWindowHeight);
        public PhysicsSpace PhysicsSpace = new PhysicsSpace();
        public Player player;

        void GameInit()
        {
            Console.SetBufferSize((int)(Console.LargestWindowWidth), (int)(Console.LargestWindowHeight));
            Console.SetWindowSize(ScreenSize.X, ScreenSize.Y);
            Console.CursorVisible = false;



            Camera.FitScreenSize();


            player = new Player(PlayerGraphic, PlayerGraphicColorMatrix) { Position = new Vector2(0, 0) };
            Camera.SetFocus(player);
            //PopulateAsteroid(new Vector2(20, 20));
            //PopulateAsteroid(new Vector2(150, 30));
            //PopulateAsteroid(new Vector2(35, 50));
            PopulateAsteroids(200);
        }



        // STAThread is only there to make the old System.Windows.Input work.
        [STAThread]
        static void Main(string[] args)
        {
            Engine.Processes();
        }




        public void Processes()
        {
            GameInit();
            while (!GameExit)
            {
                DateTime time = DateTime.Now;
                Console.CursorVisible = false;
                Systems.Update.Process(time);
            }
        }


        public void PopulateAsteroids(int amountAsteroids)
        {
            Random random = new Random();
            GameObject[] asteroids = new GameObject[amountAsteroids];
            foreach(GameObject gameObject in asteroids)
            {
                PopulateAsteroid(new Vector2(random.Next(5, 380), random.Next(5, 380)));
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
