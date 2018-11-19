using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsciiEngine
{
    public class CoreUpdate
    {
        public long EngineTicks => Core.Time.ElapsedTicks;
        public long TicksResolution = Stopwatch.Frequency;
        public double DeltaTime;

        private double DeltaOffset = 1;
        public double DeltaTimeDisplay = 0;
        public int FrameRate = 60;
        public int FixedProcessUPS = 60;

        private long fixedProcessTicks = Core.Time.ElapsedTicks;
        private long displayProcessTicks = Core.Time.ElapsedTicks;


        private int metricsCounter = 0;
        private double gameEngineUpdateMetrics = 0.0;
        private double gameObjectUpdateMetrics = 0.0;
        private double physicsUpdateMetrics = 0.0;
        private double renderUpdateMetrics = 0.0;
        private double displayUpdateMetrics = 0.0;

        public void UpdateMetrics(double objDelta, double physicsDelta, double renderDelta, double displayDelta)
        {
            ++metricsCounter;
            gameEngineUpdateMetrics += DeltaTime;
            gameObjectUpdateMetrics += objDelta;
            physicsUpdateMetrics += physicsDelta;
            renderUpdateMetrics += renderDelta;
            displayUpdateMetrics += displayDelta;

            if (metricsCounter >= 60)
            {
                metricsCounter = 0;
                gameEngineUpdateMetrics /= 60;
                gameObjectUpdateMetrics /= 60;
                physicsUpdateMetrics /= 60;
                renderUpdateMetrics /= 60;
                displayUpdateMetrics /= 60;

                if (!Core.Engine.ShowGameMetrics) return;

                Console.SetCursorPosition(0, Console.WindowHeight - 2);
                Console.Write("Update : {0:N3}ms    GameObjects : {1:N3}ms    Physics : {2:N3}ms    Rendering : {3:N3}ms    " +
                    "Display : {4:N3}ms       RenderType : {5}    ",
                   DeltaTime, objDelta, physicsDelta, renderDelta, DeltaTimeDisplay,
                   Core.Engine.RenderType, FrameRate);

            }

            if (!Core.Engine.ShowGameMetrics) return;

            Console.SetCursorPosition(0, Console.WindowHeight - 1);
            Console.Write(
                "PlayerBiomePosition : {0}   Camera Area : {1}:{2}    Velocity : {3:N3};{5:N3}" +
                "      PhysicsPosition: {4:N3};{6:N3}    BiomeType: {7}   BiomePosition: {8}      ",
                (World.Player.Parent as Chunk).Position,
                Core.Engine.Camera.Position,
                Core.Engine.Camera.Position + Core.Engine.Camera.Size,
                World.Player.PhysicsBody.Velocity.X,
                World.Player.PhysicsBody.Position.X,
                World.Player.PhysicsBody.Velocity.Y,
                World.Player.PhysicsBody.Position.Y,
                (World.Instance.Children[4] as Chunk).Biome.Type,
                (World.Instance.Children[4] as Chunk).Position
                );
        }

        public void Processes()
        {
            List<Task> taskList = new List<Task>();
            if (1.0 * (EngineTicks - fixedProcessTicks) / TicksResolution * 1000.0 > 16.0)
            {
                DeltaTime = 1.0 * (EngineTicks - fixedProcessTicks) / TicksResolution * 1000.0;
                fixedProcessTicks = EngineTicks;
                UpdateFixedProcesses();
                UpdateDisplay();
            }
            
        }
        private void UpdateDisplay()
        {
            Core.Engine.Display.Update();
        }

        private void UpdateFixedProcesses()
        {
            Vector2 renderingPosition = Core.Engine.Renderer.Position;
            Vector2 renderingSize = Core.Engine.Renderer.Size;

            long ticks = EngineTicks;
            /*foreach (GameObject obj in GameObject.List)
            {
                if (obj.Position.X < renderingPosition.X || obj.Position.Y < renderingPosition.Y) continue;
                if (obj.Position.X >= renderingPosition.X + renderingSize.X || obj.Position.Y >= renderingPosition.Y + renderingSize.Y) continue;
                if ((obj as IUpdate) == null) continue;

                (obj as IUpdate).Update();
            }*/
            World.Instance.Update();
            double deltaTimeObject = 1.0 * (EngineTicks - ticks) / TicksResolution * 1000.0;
            ticks = EngineTicks;
            Core.Engine.PhysicsSpace.Update();
            double deltaTimePhysics = 1.0 * (EngineTicks - ticks) / TicksResolution * 1000.0;
            ticks = EngineTicks;
            Core.Engine.Renderer.Update();
            double deltaTimeRender = 1.0 * (EngineTicks - ticks) / TicksResolution * 1000.0;


            UpdateMetrics(deltaTimeObject, deltaTimePhysics, deltaTimeRender, DeltaTimeDisplay);
        }

    }
}
