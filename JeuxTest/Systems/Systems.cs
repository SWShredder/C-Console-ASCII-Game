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

                ScreenRenderer.Instance.ScreenBuffer.Draw();

            }



            public void DrawNoColor(object frame, bool isLayer = false)
            {
                Vector2 framePosition = (frame as IPosition) != null ? (frame as IPosition).Position : Vec2(0, 0);
                Vector2 frameSize = (frame as ISize) != null ? (frame as ISize).Size : Vec2(0, 0);
                Sprite frameSprite = (frame as IGraphics) != null ? (frame as IGraphics).Graphics : (Sprite)frame;

                string[] screenBuffer = new string[frameSize.Y];

                for (int y = 0; y < frameSize.Y; y++)
                {
                    string bufferLine = "";
                    for (int x = 0; x < frameSize.X; x++)
                    {
                        if (frameSprite[x, y].Color == ConsoleColor.Gray || isLayer == false)
                            bufferLine += frameSprite[x, y].Char;
                        else
                            bufferLine += " ";
                    }
                    screenBuffer[y] = bufferLine;
                }
                Console.SetCursorPosition(0, 0);
                foreach (string line in screenBuffer)
                {
                    Console.WriteLine(line);
                }
            }



            public void Draw(object frame)
            {
                Vector2 objPosition = (frame as IPosition) != null ? (frame as IPosition).Position : Vec2(0, 0);
                Vector2 objSize = (frame as ISize) != null ? (frame as ISize).Size : Vec2(0, 0);
                Sprite objSprite = (frame as IGraphics) != null ? (frame as IGraphics).Graphics : (Sprite)frame;

                Vector2 CameraPosition = Core.Camera.Position;
                Vector2 CameraSize = Core.Camera.Size;
                Vector2 objScreenPosition = objPosition - CameraPosition;

                if ((objScreenPosition + objSize).X < 0 || (objScreenPosition + objSize).Y < 0)
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
                            Console.SetCursorPosition(x + objScreenPosition.X, y + objScreenPosition.Y);
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

                Vector2 CameraPosition = Core.Camera.Position;
                Vector2 CameraSize = Core.Camera.Size;
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



        public class ScreenRenderer
        {

            static private ScreenRenderer instance;
            static public ScreenRenderer Instance
            {
                private set => instance = value;
                get => instance;
            }

            public Frame Render = new Frame() { Size = Core.ScreenSize };
            public ScreenBuffer ScreenBuffer
            {
                get
                {
                    return new ScreenBuffer() { Size = Core.ScreenSize, Sprite = Render.Sprite };
                }
            }

            List<DrawRequestEventArgs> DrawRequests;
            // List<EraseRequestEventArgs> EraseRequests;

            public ScreenRenderer()
            {
                Instance = this;
                DrawRequests = new List<DrawRequestEventArgs>();

            }

            public void OnDrawRequest(object source, DrawRequestEventArgs args)
            {
                //DrawRequests.Add(args);
                if (Core.RenderMode >= 2)
                    RenderRequest(source, args);
            }

            public void OnCameraPositionChanged(object source, CameraPositionEventArgs args)
            {
            }

            public void Update()
            {
                //DoDrawRequests(ref DrawRequests);
            }





            private void DoDrawRequests(ref List<DrawRequestEventArgs> requestList)
            {
                foreach (DrawRequestEventArgs args in requestList)
                {
                    //RenderRequest(args);
                }
                requestList = new List<DrawRequestEventArgs>();
            }



            private void RenderRequest(object source, DrawRequestEventArgs args)
            {
                GameObject gameObject = source as GameObject;
                Vector2 oldPosition = args.OldPosition;
                Vector2 newPosition = args.NewPosition;
                Sprite spriteToDraw = args.Sprite;
                Vector2 cameraPosition = Camera.Instance.Position;
                Vector2 cameraSize = Camera.Instance.Size;


                //Vector2 oldScreenPosition = oldPosition - cameraPosition;
                //Vector2 newScreenPosition = newPosition - cameraPosition;
                if (!(oldPosition + spriteToDraw.Size < 0) && !(oldPosition > cameraSize))
                {
                    if (oldPosition != null)
                        Draw(spriteToDraw, oldPosition, true);

                }

                if (newPosition == oldPosition) return;
                if (spriteToDraw == null && newPosition == null) return;
                if (newPosition + spriteToDraw.Size < 0) return;
                if (!(newPosition < cameraSize)) return;

                Draw(spriteToDraw, newPosition);
            }

            private void Draw(Sprite sprite, Vector2 position, bool erase = false)
            {
                Vector2 cameraPosition = Camera.Instance.Position;
                Vector2 cameraSize = Camera.Instance.Size;

                for (int y = 0; y < sprite.Size.Y; y++)
                {
                    for (int x = 0; x < sprite.Size.X; x++)
                    {
                        if ((position.X + x) < 0 || (position.Y + y) < 0) continue;
                        if ((position.X + x) >= cameraSize.X || (position.Y + y) >= cameraSize.Y) continue;
                        if (sprite[x, y].Char != ' ')
                        {
                            if (Console.CursorLeft != position.X + x || Console.CursorTop != position.Y + y)
                                Console.SetCursorPosition(position.X + x, position.Y + y);

                            if (erase)
                            {
                                //Console.Write(' ');
                                Render.Sprite[position.X + x, position.Y + y] = new Sprite.Tile(' ');
                            }


                            else
                            {
                                Render.Sprite[position.X + x, position.Y + y] = sprite[x, y];
                            }
                            /*if (sprite[x, y].Color != ConsoleColor.Gray)
                                Console.ForegroundColor = sprite[x, y].Color;
                            Console.Write(sprite[x, y].Char);
                            Console.ForegroundColor = ConsoleColor.Gray;*/


                        }
                    }
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
                if ((time.Ticks - updateTicks) / 10000.0 >= 15.0)
                {
                    // Handles the update of the ticks and time data for the game systems.
                    DeltaTime = (time.Ticks - updateTicks) / 10000.0;




                    updateTicks = time.Ticks;

                    //Propagate the Update pulse among the registered objects
                    foreach (object obj in Registry)
                    {
                        UpdateComponents(obj as IUpdate);
                    }
                    if (Core.RenderMode == 2)
                        ScreenRenderer.Instance.Update();
                    else if (Core.RenderMode <= 1)
                        CurrentDisplay.Update();

                    Camera.Instance.Update();
                    if (Core.RenderMode == 2)
                    {
                        CurrentDisplay.Update();
                    }


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
