/*
 *      Author : Yanik Sweeney
 *      Date of creation : 25/10/2018
 *      Latest update : 11/11/2018
 * 
 */ 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input; // Nécéssite une référence dll
using static System.ConsoleColor;
using System.Threading;


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
        public static Core Engine = new Core();
        //public static Systems.ScreenRenderer ScreenRenderer = new Systems.ScreenRenderer();
        public static bool EndProcesses = false;


        public Renderer Renderer = new Renderer();
        public Vector2 RenderingSize = new Vector2(Console.LargestWindowWidth, Console.LargestWindowHeight);
        public PhysicsSpace PhysicsSpace = new PhysicsSpace();
        public SystemsUpdate GameUpdate = new SystemsUpdate();
        public Display Display = new Display();
        public Camera Camera = new Camera();
        public Input Input = new Input();
        public bool ShowGameMetrics = true;
        public Player player;
        public int RenderType = 1;
        public int ProcessorCoresCount;


        void GameInit()
        {
            //System.Threading.
            ProcessorCoresCount = Environment.ProcessorCount;
            Console.SetBufferSize((int)(Console.LargestWindowWidth), (int)(Console.LargestWindowHeight));
            Console.SetWindowSize(ScreenSize.X, ScreenSize.Y);
            Console.CursorVisible = false;
            Camera.FitScreenSize();
            player = new Player(PlayerGraphic, PlayerGraphicColorMatrix) { Position = new Vector2(0, 0) };
            Camera.SetFocus(player);
            PopulateAsteroids(700);
        }

        //[STAThread]
        static void Main(string[] args)
        {
            Engine.Processes();
        }

        public void Processes()
        {
            GameInit();
            while (!EndProcesses)
            {
                //Console.CursorVisible = false;
                //Systems.Update.Process();
                GameUpdate.Processes();
                Thread.Sleep(1);
            }
        }


        public void PopulateAsteroids(int amountAsteroids)
        {
            Random random = new Random();
            GameObject[] asteroids = new GameObject[amountAsteroids];
            foreach(GameObject gameObject in asteroids)
            {
                Vector2 randomPosition = new Vector2(random.Next(-400, 400), random.Next(-400, 400));
                while(randomPosition.X < 8 && randomPosition.X > -8 && randomPosition.Y < 8 && randomPosition.Y > -8)
                {
                    randomPosition = new Vector2(random.Next(-400, 400), random.Next(-400, 400));
                }
                PopulateAsteroid(randomPosition);
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
