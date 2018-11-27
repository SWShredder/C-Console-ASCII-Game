using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AsciiEngine.Utility;

namespace AsciiEngine
{

    public class ObjectGraphics : INodes
    {
        private byte[,] parentByteMapArray;
        private byte[,] cacheByteMapArray;

        public List<Animation> CurrentAnimations { set; get; }
        public bool IsVisible { set; get; }
        public INodes Parent { set; get; }
        public List<INodes> Children { set; get; }
        public byte[,] ByteMap
        {
            get
            {
                return cacheByteMapArray;
            }
        }

        public ObjectGraphics(INodes parent)
        {
            Parent = parent;
            this.parentByteMapArray = (Parent as GameObject).Body;
            this.cacheByteMapArray = this.parentByteMapArray;
            this.CurrentAnimations = new List<Animation>();
            this.IsVisible = true;
        }

        public void UpdateCache()
        {
            var parent = Parent as GameObject;
            int rotation = 0;
            if (parent.FacingDirection == Direction.East) rotation = 90;
            if (parent.FacingDirection == Direction.South) rotation = 180;
            if (parent.FacingDirection == Direction.West) rotation = 270;
            ApplyRotation(rotation);
        }

        public void ApplyRotation(int degree)
        {
            var array = parentByteMapArray;
            switch (degree)
            {
                case 90:
                    Utility.GetRotated90DegreeMatrix<byte>(parentByteMapArray, out cacheByteMapArray);
                    break;
                case 180:
                    Utility.GetRotated90DegreeMatrix<byte>(parentByteMapArray, out cacheByteMapArray);
                    array = cacheByteMapArray;
                    Utility.GetRotated90DegreeMatrix<byte>(cacheByteMapArray, out cacheByteMapArray);
                    break;
                case 270:
                    Utility.GetRotated90DegreeMatrix<byte>(parentByteMapArray, out cacheByteMapArray);
                    array = cacheByteMapArray;
                    Utility.GetRotated90DegreeMatrix<byte>(cacheByteMapArray, out cacheByteMapArray);
                    array = cacheByteMapArray;
                    Utility.GetRotated90DegreeMatrix<byte>(cacheByteMapArray, out cacheByteMapArray);
                    break;
                default:
                    cacheByteMapArray = parentByteMapArray;
                    break;
            }
        }

        public void Update()
        {
            ProcessAnimations();   
            if (IsVisible)
            {
                Engine.Instance.Renderer.AddRenderUpdateQuery(new RenderUpdateSignal()
                {
                    ByteMap = this.ByteMap,
                    Position = (Parent as GameObject).Position
                });
            }

        }

        public void AddChild(INodes child)
        {
            Children.Add(child);
        }

        public void RemoveChild(INodes child)
        {
            Children.Remove(child);
        }

        public void AddAnimation(Animation animation)
        {
            CurrentAnimations.Add(animation);
        }

        public void ProcessAnimations()
        {
            for(int i = 0; i < CurrentAnimations.Count; ++i)
            {
                CurrentAnimations[i].Update();
                if (CurrentAnimations[i].IsFinished)
                {
                    CurrentAnimations.RemoveAt(i);
                    --i;
                }
            }

        }
    }
}