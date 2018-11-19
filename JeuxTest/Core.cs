/*
 *      Programmer : Yanik Sweeney
 *      Date of creation : 25/10/2018
 *      Latest update : 18/11/2018
 * 
 */

using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

namespace AsciiEngine
{
    public partial class Core : INodes
    {


        // Generates temp files.
        public static Core Engine;

        public Renderer Renderer;
        public Vector2 RenderingSize;
        public PhysicsSpace PhysicsSpace;
        public CoreUpdate CoreUpdate;
        public Display Display;
        public Camera Camera;
        public CoreInput Input;
        public Player player;
        public World World;

        public bool isDisplayParallel = true;
        public bool ShowGameMetrics = true;
        public int RenderType = 0;
        public int ProcessorCoresCount;
        public bool EndProcesses = false;
        private Vector2 ScreenSize;
        public long ElaspedTicks;
        public static Stopwatch Time = Stopwatch.StartNew();
        public List<INodes> Children { set; get; }
        public INodes Parent { set; get; }

        static void Main(string[] args)
        {
            Core Engine = new Core();
            Engine.Update();
        }


        void GameInit()
        {
            //System.Threading.
            ProcessorCoresCount = Environment.ProcessorCount;
            Console.SetBufferSize((int)(Console.LargestWindowWidth), (int)(Console.LargestWindowHeight));
            Console.SetWindowSize(ScreenSize.X, ScreenSize.Y);
            Console.CursorVisible = false;
            Camera.FitScreenSize();
            //player = new Player(PlayerGraphic, PlayerGraphicColorMatrix) { Position = new Vector2(0, 0) };
            //Camera.SetFocus(player);
            //PopulateAsteroids(8000);
            //PopulateStardust(10000);
            World.Initialize();
        }

        public Core()
        {
            Children = new List<INodes>();
            Engine = this;
            Renderer = new Renderer(Engine);
            RenderingSize = new Vector2(Console.LargestWindowWidth, Console.LargestWindowHeight);
            PhysicsSpace = new PhysicsSpace(Engine);
            CoreUpdate = new CoreUpdate();
            Display = new Display();
            Camera = new Camera();
            Input = new CoreInput();         
            ScreenSize = new Vector2((int)(Console.LargestWindowWidth / 1.5), (int)(Console.LargestWindowHeight / 1.5));
            World = new World(this);
        }

        public void Update()
        {
            GameInit();
            while (!EndProcesses)
            {
                //Console.CursorVisible = false;
                //Systems.Update.Process();
                CoreUpdate.Processes();
                if (Engine.CoreUpdate.DeltaTime <= 14.9)
                    Thread.Sleep(1);
            }
        }

        public void AddChild(INodes child)
        {
            Children.Add(child);
        }

        public void RemoveChild(INodes child)
        {
            Children.Remove(child);
        }

        public string[] GetAsteroidGraphic(int random)
        {
            string[] newGraphic;
            switch (random)
            {
                case 1:
                case 2:
                case 3:
                    newGraphic = AsteroidGraphic;
                    break;
                case 4:
                case 5:
                    newGraphic = AsteroidGraphic2;
                    break;
                case 7:
                    newGraphic = AsteroidGraphic4;
                    break;
                default:
                    newGraphic = AsteroidGraphic3;
                    break;

            }
            return newGraphic;
        }

        public void PopulateAsteroids(int amountAsteroids)
        {
            Random random = new Random();
            GameObject[] asteroids = new GameObject[amountAsteroids];
            foreach (GameObject gameObject in asteroids)
            {
                Vector2 randomPosition = new Vector2(random.Next(-1200, 1200), random.Next(-1200, 1200));
                while (randomPosition.X < 8 && randomPosition.X > -8 && randomPosition.Y < 8 && randomPosition.Y > -8)
                {
                    randomPosition = new Vector2(random.Next(-1200, 1200), random.Next(-1200, 1200));
                }
                PopulateAsteroid(randomPosition, random);
            }
        }

        void PopulateAsteroid(Vector2 position, Random random)
        {

            GameObject newAsteroid = new GameObject(GetAsteroidGraphic(random.Next(0, 8)))
            {
                Position = position
            };
        }

        void PopulateStardust(int amount)
        {
            var newList = new List<GameObject>();
            Random random = new Random();
            for (int i = 0; i < amount; i++)
            {
                GameObject newStardust = new GameObject();
                newStardust.Initialize();
                newStardust.Body = Tiles.GenerateByteArrayMap(StarDust);
                newStardust.Graphics = new Graphics(newStardust);
                newStardust.Position = (new Vector2(random.Next(-1200, 1200), random.Next(-1200, 1200)));
            }

        }
    }
}
