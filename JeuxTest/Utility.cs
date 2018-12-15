using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsciiEngine
{
    public enum Direction
    {
        North, South, East, West
    }



    public abstract class ObjectSignal
    {
    
    }

    public class ObjectStatsSignal : ObjectSignal
    {
        public GameObject source { set; get; }
        public double Damage { set; get; }
    }

    public class ObjectPhysicsSignal : ObjectSignal
    {
        public double CollisionVectorX { set; get; }
        public double CollisionVectorY { set; get; }
        public double Force { set; get; }
    }

    public class RenderUpdateSignal
    {
        public Vector2 Position;
        public byte[,] ByteMap;
    }
    public class CollisionUpdateSignal
    {
        public bool IsTrigger = false;
        public INodes Source;
        public Vector2 Position;
        public bool[,] CollisionMap;
    }
    public class PositionUpdateSignal
    {
        public INodes Source;
        public Vector2 NewPosition;
    }

    public class ChunkTransferSignal
    {
        public INodes Source;
        public Vector2 NewChunkPosition;
    }

    static public class Utility
    {
        public static bool GetRotated90degreeMatrix(byte[,] matrix, out byte[,] newByteArray)
        {
            if (matrix == null)
            {
                newByteArray = new byte[0, 0];
                return false;
            }
                
            newByteArray = new byte[matrix.GetLength(1), matrix.GetLength(0)];
            int indexY = matrix.GetLength(1);
            for(int x = 0; x < matrix.GetLength(1); ++x)
            {
                --indexY;
                for(int y = 0; y < matrix.GetLength(0); ++y)
                {
                    newByteArray[x, y] = matrix[y, indexY];
                }
            }
            return true;
        }

        public static bool GetRotated90DegreeMatrix(bool[,] matrix, out bool[,] newBoolArray)
        {
            if (matrix == null)
            {
                newBoolArray = new bool[0, 0];
                return false;
            }

            newBoolArray = new bool[matrix.GetLength(1), matrix.GetLength(0)];
            int indexY = matrix.GetLength(1);
            for (int x = 0; x < matrix.GetLength(1); ++x)
            {
                --indexY;
                for (int y = 0; y < matrix.GetLength(0); ++y)
                {
                    newBoolArray[x, y] = matrix[y, indexY];
                }
            }
            return true;
        }

        public static Vector2 GetWindowSize()
        {
            return new Vector2(Console.WindowWidth, Console.WindowHeight);
        }
        public static Vector2 Vec2(int x, int y)
        {
            return new Vector2(x, y);
        }
        public static long GetEngineTicks()
        {
            return Engine.Instance.CoreUpdate.EngineTicks;
        }

    }

 

    
    public class VectorP
    {
        public double X;
        public double Y;

        public VectorP()
        {
            X = 0;
            Y = 0;
        }

        public VectorP(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static VectorP operator - (VectorP vec1, VectorP vec2)
        {
            return new VectorP(vec1.X - vec2.X, vec1.Y - vec2.Y); 
        }
        public static VectorP operator +(VectorP vec1, VectorP vec2)
        {
            return new VectorP(vec1.X + vec2.X, vec1.Y + vec2.Y);
        }

        public override string ToString()
        {
            string newString = String.Format("({0:N3};{1:N3})", this.X, this.Y);
            return newString;
        }
    }

    public class Vector2
    {
        private int x;
        private int y;

        public override bool Equals(object obj)
        {
            Vector2 vec = (Vector2)obj;
            if (obj == null)
                return false;
            return this.X == vec.X && this.Y == vec.Y;
        }

        public override int GetHashCode()
        {
            return this.X.GetHashCode() * 17 + this.Y.GetHashCode();
        }

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
}
