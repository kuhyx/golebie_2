using System;

namespace PigeonRacingManager.Models
{
    [Serializable]
    public class PigeonStats
    {
        public const int MinStat = 0;
        public const int MaxStat = 100;

        private int _speed;
        private int _endurance;
        private int _navigation;
        private int _health;

        public int Speed
        {
            get => _speed;
            set => _speed = Clamp(value);
        }

        public int Endurance
        {
            get => _endurance;
            set => _endurance = Clamp(value);
        }

        public int Navigation
        {
            get => _navigation;
            set => _navigation = Clamp(value);
        }

        public int Health
        {
            get => _health;
            set => _health = Clamp(value);
        }

        public int Overall => (int)Math.Round(
            (Speed * 0.3) + (Endurance * 0.3) + (Navigation * 0.25) + (Health * 0.15));

        public PigeonStats(int speed, int endurance, int navigation, int health)
        {
            Speed = speed;
            Endurance = endurance;
            Navigation = navigation;
            Health = health;
        }

        public PigeonStats() : this(50, 50, 50, 50)
        {
        }

        public PigeonStats Clone()
        {
            return new PigeonStats(Speed, Endurance, Navigation, Health);
        }

        private static int Clamp(int value)
        {
            return Math.Max(MinStat, Math.Min(MaxStat, value));
        }
    }
}
