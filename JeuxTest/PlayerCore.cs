
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
        private double ThrusterParticlesDecayRate = 0.2;
        private double BulletRateDeltaT;
        private double TimePassed;

        public Player(string[] graphic, ConsoleColor[,] colorMatrix) : base(graphic, colorMatrix)
        {
            PhysicsBody.Mass = 50;
        }

        public override void Update()
        {
            TimePassed += Engine.Instance.CoreUpdate.DeltaTime;
            ProcessInput();
            //PhysicsBody.Update();
            base.Update();

            //Graphics.Update();
        }

        public void ProcessInput()
        {
            Vector2 newDirection = new Vector2();

            if (Engine.Instance.Input.KeyDictionary[Key.Down])
            {
                newDirection.Y++;
                this.FacingDirection = Direction.South;
                AddThrusterParticles(ThrusterParticlesDecayRate);
            }
            if (Engine.Instance.Input.KeyDictionary[Key.Up])
            {
                newDirection.Y--;
                this.FacingDirection = Direction.North;
                AddThrusterParticles(ThrusterParticlesDecayRate);
            }
            if (Engine.Instance.Input.KeyDictionary[Key.Left])
            {
                newDirection.X--;
                AddThrusterParticles(ThrusterParticlesDecayRate);
                //this.SetDirection(Direction.West);
            }
            if (Engine.Instance.Input.KeyDictionary[Key.Right])
            {
                newDirection.X++;
                AddThrusterParticles(ThrusterParticlesDecayRate);
                //this.SetDirection(Direction.East);
            }

            if (Engine.Instance.Input.KeyDictionary[Key.Escape])
            {
                Engine.EndProcesses = true;
            }

            if (Engine.Instance.Input.KeyDictionary[Key.X])
            {
                FireBullets();
                //this.SetDirection(Direction.East);
            }
            HasCollision = !Engine.Instance.Input.KeyDictionary[Key.Z];
            Movement.X = newDirection.X;
            Movement.Y = newDirection.Y;
            //PhysicsBody.SetParentMovement(newDirection);

        }

        private void AddThrusterParticles(double decayRate)
        {
            if (FacingDirection == Direction.North)
            {
                CreateParticle(1, 6, decayRate);
                CreateParticle(2, 6, decayRate);
                CreateParticle(5, 6, decayRate);
                CreateParticle(6, 6, decayRate);
            }
            else if (FacingDirection == Direction.South)
            {
                CreateParticle(1, 0, decayRate);
                CreateParticle(2, 0, decayRate);
                CreateParticle(5, 0, decayRate);
                CreateParticle(6, 0, decayRate);
            }

        }

        public void FireBullets()
        {
            if (!((TimePassed - BulletRateDeltaT) > 80)) return;
            BulletRateDeltaT = TimePassed;
            CreateBullet(0, 0, 3, 2, FacingDirection);
            CreateBullet(3, 0, 3, 2, FacingDirection);

        }

        private void CreateBullet(int positionX, int positionY, double decayRate, double damage, Direction direction)
        {
            GameObject newBullet = new GameObject(Core.Missile, Core.MissileColorMatrix)
            {
                Position = direction == Direction.North ? new Vector2(this.Position.X + 3 + positionX, this.Position.Y - 1 + positionY) 
                : new Vector2(this.Position.X + 3 + positionX, this.Position.Y + 5),

                Parent = this.Parent,
                ObjectParent = this,
                IsTrigger = true
            };
            Parent.AddChild(newBullet);
            newBullet.PhysicsBody.Mass = 1;
            //newBullet.PhysicsBody.ExternalForce = new VectorP(0, -4);
            newBullet.PhysicsBody.Velocity = direction == Direction.North ? new VectorP(0, -2 + this.PhysicsBody.Velocity.Y) : new VectorP(0, 2 + this.PhysicsBody.Velocity.Y);
            newBullet.GameStats.Damage = damage;
            newBullet.GameStats.CurrentHealth = 100;
            newBullet.GameStats.HealthRegenerationRate = -1 * decayRate;
        }

        private void CreateParticle(int positionX, int positionY, double decayRate)
        {
            GameObject newParticle = new GameObject(Core.ThrusterParticles, Core.ThrusterParticlesColorMatrix)
            {
                Position = new Vector2(this.Position.X + 1 + positionX, this.Position.Y -1 + positionY),
                Parent = this.Parent
            };
            Parent.AddChild(newParticle);
            newParticle.CollisionBody.CollisionMap = new bool[,] { { false } };
            newParticle.PhysicsBody.Mass = 1;
            newParticle.GameStats.HealthRegenerationRate = -1 * decayRate;
        }


    }
}
