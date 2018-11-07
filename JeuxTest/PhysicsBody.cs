using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsciiEngine
{
    public class PhysicsBody2 : ISize
    {
        // FIELDS //
        public bool IsSolid = true;
        private bool[,] collisionPoints;
        private object parent;
        private Vector2 velocity = new Vector2();
        public Vector2 Velocity { set; get; }


        // PROPERTIES //
        /// <summary>
        /// The parent object is the object to which PhysicsBody is linked.
        /// </summary>
        public object Parent => parent;
        public bool this[int x, int y] => CollisionPoints[x, y];
        /// <summary>
        /// Represents a matrix of boolean values that indicate if the corresponding parent object has a physical
        /// position in a give coordinate. 
        /// </summary>
        public bool[,] CollisionPoints => collisionPoints;
        /// <summary>
        /// A Vector2 that represents the size of the matrix.
        /// </summary>
        public Vector2 Size => new Vector2(collisionPoints.GetLength(0), collisionPoints.GetLength(1));

        // CONSTRUCTOR //
        /// <summary>
        /// A sprite graphic is necessary to generate a PhysicsBody2. The constructor calls GetCollisionPointsFromGraphic
        /// to generate the points.
        /// </summary>
        /// <param name="graphic">A Sprite that represents the graphic of the parent object</param>
        public PhysicsBody2(Sprite graphic)
        {
            collisionPoints = SpriteToCollisionMap(graphic);
        }
        /// <summary>
        /// This is the default constructor that uses a generic object and interfaces with Graphics or Sprite to get
        /// the collision points map. Also adds the generic object as parent.
        /// </summary>
        /// <param name="obj"></param>
        public PhysicsBody2(object obj)
        {
            Sprite sprite;
            if (obj is IGraphics graphics)
                sprite = graphics.Graphics.CurrentFrame.Sprite;
            else if (obj is ISprite _sprite)
                sprite = _sprite.SpriteGraphics;
            else
                return;
                
            collisionPoints = SpriteToCollisionMap(sprite);
            parent = obj;
        }
        // METHOD //
        /// <summary>
        /// Returns a 2 dimensional array of boolean values. True means the PhysicsBody has a no pass through point
        /// at the corresponding array coordinates.
        /// </summary>
        /// <param name="sprite">Sprite that represents a graphic from which the method will build a physicsBody2</param>
        /// <returns></returns>
        private bool[,] SpriteToCollisionMap(Sprite sprite)
        {
            bool[,] points = new bool[sprite.Size.X, sprite.Size.Y];
            for (int y = 0; y < sprite.Size.Y; y++)
            {
                for (int x = 0; x < sprite.Size.X; x++)
                {
                    if (sprite[x, y].Char != ' ')
                        points[x, y] = true;
                    else
                        points[x, y] = false;
                }
            }
            return points;
        }
    }
}
