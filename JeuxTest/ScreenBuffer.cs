using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using static Game.Utility;

namespace Game
{
    public class ScreenBuffer : IPosition, ISize
    {
        private Vector2 position;
        private Vector2 size;
        private CharInfo[] buffer;
        private SmallRect area;
        private SafeFileHandle Handler;

        public Sprite Sprite
        {
            set => buffer = SpriteToBuffer(value);
        }
        public Vector2 Position
        {
            set
            {
                position = value;
                area.Left = (short)Position.X;
                area.Top = (short)Position.Y;
            }
            get
            {
                if (position == null)
                    return Vec2(0, 0);
                else
                    return position;
            }
        }

        public Vector2 Size
        {
            set
            {
                size = value;
                area.Right = (short)Size.X;
                area.Bottom = (short)Size.Y;
            }
            get
            {
                if (size == null)
                    return Vec2(0, 0);
                else
                    return size;
            }
        }

        public ScreenBuffer()
        {
            Handler = CreateFile("CONOUT$", 0x40000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);
        }
        public void Draw()
        {
            bool b = WriteConsoleOutput(Handler, buffer,
              new Coord() { X = (short)Size.X, Y = (short)Size.Y },
              new Coord() { X = (short)Position.X, Y = (short)Position.Y },
              ref area);

        }

        private CharInfo[] SpriteToBuffer(Sprite sprite)
        {
            CharInfo[] newBuffer = new CharInfo[sprite.Size.Y * sprite.Size.X];
            if (sprite == null)
                return newBuffer;

            for (int y = 0; y < sprite.Size.Y; ++y)
            {
                for (int x = 0; x < sprite.Size.X; ++x)
                {
                    newBuffer[(y * sprite.Size.X) + x].Char.UnicodeChar = sprite[x, y].Char;
                    newBuffer[(y * sprite.Size.X) + x].Attributes = (short)sprite[x, y].Color;
                }
            }
            return newBuffer;
        }



        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern SafeFileHandle CreateFile(
            string fileName,
            [MarshalAs(UnmanagedType.U4)] uint fileAccess,
            [MarshalAs(UnmanagedType.U4)] uint fileShare,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] int flags,
            IntPtr template);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteConsoleOutput(
          SafeFileHandle hConsoleOutput,
          CharInfo[] lpBuffer,
          Coord dwBufferSize,
          Coord dwBufferCoord,
          ref SmallRect lpWriteRegion);

        [StructLayout(LayoutKind.Sequential)]
        public struct Coord
        {
            public short X;
            public short Y;

            public Coord(short X, short Y)
            {
                this.X = X;
                this.Y = Y;
            }
        };

        [StructLayout(LayoutKind.Explicit)]
        public struct CharUnion
        {
            [FieldOffset(0)] public char UnicodeChar;
            [FieldOffset(0)] public byte AsciiChar;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct CharInfo
        {
            [FieldOffset(0)] public CharUnion Char;
            [FieldOffset(2)] public short Attributes;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SmallRect
        {
            public short Left;
            public short Top;
            public short Right;
            public short Bottom;
        }

    }
}
