using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using static AsciiEngine.Utility;

namespace AsciiEngine
{

    /// <summary>
    /// This class's instance is used to generate a ScreenBack buffer to make rendering colors faster.
    /// The class is a wrapper around Kernel32 CreatFile and the low level WriteConsoleOutput.
    /// </summary>
    public class ScreenBuffer : IPosition, ISize
    {
        private readonly SafeFileHandle Handler;
        // FIELDS //
        private Vector2 position;
        private Vector2 size;
        private CharInfo[] buffer;
        private SmallRect area;
        //private SafeFileHandle handler;

        // PROPERTIES //
        /// <summary>
        /// canvas is used by the engine and ScreenBuffer's method canvasToBuffer to render the backbuffer
        /// </summary>
        public Canvas Canvas
        {
            set => buffer = CanvasToBuffer(value);
        }
        /// <summary>
        /// Position is used to determine the render area's position on the screen. Never returns null since
        /// default value is a vector2(0,0)
        /// </summary>
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
        public void SetPosition(Vector2 newPosition)
        {
            position = newPosition;
        }

        public Vector2 GetPosition()
        {
            if (position == null)
                return Vec2(0, 0);
            else
                return position;
        }

 
        /// <summary>
        /// A vector2 that is used to represents the size of the render area on the screen. Always returns at least
        /// vector2(0,0)
        /// </summary>
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
        public Vector2 GetSize()
        {
            if (size == null)
                return Vec2(0, 0);
            else
                return size;
        }

        private void SetSize(Vector2 newSize)
        {
            size = newSize;

        }
        // CONSTRUCTOR //
        /// <summary>
        /// A handler is created to handle the file which is a console buffer at the creation of ScreenBuffer's instances
        /// </summary>
        public ScreenBuffer()
        {
            if(Handler == null)
                Handler = CreateFile("CONOUT$", 0x40000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);
        }

        // METHODS //
        /// <summary>
        /// The method used to draw the ScreenBuffer to the speed. It uses ScreenBuffer's size and position to determine
        /// the area of the screen where to draw.
        /// </summary>
        public void Draw()
        {
            area.Left = (short)position.X;
            area.Top = (short)position.Y;
            area.Right = (short)(position.X + size.X);
            area.Bottom = (short)(position.Y + size.Y);

            bool b = WriteConsoleOutput(Handler, buffer,
              new Coord() { X = (short)Size.X, Y = (short)Size.Y },
              //new Coord() { X = (short)Position.X, Y = (short)Position.Y },
              new Coord() { X = 0, Y = 0 },
              ref area);

        }



        /// <summary>
        /// Returns an array of charInfo that is used by the WriteConsoleOutput to render the buffer. CharInfo
        /// is the same as a Tile in canvas.
        /// </summary>
        /// <param name="canvas">The canvas represents the object from which the CharInfo will be extracted</param>
        /// <returns></returns>
        private CharInfo[] CanvasToBuffer(Canvas canvas)
        {
            CharInfo[] newBuffer = new CharInfo[canvas.Size.Y * canvas.Size.X];
            if (canvas == null)
                return newBuffer;

            for (int y = 0; y < canvas.Size.Y; ++y)
            {
                for (int x = 0; x < canvas.Size.X; ++x)
                {
                    newBuffer[(y * canvas.Size.X) + x].Char.UnicodeChar = canvas[x, y].Char;
                    newBuffer[(y * canvas.Size.X) + x].Attributes = (short)canvas[x, y].Color;
                }
            }
            return newBuffer;
        }
        public void RenderCanvasToBuffer(Canvas renderArea, Vector2 size, Vector2 position)
        {
            CharInfo[] newBuffer = new CharInfo[size.Y * size.X];
            if (renderArea == null)
            {
                buffer = newBuffer;
                return;
            }
            SetSize(size);


            for (int y = position.Y, y2 = 0; y < position.Y + size.Y; ++y, ++y2)
            {
                for (int x = position.X, x2 = 0; x < position.X + size.X; ++x, ++x2)
                {
                    newBuffer[(y2 * size.X) + x2].Char.UnicodeChar = renderArea[x, y].Char;
                    newBuffer[(y2 * size.X) + x2].Attributes = (short)renderArea[x, y].Color;
                }
            }
            buffer = newBuffer;
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
