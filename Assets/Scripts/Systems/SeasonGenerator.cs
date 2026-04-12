using System;
using System.Collections.Generic;
using PigeonRacingManager.Models;

namespace PigeonRacingManager.Systems
{
    public class SeasonGenerator
    {
        private readonly Random _rng;

        private static readonly string[] RaceNames =
        {
            "Spring Derby", "City Classic", "Mountain Challenge", "River Run",
            "National Sprint", "Grand Endurance", "Navigator's Cup", "Champion Stakes",
            "Regional Open", "President's Trophy", "Golden Wing", "Iron Feather",
            "Dawn Flight", "Heritage Race", "Thunder Cup", "Eagle's Path",
        };

        public SeasonGenerator(Random rng)
        {
            _rng = rng ?? throw new ArgumentNullException(nameof(rng));
        }

        public Season GenerateSeason(int year)
        {
            var races = new List<Race>();
            int raceCount = 10 + _rng.Next(0, 5);

            int availableWeeks = Season.RaceSeasonEnd - Season.RaceSeasonStart;
            int spacing = Math.Max(2, availableWeeks / raceCount);

            int currentWeek = Season.RaceSeasonStart + _rng.Next(0, 3);

            for (int i = 0; i < raceCount && currentWeek <= Season.RaceSeasonEnd; i++)
            {
                RaceDistance distance = PickDistance(i, raceCount);
                int distanceKm = Race.GetTypicalDistanceKm(distance) + _rng.Next(-30, 31);
                distanceKm = Math.Max(100, distanceKm);

                int basePrize = GetBasePrize(distance);
                int prize = basePrize + (_rng.Next(-200, 201));

                string name = RaceNames[i % RaceNames.Length];
                if (i >= RaceNames.Length)
                {
                    name += $" {(i / RaceNames.Length) + 1}";
                }

                var race = new Race(name, distance, distanceKm, currentWeek, prize);
                races.Add(race);

                currentWeek += spacing + _rng.Next(0, 2);
            }

            return new Season(year, races);
        }

        private RaceDistance PickDistance(int raceIndex, int totalRaces)
        {
            if (raceIndex < 3)
            {
                return RaceDistance.Short;
            }

            if (raceIndex >= totalRaces - 2)
            {
                return RaceDistance.Long;
            }

            int roll = _rng.Next(3);
            switch (roll)
            {
                case 0:
                    return RaceDistance.Short;
                case 1:
                    return RaceDistance.Medium;
                default:
                    return RaceDistance.Long;
            }
        }

        private static int GetBasePrize(RaceDistance distance)
        {
            switch (distance)
            {
                case RaceDistance.Short:
                    return 500;
                case RaceDistance.Medium:
                    return 1000;
                case RaceDistance.Long:
                    return 2000;
                default:
                    return 500;
            }
        }
    }
}
