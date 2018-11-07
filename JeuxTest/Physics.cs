using System;
using System.Collections.Generic;
using System.Collections;


namespace AsciiEngine
{
    public class Physics
    {
        public static double Scale = 5.0;
        public static int Drag = 12;

        public class Space
        {
            public GameObject[,] SpaceArray;

            public Vector2 Size
            {
                get
                {
                    return new Vector2(SpaceArray.GetLength(0), SpaceArray.GetLength(1));
                }
            }
            public Space(Vector2 size)
            {
                SpaceArray = new GameObject[size.X, size.Y];

            }
            public void OnPositionUpdate(GameObject gameObject, Vector2 newPosition)
            {
                IPositionMatrix gameObjMatrix = gameObject as IPositionMatrix;
                IPosition gameObjPosition = gameObject as IPosition;
                if (gameObjMatrix == null)
                    return;

                ClearPositionMatrix(gameObjMatrix, gameObjPosition, gameObject.SpriteGraphics);
                UpdateSpacePosition(gameObject, newPosition, gameObjMatrix);
                /*
                if (gameObject.Body == null)
                {
                    Console.WriteLine("Problem with GameObject Body");
                    return;
                }
                // Erase old position matrix.

                foreach (Vector2 vector2 in gameObject.Body)
                {
                    SpaceArray[vector2.X + gameObject.Position.X, vector2.Y + gameObject.Position.Y] = null;
                }

                // Write new position matrix.
                foreach (Vector2 vector2 in gameObject.Body)
                {
                    SpaceArray[vector2.X + position.X, vector2.Y + position.Y] = gameObject;
                }*/
            }
            public GameObject this[int x, int y]
            {
                get
                {
                    return SpaceArray[x, y];
                }
            }

            private void UpdateSpacePosition(GameObject gameObject, Vector2 newPosition, IPositionMatrix gameObjMatrix)
            {
                PositionMatrix newPosMatrix = new PositionMatrix(gameObjMatrix.PositionMatrix.Size, newPosition);
                Console.SetCursorPosition(0, Console.WindowHeight - 1);
                for (int x = 0; x < newPosMatrix.Size.X; x++)
                {
                    for (int y = 0; y < newPosMatrix.Size.Y; y++)
                    {
                        if(gameObject.SpriteGraphics[x , y].Char != ' ')
                            SpaceArray[newPosMatrix[x, y].X, newPosMatrix[x, y].Y] = gameObject;
                    }
                }
            }

            private void ClearPositionMatrix(IPositionMatrix gameObjMatrix, IPosition gameObjPosition, Sprite graphics)
            {
                for (int x = 0; x < gameObjMatrix.PositionMatrix.Size.X; x++)
                {
                    for (int y = 0; y < gameObjMatrix.PositionMatrix.Size.Y; y++)
                    {
                        if (graphics[x, y].Char != ' ')
                            SpaceArray[gameObjMatrix.PositionMatrix[x, y].X, gameObjMatrix.PositionMatrix[x, y].Y] = null;
                    }
                }
            }

            public Space GetSubSpace(Vector2 size, Vector2 origin)
            {
                Vector2 index = new Vector2(0, 0);
                Space subspace = new Space(size);
                for (int x = origin.X; x < origin.X + size.X; x++)
                {
                    for (int y = origin.Y; y < origin.Y + size.Y; y++)
                    {
                        subspace.SpaceArray[index.X, index.Y] = this.SpaceArray[x, y];
                        index.Y++;
                    }
                    index.X++;
                }
                return subspace;
            }

            public bool CollisionCheck(GameObject initiatorObject, Vector2 position)
            {
                PhysicsBody2 body = initiatorObject.PhysicsBody2;
                for(int y = 0; y < body.Size.Y; y++)
                {
                    for(int x = 0; x < body.Size.X; x++)
                    {
                        if(this.SpaceArray[x + position.X , y + position.Y] != null)
                        {
                            GameObject gameObject = this.SpaceArray[x + position.X, y + position.Y];
                            if(gameObject.PhysicsBody2 == body)
                            {
                                continue;
                            }
                            Vector2 pointPosition = new Vector2(x + position.X, y + position.Y);
                            Vector2 targetPosition = pointPosition - gameObject.Position;
                            Vector2 selfPosition = pointPosition - position;
                            /*if (gameObject.PhysicsBody2.Size.X <= targetPosition.X || gameObject.PhysicsBody2.Size.Y <= targetPosition.Y)
                                continue;*/
                            if (gameObject.PhysicsBody2[targetPosition.X, targetPosition.Y] && body[selfPosition.X, selfPosition.Y])
                                return true;
                        }
                    }
                }
                return false;
            }
        }
    }
    /*
    public class PhysicsBody2 : ISize
    {
        public bool IsSolid = true;
        private bool[,] collisionPoints;
        public bool[,] CollisionPoints
        {
            get
            {
                return collisionPoints;
            }
        }
        public Vector2 Size => new Vector2(collisionPoints.GetLength(0), collisionPoints.GetLength(1));

        public PhysicsBody2(Sprite graphic)
        {
            collisionPoints = GetCollisionPointsFromGraphic(graphic);
        }
        public bool[,] GetCollisionPointsFromGraphic(Sprite graphic)
        {
            bool[,] points = new bool[graphic.Size.X, graphic.Size.Y];
            for (int y = 0; y < graphic.Size.Y; y++)
            {
                for (int x = 0; x < graphic.Size.X; x++)
                {
                    if (graphic[x, y].Char != ' ')
                        points[x, y] = true;
                    else
                        points[x, y] = false;
                }
            }
            return points;
        }
        public bool this[int x, int y] => CollisionPoints[x, y];
    }*/

    
}

