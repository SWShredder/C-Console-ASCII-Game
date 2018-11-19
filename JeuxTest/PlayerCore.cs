using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AsciiEngine
{
    public class Player : GameObject, IUpdate
    {
        public Player(string[] graphic, ConsoleColor[,] colorMatrix) : base(graphic, colorMatrix)
        {
        }

        public override void Update()
        {
            
            ProcessInput();
            //PhysicsBody.Update();
            base.Update();
            
            //Graphics.Update();
        }

        public void ProcessInput()
        {
            Vector2 newDirection = new Vector2();

            if (Core.Engine.Input.KeyDictionary[Key.Down])
            {
                newDirection.Y++;
                //his.SetDirection(Direction.South);
            }
            if (Core.Engine.Input.KeyDictionary[Key.Up])
            {
                newDirection.Y--;
                //this.SetDirection(Direction.North);
            }
            if (Core.Engine.Input.KeyDictionary[Key.Left])
            {
                newDirection.X--;
                //this.SetDirection(Direction.West);
            }
            if (Core.Engine.Input.KeyDictionary[Key.Right])
            {
                newDirection.X++;
                //this.SetDirection(Direction.East);
            }
            if (Core.Engine.Input.KeyDictionary[Key.Z])
            {

            }
            if (Core.Engine.Input.KeyDictionary[Key.Escape])
            {
                Core.Engine.EndProcesses = true;
            }
            Movement.X = newDirection.X;
            Movement.Y = newDirection.Y;
            //PhysicsBody.SetParentMovement(newDirection);

        }

    }
}
