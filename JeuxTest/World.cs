using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsciiEngine
{
    public class World : INodes
    {
        static public int Seed { get; private set; }
        static public Player Player { private set; get; }
        static public World Instance { private set; get; }
        public static Random Random { private set; get; }
        static public readonly double BiomeVoidOccurence = 0.25;
        static public readonly double BiomeBalancedOccurence = 0.20;
        static public readonly double BiomeStarryOccurence = 0.5;
        static public readonly double BiomeAsteroidsOccurence = 0.05;

        public Dictionary<Vector2, Chunk> HashTable;
        public string Name { set; get; }
        public INodes Parent { set; get; }
        public List<INodes> Children { private set; get; }      
        public Vector2 PlayerChunk { set; get; }
        public List<ChunkTransferSignal> TransferQueries { set; get; }


        public World(INodes parent)
        {
            Instance = this;
            Seed = new Random().Next();
            Parent = parent;
            Parent.AddChild(this);
        }

        public void Initialize()
        {
            Random = new Random(Seed);
            Children = new List<INodes>();    
            HashTable = new Dictionary<Vector2, Chunk>();
            TransferQueries = new List<ChunkTransferSignal>();
            Player = new Player(Core.PlayerGraphic, Core.PlayerGraphicColorMatrix);
            Player.Position = new Vector2();
            var playerChunk = GetChunk(Player.Position);
            Player.Parent = playerChunk;
            playerChunk.AddChild(Player);
            Core.Engine.Camera.SetFocus(Player);
            UpdateChildrenList();
        }


        public void Update()
        {
            UpdateChildrenList();
            foreach(INodes chunk in Children)
            {
                chunk.Update();
            }
            ProcessTransferQueries();
        }

        private void UpdateChildrenList()
        {
            List<INodes> newActiveChunks = new List<INodes>();
            Vector2[] newChunkPositionArray = GetActiveChunksPositions(Player.Position);
            foreach (Vector2 chunkPosition in newChunkPositionArray)
            {
                var chunk = GetChunk(chunkPosition);
                newActiveChunks.Add(chunk as INodes);
                chunk.Parent = (this as INodes);
            }
            Children = newActiveChunks;

        }

        public void ProcessTransferQueries()
        {
            foreach(ChunkTransferSignal signal in TransferQueries)
            {
                var newChunkPosition = signal.NewChunkPosition;
                INodes source = signal.Source;
                var newChunk = GetChunk(newChunkPosition);
                newChunk.AddChild(source);
                source.Parent = newChunk;

            }
            TransferQueries = new List<ChunkTransferSignal>();
        }

        public void AddTransferQuery(INodes child, Vector2 chunkPosition)
        {
            TransferQueries.Add(new ChunkTransferSignal()
            {
                Source = child,
                NewChunkPosition = chunkPosition
            });

        }

        private Chunk GetChunk(Vector2 chunkPosition)
        {
            if (!HashTable.ContainsKey(chunkPosition))
            {
                return GenerateChunk(chunkPosition);
            }
            return HashTable[chunkPosition];
        }

        private Chunk GenerateChunk(Vector2 chunkPosition)
        {
            Chunk newChunk = new Chunk(chunkPosition);
            HashTable.Add(chunkPosition, newChunk);
            return newChunk;
        }

        private Vector2[] GetActiveChunksPositions(Vector2 playerPosition)
        {
            var newList = new Vector2[9];
            int chunkPositionX = Player.Position.X >= 0 ? Player.Position.X / Engine.ChunkSize.X * Engine.ChunkSize.X 
                : (Player.Position.X - Engine.ChunkSize.X) / Engine.ChunkSize.X * Engine.ChunkSize.X;
            int chunkPositionY = Player.Position.Y >= 0 ? Player.Position.Y / Engine.ChunkSize.Y * Engine.ChunkSize.Y
                : (Player.Position.Y - Engine.ChunkSize.Y) / Engine.ChunkSize.Y * Engine.ChunkSize.Y;
            Vector2 chunkSize = Engine.ChunkSize;

            newList[0] = new Vector2(chunkPositionX - chunkSize.X, chunkPositionY - chunkSize.Y);
            newList[1] = new Vector2(chunkPositionX, chunkPositionY - chunkSize.Y);
            newList[2] = new Vector2(chunkPositionX + chunkSize.X, chunkPositionY - chunkSize.Y);
            newList[3] = new Vector2(chunkPositionX - chunkSize.X, chunkPositionY);
            newList[4] = new Vector2(chunkPositionX, chunkPositionY);
            newList[5] = new Vector2(chunkPositionX + chunkSize.X, chunkPositionY);
            newList[6] = new Vector2(chunkPositionX - chunkSize.X, chunkPositionY + chunkSize.Y);
            newList[7] = new Vector2(chunkPositionX, chunkPositionY + chunkSize.Y);
            newList[8] = new Vector2(chunkPositionX + chunkSize.X, chunkPositionY + chunkSize.Y);

            return newList;
        }

        public void AddChild(INodes child)
        {
            Children.Add(child);
        }
        public void RemoveChild(INodes child)
        {
            Children.Remove(child);
        }
    }
}
