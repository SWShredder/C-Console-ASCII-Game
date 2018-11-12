using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using static AsciiEngine.Utility;

namespace AsciiEngine
{
    public class Renderer : ISize
    {
        private static Renderer instance;
        public static Renderer Instance => instance;
        private Canvas cameraCanvas;

        public Canvas CameraCanvas => cameraCanvas;
        private Canvas renderArea;
        // Single Buffer
        private ScreenBuffer singleBuffer = new ScreenBuffer();
        // Dual Buffers
        private ScreenBuffer topBuffer = new ScreenBuffer();
        private ScreenBuffer bottomBuffer = new ScreenBuffer();
        // QUadrant Buffers
        private ScreenBuffer topLeftBuffer = new ScreenBuffer();
        private ScreenBuffer bottomLeftBuffer = new ScreenBuffer();
        private ScreenBuffer topRightBuffer = new ScreenBuffer();
        private ScreenBuffer bottomRightBuffer = new ScreenBuffer();
        public Vector2 Position
        {
            get
            {
                Vector2 offset = this.GetSize() - Camera.Instance.GetSize();
                offset = new Vector2(offset.X / 2, offset.Y / 2);
                return Camera.Instance.Position - offset;
            }
        }

        public Vector2 GetSize()
        {
            return GetRenderAreaSize();
        }

        public Canvas GetRenderArea()
        {
            return renderArea;
        }

        public void Update()
        {
            UpdateRenderArea();
            UpdateScreenBufferAreas(Core.Engine.RenderType);
        }

        private void UpdateScreenBufferAreas(int renderType)
        {
            if (renderType == 0)
                UpdateDualScreenBuffers();
            else if (renderType == 1)
                UpdateQuadrantScreenBuffers();
            else
                UpdateSingleScreenBuffer();

        }

        private void UpdateSingleScreenBuffer()
        {
            Vector2 cameraRelativePosition = Core.Engine.Camera.GetPosition() - this.Position;
            Vector2 bufferSize = Core.Engine.Camera.GetSize();

            singleBuffer.SetPosition(new Vector2());
            Canvas renderArea = GetRenderArea();

            singleBuffer.RenderCanvasToBuffer(renderArea, bufferSize, cameraRelativePosition);

        }

        private async void UpdateDualScreenBuffers()
        {
            Vector2 cameraRelativePosition = Core.Engine.Camera.GetPosition() - this.Position;
            Vector2 topBufferSize = new Vector2(Core.Engine.Camera.GetSize().X, Core.Engine.Camera.GetSize().Y / 2);
            Vector2 bottomBufferSize = new Vector2(Core.Engine.Camera.GetSize().X, Core.Engine.Camera.GetSize().Y - Core.Engine.Camera.GetSize().Y / 2);
            Vector2 bottomBufferRelativePosition = new Vector2(cameraRelativePosition.X, cameraRelativePosition.Y + (Core.Engine.Camera.GetSize().Y - Core.Engine.Camera.GetSize().Y / 2));

            topBuffer.SetPosition(new Vector2());
            bottomBuffer.SetPosition(new Vector2(0, Core.Engine.Camera.GetSize().Y - Core.Engine.Camera.GetSize().Y / 2));//Core.Engine.Camera.GetSize().Y - Core.Engine.Camera.GetSize().Y / 2));
            Canvas renderArea = GetRenderArea();
            //cameraCanvas = RenderCameraArea();
            var renderTopBuffer = Task.Run(() => topBuffer.RenderCanvasToBuffer(renderArea, topBufferSize, cameraRelativePosition));
            var renderBottomBuffer = Task.Run(
                () => bottomBuffer.RenderCanvasToBuffer(renderArea, bottomBufferSize, bottomBufferRelativePosition));

            await Task.WhenAll(renderTopBuffer, renderBottomBuffer);
        }

        private async void UpdateQuadrantScreenBuffers()
        {
            Vector2 cameraRelativePosition = Core.Engine.Camera.GetPosition() - this.Position;
            Vector2 topBufferSize = new Vector2(Core.Engine.Camera.GetSize().X / 2, Core.Engine.Camera.GetSize().Y / 2);
            Vector2 bottomBufferSize = new Vector2(Core.Engine.Camera.GetSize().X - Core.Engine.Camera.GetSize().X / 2, Core.Engine.Camera.GetSize().Y - Core.Engine.Camera.GetSize().Y / 2);

            Vector2 topLeftBufferRelativePosition = new Vector2(cameraRelativePosition.X, cameraRelativePosition.Y);
            Vector2 topRightBufferRelativePosition = new Vector2(cameraRelativePosition.X + topBufferSize.X, cameraRelativePosition.Y);
            Vector2 bottomRightBufferRelativePosition = new Vector2(cameraRelativePosition.X + bottomBufferSize.X, cameraRelativePosition.Y + bottomBufferSize.Y);
            Vector2 bottomLeftBufferRelativePosition = new Vector2(cameraRelativePosition.X, cameraRelativePosition.Y + bottomBufferSize.Y);

            Canvas renderArea = GetRenderArea();

            topLeftBuffer.SetPosition(new Vector2());
            topRightBuffer.SetPosition(new Vector2(topBufferSize.X, 0));
            bottomLeftBuffer.SetPosition(new Vector2(0, bottomBufferSize.Y));
            bottomRightBuffer.SetPosition(new Vector2(bottomBufferSize.X, bottomBufferSize.Y));


            var topLeftBufferTask = Task.Run(() => topLeftBuffer.RenderCanvasToBuffer(renderArea, topBufferSize, topLeftBufferRelativePosition));
            var topRightBufferTask = Task.Run(() => topRightBuffer.RenderCanvasToBuffer(renderArea, topBufferSize, topRightBufferRelativePosition));
            var bottomLeftBufferTask = Task.Run(() => bottomLeftBuffer.RenderCanvasToBuffer(renderArea, bottomBufferSize, bottomLeftBufferRelativePosition));
            var bottomRightBufferTask = Task.Run(() => bottomRightBuffer.RenderCanvasToBuffer(renderArea, bottomBufferSize, bottomRightBufferRelativePosition));

            await Task.WhenAll(topLeftBufferTask, topRightBufferTask, bottomLeftBufferTask, bottomRightBufferTask);
        }

        public Renderer()
        {
            instance = this;
        }

        public ScreenBuffer GetSingleScreenBuffer()
        {
            return singleBuffer;
        }

        public ScreenBuffer GetQuadrantScreenBuffer(int index)
        {
            if (index == 0)
                return topLeftBuffer;
            else if (index == 1)
                return bottomLeftBuffer;
            else if (index == 2)
                return topRightBuffer;
            else
                return bottomRightBuffer;

        }
        public ScreenBuffer GetTopScreenBuffer()
        {
            return topBuffer;
        }
        public ScreenBuffer GetBottomScreenBuffer()
        {
            return bottomBuffer;
        }

        private void UpdateRenderArea()
        {
            Canvas newRenderArea = new Canvas(this.GetSize());
            Vector2 RenderingPosition = this.Position;
            Vector2 RenderingSize = this.GetSize();

            foreach (GameObject obj in GameObject.List)
            {
                if (obj == null) continue;
                Vector2 objSize = (obj as ISize).GetSize();
                Vector2 objPosition = (obj as IPosition).GetPosition();
                Vector2 objRelativePosition = objPosition - this.Position;
                Sprite objSprite = (obj as IGraphics).GetFrame();

                if (objRelativePosition > 0 && objRelativePosition < RenderingSize)
                {

                    for (int x2 = 0, x = objRelativePosition.X; x < objSize.X + objRelativePosition.X; ++x, ++x2)
                    {
                        for (int y2 = 0, y = objRelativePosition.Y; y < objSize.Y + objRelativePosition.Y; ++y, ++y2)
                        {
                            if (x >= RenderingSize.X || y >= RenderingSize.Y)
                                continue;

                            if (objSprite[x2, y2].Char != ' ')
                            {
                                newRenderArea[x, y] = objSprite[x2, y2];
                            }

                        }
                    }
                }
            }
            renderArea = newRenderArea;

        }

        private Vector2 GetRenderAreaSize()
        {
            Vector2 cameraSize = Core.Engine.Camera.GetSize();
            Vector2 newSize = new Vector2((int)Math.Round(cameraSize.X * 1.3), (int)Math.Round(cameraSize.Y * 1.3));
            return newSize;
        }

        private Canvas RenderCameraArea()
        {
            Vector2 cameraPosition = Camera.Instance.Position;
            Vector2 cameraSize = Camera.Instance.GetSize();
            Vector2 cameraRelativePosition = cameraPosition - this.Position;

            Canvas newCanvas = new Canvas(cameraSize);
            for (int x = 0; x < cameraSize.X; ++x)
            {
                for (int y = 0; y < cameraSize.Y; ++y)
                {
                    newCanvas[x, y] = renderArea[cameraRelativePosition.X + x, cameraRelativePosition.Y + y];
                }
            }
            return newCanvas;
        }


    }
}
