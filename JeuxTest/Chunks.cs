using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsciiEngine
{

    public class Chunk : INodes
    {
        public long ChunkId { set; get; }
        public Vector2 Size { get; }
        public Vector2 Position { get; }
        public Biome Biome { get; }
        public List<INodes> Children { set; get; }
        public INodes Parent { set; get; }
        public List<PositionUpdateSignal> MovementQueries { set; get; }
        public List<INodes> DeletionQueries { set; get; }

        public Chunk(Vector2 chunkPosition)
        {
            Position = chunkPosition;
            Size = new Vector2(Engine.ChunkSize.X, Engine.ChunkSize.Y);
            Children = new List<INodes>();
            Biome = GetBiome(GetBiomeType());
            Initialize();
        }

        public void Initialize()
        {
            Children = new List<INodes>();
            Generate(Position);
            MovementQueries = new List<PositionUpdateSignal>();
            DeletionQueries = new List<INodes>();
        }

        public void Update()
        {
            List<INodes> childrenList = Children;
            for (int i = 0; i < childrenList.Count; ++i)
            {
                childrenList[i].Update();
            }
            ProcessMovementQueries();
            ProcessDeletionQueries();
        }

        public void ProcessDeletionQueries()
        {
            foreach (INodes child in DeletionQueries)
            {
                child.Parent = null;
                RemoveChild(child);
            }
            DeletionQueries = new List<INodes>();
        }

        public void ProcessMovementQueries()
        {
            foreach (PositionUpdateSignal signal in MovementQueries)
            {
                var newPosition = signal.NewPosition;
                int chunkPositionX = World.Player.Position.X >= 0 ? World.Player.Position.X / Engine.ChunkSize.X * Engine.ChunkSize.X
                    : (World.Player.Position.X - Engine.ChunkSize.X) / Engine.ChunkSize.X * Engine.ChunkSize.X;
                int chunkPositionY = World.Player.Position.Y >= 0 ? World.Player.Position.Y / Engine.ChunkSize.Y * Engine.ChunkSize.Y
                    : (World.Player.Position.Y - Engine.ChunkSize.Y) / Engine.ChunkSize.Y * Engine.ChunkSize.Y;

                if (chunkPositionX != this.Position.X || chunkPositionY != this.Position.Y)
                {
                    INodes source = signal.Source as INodes;
                    World.Instance.AddTransferQuery(source, new Vector2(chunkPositionX, chunkPositionY));
                    RemoveChild(source);
                }
            }
            MovementQueries = new List<PositionUpdateSignal>();
        }

        public void AddDeletionQuery(INodes source)
        {
            DeletionQueries.Add(source);
        }

        public void AddMovementQuery(INodes source, Vector2 newPosition)
        {
            MovementQueries.Add(new PositionUpdateSignal()
            {
                Source = source,
                NewPosition = newPosition
            });

        }

        public void AddChild(INodes child)
        {
            Children.Add(child);
        }
        public void RemoveChild(INodes child)
        {
            Children.Remove(child);
        }

        public BiomeType GetBiomeType()
        {
            double random = World.Random.NextDouble();
            if (random >= (World.BiomeBalancedOccurence + World.BiomeStarryOccurence + World.BiomeAsteroidsOccurence)) return BiomeType.Void;
            else if (random >= (World.BiomeBalancedOccurence + World.BiomeAsteroidsOccurence)) return BiomeType.Starry;
            else if (random >= (World.BiomeBalancedOccurence)) return BiomeType.Asteroids;
            else return BiomeType.Balanced;
        }

        public Biome GetBiome(BiomeType biomeType)
        {
            if (biomeType == BiomeType.Balanced)
                return new BiomeBalanced();
            if (biomeType == BiomeType.Starry)
                return new BiomeStarry();
            if (biomeType == BiomeType.Void)
                return new BiomeVoid();
            if (biomeType == BiomeType.Asteroids)
                return new BiomeAsteroids();

            return new BiomeVoid();
        }

        public void Generate(Vector2 chunkPosition)
        {
            Random RandomVal = World.Random;
            PopulateStardust(Biome.StarDensity + RandomVal.Next(0, Biome.StarDensity), chunkPosition);
            PopulateAsteroids(Biome.AsteroidDensity + RandomVal.Next(0, Biome.AsteroidDensity), chunkPosition);
        }

        public string[] GetAsteroidGraphic(int random)
        {
            string[] newGraphic;
            switch (random)
            {
                case 1:
                case 2:
                case 3:
                    newGraphic = Core.AsteroidGraphic;
                    break;
                case 4:
                case 5:
                    newGraphic = Core.AsteroidGraphic2;
                    break;
                case 7:
                    newGraphic = Core.AsteroidGraphic4;
                    break;
                default:
                    newGraphic = Core.AsteroidGraphic3;
                    break;

            }
            return newGraphic;
        }

        public void PopulateAsteroids(int amountAsteroids, Vector2 chunkPosition)
        {
            Random random = World.Random;
            GameObject[] asteroids = new GameObject[amountAsteroids];
            foreach (GameObject gameObject in asteroids)
            {
                Vector2 randomPosition = new Vector2(random.Next(chunkPosition.X, chunkPosition.X +
                    Engine.ChunkSize.X), random.Next(chunkPosition.Y, chunkPosition.Y + Engine.ChunkSize.Y));
                PopulateAsteroid(randomPosition, random);
            }
        }

        void PopulateAsteroid(Vector2 position, Random random)
        {
            double newMass;
            var newGraphic = GetAsteroidGraphic(random.Next(0, 8));
            if (newGraphic == Core.AsteroidGraphic)
                newMass = 5;
            else if (newGraphic == Core.AsteroidGraphic2)
                newMass = 2;
            else if (newGraphic == Core.AsteroidGraphic3)
                newMass = 2;
            else if (newGraphic == Core.AsteroidGraphic4)
                newMass = 20;
            else
                newMass = 200;

            GameObject newAsteroid = new GameObject(newGraphic)
            {
                Position = position,
                Parent = this,

            };
            Children.Add(newAsteroid as INodes);
            newAsteroid.PhysicsBody.Mass = newMass;
        }

        void PopulateStardust(int amount, Vector2 chunkPosition)
        {
            var newList = new List<GameObject>();
            Random random = World.Random;
            for (int i = 0; i < amount; i++)
            {
                PopulateStar(chunkPosition, random);
            }
        }

        private void PopulateStar(Vector2 chunkPosition, Random random)
        {
            GameObject newStardust = new GameObject();
            newStardust.Initialize();
            //newStardust.SetSprite(Sprite.Generate(Core.StarDust));
            newStardust.Body = Tiles.GenerateByteArrayMap(Core.StarDust);
            newStardust.Graphics = new ObjectGraphics(newStardust);
            newStardust.Position = (new Vector2(random.Next(chunkPosition.X, chunkPosition.X + Engine.ChunkSize.X),
                random.Next(chunkPosition.Y, chunkPosition.Y + Engine.ChunkSize.Y)));
            newStardust.Parent = this;
            Children.Add(newStardust as INodes);
        }
    }
}
