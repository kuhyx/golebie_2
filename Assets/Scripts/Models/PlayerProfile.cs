using System;

namespace PigeonRacingManager.Models
{
    [Serializable]
    public class PlayerProfile
    {
        public string PlayerName { get; set; }
        public int Money { get; private set; }
        public int Reputation { get; private set; }

        public PlayerProfile(string playerName, int startingMoney)
        {
            PlayerName = playerName ?? throw new ArgumentNullException(nameof(playerName));
            if (startingMoney < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(startingMoney), "Cannot be negative.");
            }

            Money = startingMoney;
            Reputation = 0;
        }

        public bool TrySpend(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Cannot be negative.");
            }

            if (Money < amount)
            {
                return false;
            }

            Money -= amount;
            return true;
        }

        public void Earn(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Cannot be negative.");
            }

            Money += amount;
        }

        public void AddReputation(int amount)
        {
            Reputation = Math.Max(0, Reputation + amount);
        }
    }
}
