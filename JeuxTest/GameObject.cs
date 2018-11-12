using System;
using System.Collections;
using System.Collections.Generic;
namespace AsciiEngine
{
    /// <summary>
    /// The class represents all object that inhabits the game space including entities, the player, projectiles, etc.
    /// </summary>
    public class GameObject : IPosition, IEnumerable<GameObject>, IMove, ISize, IGraphics, ISprite, ICollision
    {

        public delegate void ObjectPositionChangeEventHandler(object source, ObjectPositionEventArgs args);
        public event ObjectPositionChangeEventHandler ObjectPositionChanged;

        static public List<GameObject> List = new List<GameObject>();

        // FIELDS//
        private Graphics graphics;
        private Sprite spriteGraphics;
        private Vector2 position;
        private CollisionShape collisionBody;

        // PROPERTIES //

        public bool[,] GetCollisionPoints()
        {
            return this.collisionBody.CollisionPoints;
        }
        public CollisionShape GetCollisionShape()
        {
            return collisionBody;
        }

        public PhysicsBody PhysicsBody { set; get; }
 
        public Sprite SpriteGraphics
        {
            get
            {
                if (spriteGraphics == null)
                    spriteGraphics = new Sprite(Core.DefaultGraphics);
                return spriteGraphics;
            }
            set
            {
                spriteGraphics = value;
            }
        }

        public Vector2 Size
        {
            get
            {
                return this.SpriteGraphics.Size;
            }
        }

        public Vector2 GetSize()
        {
            return this.SpriteGraphics.GetSize();
        }
        public Vector2 Position
        {
            set
            {
                //Core.Map.OnPositionUpdate(this, value);
                if (position == null)
                    position = new Vector2();
                Vector2 oldPosition = position;    
                
                this.position = value;
                if(this == Camera.Instance.ObjectFocused)
                    Camera.Instance.NotifyCameraPositionChange(this, value);
                OnObjectPositionChanged(oldPosition, value);
            }
            get
            {
                if (position == null)
                    position = new Vector2(0, 0);
                return position;
            }

        }

        public Vector2 GetPosition()
        {
            if (position == null)
                position = new Vector2(0, 0);
            return position;
        }
        public void SetPosition(Vector2 newPosition)
        {
            if (position == null)
                position = new Vector2();
            Vector2 oldPosition = position;


            this.position = newPosition;
            if (this == Camera.Instance.ObjectFocused)
                Camera.Instance.NotifyCameraPositionChange(this, newPosition);
            OnObjectPositionChanged(oldPosition, newPosition);
            PhysicsBody.Position.X = newPosition.X;
            PhysicsBody.Position.Y = newPosition.Y;

        }

        public void SetPositionSpecial(Vector2 newPosition)
        {
            if (position == null)
                position = new Vector2();
            Vector2 oldPosition = position;


            this.position = newPosition;
            if (this == Camera.Instance.ObjectFocused)
                Camera.Instance.NotifyCameraPositionChange(this, newPosition);
            OnObjectPositionChanged(oldPosition, newPosition);

        }
        public Graphics Graphics => graphics;

        public Sprite GetFrame()
        {
            return this.SpriteGraphics;
        }

        public Vector2 GetFramePosition()
        {
            return this.position;
        }
        private void Initialize()
        {
            List.Add(this);
            graphics = new Graphics(this);
            collisionBody = new CollisionShape(this);
            PhysicsBody = new PhysicsBody(this);
            
        }

        public GameObject(string[] asciiSprite)
        {
            SpriteGraphics = new Sprite(asciiSprite);
            Initialize();
            
        }

        public GameObject(string[] asciiSprite, ConsoleColor[,] colorMatrix)
        {
            SpriteGraphics = new Sprite(asciiSprite, colorMatrix);
            Initialize();
        }

        protected virtual void OnObjectPositionChanged(Vector2 oldPosition, Vector2 newPosition)
        {
            ObjectPositionChanged?.Invoke(this, new ObjectPositionEventArgs()
            {
                OldPosition = oldPosition,
                NewPosition = newPosition,
            });
        }

        public IEnumerator<GameObject> GetEnumerator()
        {
            for (int i = 0; i < List.Count; i++)
            {
                yield return List[i];
            }
        }
        // Explicit interface implementation for nongeneric interface
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }
 
}
