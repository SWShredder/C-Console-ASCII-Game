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
            /*if (direction.X < 0 || direction.Y < 0)
                return true;*/
            //return Core.Map.CollisionCheck(this, direction);
            return Core.Engine.PhysicsSpace.CheckCollision(this, direction);
        }

    }
    public class Player : Entity, IUpdate
    {
        private double deltaTime;
        private double localTicks = 0;
        private Vector2 moveSpeed = new Vector2(20, 20);

        public Player() : base(Core.PlayerGraphic)
        {
        }

        public Player(string[] graphic, ConsoleColor[,] colorMatrix) : base(graphic, colorMatrix)
        {
        }
        // Update method is used to update ticks for the Player.
        public void Update()
        {

            // The amount of ticks between update cycles.
            // amount of ticks since last movement.
            ProcessInput();
            PhysicsBody.Update();
        }

        public void ProcessInput()
        {
            Vector2 newDirection = new Vector2();

            if (Core.Engine.Input.KeyDictionary[Key.Down])
            {
                newDirection.Y++;
            }
            if (Core.Engine.Input.KeyDictionary[Key.Up])
            {
                newDirection.Y--;
            }
            if (Core.Engine.Input.KeyDictionary[Key.Left])
            {
                newDirection.X--;
            }
            if (Core.Engine.Input.KeyDictionary[Key.Right])
            {
                newDirection.X++;
            }
            if (Core.Engine.Input.KeyDictionary[Key.Z])
            {
                Core.Engine.Camera.FitScreenSize();

            }
            if (Core.Engine.Input.KeyDictionary[Key.Escape])
            {
                Core.EndProcesses = true;
            }
            PhysicsBody.SetParentMovement(newDirection);
            /*
            if (PlayerInput.IsKeyDown(Key.Down))
            {
                newDirection.Y++;
            }
            if (System.Windows.Input.Keyboard.IsKeyDown(Key.Up))
            {
                newDirection.Y--;
            }
            if (System.Windows.Input.Keyboard.IsKeyDown(Key.Left))
            {
                newDirection.X--;
            }
            if (System.Windows.Input.Keyboard.IsKeyDown(Key.Right))
            {
                newDirection.X++;
            }
            if (System.Windows.Input.Keyboard.IsKeyDown(Key.Z))
            {
                Core.Camera.FitScreenSize();

            }
            if (System.Windows.Input.Keyboard.IsKeyDown(Key.Escape))
            {
                Core.EndProcesses = true;
            }
            PhysicsBody.SetParentMovement(newDirection);*/
        }

        public void Move(Vector2 coords)
        {
            if (this.CheckCollision(coords))
                return;


            localTicks = 0;
            this.Position += coords;
        }
    }
}
