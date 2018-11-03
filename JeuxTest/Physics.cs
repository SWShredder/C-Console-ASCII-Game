using System;
using System.Collections.Generic;
using System.Collections;


namespace Game
{
    public class Physics
    {

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

                ClearPositionMatrix(gameObjMatrix, gameObjPosition, gameObject.Graphics);
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
                        if(gameObject.Graphics[x , y].Char != ' ')
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
    }


    public class PhysicsBody : IEnumerable
    {
        private List<Vector2> body = new List<Vector2>();
        private List<Vector2> Pos = new List<Vector2>();

        // Setters and Getters.
        public List<Vector2> Body => body;
        // Constructors
        public PhysicsBody()
        {
            Body.Add(new Vector2(0, 0));
        }
        public PhysicsBody(Vector2[] vec2Array)
        {
            foreach (Vector2 vec2 in vec2Array)
            {
                Body.Add(vec2);
            }
        }
        public PhysicsBody(string[] graphic)
        {
            this.body = GenerateBody(graphic);
        }

        // Methods
        public string GetBodyInfo()
        {
            string bodyInfo = "";
            foreach (Vector2 vec2 in this.Body)
            {
                bodyInfo += vec2.ToString() + ";";
            }
            return bodyInfo;
        }
        public string GetPosCoordinates()
        {
            string posCoordinates = "";
            foreach (Vector2 vec2 in this.Pos)
            {
                posCoordinates += vec2.ToString() + ";";
            }
            return posCoordinates;
        }
        public void SetPos(Vector2 pos)
        {
            for (int i = 0; i < this.Pos.Count; i++)
            {
                this.Pos[i] = this.Body[i] + pos;
            }

        }
        public Vector2 GetPos()
        {
            if (this.Pos.Count > 0)
                return this.Pos[0];
            else
                return new Vector2(0, 0);
        }
        public bool CollisionCheck(Vector2[] points)
        {
            foreach (Vector2 vector in points)
            {
                foreach (Vector2 vec2 in Body)
                {
                    if (vec2 == vector)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public bool CollisionCheck(Vector2 movement, PhysicsBody physicsBody)
        {
            foreach (Vector2 selfPoints in this.Pos)
            {
                foreach (Vector2 targetPoints in physicsBody.Pos)
                {
                    if (selfPoints + movement == targetPoints)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void Move(Vector2 vector)
        {
            this.SetPos(this.GetPos() + vector);
        }

        public List<Vector2> GenerateBody(string[] graphic)
        {
            List<Vector2> body = new List<Vector2>();
            for (int y = 0; y < graphic.Length; y++)
            {
                for (int x = 0; x < graphic[y].Length; x++)
                {
                    if (graphic[y][x] == ' ')
                    {
                        //continue;
                        //body.Add(new Vector2(x, y));
                        //Pos.Add(new Vector2(x, y));
                    }
                    body.Add(new Vector2(x, y));
                    Pos.Add(new Vector2(x, y));
                }
            }

            return body;
        }

        public IEnumerator<Vector2> GetEnumerator()
        {
            for (int i = 0; i < this.body.Count; i++)
            {
                yield return body[i];
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

