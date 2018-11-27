using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsciiEngine
{
    public class Engine : INodes
    {

        public static readonly bool isDisplayParallel = true;
        public static readonly Vector2 ChunkSize = new Vector2(320, 100);
        public static readonly int RenderType = 2;
        public static readonly bool ShowGameMetrics = true;

        public static Engine Instance { set; get; }
        public static bool EndProcesses { set; get; }
        public static Stopwatch Timer { private set; get; }

        public World World { set; get; }
        public CoreRenderer Renderer { get; }
        public CorePhysics PhysicsSpace { get; }
        public CoreUpdate CoreUpdate { get; }
        public CoreDisplay Display { get; }
        public Camera Camera { get; }
        public CoreInput Input { get; }
        public Vector2 SystemScreenSize { private set; get; }
        public Vector2 SystemWindowSize { private set; get; }
        public List<INodes> Children { set; get; }
        public INodes Parent { set; get; }
        public Vector2 RenderingSize { set; get; }

        public void Initialize()
        {
            SystemScreenSize = new Vector2(Console.LargestWindowWidth, Console.LargestWindowHeight);
            SystemWindowSize = new Vector2(Console.WindowWidth, Console.WindowHeight);
            Console.SetWindowSize((int)(SystemScreenSize.X / 1.5), (int)(SystemScreenSize.Y / 1.5));
            RenderingSize = new Vector2(Console.LargestWindowWidth * 2, Console.LargestWindowHeight * 2);
            World.Initialize();
            Update();
        }

        public Engine()
        {
            Instance = this;
            Timer = Stopwatch.StartNew();
            Children = new List<INodes>();
            Renderer = new CoreRenderer(this);
            PhysicsSpace = new CorePhysics(this);
            CoreUpdate = new CoreUpdate();
            Display = new CoreDisplay();
            Camera = new Camera();
            Input = new CoreInput();
            World = new World(this);
        }

        public void Update()
        {
            while (!EndProcesses)
            {
                CoreUpdate.Processes();
                if (CoreUpdate.DeltaTime <= 14.9)
                    Thread.Sleep((int)(16 - (CoreUpdate.EngineTicks / CoreUpdate.TicksResolution * 1000 )));
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


    }
}
