using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
namespace AsciiEngine
{
    /// <summary>
    /// The class represents all object that inhabits the game space including entities, the player, projectiles, etc.
    /// </summary>
    public class GameObject : IPosition, ISize, INodes, IUpdate
    {
        static public List<GameObject> List = new List<GameObject>();

        private Vector2 position;
        private Direction direction;

        public bool HasCollision { set; get; }
        public bool HasCollided { set; get; }
        public bool IsTrigger { set; get; }
        public List<INodes> Children { set; get; }
        public INodes Parent { set; get; }
        public byte[,] Body { set; get; }
        public ObjectGraphics Graphics { set; get; }
        public ObjectCollision CollisionBody { set; get; }
        public ObjectPhysics PhysicsBody { set; get; }
        public ObjectStats GameStats { set; get; }
        public VectorP Movement { set; get; }
        public List<ObjectSignal> ObjectSignalQueries { set; get; }
        public Direction FacingDirection
        {
            get => direction;
            set => UpdateFacingDirection(value);
        }
        public Vector2 Size => new Vector2(this.Graphics.ByteMap.GetLength(0), Graphics.ByteMap.GetLength(1));
        public Vector2 Position
        {
            set
            {
                UpdatePosition(value);
                PhysicsBody.Position.X = value.X;
                PhysicsBody.Position.Y = value.Y;
            }
            get
            {
                if (position == null)
                    position = new Vector2(0, 0);
                return position;
            }
        }

        public void Initialize()
        {
            Children = new List<INodes>();
            Graphics = new ObjectGraphics(this);
            CollisionBody = new ObjectCollision(this);
            PhysicsBody = new ObjectPhysics(this);
            Movement = new VectorP();
            GameStats = new ObjectStats(this);
            IsTrigger = false;
            ObjectSignalQueries = new List<ObjectSignal>();
            HasCollision = true;
        }

        public GameObject()
        {
            Body = Tiles.GenerateByteArrayMap(Core.DefaultGraphics);
        }
        public GameObject(string[] asciiSprite)
        {
            Body = Tiles.GenerateByteArrayMap(asciiSprite);
            Initialize();

        }
        public GameObject(string[] asciiSprite, ConsoleColor[,] colorMatrix)
        {
            Body = Tiles.GenerateByteArrayMap(asciiSprite, colorMatrix);
            Initialize();
        }

        public virtual void Update()
        {
            ProcessObjectSignalQueries();
            if (HasCollision) CollisionBody.Update();
            PhysicsBody.Update();
            Graphics.Update();
            GameStats.Update();
            foreach (INodes child in Children)
            {
                if (child as GameObject != null)
                    child.Update();
            }
        }

        public void UpdateFacingDirection(Direction direction)
        {

            this.direction = direction;
            Graphics.UpdateCache();
            CollisionBody.UpdateCollisionMap();
        }

        public void UpdatePosition(Vector2 newPosition)
        {
            this.position = newPosition;
            if ((this.Parent as Chunk) != null)
            {
                (this.Parent as Chunk).AddMovementQuery(this, newPosition);
            }
            if (this == Camera.Instance.ObjectFocused)
                Camera.Instance.NotifyCameraPositionChange(this, newPosition);
        }

        public void AddChild(INodes child)
        {
            Children.Add(child);
        }
        public void RemoveChild(INodes child)
        {
            Children.Remove(child);
        }

        public void OnDeathEvent()
        {
            (Parent as Chunk).AddDeletionQuery(this);
        }

        public void AddObjectSignalQuery(ObjectSignal objectSignal)
        {
            ObjectSignalQueries.Add(objectSignal);
        }

        private void ProcessObjectSignalQueries()
        {
            foreach (ObjectSignal signal in ObjectSignalQueries)
            {
                foreach (INodes child in Children)
                {
                    if ((signal as ObjectPhysicsSignal) != null)
                    {
                        PhysicsBody.AddObjectPhysicsQuery(signal as ObjectPhysicsSignal);
                    }
                }
            }
            ObjectSignalQueries = new List<ObjectSignal>();
        }
    }
}
