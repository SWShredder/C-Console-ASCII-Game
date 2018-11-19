using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsciiEngine
{
    public class Sprite : ISize
    {
        private Vector2 size;
        public byte[,] TilesByteMap;
     
        public Vector2 Size
        {
            get
            {
                if (size == null)
                    size = new Vector2(this.TilesByteMap.GetLength(0), this.TilesByteMap.GetLength(1));
                return size;
            }
        }
        public Byte this[int x, int y]
        {
            get => TilesByteMap[x, y];
            set => TilesByteMap[x, y] = value;
        }

        // INITIALIZATION
        public Sprite(string[] charArray)
        {
            TilesByteMap = Tiles.GenerateByteArrayMap(charArray);
        }

        public Sprite(string[] charArray, ConsoleColor[,] colorMatrix)
        {
            TilesByteMap = Tiles.GenerateByteArrayMap(charArray, colorMatrix);
        }

        public static Sprite Generate(string[] graphics, ConsoleColor[,] colorMatrix = null)
        {
            Sprite newSprite = colorMatrix == null ? new Sprite(graphics) : new Sprite(graphics, colorMatrix);
            //newSprite.TilesByteMap = colorMatrix == null ? Tiles.GenerateByteArrayMap(graphics) : Tiles.GenerateByteArrayMap(graphics, colorMatrix);
            return newSprite;
        }
    }
}

