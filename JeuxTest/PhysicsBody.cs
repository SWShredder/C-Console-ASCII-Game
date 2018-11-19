using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsciiEngine
{
    public class PhysicsBody : INodes
    {
        public double Mass { set; get; }
        public double Bounce { set; get; }
        public INodes Parent { set; get; }
        public VectorP Brake { set; get; }
        public VectorP Velocity { set; get; }
        public VectorP Position { set; get; }
        public VectorP MaxVelocity { set; get; }
        public VectorP Acceleration { set; get; }
        public List<INodes> Children { set; get; }


        public void Initialize()
        {
            Velocity = new VectorP();
            MaxVelocity = new VectorP(1.8, 0.9);
            Acceleration = new VectorP(1.2, 0.8);
            Brake = new VectorP(0.8, 0.6);
            Position = new VectorP();
        }

        public PhysicsBody(INodes parent)
        {
            Children = new List<INodes>();
            Parent = parent;
            var parentObj = parent as GameObject;
            parentObj.AddChild(this);
            Initialize();
            this.Position.X = parentObj.Position.X;
            this.Position.Y = parentObj.Position.Y;
        }

        public void Update()
        {
            var parent = Parent as GameObject;
            var parentMovement = parent.Movement;
            double delta = Core.Engine.CoreUpdate.DeltaTime / 1000;

            if ((parentMovement.X == 0 & Velocity.X != 0) || (parentMovement.Y == 0 && Velocity.Y != 0))
                ApplyBrakeForce(delta, parentMovement);

            ApplyMovementForce(delta, parentMovement);
            ApplyMaximumVelocity(Velocity);
            UpdateParentDirection(parent, Velocity);
            if (CollisionDetection(Position + Velocity)) Velocity = new VectorP();

            Position.X += Velocity.X;
            Position.Y += Velocity.Y;
            UpdateParentPosition(Position);
        }


        public void ApplyMovementForce(double deltaTime, VectorP movement)
        {
            Velocity.X += movement.X * deltaTime * Acceleration.X;
            Velocity.Y += movement.Y * deltaTime * Acceleration.Y;
        }

        public void ApplyBrakeForce(double deltaTime, VectorP movement)
        {
            double velocityX = Velocity.X;
            double velocityY = Velocity.Y;
            double absVelocityX = Math.Abs(velocityX);
            double absVelocityY = Math.Abs(velocityY);
            int coefficientX = velocityX < 0 ? -1 : 1;
            int coefficientY = velocityY < 0 ? -1 : 1;

            if (movement.X == 0)
            {
                absVelocityX -= Brake.X * deltaTime;
                absVelocityX = Math.Max(absVelocityX, 0);
                Velocity.X = absVelocityX * coefficientX;
            }
            if (movement.Y == 0)
            {
                absVelocityY -= Brake.Y * deltaTime;
                absVelocityY = Math.Max(absVelocityY, 0);
                Velocity.Y = absVelocityY * coefficientY;
            }
        }
        private void ApplyMaximumVelocity(VectorP velocity)
        {
            VectorP newVelocity = velocity;
            if (newVelocity.X < -1 * MaxVelocity.X) newVelocity.X = -1 * MaxVelocity.X;
            if (newVelocity.Y < -1 * MaxVelocity.Y) newVelocity.Y = -1 * MaxVelocity.Y;
            if (newVelocity.X > MaxVelocity.X) newVelocity.X = MaxVelocity.X;
            if (newVelocity.Y > MaxVelocity.Y) newVelocity.Y = MaxVelocity.Y;

            Velocity = newVelocity;
        }

        private void UpdateParentPosition(VectorP position)
        {
            var parent = Parent as GameObject;
            Vector2 parentPosition = parent.Position;
            Vector2 newPosition = new Vector2();
            newPosition.X = (int)Math.Round(position.X);
            newPosition.Y = (int)Math.Round(position.Y);

            if (parentPosition.X != newPosition.X || parentPosition.Y != newPosition.Y)
            {
                parent.UpdatePosition(newPosition);
            }
        }

        public void UpdateParentDirection(GameObject parent, VectorP velocity)
        {
            if (Velocity.Y > 0)
                parent.FacingDirection = (Direction.South);
            else if (Velocity.Y < 0)
                parent.FacingDirection = (Direction.North);
        }

        private bool CollisionDetection(VectorP targetPosition)
        {
            Vector2 newPosition = new Vector2();
            newPosition.X = (int)Math.Round(targetPosition.X);
            newPosition.Y = (int)Math.Round(targetPosition.Y);
            return Core.Engine.PhysicsSpace.CheckCollision((GameObject)Parent, newPosition, out var collisionEvent);
        }

        // INodes Interface
        public void AddChild(INodes child)
        {
            Children.Add(child);
        }

        public void RemoveChild(INodes child)
        {
            Children.Remove(this);
        }
    }
}
