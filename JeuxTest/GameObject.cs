using System;
using System.Collections;
using System.Collections.Generic;
namespace AsciiEngine
{
    /// <summary>
    /// The class represents all object that inhabits the game space including entities, the player, projectiles, etc.
    /// </summary>
    public class GameObject : IPosition, IEnumerable<GameObject>, IMove, ISize, IPositionMatrix, IGraphics, ISprite
    {
        /// <summary>
        /// The event handler of the GameObject class to raise events when the GameObject changes position.
        /// </summary>
        /// <param name="source">A generic object instance that will be used by the observer</param>
        /// <param name="args">Args contain the vector2 values of the old position and the new position of the GameObject's instance</param>
        public delegate void ObjectPositionChangeEventHandler(object source, ObjectPositionEventArgs args);
        public event ObjectPositionChangeEventHandler ObjectPositionChanged;

        /// <summary>
        /// A list of all GameObject instances. Every GameObject instances registers to the list at creation
        /// </summary>
        static public List<GameObject> List = new List<GameObject>();

        // FIELDS//
        private Graphics graphics;
        private Sprite spriteGraphics;
        private Vector2 position;

        // PROPERTIES //
        public PhysicsBody2 PhysicsBody2 { set; get; }
        /// <summary>
        /// Returns a Position Matrix constructed from this instance's size and position
        /// </summary>
        public PositionMatrix PositionMatrix => new PositionMatrix(this.Size, this.Position);
        /// <summary>
        /// Returns GameObject's instance's Sprite and if null, returns a DefaultGraphics.
        /// </summary>
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
        /// <summary>
        /// Returns the size of GameObject's instance's SpriteGraphics size.
        /// </summary>
        public Vector2 Size
        {
            get
            {
                return this.SpriteGraphics.Size;
            }
        }
        ////// NEEDS TO BE REWORKED /////
        /// <summary>
        /// (WIP) When Position is set, the values that depend on GameObject's instance's position are updated
        /// at the same time. If no position exists and the engine needs to check the instance's position it is assumed
        /// to be (0,0)
        /// </summary>
        public Vector2 Position
        {
            set
            {
                Core.Map.OnPositionUpdate(this, value);
                //this.Body.SetPos(value);
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
        public Graphics Graphics => graphics;

        /// METHODS ////
        /// <summary>
        /// (WIP)The common initialize method to all constructors. It adds a Graphics to all object created, adds the instance to
        /// the list and registers for update.
        /// </summary>
        private void Initialize()
        {
            graphics = new Graphics(this);
            List.Add(this);
            Systems.Update.Register(this);
        }
        /// <summary>
        /// (WIP) Basic constructor with no color matrix. Adds a physicsBody2 to the instance.
        /// </summary>
        /// <param name="graphics">Represents a Sprite graphics assigned to a GameObject's instance</param>
        public GameObject(string[] graphics)
        {
            Initialize();
            SpriteGraphics = new Sprite(graphics);
            PhysicsBody2 = new PhysicsBody2(SpriteGraphics);
        }
        /// <summary>
        /// (WIP) Overdriven constructor to handle color matrixes.
        /// </summary>
        /// <param name="graphics">Represents a Sprite graphics assigned to GameObject's instance</param>
        /// <param name="colorMatrix">Represents the color Matric assigned </param>
        public GameObject(string[] graphics, ConsoleColor[,] colorMatrix)
        {
            Initialize();
            SpriteGraphics = new Sprite(graphics, colorMatrix);
            PhysicsBody2 = new PhysicsBody2(SpriteGraphics);
        }
        /// <summary>
        /// The method raises an event when the GameObject's position is changed. The event is sent to all
        /// connected modules that belong to GameObject.
        /// </summary>
        /// <param name="oldPosition">A vector2 that represents the old position of the GameObject before the position was updated</param>
        /// <param name="newPosition">A vector2 that represents the new position after the GameObject's position was updated</param>
        protected virtual void OnObjectPositionChanged(Vector2 oldPosition, Vector2 newPosition)
        {
            ObjectPositionChanged?.Invoke(this, new ObjectPositionEventArgs()
            {
                OldPosition = oldPosition,
                NewPosition = newPosition,
            });
        }
        /// <summary>
        /// (UNUSED) Updates the block inside Update() every game updates.
        /// </summary>

        // The code to make GameObject Enumeratable.
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
