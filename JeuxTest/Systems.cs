using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static Game.Utility;


namespace Game
{
    public class Systems
    {






        public class Display : IUpdate
        {
            // Initialization
            public Display()
            {

            }

            // Update method from IUpdate interface
            public void Update()
            {
                // No color but the fastest render.
                if (Program.RenderMode == 1)
                {
                    Program.Camera.ScreenRender.Draw();
                }
                // Color Mode with render from layer and backbuffer.
                if (Program.RenderMode == 0)
                {
                    Program.Camera.ScreenRender.Draw();
                    DrawColorLayer(Program.Camera.ScreenRender);
                }

            }



            public void Draw(object frame)
            {
                Vector2 objPosition = (frame as IPosition) != null ? (frame as IPosition).Position : Vec2(0, 0);
                Vector2 objSize = (frame as ISize) != null ? (frame as ISize).Size : Vec2(0, 0);
                Sprite objSprite = (frame as IGraphics) != null ? (frame as IGraphics).Graphics : (Sprite)frame;

                Vector2 CameraPosition = Program.Camera.Position;
                Vector2 CameraSize = Program.Camera.Size;
                Vector2 objScreenPosition = objPosition - CameraPosition;

                if((objScreenPosition + objSize).X < 0 || (objScreenPosition + objSize).Y < 0)
                    return;
                if ((objScreenPosition).X > CameraSize.X || (objScreenPosition).Y > CameraSize.Y)
                    return;


                for (int y = 0; y < objSize.Y; y++)
                {
                    for (int x = 0; x < objSize.X; x++)
                    {
                        if (objSprite[x, y].Color != ConsoleColor.Gray)
                        {
                            Console.ForegroundColor = (objSprite[x, y]).Color;
                            Console.SetCursorPosition(x + objScreenPosition.X , y + objScreenPosition.Y);
                            Console.Write(objSprite[x, y].Char);
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.ForegroundColor = (objSprite[x, y]).Color;
                            Console.SetCursorPosition(x + objScreenPosition.X, y + objScreenPosition.Y);
                            Console.Write(objSprite[x, y].Char);
                            Console.ResetColor();
                        }
                    }
                }
            }

            public void Erase(object frame)
            {
                Vector2 objPosition = (frame as IPosition) != null ? (frame as IPosition).Position : Vec2(0, 0);
                Vector2 objSize = (frame as ISize) != null ? (frame as ISize).Size : Vec2(0, 0);
                Sprite objSprite = (frame as IGraphics) != null ? (frame as IGraphics).Graphics : (Sprite)frame;

                Vector2 CameraPosition = Program.Camera.Position;
                Vector2 CameraSize = Program.Camera.Size;
                Vector2 objScreenPosition = objPosition - CameraPosition;

                if ((objScreenPosition + objSize).X < 0 || (objScreenPosition + objSize).Y < 0)
                    return;
                if ((objScreenPosition).X > CameraSize.X || (objScreenPosition).Y > CameraSize.Y)
                    return;


                for (int y = 0; y < objSize.Y; y++)
                {
                    for (int x = 0; x < objSize.X; x++)
                    {
                        if (objSprite[x, y].Char != ' ')
                        {
                            Console.SetCursorPosition(x + objScreenPosition.X, y + objScreenPosition.Y);
                            Console.Write(' ');
                        }
                    }
                }
            }


            public void DrawColorLayer(object frame)
            {
                Vector2 objPosition = (frame as IPosition) != null ? (frame as IPosition).Position : Vec2(0, 0);
                Vector2 objSize = (frame as ISize) != null ? (frame as ISize).Size : Vec2(0, 0);
                Sprite objSprite = (frame as IGraphics) != null ? (frame as IGraphics).Graphics : (Sprite)frame;

                for (int y = 0; y < objSize.Y; y++)
                {
                    for (int x = 0; x < objSize.X; x++)
                    {
                        if (objSprite[x, y].Color != ConsoleColor.Gray)
                        {
                            Console.ForegroundColor = objSprite[x, y].Color;
                            Console.SetCursorPosition(x, y);
                            Console.Write(objSprite[x, y].Char);
                            Console.ResetColor();
                        }
                    }
                }
            }
        }






        public class Camera : IUpdate, IPosition
        {

            const int SizeOffsetX = 0;
            const int SizeOffsetY = 2;
            private Vector2 position = new Vector2(0, 0);
            static private Camera ActiveCamera;
            private object ObjectFocused;
            public Vector2 Offset;
            public Vector2 RelativePosition;
            public string[] EmptyCanvas;
            public Sprite ScreenRender;
            private GameObject objectFocused;
            private Vector2 size = new Vector2(Console.WindowWidth - SizeOffsetX, Console.WindowHeight - SizeOffsetY);
            public Vector2 Size
            {
                get
                {
                    return this.size;
                }
                set
                {
                    size = value;
                    Offset = new Vector2(value.X / 2, value.Y / 2);
                }
            }
            private ScreenCanvas cameraCanvas;
            private ScreenCanvas CameraCanvas
            {
                get
                {
                    if (cameraCanvas == null || (cameraCanvas as ISize).Size != this.Size)
                    {
                        cameraCanvas = new ScreenCanvas(this.Size);
                    }
                    return cameraCanvas;
                }
                set
                {
                    cameraCanvas = value;
                }
            }

            public Vector2 Position
            {
                set
                {
                    if (value.X < 0)
                        value.X = 0;
                    if (value.Y < 0)
                        value.Y = 0;
                    if (value.X > Program.Map.Size.X - this.Size.X)
                        value.X = Program.Map.Size.X - this.Size.X;
                    if (value.Y > Program.Map.Size.Y - this.Size.Y)
                        value.Y = Program.Map.Size.Y - this.Size.Y;
                    position = value;
                }
                get
                {
                    return position;
                }
            }
            // Implementation of IUpdate interface
            public void Update()
            {
                if (objectFocused != null)
                    Position = objectFocused.Position + objectFocused.Size - Offset;
                else
                    Position = new Vector2(0, 0);
                ScreenRender = RenderSpace(Program.Map);
            }
            public Camera()
            {
                Offset = new Vector2(Size.X / 2, Size.Y / 2);
                RelativePosition = Position + Offset;
                Systems.Update.Register(this);
            }

            public void SetFocus(GameObject obj)
            {
                objectFocused = obj;
            }

            public void SetFocus(Vector2 vector)
            {
                Position = vector - Offset;
            }
            public Sprite RenderSpace(Physics.Space space)
            {

                Vector2 index = new Vector2(0, 0);
                Sprite render = CameraCanvas.Canvas;
                for (int y = this.Position.Y; y < this.Position.Y + this.Size.Y; y++)
                {
                    index.X = 0;
                    for (int x = this.Position.X; x < this.Position.X + this.Size.X; x++)
                    {
                        if (space[x, y] != null)
                        {
                            GameObject gameObject = space[x, y];
                            Vector2 relativePosition = new Vector2(x, y) - gameObject.Position;
                            if (relativePosition.X < gameObject.Graphics.Size.X
                                && relativePosition.Y < gameObject.Graphics.Size.Y)
                            {
                                render[index.X, index.Y] = gameObject.Graphics[relativePosition.X, relativePosition.Y];
                            }
                        }
                        index.X++;
                    }
                    index.Y++;
                }
                return render;
            }
            public void FitScreenSize()
            {
                this.Size = GetWindowSize() - new Vector2(SizeOffsetX, SizeOffsetY);
            }

            private class ScreenCanvas : ISize
            {
                public Sprite Canvas;
                public Vector2 Size => (Canvas as ISize).Size;

                public ScreenCanvas(Vector2 size)
                {
                    Canvas = new Sprite(GenerateCanvas(size));
                }

                private string[] GenerateCanvas(Vector2 size)
                {
                    string[] canvas = new string[size.Y];
                    for (int y = 0; y < size.Y; y++)
                    {
                        canvas[y] = new string(' ', size.X);
                    }
                    return canvas;
                }
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
                if ((time.Ticks - updateTicks) / 10000.0 >= 15)
                {
                    // Handles the update of the ticks and time data for the game systems.
                    DeltaTime = (time.Ticks - updateTicks) / 10000.0;
                    Console.SetCursorPosition(0, Console.WindowHeight - 2);

                    if (DeltaTime < 200)
                        Console.Write(DeltaTime);
                    Console.Write("\n" + Program.Game.player.Position + "  ");
                    Console.Write(Program.Map.SpaceArray[0, 6]);


                    updateTicks = time.Ticks;
                    //Propagate the Update pulse among the registered objects
                    foreach (object obj in Registry)
                    {
                        UpdateComponents(obj as IUpdate);
                    }
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
