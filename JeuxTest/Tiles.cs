using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AsciiEngine
{
    public struct Tile
    {
        public char Char;
        public short Color;
    }

    public static class Tiles
    {
        public static Tile GetTile2FromByte(byte tileId)
        {
            byte tile = (byte)(tileId & 0xF);
            byte color = (byte)(tileId & 0xF0);
            Tile newTile = new Tile();
            newTile.Char = GetCharFromByte(tile);
            newTile.Color = GetColorFromByte(color);
            return newTile;
        }

        public static byte[,] GenerateByteArrayMap(string[] graphic, short[,] colorMatrix = null)
        {
            byte[,] newByteArrayMap = new byte[graphic[0].Length, graphic.Length];
            for(int y = 0; y < graphic.Length; y++)
            {
                for(int x = 0; x < graphic[0].Length; x++)
                {
                    byte newTileId = GetByteFromChar(graphic[y][x]);
                    if(colorMatrix != null)
                        newTileId = (byte)(newTileId | Convert.ToByte(colorMatrix[x, y]));
                    newByteArrayMap[x, y] = newTileId;
                }
            }
            return newByteArrayMap;
        }

        public static byte[,] GenerateByteArrayMap(string[] graphic, ConsoleColor[,] colorMatrix)
        {
            byte[,] newByteArrayMap = new byte[graphic[0].Length, graphic.Length];
            for (int y = 0; y < graphic.Length; y++)
            {
                for (int x = 0; x < graphic[0].Length; x++)
                {
                    byte newTileId = GetByteFromChar(graphic[y][x]);
                    byte newColorId = GetByteFromConsoleColor(colorMatrix[y, x]);
                    newTileId = (byte)(newTileId | newColorId);
                    newByteArrayMap[x, y] = newTileId;
                }
            }
            return newByteArrayMap;
        }

        public static byte GetByteFromConsoleColor(ConsoleColor color)
        {
            byte newByte;
            switch (color)
            {
                case ConsoleColor.Gray:
                    newByte = (byte)0x0;
                    break;
                case ConsoleColor.Blue:
                    newByte = (byte)0x10;
                    break;
                case ConsoleColor.DarkBlue:
                    newByte = (byte)0x20;
                    break;
                case ConsoleColor.Red:
                    newByte = (byte)0x30;
                    break;
                case ConsoleColor.DarkRed:
                    newByte = (byte)0x40;
                    break;
                case ConsoleColor.Yellow:
                    newByte = (byte)0x50;
                    break;
                case ConsoleColor.DarkYellow:
                    newByte = (byte)0x60;
                    break;
                case ConsoleColor.DarkMagenta:
                    newByte = (byte)0x70;
                    break;
                case ConsoleColor.DarkGray:
                    newByte = (byte)0x80;
                    break;
                default:
                    newByte = (byte)0x0;
                    break;
                
            }
            return newByte;
        }

        public static short GetColorFromByte(byte color)
        {
            short tileColor;
            switch (color)
            {
                case (byte)0x0:
                    tileColor = (short)ConsoleColor.Gray;
                    break;
                case (byte)0x10:
                    tileColor = (short)ConsoleColor.Blue;
                    break;
                case (byte)0x20:
                    tileColor = (short)ConsoleColor.DarkBlue;
                    break;
                case (byte)0x30:
                    tileColor = (short)ConsoleColor.Red;
                    break;
                case (byte)0x40:
                    tileColor = (short)ConsoleColor.DarkRed;
                    break;
                case (byte)0x50:
                    tileColor = (short)ConsoleColor.Yellow;
                    break;
                case (byte)0x60:
                    tileColor = (short)ConsoleColor.DarkYellow;
                    break;
                case (byte)0x70:
                    tileColor = (short)ConsoleColor.DarkMagenta;
                    break;
                case (byte)0x80:
                    tileColor = (short)ConsoleColor.DarkGray;
                    break;
                default:
                    tileColor = (short)ConsoleColor.Gray;
                    break;
            }
            return tileColor;
        }
        public static byte GetByteFromChar(char charGraphic)
        {
            byte newByte;
            switch (charGraphic)
            {
                case ' ':
                    newByte = (byte)0x0;
                    break;
                case '#':
                    newByte = (byte)0x1;
                    break;
                case 'O':
                    newByte = (byte)0x2;
                    break;
                case 'X':
                    newByte = (byte)0x3;
                    break;
                case '*':
                    newByte = (byte)0x4;
                    break;
                case '+':
                    newByte = (byte)0x5;
                    break;
                case '.':
                    newByte = (byte)0x6;
                    break;
                default:
                    newByte = (byte)0x0;
                    break;
            }
            return newByte;

        }


        public static char GetCharFromByte(byte tile)
        {
            char tileChar;
            switch (tile)
            {
                case (byte)0x0:
                    tileChar = ' ';
                    break;
                case (byte)0x1:
                    tileChar = '#';
                    break;
                case (byte)0x2:
                    tileChar = 'O';
                    break;
                case (byte)0x3:
                    tileChar = 'X';
                    break;
                case (byte)0x4:
                    tileChar = '*';
                    break;
                case (byte)0x5:
                    tileChar = '+';
                    break;
                case (byte)0x6:
                    tileChar = '.';
                    break;
                default:
                    tileChar = ' ';
                    break;
            }
            return tileChar;
        }
    }
   
}
