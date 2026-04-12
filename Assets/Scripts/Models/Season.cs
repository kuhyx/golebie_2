using System;
using System.Collections.Generic;

namespace PigeonRacingManager.Models
{
    [Serializable]
    public class Season
    {
        public const int WeeksPerSeason = 52;
        public const int RaceSeasonStart = 10;
        public const int RaceSeasonEnd = 40;

        public int Year { get; }
        public int CurrentWeek { get; private set; }
        public List<Race> Races { get; }
        public bool IsOver => CurrentWeek > WeeksPerSeason;
        public bool IsRaceSeason => (CurrentWeek >= RaceSeasonStart) && (CurrentWeek <= RaceSeasonEnd);

        public Season(int year, List<Race> races)
        {
            Year = year;
            Races = races ?? throw new ArgumentNullException(nameof(races));
            CurrentWeek = 1;
        }

        public Race GetRaceForWeek(int week)
        {
            return Races.Find(r => r.SeasonWeek == week);
        }

        public Race GetCurrentRace()
        {
            return GetRaceForWeek(CurrentWeek);
        }

        public void AdvanceWeek()
        {
            if (!IsOver)
            {
                CurrentWeek++;
            }
        }

        public int CompletedRaceCount()
        {
            int count = 0;
            foreach (Race race in Races)
            {
                if (race.IsCompleted)
                {
                    count++;
                }
            }

            return count;
        }

        public int TotalRaceCount()
        {
            return Races.Count;
        }
    }
}
