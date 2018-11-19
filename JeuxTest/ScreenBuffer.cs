using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using static AsciiEngine.Utility;

namespace AsciiEngine
{ 

    public class ScreenBuffer : IPosition, ISize
    {
        private readonly SafeFileHandle Handler;
        private Vector2 position;
        private Vector2 size;
        private CharInfo[] buffer;
        private SmallRect area;


        public Vector2 Position
        {
            set
            {
                position = value;
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
            buffer = new CharInfo[Console.LargestWindowWidth * Console.LargestWindowHeight];
        }

 
        public void Draw()
        {
            area.Left = (short)position.X;
            area.Top = (short)position.Y;
            area.Right = (short)(position.X + size.X);
            area.Bottom = (short)(position.Y + size.Y);

            bool b = WriteConsoleOutput(Handler, buffer,
              new Coord() { X = (short)Size.X, Y = (short)Size.Y },
              new Coord() { X = 0, Y = 0 },
              ref area);

        }
        /*
        public void RenderByteMapToBuffer(byte[,] renderArea, Vector2 size, Vector2 position)
        {
            CharInfo[] newBuffer = new CharInfo[size.Y * size.X];
            if (renderArea == null)
            {
                buffer = newBuffer;
                return;
            }
            this.Size = size;

            for (int y = position.Y, y2 = 0; y < position.Y + size.Y; ++y, ++y2)
            {
                for (int x = position.X, x2 = 0; x < position.X + size.X; ++x, ++x2)
                {
                    byte newChar = (byte)(renderArea[x, y] & 0xF);
                    byte newColor = (byte)(renderArea[x, y] & 0xF0);
                    newBuffer[(y2 * size.X) + x2].Char.UnicodeChar = Tiles.GetCharFromByte(newChar);
                    newBuffer[(y2 * size.X) + x2].Attributes = Tiles.GetColorFromByte(newColor);

                }

            }
            buffer = newBuffer;
        }
        */
        public void DrawByteMapToBuffer(byte[,] renderArea, Vector2 size, Vector2 position)
        {
            if (renderArea == null) return;
            if (size == null)
                size = new Vector2();
            int sizeY = size.Y;
            int sizeX = size.X;
            if (position == null)
                position = new Vector2();
            int positionX = position.X;
            int positionY = position.Y;

            buffer = new CharInfo[size.Y * sizeX];
            this.Size = size;

            for (int y = positionY, y2 = 0; y < positionY + sizeY; ++y, ++y2)
            {
                for (int x = positionX, x2 = 0; x < positionX + sizeX; ++x, ++x2)
                {
                    byte newChar = (byte)(renderArea[x, y] & 0xF);
                    byte newColor = (byte)(renderArea[x, y] & 0xF0);
                    buffer[(y2 * sizeX) + x2].Char.UnicodeChar = Tiles.GetCharFromByte(newChar);
                    buffer[(y2 * sizeX) + x2].Attributes = Tiles.GetColorFromByte(newColor);
                }
            }
        }

        // Used by the class to get access to the WriteConsoleOutput and CreateFile
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
        private struct Coord
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
        private struct CharUnion
        {
            [FieldOffset(0)] public char UnicodeChar;
            [FieldOffset(0)] public byte AsciiChar;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct CharInfo
        {
            [FieldOffset(0)] public CharUnion Char;
            [FieldOffset(2)] public short Attributes;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SmallRect
        {
            public short Left;
            public short Top;
            public short Right;
            public short Bottom;
        }

    }
}
