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
        public Tile[,] TileTable;

        // PROPERTIES //       
        public Vector2 Size
        {
            get
            {
                if (size == null)
                    size = new Vector2(this.TileTable.GetLength(0), this.TileTable.GetLength(1));
                return size;
            }
        }

        public Vector2 GetSize()
        {
            if (size == null)
                size = new Vector2(this.TileTable.GetLength(0), this.TileTable.GetLength(1));
            return size;
        }
        // Indexer
        public Tile this[int x, int y]
        {
            get => TileTable[x, y];
            set => TileTable[x, y] = value;
        }

        // INITIALIZATION
        public Sprite(string[] charArray)
        {
            TileTable = GenerateTileTable(charArray);
        }

        public Sprite(string[] charArray, ConsoleColor[,] colorMatrix)
        {

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

    }
}

