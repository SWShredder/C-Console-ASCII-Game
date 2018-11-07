using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AsciiEngine
{
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
            if (direction.X < 0 || direction.Y < 0)
                return true;
            return Core.Map.CollisionCheck(this, direction);
        }

    }
    public class Player : Entity, IUpdate
    {
        private double deltaTime;
        private double localTicks = 0;
        private double moveSpeed = 80.0;


        public Player() : base(Core.PlayerGraphic)
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
                Core.GameExit = true;
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
            if ((this.Position + coords).X > Core.Map.Size.X || (this.Position + coords).Y > Core.Map.Size.Y)
                return;
            if ((Position + coords).X < 0 || (Position + coords).Y < 0)
                return;
            // Amount of ticks is reset after movement until next move.
            localTicks = 0;
            this.Position += coords;
        }
    }
}
