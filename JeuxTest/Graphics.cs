using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AsciiEngine.Utility;

namespace AsciiEngine
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
        public Sprite Sprite
        {
            set
            {
                sprite = value;
            }
            get
            {
                return sprite;
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



        // Allows the class to generate an empty frame of any size.
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



    public class Animation
    {
        public string Name;
        public Frame[] Frames;
        public int Length;
        public int FrameIndex;
        public bool IsPlaying;
        public int Speed;

    }




    public class Graphics
    {
        public delegate void DrawEventHandler(object source, DrawRequestEventArgs args);
        public event DrawEventHandler DrawRequest;

        public Dictionary<string, Animation> Animations = new Dictionary<string, Animation>();
        private object parent;
        public object Parent => parent;
        public Animation CurrentAnimation;
        private Frame currentFrame;
        public Frame CurrentFrame
        {
            get
            {
                if (currentFrame == null)
                {
                    currentFrame = new Frame((parent as GameObject).Graphics);

                }
                return currentFrame;
            }
            set
            {
                currentFrame = value;
            }
        }

        private Vector2 screenPosition;
        public Vector2 ScreenPosition
        {
            get
            {
                return screenPosition;
            }
            set
            {
                screenPosition = value;
            }
        }

        public Graphics(object _parent)
        {
            parent = _parent;
            // Signal registry for Camera
            Camera.Instance.CameraPositionChanged += OnCameraPositionChanged;
            // Signal registry for ScreenRenderer
            DrawRequest += Core.ScreenRenderer.OnDrawRequest;
            // Signal registry with Parent
            (parent as GameObject).ObjectPositionChanged += OnParentObjectPositionChanged;

        }



        protected virtual void OnDrawRequest(Vector2 oldPosition, Vector2 newPosition)
        {
            DrawRequest?.Invoke(parent, new DrawRequestEventArgs()
            {
                OldPosition = oldPosition,
                NewPosition = newPosition,
                Sprite = CurrentFrame.Sprite,
            });
        }


        public void OnParentObjectPositionChanged(object source, ObjectPositionEventArgs args)
        {
            // Insert New code Here; 
            /*Vector2 oldPosition = args.OldPosition;
            Vector2 newPosition = args.NewPosition;
            Vector2 parentPosition = (source as IPosition).Position;
            OnDrawRequest(oldPosition, newPosition);*/

            Vector2 oldPosition = args.OldPosition - Camera.Instance.Position;
            Vector2 newPosition = args.NewPosition - Camera.Instance.Position;
            OnDrawRequest(oldPosition, newPosition);
        }

        public void OnCameraPositionChanged(object source, CameraPositionEventArgs args)
        {
            Vector2 oldCameraPosition = args.OldPosition;
            Vector2 newCameraPosition = args.NewPosition;

           if (Camera.Instance.ObjectFocused == Parent)
                return;
            Vector2 parentPosition = (Parent as IPosition) != null ? (Parent as IPosition).Position : new Vector2(0, 0);

            Vector2 oldParentScreenPosition = parentPosition - oldCameraPosition;
            Vector2 newParentScreenPosition = parentPosition - newCameraPosition;

            OnDrawRequest(oldParentScreenPosition , newParentScreenPosition);
        }
    }
}