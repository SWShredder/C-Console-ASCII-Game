using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Game.Utility;

namespace Game
{

    public class Sprite : ISize, IPosition
    {



        static public List<Sprite> List = new List<Sprite>();


        public Tile[,] TileTable;
        public string[] spriteBuffer;
        public Vector2 ScreenPosition;
        private Vector2 position;

        // PROPERTIES //
        public Vector2 Position
        {
            set
            {
                position = value;
            }
            get
            {
                if (position == null)
                    return new Vector2(0, 0);
                return position;
            }
        }
        public Vector2 Size
        {
            get
            {
                return new Vector2(this.TileTable.GetLength(0), this.TileTable.GetLength(1));
            }
        }

        // Indexer
        public Tile this[int x, int y]
        {
            get => TileTable[x, y];
            set => TileTable[x, y] = value;
        }



        // INITIALIZATION
        private void Initialize(string[] charArray)
        {
            List.Add(this);
            spriteBuffer = charArray;
        }

        public Sprite(string[] charArray)
        {
            Initialize(spriteBuffer = charArray);
            TileTable = GenerateTileTable(charArray);
        }

        public Sprite(string[] charArray, ConsoleColor[,] colorMatrix)
        {
            Initialize(spriteBuffer = charArray);
            TileTable = GenerateTileTable(charArray, colorMatrix);
        }





        private Tile[,] GenerateTileTable(string[] charArray, ConsoleColor[,] colorMatrix)
        {
            Tile[,] newTileTable = new Tile[charArray[0].Length, charArray.Length];
            for (int y = 0; y < charArray.Length; y++)
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





    public class Frame : IPosition, ISize
    {
        private Sprite sprite;
        private Vector2 position = Vec2(0, 0);
        private Vector2 size = Vec2(0, 0);
        public Vector2 Position
        {
            set;
            get;
        }
        public Vector2 Size
        {
            set
            {
                sprite = EmptyFrame(value);
            }
            get
            {
                if (sprite == null)
                    return size;
                else
                    return sprite.Size;
            }
        }

        public Frame(Sprite sprite)
        {
            this.sprite = sprite;
            this.size = sprite.Size;
        }

        public Frame()
        {

        }

        private Sprite EmptyFrame(Vector2 size)
        {
            string[] emptyFrame = new string[size.Y];
            for (int y = 0; y < size.Y; y++)
            {
                string emptyLine = "";
                for (int x = 0; x < size.X; x++)
                {
                    emptyLine += " ";
                }
                emptyFrame[y] = emptyLine;
            }
            return new Sprite(emptyFrame);
        }
    }

}