using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsciiEngine
{
    public class Engine : INodes
    {
        
        public static readonly Vector2 ChunkSize = new Vector2(320, 100);
        public static bool EndProcesses { set; get; }
        public static Stopwatch Timer { private set; get; }
        
        private bool isDisplayParallel = true;
        private bool ShowGameMetrics = true;
        private int RenderType = 0;

        public World World { set; get; }
        public Renderer Renderer { get; }
        public PhysicsSpace PhysicsSpace { get; }
        public CoreUpdate CoreUpdate { get; }
        public Display Display { get; }
        public Camera Camera { get; }
        public CoreInput Input { get; }
        public Vector2 SystemScreenSize { private set; get; }
        public Vector2 SystemWindowSize { private set; get; }
        public List<INodes> Children { set; get; }
        public INodes Parent { set; get; }

        public void Initialize()
        {
            Console.SetBufferSize((Console.LargestWindowWidth), (Console.LargestWindowHeight));
            SystemScreenSize = new Vector2(Console.LargestWindowWidth, Console.LargestWindowHeight);
            SystemWindowSize = new Vector2(Console.WindowWidth, Console.WindowHeight);
            Console.CursorVisible = false;
            Timer = Stopwatch.StartNew();
        }

        public Engine()
        {
            Children = new List<INodes>();
            Renderer = new Renderer(this);
            PhysicsSpace = new PhysicsSpace(this);
            CoreUpdate = new CoreUpdate();
            Display = new Display();
            Camera = new Camera();
            Input = new CoreInput();
        }

        public void Update()
        {

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
