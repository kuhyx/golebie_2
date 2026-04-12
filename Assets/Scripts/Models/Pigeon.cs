using System;

namespace PigeonRacingManager.Models
{
    [Serializable]
    public class Pigeon
    {
        private static int _nextId = 1;

        public int Id { get; }
        public string Name { get; set; }
        public PigeonSex Sex { get; }
        public PigeonColor Color { get; }
        public int AgeInWeeks { get; private set; }
        public PigeonStats Stats { get; }
        public int Fatigue { get; private set; }
        public int Condition { get; private set; }
        public int RacesCompleted { get; private set; }
        public int RacesWon { get; private set; }

        public bool IsRaceReady => (Fatigue < 70) && (Condition > 30) && (AgeInWeeks >= 26);

        public string AgeDisplay
        {
            get
            {
                int years = AgeInWeeks / 52;
                int weeks = AgeInWeeks % 52;
                if (years > 0)
                {
                    return $"{years}y {weeks}w";
                }

                return $"{weeks}w";
            }
        }

        public Pigeon(string name, PigeonSex sex, PigeonColor color, int ageInWeeks, PigeonStats stats)
        {
            Id = _nextId++;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Sex = sex;
            Color = color;
            AgeInWeeks = ageInWeeks;
            Stats = stats ?? throw new ArgumentNullException(nameof(stats));
            Fatigue = 0;
            Condition = 100;
        }

        public void AdvanceWeek()
        {
            AgeInWeeks++;
            int fatigueRecovery = Math.Min(Fatigue, 15 + (Stats.Health / 10));
            Fatigue = Math.Max(0, Fatigue - fatigueRecovery);

            if (Condition < 100)
            {
                int conditionRecovery = 5 + (Stats.Health / 20);
                Condition = Math.Min(100, Condition + conditionRecovery);
            }
        }

        public void AddFatigue(int amount)
        {
            Fatigue = Math.Min(100, Fatigue + Math.Max(0, amount));
        }

        public void ReduceFatigue(int amount)
        {
            Fatigue = Math.Max(0, Fatigue - Math.Max(0, amount));
        }

        public void ReduceCondition(int amount)
        {
            Condition = Math.Max(0, Condition - Math.Max(0, amount));
        }

        public void RecordRaceResult(bool won)
        {
            RacesCompleted++;
            if (won)
            {
                RacesWon++;
            }
        }

        public static void ResetIdCounter()
        {
            _nextId = 1;
        }
    }
}
