using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AsciiEngine.Utility;

namespace AsciiEngine
{
    public abstract class Animation
    {
        public string Name { set;  get; }
        public double ElapsedTime { set; get; }
        public GameObject Target { get; }
        public bool IsPlaying { set;  get; }
        public bool IsFinished { set; get; }
        public double Length { set;  get; }

        public virtual void Update()
        {
            ElapsedTime += IsPlaying ? GetDeltaTime() : 0;
            if (ElapsedTime >= Length) OnFinish();
        }

        public virtual void Start()
        {
            IsPlaying = true;
        }

        public Animation(GameObject target)
        {
            Target = target;
            ElapsedTime = 0;
            Start();

        }

        public virtual void OnFinish()
        {
            IsPlaying = false;
            IsFinished = true;
        }

    }

    public class AnimationOnHit : Animation
    {

        public override void Start()
        {
            base.Start();
            Target.Graphics.IsVisible = false;
        }

        public AnimationOnHit(GameObject target) : base(target)
        {
            Name = "On Hit";
            Length = 20;
        }

        public override void Update()
        {
            base.Update();
        }

        public override void OnFinish()
        {
            Target.Graphics.IsVisible = true;
            base.OnFinish();
        }
    }

}
