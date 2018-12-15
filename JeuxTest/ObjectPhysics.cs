using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsciiEngine
{
    public class ObjectPhysics : INodes
    {
        public double Mass { set; get; }
        public double Bounce { set; get; }
        public INodes Parent { set; get; }
        public VectorP Friction { set; get; }
        public VectorP Velocity { set; get; }
        public VectorP Position { set; get; }
        public VectorP MaxMovementVelocity { set; get; }
        public VectorP Acceleration { set; get; }
        public List<INodes> Children { set; get; }
        public VectorP TerminalVelocity { set; get; }
        public VectorP ExternalForce { set; get; }
        public List<ObjectPhysicsSignal> ObjectPhysicsQueries { set; get; }

        public void Initialize()
        {
            Velocity = new VectorP();
            MaxMovementVelocity = new VectorP(1.8, 0.9);
            Acceleration = new VectorP(1.0, 0.6);
            Friction = new VectorP(0.8, 0.6);
            Position = new VectorP();
            Bounce = 0.05;
            Mass = 1;
            ExternalForce = new VectorP();
            ObjectPhysicsQueries = new List<ObjectPhysicsSignal>();
            TerminalVelocity = new VectorP(3.0, 1.5);
        }

        public ObjectPhysics(INodes parent)
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
            double delta = Engine.Instance.CoreUpdate.DeltaTime / 1000;

            if ((parentMovement.X == 0 & Velocity.X != 0) || (parentMovement.Y == 0 && Velocity.Y != 0))
                ApplyBrakeForce(delta, parentMovement);

            //ApplyMovementForce(delta, parentMovement);
            //ApplyMaximumVelocity(GetMovementForce(delta, parentMovement));
            //UpdateParentDirection(parent, Velocity);
            ApplyMovementVelocity(GetMovementForce(delta, parentMovement));
            //CollisionDetection(Position + Velocity);
            if (CollisionDetection(Position + Velocity)) Velocity = new VectorP();
            ApplyExternalForce(delta, Velocity);
            ApplyTerminalVelocity(Velocity);
            //if (CollisionDetection(Position + Velocity)) Velocity = new VectorP();         
            Position.X += Velocity.X;
            Position.Y += Velocity.Y;
            UpdateParentPosition(Position);
        }

        public void ApplyExternalForce(double deltaTime, VectorP velocity)
        {
            if (ExternalForce.X == 0 && ExternalForce.Y == 0) return;

            double forceAppliedX;
            double forceAppliedY;

            if (ExternalForce.X != 0)
                forceAppliedX = ExternalForce.X;//Math.Max(ExternalForce.X * deltaTime, 0.025);
            else
                forceAppliedX = 0;
            if (ExternalForce.Y != 0)
                forceAppliedY = ExternalForce.Y;//Math.Max(ExternalForce.Y * deltaTime, 0.0125);
            else
                forceAppliedY = 0;

            Velocity.X += forceAppliedX;
            Velocity.Y += forceAppliedY;

            ExternalForce.X = 0;
            ExternalForce.Y = 0;
            return;

            if (forceAppliedY > 0 && (ExternalForce.Y - forceAppliedY) <= 0) ExternalForce.Y = 0;
            else if (forceAppliedY < 0 && (ExternalForce.Y - forceAppliedY) >= 0) ExternalForce.Y = 0;
            else ExternalForce.Y -= forceAppliedY;

            if (forceAppliedX > 0 && (ExternalForce.X - forceAppliedX) <= 0) ExternalForce.X = 0;
            else if (forceAppliedX < 0 && (ExternalForce.X - forceAppliedX) >= 0) ExternalForce.X = 0;
            else ExternalForce.X -= forceAppliedX;

        }

        public void ApplyMovementForce(double deltaTime, VectorP movement)
        {
            Velocity.X += movement.X * deltaTime * Acceleration.X;
            Velocity.Y += movement.Y * deltaTime * Acceleration.Y;
        }

        public VectorP GetMovementForce(double deltaTime, VectorP movement)
        {
            VectorP newVelocity = new VectorP();
            newVelocity.X = movement.X * deltaTime * Acceleration.X;
            newVelocity.Y = movement.Y * deltaTime * Acceleration.Y;
            return newVelocity;
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
                absVelocityX -= Friction.X * deltaTime;
                absVelocityX = Math.Max(absVelocityX, 0);
                Velocity.X = absVelocityX * coefficientX;
            }
            if (movement.Y == 0)
            {
                absVelocityY -= Friction.Y * deltaTime;
                absVelocityY = Math.Max(absVelocityY, 0);
                Velocity.Y = absVelocityY * coefficientY;
            }
        }
        private void ApplyTerminalVelocity(VectorP velocity)
        {
            VectorP newVelocity = velocity;
            if (newVelocity.X < -1 * TerminalVelocity.X) newVelocity.X = -1 * TerminalVelocity.X;
            if (newVelocity.Y < -1 * TerminalVelocity.Y) newVelocity.Y = -1 * TerminalVelocity.Y;
            if (newVelocity.X > TerminalVelocity.X) newVelocity.X = TerminalVelocity.X;
            if (newVelocity.Y > TerminalVelocity.Y) newVelocity.Y = TerminalVelocity.Y;

            Velocity = newVelocity;
        }

        private void ApplyMovementVelocity(VectorP velocity)
        {
            if (!(Velocity.X < 0 && Velocity.X <= (MaxMovementVelocity.X * -1)))
            {
                if (Velocity.X + velocity.X <= MaxMovementVelocity.X * -1)
                    Velocity.X = MaxMovementVelocity.X * -1;
                else
                    Velocity.X += velocity.X;
            }

            if (!(Velocity.X > 0 && Velocity.X >= MaxMovementVelocity.X))
            {
                if (Velocity.X + velocity.X >= MaxMovementVelocity.X)
                    Velocity.X = MaxMovementVelocity.X;
                else
                    Velocity.X += velocity.X;
            }

            if (!(Velocity.Y < 0 && Velocity.Y <= (MaxMovementVelocity.Y * -1)))
            {
                if (Velocity.Y + velocity.Y <= MaxMovementVelocity.Y * -1)
                    Velocity.Y = MaxMovementVelocity.Y * -1;
                else
                    Velocity.Y += velocity.Y;
            }

            if (!(Velocity.Y > 0 && Velocity.Y >= MaxMovementVelocity.Y))
            {
                if (Velocity.Y + velocity.Y >= MaxMovementVelocity.Y)
                    Velocity.Y = MaxMovementVelocity.Y;
                else
                    Velocity.Y += velocity.Y;
            }

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
            if (!(Parent as GameObject).HasCollision) return false;
            Vector2 newPosition = new Vector2();
            newPosition.X = (int)Math.Round(targetPosition.X);
            newPosition.Y = (int)Math.Round(targetPosition.Y);
            return Engine.Instance.PhysicsSpace.CheckCollision((GameObject)Parent, newPosition);//Engine.Instance.PhysicsSpace.CheckCollision((GameObject)Parent, newPosition);
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


        public void AddObjectPhysicsQuery(ObjectPhysicsSignal signal)
        {
            //ObjectPhysicsQueries.Add(signal);
            ProcessObjectPhysicsQueries(signal);
        }

        public void ProcessObjectPhysicsQueries(ObjectPhysicsSignal query)
        {
            /*foreach(ObjectPhysicsSignal query in ObjectPhysicsQueries)
            {
                double collisionVectorX = query.CollisionVectorX;
                double collisionVectorY = query.CollisionVectorY;
                double force = query.Force;

                ExternalForce.X += collisionVectorX * force / Mass;
                ExternalForce.Y += collisionVectorY * force / Mass;
            }
            ObjectPhysicsQueries = new List<ObjectPhysicsSignal>();*/
            double collisionVectorX = query.CollisionVectorX;
            double collisionVectorY = query.CollisionVectorY;
            double force = query.Force;

            ExternalForce.X += collisionVectorX * force / Mass;
            ExternalForce.Y += collisionVectorY * force / Mass;
        }
    }
}
