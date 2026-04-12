using System;
using System.Collections.Generic;
using PigeonRacingManager.Models;

namespace PigeonRacingManager.Systems
{
    public class PigeonGenerator
    {
        private readonly Random _rng;

        private static readonly string[] MaleNames =
        {
            "Lotnik", "Wicher", "Grom", "Sokol", "Hetman",
            "Piorun", "Orzel", "Krol", "Goniec", "Dymek",
            "Rudy", "Cichy", "Szybki", "Mocny", "Dzielny",
        };

        private static readonly string[] FemaleNames =
        {
            "Burza", "Strzala", "Iskra", "Perla", "Chmura",
            "Gwiazda", "Luna", "Mila", "Biala", "Zlota",
            "Czarna", "Srebrna", "Ladna", "Szybka", "Cicha",
        };

        public PigeonGenerator(Random rng)
        {
            _rng = rng ?? throw new ArgumentNullException(nameof(rng));
        }

        public Pigeon GenerateRandom(int minStat, int maxStat)
        {
            PigeonSex sex = (_rng.Next(2) == 0) ? PigeonSex.Male : PigeonSex.Female;
            string[] namePool = (sex == PigeonSex.Male) ? MaleNames : FemaleNames;
            string name = namePool[_rng.Next(namePool.Length)];
            PigeonColor color = (PigeonColor)_rng.Next(Enum.GetValues(typeof(PigeonColor)).Length);
            int age = 26 + _rng.Next(0, 156);

            var stats = new PigeonStats(
                RandomStat(minStat, maxStat),
                RandomStat(minStat, maxStat),
                RandomStat(minStat, maxStat),
                RandomStat(minStat, maxStat));

            return new Pigeon(name, sex, color, age, stats);
        }

        public List<Pigeon> GenerateStarterFlock(int count)
        {
            var flock = new List<Pigeon>();
            for (int i = 0; i < count; i++)
            {
                flock.Add(GenerateRandom(30, 65));
            }

            return flock;
        }

        private int RandomStat(int min, int max)
        {
            return min + _rng.Next(0, (max - min) + 1);
        }
    }
}
