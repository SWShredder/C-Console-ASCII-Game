using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsciiEngine
{
    public class PhysicsSpace : ISize
    {
        Dictionary<Vector2, GameObject> PhysicsBodyDictionary;
        public bool[,] CollisionMap;
        public Vector2 Position
        {
            get
            {
                Vector2 offset = this.GetSize() - Camera.Instance.GetSize();
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

        public Vector2 GetSize()
        {
            return Core.Engine.RenderingSize;
        }
        public void Update()
        {
            CollisionMap = RenderCollisionMap(out PhysicsBodyDictionary);
        }


        public bool CheckCollision(GameObject source, Vector2 targetPosition)
        {
            CollisionShape collisionBody = (source as ICollision).GetCollisionShape();
            Vector2 relativeTargetPosition = targetPosition - this.Position;

            for(int x = 0; x< collisionBody.Size.X; ++x)
            {
                for(int y = 0; y < collisionBody.Size.Y; ++y)
                {
                    if(collisionBody[x, y])
                    {
                        if (CollisionMap == null) return false;
                        if(CollisionMap[x + relativeTargetPosition.X, y + relativeTargetPosition.Y])
                        {
                            Vector2 relativePositionAdjusted = new Vector2(x + targetPosition.X, y + targetPosition.Y);
                            PhysicsBodyDictionary.TryGetValue(relativePositionAdjusted, out GameObject targetObject);
                            if (targetObject != source)
                                return true;
                        }

                    }
                }
            }
            return false;
        }

        private bool[,] RenderCollisionMap(out Dictionary<Vector2, GameObject> dict)
        {
            dict = new Dictionary<Vector2, GameObject>();
            bool[,] newCollisionMap = new bool[this.Size.X, this.Size.Y];
            Vector2 RenderingPosition = this.Position;
            Vector2 RenderingSize = this.Size;

            foreach (GameObject obj in GameObject.List)
            {
                Vector2 objPosition = (obj as IPosition).GetPosition();
                Vector2 objSize = (obj as ISize).GetSize();

                if (objPosition == null || objSize == null)
                    continue;

                Vector2 objRelativePosition = objPosition - RenderingPosition;
                bool[,] objCollision = (obj as ICollision).GetCollisionPoints();

                if (objRelativePosition > 0 && objRelativePosition < RenderingSize)
                {

                    for(int x2 = 0, x = objRelativePosition.X; x < objSize.X + objRelativePosition.X; ++x, ++x2)
                    {
                        for(int y2 = 0, y = objRelativePosition.Y; y < objSize.Y + objRelativePosition.Y; ++y, ++y2)
                        {
                            if (x >= RenderingSize.X || y >= RenderingSize.Y)
                                continue;

                            newCollisionMap[x, y] = objCollision[x2, y2];

                            if(objCollision[x2, y2] != false && !dict.ContainsKey(new Vector2(objPosition.X + x2, objPosition.Y +y2)))
                                dict.Add(new Vector2(objPosition.X + x2, objPosition.Y + y2), obj);
                        }
                    }
                }

            }
            return newCollisionMap;
        }
    }
}
