using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsciiEngine
{
    public class PhysicsSpace : ISize, INodes
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
                return Core.Engine.RenderingSize;
            }
        }

        public PhysicsSpace(INodes parent)
        {
            Parent = parent;
            Parent.AddChild(this);
            Children = new List<INodes>();
            CollisionUpdateQueries = new List<CollisionUpdateSignal>();
        }

        public void Update()
        {
            //CollisionMap = RenderCollisionMap(out PhysicsBodyDictionary);
            CollisionMap = ProcessCollisionUpdateQueries();
        }
    
        
        public bool CheckCollision(GameObject source, Vector2 targetPosition, out CollisionEventSignal collisionEvent)
        {
            CollisionBody collisionBody = source.CollisionBody;
            Vector2 relativeTargetPosition = targetPosition - this.Position;
            int relativeTargetPositionX = relativeTargetPosition.X;
            int relativeTargetPositionY = relativeTargetPosition.Y;
            int collisionBodySizeX = collisionBody.Size.X;
            int collisionBodySizeY = collisionBody.Size.Y;

            collisionEvent = null;

            for (int x = 0; x < collisionBodySizeX; ++x)
            {
                for (int y = 0; y < collisionBodySizeY; ++y)
                {
                    if (collisionBody[x, y])
                    {
                        if (CollisionMap == null) return false;
                        if (x + relativeTargetPositionX >= CollisionMap.GetLength(0) || y + relativeTargetPositionY >= CollisionMap.GetLength(1)) return false;
                        if (x + relativeTargetPositionX < 0 || y + relativeTargetPosition.Y < 0) return false;
                        if (CollisionMap[x + relativeTargetPositionX, y + relativeTargetPositionY])
                        {
                            Vector2 relativePositionAdjusted = new Vector2(x + targetPosition.X, y + targetPosition.Y);
                            PhysicsBodyDictionary.TryGetValue(relativePositionAdjusted, out GameObject targetObject);
                            if (targetObject != source)
                            {
                                collisionEvent = GenerateCollisionEvent(source, targetObject);
                                return true;
                            }

                        }

                    }
                }
            }
            collisionEvent = null;
            return false;
        }
        
        private CollisionEventSignal GenerateCollisionEvent(GameObject source, GameObject target)
        {
            var newCollisionEvent = new CollisionEventSignal()
            {
                DamageQuery = new DamageQuery(),
                ImpactSignal = new PhysicsImpactSignal()
            };
            if (source.IsTrigger)
                newCollisionEvent.DamageQuery.Damage = source.GameStats.Damage;
            else
                newCollisionEvent.DamageQuery = null;

            return newCollisionEvent;
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
