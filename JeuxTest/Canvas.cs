using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsciiEngine
{
    public class Canvas
    {
        private Vector2 size;
        public Tile[,] TileTable;

        public Vector2 Size => size;

        public Tile this[int x, int y]
        {
            get => TileTable[x, y];
            set => TileTable[x, y] = value;
        }

        public Canvas(Vector2 size)
        {
            this.size = size;
            TileTable = GetCanvas(size);
        }

        public Tile[,] GetCanvas(Vector2 size)
        {
            Tile[,] newCanvas = new Tile[size.X, size.Y];
            for (int y = 0; y < size.Y; ++y)
            {
                for (int x = 0; x < size.X; ++x)
                {
                    newCanvas[x, y] = new Tile(' ');
                }
            }
            return newCanvas;
        }
    }
}
