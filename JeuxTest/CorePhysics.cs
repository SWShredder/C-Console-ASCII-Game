using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsciiEngine
{
    public class CorePhysics : ISize, INodes
    {
        private Dictionary<Vector2, GameObject> PhysicsBodyDictionary;
        public INodes Parent { set; get; }
        public List<INodes> Children { set; get; }
        public bool[,] CollisionMap { private set; get; }
        public List<CollisionUpdateSignal> CollisionUpdateQueries { set; get; }
        public Vector2 Position
        {
            get
            {
                Vector2 offset = Size - Camera.Instance.Size;
                offset = new Vector2(offset.X / 2, offset.Y / 2);
                return Camera.Instance.Position - offset;
            }
        }
        public Vector2 Size
        {
            get
            {
                return new Vector2((int)(Engine.Instance.RenderingSize.X), (int)(Engine.Instance.RenderingSize.Y));
            }
        }

        public CorePhysics(INodes parent)
        {
            Parent = parent;
            Parent.AddChild(this);
            Children = new List<INodes>();
            CollisionUpdateQueries = new List<CollisionUpdateSignal>();
        }

        public void Update()
        {
            CollisionMap = ProcessCollisionUpdateQueries();
        }


        public bool CheckCollision(GameObject source, Vector2 targetPosition)
        {
            if (CollisionMap == null) return false;
            Vector2 thisPosition = this.Position;
            Vector2 eventTargetPosition = targetPosition;
            ObjectCollision collisionBody = source.CollisionBody;
            int relativeTargetPositionX = eventTargetPosition.X - thisPosition.X;
            int relativeTargetPositionY = eventTargetPosition.Y - thisPosition.Y;
            int collisionBodySizeX = collisionBody.Size.X;
            int collisionBodySizeY = collisionBody.Size.Y;

            for (int x = 0; x < collisionBodySizeX; ++x)
            {
                for (int y = 0; y < collisionBodySizeY; ++y)
                {
                    if (collisionBody[x, y])
                    {

                        if (x + relativeTargetPositionX >= CollisionMap.GetLength(0) || y + relativeTargetPositionY >= CollisionMap.GetLength(1)) return false;
                        if (x + relativeTargetPositionX < 0 || y + relativeTargetPositionY < 0) return false;
                        if (CollisionMap[x + relativeTargetPositionX, y + relativeTargetPositionY])
                        {
                            Vector2 collisionPoints = new Vector2(x + targetPosition.X, y + targetPosition.Y);
                            PhysicsBodyDictionary.TryGetValue(collisionPoints, out GameObject targetObject);
                            if (targetObject != source && targetObject != null)
                            {
                                if (source.IsTrigger || targetObject.IsTrigger)
                                {
                                    GenerateDamageSignal(source, targetObject);
                                    return false;
                                }                                  
                                GenerateCollisionEvents(source, targetObject, collisionPoints);
                                return true;
                            }

                        }

                    }
                }
            }
            return false;
        }

        private void GenerateCollisionEvents(GameObject source, GameObject target, Vector2 collisionPoints)
        {
            double sourceVelocityX = source.PhysicsBody.Velocity.X;
            double sourceVelocityY = source.PhysicsBody.Velocity.Y;
            double targetVelocityX = target.PhysicsBody.Velocity.X;
            double targetVelocityY = target.PhysicsBody.Velocity.Y;

            double collisionVectorX = targetVelocityX - sourceVelocityX;
            double collisionVectorY = targetVelocityY - sourceVelocityY;
            double force;

            if (collisionVectorX == 0 || collisionVectorY == 0)
                force = Math.Pow((collisionVectorX + collisionVectorY), 2) * 0.5 * source.PhysicsBody.Mass;
            else
                force = Math.Pow((Math.Abs(collisionVectorX) + Math.Abs(collisionVectorY)) / 2, 2) * 0.5 * source.PhysicsBody.Mass;

            GetNormalizedVector(ref collisionVectorX, ref collisionVectorY);


            double targetForceImpact = force / target.PhysicsBody.Mass / (force / target.PhysicsBody.Mass + 1) * force;
            double sourceForceImpact = force - targetForceImpact;


            source.AddObjectSignalQuery(new ObjectPhysicsSignal()
            {
                CollisionVectorX = collisionVectorX,
                CollisionVectorY = collisionVectorY,
                Force = sourceForceImpact
            });

            target.AddObjectSignalQuery(new ObjectPhysicsSignal()
            {
                CollisionVectorX = collisionVectorX * -1,
                CollisionVectorY = collisionVectorY * -1,
                Force = targetForceImpact
            });

        }

        private void GenerateDamageSignal(GameObject source, GameObject target)
        {
            if (source.IsTrigger && target != source.ObjectParent)
            {
                target.AddObjectSignalQuery(new ObjectDamageSignal()
                {
                    Damage = source.GameStats.Damage,
                    source = source
                });
                source.AddObjectSignalQuery(new ObjectDamageSignal()
                {
                    Damage = 99999,
                    source = source
                });
            }
        }

        private void GetNormalizedVector(ref double x, ref double y)
        {
            if (x > 0) x = 0.5;
            else if (x < 0) x = -0.5;
            else x = 0;

            if (y > 0) y = 0.5;
            else if (y < 0) y = -0.5;
            else y = 0;
        }


        private bool[,] RenderCollisionMap(out Dictionary<Vector2, GameObject> dict)
        {
            dict = new Dictionary<Vector2, GameObject>();
            bool[,] newCollisionMap = new bool[this.Size.X, this.Size.Y];
            Vector2 RenderingPosition = this.Position;
            Vector2 RenderingSize = this.Size;

            int renderingPositionX = RenderingPosition.X;
            int renderingPositionY = RenderingPosition.Y;
            int renderingSizeX = RenderingSize.X;
            int renderingSizeY = RenderingSize.Y;

            foreach (GameObject obj in GameObject.List)
            {
                Vector2 objPosition = obj.Position;
                Vector2 objSize = obj.Size;

                if (objPosition == null || objSize == null) continue;

                Vector2 objRelativePosition = objPosition - RenderingPosition;
                bool[,] objCollision = obj.CollisionBody.CollisionMap;

                if (!(objRelativePosition > 0 && objRelativePosition < RenderingSize)) continue;

                for (int x2 = 0, x = objRelativePosition.X; x < objSize.X + objRelativePosition.X; ++x, ++x2)
                {
                    for (int y2 = 0, y = objRelativePosition.Y; y < objSize.Y + objRelativePosition.Y; ++y, ++y2)
                    {
                        if (x >= renderingPositionX || y >= renderingPositionY) continue;
                        if (!objCollision[x2, y2]) continue;

                        newCollisionMap[x, y] = objCollision[x2, y2];
                        if (objCollision[x2, y2] != false && !dict.ContainsKey(new Vector2(objPosition.X + x2, objPosition.Y + y2)))
                            dict.Add(new Vector2(objPosition.X + x2, objPosition.Y + y2), obj);
                    }
                }
            }
            return newCollisionMap;
        }

        private bool[,] ProcessCollisionUpdateQueries()
        {
            var queries = CollisionUpdateQueries;
            PhysicsBodyDictionary = new Dictionary<Vector2, GameObject>();
            bool[,] newCollisionMap = new bool[this.Size.X, this.Size.Y];
            Vector2 RenderingPosition = this.Position;
            Vector2 RenderingSize = this.Size;

            int renderingPositionX = RenderingPosition.X;
            int renderingPositionY = RenderingPosition.Y;
            int renderingSizeX = RenderingSize.X;
            int renderingSizeY = RenderingSize.Y;

            foreach (CollisionUpdateSignal query in queries)
            {
                GameObject source = query.Source as GameObject;
                Vector2 objPosition = query.Position;
                bool[,] objCollision = query.CollisionMap;
                Vector2 objRelativePosition = objPosition - RenderingPosition;
                Vector2 objSize = new Vector2(objCollision.GetLength(0), objCollision.GetLength(1));

                int objRelativePositionX = objRelativePosition.X;
                int objRelativePositionY = objRelativePosition.Y;
                int objSizeX = objSize.X;
                int objSizeY = objSize.Y;

                if (objPosition == null || objSize == null) continue;
                if (!(objRelativePosition > 0 && objRelativePosition < RenderingSize)) continue;

                for (int x2 = 0, x = objRelativePositionX; x < objSizeX + objRelativePositionX; ++x, ++x2)
                {
                    for (int y2 = 0, y = objRelativePositionY; y < objSizeY + objRelativePositionY; ++y, ++y2)
                    {
                        if (x >= renderingSizeX || y >= renderingSizeY) continue;
                        if (!objCollision[x2, y2]) continue;

                        newCollisionMap[x, y] = objCollision[x2, y2];

                        if (!PhysicsBodyDictionary.ContainsKey(new Vector2(objPosition.X + x2, objPosition.Y + y2)))
                            PhysicsBodyDictionary.Add(new Vector2(objPosition.X + x2, objPosition.Y + y2), source);
                    }
                }
            }
            CollisionUpdateQueries = new List<CollisionUpdateSignal>();
            return newCollisionMap;
        }

        public void AddCollisionUpdateQuery(CollisionUpdateSignal query)
        {
            CollisionUpdateQueries.Add(query);
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
