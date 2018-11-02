using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{

    public class Sprite : ISize
    {
        public object Parent;
        public Vector2 ScreenPosition;

        public Vector2 Size
        {
            get
            {
                return new Vector2(this.TileTable.GetLength(0), this.TileTable.GetLength(1));
            }
        }
        public Tile[,] TileTable;
        public string[] GraphicsBuffer;

        public Sprite(string[] charArray)
        {
            TileTable = GenerateTileTable(charArray);
            GraphicsBuffer = charArray;
        }
        public Sprite(string[] charArray, ConsoleColor[,] colorMatrix)
        {
            TileTable = GenerateTileTable(charArray, colorMatrix);
            GraphicsBuffer = charArray;
        }
        // Indexer
        public Tile this[int x, int y]
        {
            get => TileTable[x, y];
            set => TileTable[x, y] = value;
        }
        public void Draw()
        {
            for(int y = 0; y < TileTable.GetLength(1); y++)
            {
                string bufferLine = "";
                for(int x = 0; x < TileTable.GetLength(0); x++)
                {
                    if (TileTable[x, y].Color == ConsoleColor.Gray || Program.RenderMode == 1)
                        bufferLine += TileTable[x, y].Char;
                    else
                        bufferLine += " ";
                }
                GraphicsBuffer[y] = bufferLine;
            }
            Console.SetCursorPosition(0, 0);
            foreach(string line in GraphicsBuffer)
            {
                Console.WriteLine(line);
            }            
        }

        public void SelfDraw()
        {

        }
        private Tile[,] GenerateTileTable(string[] charArray, ConsoleColor[,] colorMatrix)
        {
            Tile[,] newTileTable = new Tile[charArray[0].Length, charArray.Length];
            for(int y = 0; y < charArray.Length; y++)
            {
                for (int x = 0; x < charArray[0].Length; x++)
                {
                    newTileTable[x, y] = new Tile(charArray[y][x], colorMatrix[y, x]);
                }
            }
            return newTileTable;
        }
        private Tile[,] GenerateTileTable(string[] charArray)
        {
            Tile[,] newTileTable = new Tile[charArray[0].Length, charArray.Length];
            for (int y = 0; y < charArray.Length; y++)
            {
                for (int x = 0; x < charArray[0].Length; x++)
                {
                    newTileTable[x, y] = new Tile(charArray[y][x], ConsoleColor.Gray);
                }
            }
            return newTileTable;
        }

        public class Tile
        {
            private ConsoleColor color;
            private Color _Color;
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
            public override string ToString()
            {
                return String.Format("'{0}':{1}", Char, Color);
            }
        }
        
    }
    public class Color
    {
        private int Red;
        private int Green;
        private int Blue;

        public Color(byte red, byte green, byte blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }
        public override string ToString()
        {
            return String.Format("{0},{1},{2}", Red, Green, Blue);
        }
    }
}
