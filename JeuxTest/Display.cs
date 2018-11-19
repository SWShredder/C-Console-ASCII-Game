using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsciiEngine
{
    public class Display
    {
        private List<Task> threadList = new List<Task>();


        public async void Update()
        {
            long ticks = Core.Engine.CoreUpdate.EngineTicks;
            UpdateDisplayManager(Core.Engine.RenderType);
            if(Core.Engine.isDisplayParallel)
                await Task.WhenAll(threadList.ToArray());
            else
                Task.WaitAll(threadList.ToArray());
            Core.Engine.CoreUpdate.DeltaTimeDisplay = 1.0 * (Core.Engine.CoreUpdate.EngineTicks - ticks) / Core.Engine.CoreUpdate.TicksResolution * 1000.0;
        }

        public void UpdateDisplayManager(int renderType)
        {
            if (renderType == 0)
                DualBufferDisplay();
            else if (renderType == 1)
                QuadrantBufferDisplay();
            else
                SingleBufferDisplay();

        }

        private void SingleBufferDisplay()
        {
            List<Task> newThreadList = new List<Task>();
            var render = Task.Run(() => Core.Engine.Renderer.GetSingleScreenBuffer().Draw());
            newThreadList.Add(render);
            this.threadList = newThreadList;
        }

        private void DualBufferDisplay()
        {
            List<Task> newThreadList = new List<Task>();

            var renderTop = Task.Run(() => Core.Engine.Renderer.GetTopScreenBuffer().Draw());
            var renderBottom = Task.Run(() => Core.Engine.Renderer.GetBottomScreenBuffer().Draw());

            newThreadList.Add(renderTop);
            newThreadList.Add(renderBottom);

            this.threadList = newThreadList;
        }

        private void QuadrantBufferDisplay()
        {
            List<Task> newThreadList = new List<Task>();

            var renderLeftTop = Task.Run(() => Core.Engine.Renderer.GetQuadrantScreenBuffer(0).Draw());
            var renderLeftBottom = Task.Run(() => Core.Engine.Renderer.GetQuadrantScreenBuffer(1).Draw());
            var renderRightTop = Task.Run(() => Core.Engine.Renderer.GetQuadrantScreenBuffer(2).Draw());
            var renderRightBottom = Task.Run(() => Core.Engine.Renderer.GetQuadrantScreenBuffer(3).Draw());

            newThreadList.Add(renderLeftTop);
            newThreadList.Add(renderLeftBottom);
            newThreadList.Add(renderRightTop);
            newThreadList.Add(renderRightBottom);

            this.threadList = newThreadList;
        }


    }

}
