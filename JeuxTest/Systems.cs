using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static AsciiEngine.Utility;


namespace AsciiEngine
{
    public class Systems
    {
        public static Display CurrentDisplay;


        public class Display : IUpdate
        {
            // Initialization
            public Display()
            {
                CurrentDisplay = this;
            }

            // Update method from IUpdate interface
            public void Update()
            {
                // No color but the fastest render.

                // Color Mode with render from layer and backbuffer.

                //ScreenRenderer.Instance.ScreenBuffer.Draw();
                Renderer.Instance.ScreenBuffer.Draw();

            }

            

        }


      
        


        public static class Update
        {
            // The delta of Ticks between each loops. 
            public static double DeltaTime { get; private set; }
            public static long updateTicks = 0;

            // A list of every objects registered to the update cycles.
            public static List<object> Registry = new List<object>();
            // Update Loop
            public static void Process(DateTime time)
            {
                // There are 10000 ticks in a millisecond.
                if ((time.Ticks - updateTicks) / 10000.0 >= 10.0)
                {
                    // Handles the update of the ticks and time data for the game systems.
                    DeltaTime = (time.Ticks - updateTicks) / 10000.0;
                    updateTicks = time.Ticks;
                    //Propagate the Update pulse among the registered objects
                    foreach (object obj in Registry)
                    {
                        UpdateComponents(obj as IUpdate);
                    }
                    Core.Engine.PhysicsSpace.Update();
                    CurrentDisplay.Update();

                    Console.SetCursorPosition(0, Console.WindowHeight - 2);

                    if (DeltaTime < 200)
                        Console.Write(DeltaTime);
                    Console.Write("\n" + Core.Engine.player.Position + "  ");
                }
            }
            private static void UpdateComponents(IUpdate obj)
            {
                if (null != obj)
                    obj.Update();
            }
            public static void Register(Object obj)
            {
                if (null != obj)
                    Registry.Add(obj);
            }

        }

    }
}
