using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class CameraPositionEventArgs : EventArgs
    {
        public Vector2 OldPosition { set; get; }
        public Vector2 NewPosition { set; get; }
        public Vector2 Size { set; get; }
    }

    public class DrawRequestEventArgs : EventArgs
    {
        public Vector2 OldPosition { set; get; }
        public Vector2 NewPosition { set; get; }
        public Sprite Sprite { set; get; }

    }

    public class ObjectPositionEventArgs : EventArgs
    {
        public Vector2 OldPosition { set; get; }
        public Vector2 NewPosition { set; get; }
    }

    static public class Utility
    {
        public static Vector2 GetWindowSize()
        {
            return new Vector2(Console.WindowWidth, Console.WindowHeight);
        }
        public static Vector2 Vec2(int x, int y)
        {
            return new Vector2(x, y);
        }

    }
    public class Vector2
    {
        private int x;
        private int y;
        

        public Vector2(int c_x, int c_y)
        {
            x = c_x;
            y = c_y;
        }
        public Vector2()
        {
            x = 0;
            y = 0;
        }
        public override string ToString()
        {
            return "(" + this.X + ";" + this.Y + ")";
        }
        public int X
        {
            get
            {
                return this.x;
            }
            set
            {
                this.x = value;
            }
        }
        public int Y
        {
            get
            {
                return this.y;
            }
            set
            {
                this.y = value;
            }
        }
        public static Vector2 operator +(Vector2 vec_a, Vector2 vec_b)
        {
            Vector2 vec2 = new Vector2();
            vec2.X = vec_a.X + vec_b.X;
            vec2.Y = vec_a.Y + vec_b.Y;
            return vec2;
        }
        public static Vector2 operator -(Vector2 vecA, Vector2 vecB)
        {
            Vector2 vec2 = new Vector2();
            vec2.X = vecA.X - vecB.X;
            vec2.Y = vecA.Y - vecB.Y;
            return vec2;
        }

        public static bool operator < (Vector2 vecA, int intA)
        {
            return vecA.X < intA && vecA.Y < intA;
        }

        public static bool operator >(Vector2 vecA, int intA)
        {
            return vecA.X > intA && vecA.Y > intA;
        }

        public static bool operator < (Vector2 vecA, Vector2 vecB)
        {
            return vecA.X < vecB.X && vecA.Y < vecB.Y;
        }

        public static bool operator >(Vector2 vecA, Vector2 vecB)
        {
            return vecA.X > vecB.X && vecA.Y > vecB.Y;
        }

    }
    public class Matrix : ISize
    {
        private object[,] objectMatrix;
        public Vector2 Size
        {
            get
            {
                return new Vector2(objectMatrix.GetLength(0), objectMatrix.GetLength(1));
            }
        }
        public Matrix(Vector2 size)
        {
            objectMatrix = new object[size.X, size.Y];
        }
        public object this[int x, int y]
        {
            set
            {
                objectMatrix[x, y] = value;
            }
            get
            {
                return objectMatrix[x, y];
            }
        }
    }


    public class PositionMatrix : ISize
    {
        private Vector2[,] PosMatrix;
        public Vector2 Size
        {
            get
            {
                return new Vector2(PosMatrix.GetLength(0), PosMatrix.GetLength(1));
            }
        }
        public PositionMatrix(Vector2 size, Vector2 position)
        {
            PosMatrix = new Vector2[size.X, size.Y];
            SetPosition(position);
        }

        public void SetPosition(Vector2 position)
        {
            UpdatePositionMatrix(position);
        }
        private void UpdatePositionMatrix(Vector2 position)
        {
            for (int x = 0; x < this.Size.X; x++)
            {
                for (int y = 0; y < this.Size.Y; y++)
                {
                    PosMatrix[x, y] = position + new Vector2(x, y);
                }
            }
        }
        public Vector2 this[int i, int j]
        {
            get
            {
                return PosMatrix[i, j];
            }
            set
            {
                PosMatrix[i, j] = value;
            }
        }
        public override string ToString()
        {
            string matrixString = "";
            for(int x = 0; x < this.Size.X; x++)
            {
                for(int y = 0; y < this.Size.Y; y++)
                {
                    matrixString += this.PosMatrix[x, y];
                }
            }
            return matrixString;
        }

    }
    // INTERFACES
    interface IUpdate
    {
        void Update();
    }
    interface IPosition
    {
        Vector2 Position { set; get; }
    }
    interface ISize
    {
        Vector2 Size { get; }
    }

    interface ICollision
    {
        PhysicsBody2 CollisionBody { set; get; }
    }
    interface IMove
    {

    }
    interface IPositionMatrix
    {
        PositionMatrix PositionMatrix { get; }
    }
    interface IGraphics
    {
        Sprite Graphics { set;  get; }
    }
    interface IInput
    {
        void Input(System.Windows.Input.Keyboard keyboard);
    }


}
