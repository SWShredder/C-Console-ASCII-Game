using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using static AsciiEngine.Utility;

namespace AsciiEngine
{
    public class CoreRenderer : ISize, INodes
    {
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

        public byte[,] ByteMap { set; get; }
        public INodes Parent { set; get; }
        public List<INodes> Children { set; get; }
        public List<RenderUpdateSignal> RenderUpdateQueries;
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
                return GetRenderAreaSize();
            }
        }
  
        public CoreRenderer(INodes parent)
        {
            Parent = parent;
            Parent.AddChild(this);
            Children = new List<INodes>();
            RenderUpdateQueries = new List<RenderUpdateSignal>();
        }

        public void Update()
        {
            //UpdateByteMapArray();
            ProcessRenderUpdateQueries();
            UpdateScreenBufferAreas(Engine.RenderType);
        }

        public void UpdateScreenBufferAreas(int renderType)
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
            Vector2 cameraRelativePosition = Engine.Instance.Camera.GetPosition() - this.Position;
            Vector2 bufferSize = Engine.Instance.Camera.Size;
            singleBuffer.Position = new Vector2();
            singleBuffer.DrawByteMapToBuffer(ByteMap, bufferSize, cameraRelativePosition);
        }
        private void UpdateDualScreenBuffers()
        {
            Vector2 cameraRelativePosition = Engine.Instance.Camera.GetPosition() - this.Position;
            Vector2 topBufferSize = new Vector2(Engine.Instance.Camera.Size.X, Engine.Instance.Camera.Size.Y / 2);
            Vector2 bottomBufferSize = new Vector2(Engine.Instance.Camera.Size.X, Engine.Instance.Camera.Size.Y / 2);
            Vector2 bottomBufferRelativePosition = new Vector2(cameraRelativePosition.X, cameraRelativePosition.Y + (Engine.Instance.Camera.Size.Y / 2));

            topBuffer.Position = new Vector2();
            bottomBuffer.Position = (new Vector2(0, Engine.Instance.Camera.Size.Y / 2));//Engine.Instance.Camera.Size.Y - Engine.Instance.Camera.Size.Y / 2));
            //cameraCanvas = RenderCameraArea();
            var renderTopBuffer = Task.Run(() => topBuffer.DrawByteMapToBuffer(ByteMap, topBufferSize, cameraRelativePosition));
            var renderBottomBuffer = Task.Run(
                () => bottomBuffer.DrawByteMapToBuffer(ByteMap, bottomBufferSize, bottomBufferRelativePosition));

            Task.WaitAll(renderTopBuffer, renderBottomBuffer);
        }

        private void UpdateQuadrantScreenBuffers()
        {
            Vector2 cameraRelativePosition = Engine.Instance.Camera.GetPosition() - this.Position;
            Vector2 topBufferSize = new Vector2(Engine.Instance.Camera.Size.X / 2, Engine.Instance.Camera.Size.Y / 2);
            Vector2 bottomBufferSize = new Vector2(Engine.Instance.Camera.Size.X / 2, Engine.Instance.Camera.Size.Y / 2);

            Vector2 topLeftBufferRelativePosition = new Vector2(cameraRelativePosition.X, cameraRelativePosition.Y);
            Vector2 topRightBufferRelativePosition = new Vector2(cameraRelativePosition.X + topBufferSize.X, cameraRelativePosition.Y);
            Vector2 bottomRightBufferRelativePosition = new Vector2(cameraRelativePosition.X + bottomBufferSize.X, cameraRelativePosition.Y + bottomBufferSize.Y);
            Vector2 bottomLeftBufferRelativePosition = new Vector2(cameraRelativePosition.X, cameraRelativePosition.Y + bottomBufferSize.Y);

            topLeftBuffer.Position = new Vector2() ;
            topRightBuffer.Position = (new Vector2(topBufferSize.X, 0));
            bottomLeftBuffer.Position = (new Vector2(0, bottomBufferSize.Y));
            bottomRightBuffer.Position = (new Vector2(bottomBufferSize.X, bottomBufferSize.Y));


            var topLeftBufferTask = Task.Run(() => topLeftBuffer.DrawByteMapToBuffer(ByteMap, topBufferSize, topLeftBufferRelativePosition));
            var topRightBufferTask = Task.Run(() => topRightBuffer.DrawByteMapToBuffer(ByteMap, topBufferSize, topRightBufferRelativePosition));
            var bottomLeftBufferTask = Task.Run(() => bottomLeftBuffer.DrawByteMapToBuffer(ByteMap, bottomBufferSize, bottomLeftBufferRelativePosition));
            var bottomRightBufferTask = Task.Run(() => bottomRightBuffer.DrawByteMapToBuffer(ByteMap, bottomBufferSize, bottomRightBufferRelativePosition));

            Task.WaitAll(topLeftBufferTask, topRightBufferTask, bottomLeftBufferTask, bottomRightBufferTask);
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

        private void UpdateByteMapArray()
        {
            byte[,] newByteArray = new byte[Size.X, Size.Y];
            Vector2 RenderingPosition = this.Position;
            Vector2 RenderingSize = this.Size;

            foreach (GameObject obj in GameObject.List)
            {
                if (obj == null) continue;
                Vector2 objSize = obj.Size;
                Vector2 objPosition = obj.Position;
                Vector2 objRelativePosition = objPosition - this.Position;
                byte[,] spriteByteMap = obj.Graphics.ByteMap;

                if (objRelativePosition > 0 && objRelativePosition < RenderingSize)
                {
                    for (int x2 = 0, x = objRelativePosition.X; x < objSize.X + objRelativePosition.X; ++x, ++x2)
                    {
                        for (int y2 = 0, y = objRelativePosition.Y; y < objSize.Y + objRelativePosition.Y; ++y, ++y2)
                        {
                            if (x >= RenderingSize.X || y >= RenderingSize.Y)
                                continue;

                            if (spriteByteMap[x2, y2] != (byte)0x0)
                            {
                                newByteArray[x, y] = spriteByteMap[x2, y2];
                            }

                        }
                    }
                }
            }
            ByteMap = newByteArray;
        }

        private void ProcessRenderUpdateQueries()
        {
            ByteMap = new byte[Size.X, Size.Y];
            Vector2 RenderingPosition = this.Position;
            Vector2 RenderingSize = this.Size;
            int renderingSizeX = RenderingSize.X;
            int renderingSizeY = RenderingSize.Y;


            foreach (RenderUpdateSignal query in RenderUpdateQueries)
            {
                byte[,] sourceByteMap = query.ByteMap;
                Vector2 sourceSize = new Vector2(sourceByteMap.GetLength(0), sourceByteMap.GetLength(1));
                Vector2 sourceRelativePosition = query.Position - RenderingPosition;

                int sourceSizeX = sourceSize.X;
                int sourceSizeY = sourceSize.Y;
                int sourceRelativePositionX = sourceRelativePosition.X;
                int sourceRelativePositionY = sourceRelativePosition.Y;

                if (sourceRelativePosition > 0 && sourceRelativePosition < RenderingSize)
                {
                    for (int x2 = 0, x = sourceRelativePositionX; x < sourceSizeX + sourceRelativePositionX; ++x, ++x2)
                    {
                        for (int y2 = 0, y = sourceRelativePositionY; y < sourceSizeY + sourceRelativePositionY; ++y, ++y2)
                        {
                            if (x >= renderingSizeX || y >= renderingSizeY)
                                continue;

                            if (sourceByteMap[x2, y2] != (byte)0x0)
                            {
                                ByteMap[x, y] = sourceByteMap[x2, y2];
                            }

                        }
                    }
                }
            }
            RenderUpdateQueries = new List<RenderUpdateSignal>();
        }

        public void AddRenderUpdateQuery(RenderUpdateSignal query)
        {
            RenderUpdateQueries.Add(query);
        }

        private Vector2 GetRenderAreaSize()
        {
            Vector2 cameraSize = Engine.Instance.Camera.Size;//Engine.Instance.Camera.Size;
            Vector2 newSize = new Vector2((cameraSize.X * 2), (cameraSize.Y * 2));
            return newSize;
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
