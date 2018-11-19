using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsciiEngine
{
    public class CollisionBody : ISize, INodes
    {
        // FIELDS //

        private bool[,] parentCollisionMap;

        public bool IsTrigger { get; }
        public INodes Parent { set;  get; }
        public List<INodes> Children { get; }
        public bool this[int x, int y] => CollisionMap[x, y];
        public Vector2 Size => new Vector2(parentCollisionMap.GetLength(0), parentCollisionMap.GetLength(1));
        public bool[,] CollisionMap
        {
            get => parentCollisionMap;
            set => parentCollisionMap = value;
        }

        public CollisionBody(INodes obj)
        {
            Parent = obj;
            Parent.AddChild(this);
            var parent = obj as GameObject;
            parentCollisionMap = ByteMapToCollisionMap(parent.Body);
            IsTrigger = parent.IsTrigger;
        }

        public void Update()
        {
            //CollisionMap = ByteMapToCollisionMap((Parent as GameObject).Graphics.ByteMap);
            Core.Engine.PhysicsSpace.AddCollisionUpdateQuery(new CollisionUpdateSignal()
            {
                Source = Parent,
                Position = (Parent as GameObject).Position,
                CollisionMap = this.CollisionMap
            });
        }

        public void UpdateCollisionMap()
        {
            CollisionMap = ByteMapToCollisionMap((Parent as GameObject).Graphics.ByteMap);
        }



        public bool CollisionDetection(Vector2 direction, out CollisionEventSignal signal)
        {
            return Core.Engine.PhysicsSpace.CheckCollision(Parent as GameObject, (Parent as IPosition).Position + direction, out signal);
        }

        private bool[,] SpriteToCollisionMap(Sprite sprite)
        {
            bool[,] points = new bool[sprite.Size.X, sprite.Size.Y];
            for (int y = 0; y < sprite.Size.Y; y++)
            {
                for (int x = 0; x < sprite.Size.X; x++)
                {
                    if (sprite[x, y] != 0)
                        points[x, y] = true;
                    else
                        points[x, y] = false;
                }
            }
            return points;
        }

        private bool[,] ByteMapToCollisionMap(byte[,] newMatrix)
        {
            bool[,] points = new bool[newMatrix.GetLength(0), newMatrix.GetLength(1)];
            for (int y = 0; y < newMatrix.GetLength(1); y++)
            {
                for (int x = 0; x < newMatrix.GetLength(0); x++)
                {
                    if (newMatrix[x, y] != 0)
                        points[x, y] = true;
                    else
                        points[x, y] = false;
                }
            }
            return points;
        }
        public void AddChild(INodes child)
        {
            Children.Add(child);
        }

        public void RemoveChild(INodes child)
        {
            Children.Remove(child);
        }



        public CollisionBody(Sprite graphic)
        {
            parentCollisionMap = SpriteToCollisionMap(graphic);
        }

        public CollisionBody(byte[,] byteMap)
        {
            parentCollisionMap = ByteMapToCollisionMap(byteMap);
        }
    }
}