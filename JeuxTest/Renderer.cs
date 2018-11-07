using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AsciiEngine.Utility;

namespace AsciiEngine
{
    public class Renderer
    {
        private static Renderer instance;
        public static Renderer Instance => instance;

        public Frame Frame => RenderFrame(Core.Map, Camera.Instance);
        public ScreenBuffer ScreenBuffer
        {
            get
            {
                return new ScreenBuffer() { Size = Camera.Instance.Size, Sprite = Frame.Sprite };
            }
        }

        public void Update()
        {

        }

        public Renderer()
        {
            instance = this;
        }

        private Frame RenderFrame(Physics.Space space, Camera camera)
        {
            Frame newFrame = new Frame() { Size = camera.Size };
            Vector2 index = new Vector2(0, 0);

            for (int y = camera.Position.Y; y < camera.Position.Y + camera.Size.Y; y++)
            {
                index.X = 0;
                for (int x = camera.Position.X; x < camera.Position.X + camera.Size.X; x++)
                {
                    if (space[x, y] != null)
                    {
                        GameObject gameObject = space[x, y];
                        Sprite objectSprite = gameObject.SpriteGraphics;
                        Vector2 relativePosition = new Vector2(x, y) - gameObject.Position;

                        if (relativePosition.X < objectSprite.Size.X
                            && relativePosition.Y < objectSprite.Size.Y)
                        {
                            newFrame.Sprite[index.X, index.Y] = objectSprite[relativePosition.X, relativePosition.Y];
                        }                      
                    }
                    index.X++;
                }
                index.Y++;
            }
            return newFrame;
        }
    }
}
