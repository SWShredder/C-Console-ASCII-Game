using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsciiEngine
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
        public static long GetEngineTicks()
        {
            return Core.Engine.GameUpdate.EngineTicks;
        }

    }

    public class Tile
    {
        private ConsoleColor color;
        private Char _Char;
        // Properties are Color and Char
        public ConsoleColor Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
            }
        }
        public char Char
        {
            private set
            {
                _Char = value;
            }
            get
            {
                return _Char;
            }
        }
        // Basic constructor.
        public Tile(char _char, ConsoleColor _color)
        {
            Char = _char;
            Color = _color;
        }
        public Tile(char _char)
        {
            Char = _char;
            Color = ConsoleColor.Gray;
        }
        public override string ToString()
        {
            return String.Format("'{0}':{1}", Char, Color);
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
