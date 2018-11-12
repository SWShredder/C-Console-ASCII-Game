using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsciiEngine
{
    public class PhysicsBody : IUpdate
    {
        // FIELDS //
        static protected VectorP Friction = new VectorP(1.8, 1.8);
        private object parent;
        public VectorP Velocity = new VectorP();
        public double Mass;
        public double Bounce;
        public VectorP Position = new VectorP();
        public VectorP MaxVelocity = new VectorP(1.70, 0.85);
        public VectorP Acceleration = new VectorP(1.8, 1.2);
        public VectorP ParentMovement = new VectorP();

        public void Update()
        {
            double delta = Core.Engine.GameUpdate.DeltaTime / 1000.0;
            if(Velocity.X < 0 && ParentMovement.X == 0)
            {
                Velocity.X += Friction.X * delta;
                if (Velocity.X > 0) Velocity.X = 0;
            }
            else if(Velocity.X > 0 && ParentMovement.X == 0)
            {
                Velocity.X -= Friction.X * delta;
                if (Velocity.X < 0) Velocity.X = 0;
            }
            if (Velocity.Y < 0 && ParentMovement.Y == 0)
            {
                Velocity.Y += Friction.Y * delta;
                if (Velocity.Y > 0) Velocity.Y = 0;
            }
            else if (Velocity.Y > 0 && ParentMovement.Y == 0)
            {
                Velocity.Y -= Friction.Y * delta;
                if (Velocity.Y < 0) Velocity.Y = 0;
            }


            Velocity.X += ParentMovement.X * delta * Acceleration.X;
            Velocity.Y += ParentMovement.Y * delta * Acceleration.Y;
            Velocity = GetNormalizedVelocity(Velocity);

            if (CollisionDetection(Position + Velocity))
            {
                
                Velocity = new VectorP();
            }


            Position.X += Velocity.X;
            Position.Y += Velocity.Y;

            UpdateParentPosition(Position);

        }
        private void UpdateParentPosition(VectorP position)
        {
            Vector2 parentPosition = (parent as IPosition).GetPosition();
            Vector2 newPosition = new Vector2();
            newPosition.X = (int)Math.Round(position.X);
            newPosition.Y = (int)Math.Round(position.Y);

            if (parentPosition.X != newPosition.X || parentPosition.Y != newPosition.Y)
            {
                (parent as GameObject).SetPositionSpecial(newPosition);
            }
        }

        private bool CollisionDetection(VectorP targetPosition)
        {
            Vector2 newPosition = new Vector2();
            newPosition.X = (int)Math.Round(targetPosition.X);
            newPosition.Y = (int)Math.Round(targetPosition.Y);
            return Core.Engine.PhysicsSpace.CheckCollision((GameObject)parent, newPosition);
        }
        public void SetParentMovement(Vector2 movement)
        {
            ParentMovement.X = movement.X;
            ParentMovement.Y = movement.Y;
        }

        private VectorP GetNormalizedVelocity(VectorP velocity)
        {
            VectorP newVelocity = velocity;
            if (newVelocity.X < -1 * MaxVelocity.X) newVelocity.X = -1 * MaxVelocity.X;
            if (newVelocity.Y < -1 * MaxVelocity.Y) newVelocity.Y = -1 * MaxVelocity.Y;
            if (newVelocity.X > MaxVelocity.X) newVelocity.X = MaxVelocity.X;
            if (newVelocity.Y > MaxVelocity.Y) newVelocity.Y = MaxVelocity.Y;

            return newVelocity;
        }

        public object Parent => parent;

        public PhysicsBody(object obj)
        {
            parent = obj;
            Position.X = (obj as IPosition).GetPosition().X;
            Position.Y = (obj as IPosition).GetPosition().Y;
        }

    }
}
