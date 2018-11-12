using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsciiEngine
{
    public class SystemsUpdate
    {
        public long EngineTicks => DateTime.Now.Ticks;
        public double DeltaTime;

        private double DeltaOffset = 1;
        private double DeltaTimeDisplay = 0;
        public int FrameRate = 60;
        public int FixedProcessUPS = 90;

        private long fixedProcessTicks = DateTime.Now.Ticks;
        private long displayProcessTicks = DateTime.Now.Ticks;


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
                    "Display : {4:N3}ms     Logical Cores : {5}    RenderType : {6}    FPS Cap : {7}    ",
                   DeltaTime, objDelta, physicsDelta, renderDelta, displayDelta,
                   Core.Engine.ProcessorCoresCount, Core.Engine.RenderType, FrameRate);

            }

            if (!Core.Engine.ShowGameMetrics) return;

            Console.SetCursorPosition(0, Console.WindowHeight - 1);
            Console.Write(
                "Player Position : {0}   Camera Area : {1}:{2}    Rendering Area : {3}:{4}    Velocity : {5:N3};{7:N3}" +
                "      PhysicsPosition: {6:N3};{8:N3}   ", 
                Core.Engine.player.Position,
                Core.Engine.Camera.Position,
                Core.Engine.Camera.Position + Core.Engine.Camera.GetSize(), 
                Core.Engine.Renderer.Position, 
                Core.Engine.Renderer.Position + Core.Engine.Renderer.GetSize(),
                Core.Engine.player.PhysicsBody.Velocity.X, 
                Core.Engine.player.PhysicsBody.Position.X,
                Core.Engine.player.PhysicsBody.Velocity.Y,
                Core.Engine.player.PhysicsBody.Position.Y
                );
        }

        public void Processes()
        {
            if ((EngineTicks - fixedProcessTicks) / 10000.0 > (1000.0 / FixedProcessUPS - DeltaOffset))
            {
                DeltaTime = (EngineTicks - fixedProcessTicks) / 10000.0;
                fixedProcessTicks = EngineTicks;
                UpdateFixedProcesses();
            }

            if((EngineTicks - displayProcessTicks) / 10000.0 > (1000.0 / FrameRate - DeltaOffset))
            {
                displayProcessTicks = EngineTicks;
                long ticks = EngineTicks;
                UpdateDisplay();
                DeltaTimeDisplay = (EngineTicks - ticks) / 10000.0;
            }
            


        }
        private void UpdateDisplay()
        {
            Core.Engine.Display.Update();
        }

        private void UpdateFixedProcesses()
        {
            long ticks = EngineTicks;
            foreach (GameObject obj in GameObject.List)
            {
                if ((obj as IUpdate) == null) continue;
                (obj as IUpdate).Update();
            }
            double deltaTimeObject = (EngineTicks - ticks) / 10000.0;
            ticks = EngineTicks;
            Core.Engine.PhysicsSpace.Update();
            double deltaTimePhysics = (EngineTicks - ticks) / 10000.0;
            ticks = EngineTicks;
            Core.Engine.Renderer.Update();
            double deltaTimeRender = (EngineTicks - ticks) / 10000.0;


            UpdateMetrics(deltaTimeObject, deltaTimePhysics, deltaTimeRender, DeltaTimeDisplay);
        }

    }
}
