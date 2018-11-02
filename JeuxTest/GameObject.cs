using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Game
{
    public class GameObject : IPosition, IEnumerable<GameObject>, IMove, ISize, IPositionMatrix, IGraphics, IUpdate
    {
        // Static list of GameObjects
        static private int GameObjectIndex = 0;
        static public List<GameObject> GameObjectList = new List<GameObject>();

        private Sprite graphics;
        public PositionMatrix PositionMatrix => new PositionMatrix(this.Size, this.Position);
        public Sprite Graphics
        {
            get
            {
                if (graphics == null)
                    graphics = new Sprite(Program.DefaultGraphics);
                return graphics;
            }
            set
            {
                graphics = value;
            }
        }
        public PhysicsBody CollisionBody = new PhysicsBody();
        public PhysicsBody2 PhysicsBody2;
        private Vector2 position;
        public Vector2 Size
        {
            get
            {
                return this.Graphics.Size;
            }
        }
        // Implementation of IPosition component.
        public Vector2 Position
        {
            set
            {
                Program.Map.OnPositionUpdate(this, value);
                //this.Body.SetPos(value);
                this.position = value;
            }
            get
            {
                if (position == null)
                    position = new Vector2(10, 10);
                return position;
            }

        }

        public void Update()
        {
            if(Program.RenderMode == 2)
            {
                Program.Display.Erase(this);
                Program.Map.OnPositionUpdate(this, this.Position);
                Program.Display.Draw(this);
            }

        }
        // Class constructor adds GameObject to the static list.
        public GameObject(string[] graphics)
        {
            GameObjectList.Add(this);
            GameObjectIndex++;
            //this.Position = new Vector2(10, 10);
            Graphics = new Sprite(graphics);
            PhysicsBody2 = new PhysicsBody2(Graphics);
            Systems.Update.Register(this);
        }
        public GameObject(string[] graphics, ConsoleColor[,] colorMatrix)
        {
            GameObjectList.Add(this);
            GameObjectIndex++;
            //this.Position = new Vector2(10, 10);
            Graphics = new Sprite(graphics, colorMatrix);
            PhysicsBody2 = new PhysicsBody2(Graphics);
            Systems.Update.Register(this);
        }
        // The code to make GameObject Enumeratable.
        public IEnumerator<GameObject> GetEnumerator()
        {
            for (int i = 0; i < GameObjectList.Count; i++)
            {
                yield return GameObjectList[i];
            }
        }
        // Explicit interface implementation for nongeneric interface
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        // Methods

    }
    public class Entity : GameObject
    {
        /* CODE NEEDS TO BE HEAVILY REWRITTEN */
        // STATIC //
        static private int entityRegistered = 0;
        static public List<Entity> EntityList = new List<Entity>();

        //private string[] graphic;
        //public PhysicsBody Body;
        private int entityId;


        public Entity(string[] p_graphic) : base(p_graphic)
        {
            entityRegistered++;
            this.entityId = entityRegistered;
            EntityList.Add(this);
        }
        public Entity(string[] p_graphic, ConsoleColor[,] colorMatrix) : base(p_graphic, colorMatrix)
        {
            entityRegistered++;
            this.entityId = entityRegistered;
            EntityList.Add(this);
        }

        public bool FindFromId(int id, out Entity entity_found)
        {
            foreach (Entity entity in EntityList)
            {
                if (entity.GetId() == id)
                {
                    entity_found = entity;
                    return true;
                }

            }
            entity_found = null;
            return false;
        }
        public int GetId()
        {
            return this.entityId;
        }

        public bool CheckCollision(Vector2 movement)
        {
            Vector2 direction = movement + this.Position;
            return Program.Map.CollisionCheck(this, direction);
        }

    }
    public class Player : Entity, IUpdate
    {
        private double deltaTime;
        private double localTicks = 0;
        private double moveSpeed = 80.0;


        public Player() : base(Program.PlayerGraphic)
        {
            Systems.Update.Register(this);
            localTicks = 0;
        }

        public Player(string[] graphic, ConsoleColor[,] colorMatrix) : base(graphic, colorMatrix)
        {
            Systems.Update.Register(this);
            localTicks = 0;
        }
        // Update method is used to update ticks for the Player.
        public new void Update()
        {
            // The amount of ticks between update cycles.
            deltaTime = Systems.Update.DeltaTime;
            // amount of ticks since last movement.
            localTicks += deltaTime;
            ProcessInput();
        }

        public void ProcessInput()
        {
            if (System.Windows.Input.Keyboard.IsKeyDown(Key.Down))
            {
                Move(new Vector2(0, 1));
            }
            if (System.Windows.Input.Keyboard.IsKeyDown(Key.Up))
            {
                Move(new Vector2(0, -1));
            }
            if (System.Windows.Input.Keyboard.IsKeyDown(Key.Left))
            {
                Move(new Vector2(-1, 0));
            }
            if (System.Windows.Input.Keyboard.IsKeyDown(Key.Right))
            {
                Move(new Vector2(1, 0));
            }
            if (System.Windows.Input.Keyboard.IsKeyDown(Key.Z))
            {

            }
            if (System.Windows.Input.Keyboard.IsKeyDown(Key.Escape))
            {
                Program.GameExit = true;
            }
        }
        
        public void Move(Vector2 coords)
        {
            // Handles player movespeed.(Needs to be improved and decoupled)
            if (localTicks < moveSpeed)
                return;
            // Handles Collisions. Will not move if CheckCollision == true.
            if (this.CheckCollision(coords))
                return;
            // Handles Out of Bound error if player gets to edge of Space. Could be Refactored.
            if ((this.Position + coords).X > Program.Map.Size.X || (this.Position + coords).Y > Program.Map.Size.Y)
                return;
            if ((Position + coords).X < 0 || (Position + coords).Y < 0)
                return;
            // Amount of ticks is reset after movement until next move.
            if(Program.RenderMode != 1)
                Program.Display.Erase(this);
            localTicks = 0;
            this.Position += coords;
            if (Program.RenderMode != 1)
                Program.Display.Draw(this);
        }
    }
}
