using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsciiEngine
{
    public enum BiomeType
    {
        Void,
        Asteroids,
        Starry,
        Balanced,
    }

    public abstract class Biome
    {
        public int AsteroidDensity;
        public int StarDensity;
        public double AsteroidSize;
        public BiomeType Type;
    }

    public class BiomeVoid : Biome
    {
        public BiomeVoid()
        {
            AsteroidDensity = 0;
            StarDensity = 40;
            AsteroidSize = 0.5;
            Type = BiomeType.Void;
        }
    }

    public class BiomeBalanced : Biome
    {
        public BiomeBalanced()
        {
            AsteroidDensity = 20;
            StarDensity = 40;
            AsteroidSize = 0.5;
            Type = BiomeType.Balanced;
        }
    }

    public class BiomeStarry : Biome
    {
        public BiomeStarry()
        {
            AsteroidDensity = 3;
            StarDensity = 80;
            AsteroidSize = 0.5;
            Type = BiomeType.Starry;
        }
    }
    public class BiomeAsteroids : Biome
    {
        public BiomeAsteroids()
        {
            AsteroidDensity = 50;
            StarDensity = 40;
            AsteroidSize = 0.8;
            Type = BiomeType.Asteroids;
        }
    }

}
