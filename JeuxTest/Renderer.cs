using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AsciiEngine.Utility;

namespace AsciiEngine
{
    public class Renderer : ISize
    {
        private static Renderer instance;
        public static Renderer Instance => instance;

        public Canvas CameraCanvas => RenderCameraArea();
        private Canvas renderArea;
        public Canvas Canvas => RenderCanvas(Core.Map, Camera.Instance);
        public Vector2 Position
        {
            get
            {
                Vector2 offset = this.Size - Camera.Instance.Size;
                offset = new Vector2(offset.X / 2, offset.Y / 2);
                return Camera.Instance.Position - offset;
            }
        }
        public Vector2 Size
        {
            get
            {
                return Core.Engine.RenderingSize;
            }
        }
        public ScreenBuffer ScreenBuffer
        {
            get
            {
                return new ScreenBuffer() { Size = Camera.Instance.Size, Canvas = this.CameraCanvas };
            }
        }

        public void Update()
        {
            renderArea = GetRenderArea();
        }

        public Renderer()
        {
            instance = this;
        }

        private Canvas GetRenderArea()
        {
            Canvas newRenderArea = new Canvas(this.Size);
            Vector2 RenderingPosition = this.Position;
            Vector2 RenderingSize = this.Size;

            foreach(GameObject obj in GameObject.List)
            {
                if (obj == null) continue;
                Vector2 objSize = obj.Size;
                Vector2 objPosition = obj.Position;
                Vector2 objRelativePosition = objPosition - this.Position;
                Sprite objSprite = obj.Graphics.CurrentSprite;

                if (objRelativePosition > 0 && objRelativePosition < RenderingSize)
                {

                    for (int x2 = 0, x = objRelativePosition.X; x < objSize.X + objRelativePosition.X; ++x, ++x2)
                    {
                        for (int y2 = 0, y = objRelativePosition.Y; y < objSize.Y + objRelativePosition.Y; ++y, ++y2)
                        {
                            if (x >= RenderingSize.X || y >= RenderingSize.Y)
                                continue;

                            if(objSprite[x2, y2].Char != ' ')
                            {
                                newRenderArea[x, y] = objSprite[x2, y2];
                            }

                        }
                    }
                }
            }
            return newRenderArea;

        }

        private Canvas RenderCameraArea()
        {
            Vector2 cameraPosition = Camera.Instance.Position;
            Vector2 cameraSize = Camera.Instance.Size;
            Vector2 cameraRelativePosition = cameraPosition - this.Position;

            Canvas newCanvas = new Canvas(cameraSize);
            for(int x = 0; x < cameraSize.X; ++x)
            {
                for(int y = 0; y < cameraSize.Y; ++y)
                {
                    newCanvas[x, y] = renderArea[cameraRelativePosition.X + x, cameraRelativePosition.Y + y];
                }
            }
            return newCanvas;
        }


        private Canvas RenderCanvas(Physics.Space space, Camera camera)
        {
            Canvas newCanvas = new Canvas(camera.Size);
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
                            newCanvas[index.X, index.Y] = objectSprite[relativePosition.X, relativePosition.Y];
                        }                      
                    }
                    index.X++;
                }
                index.Y++;
            }
            return newCanvas;
        }
    }
}
