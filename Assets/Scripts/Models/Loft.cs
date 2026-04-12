using System;
using System.Collections.Generic;

namespace PigeonRacingManager.Models
{
    [Serializable]
    public class Loft
    {
        private readonly List<Pigeon> _pigeons = new List<Pigeon>();

        public int Capacity { get; private set; }
        public IReadOnlyList<Pigeon> Pigeons => _pigeons;
        public int Count => _pigeons.Count;
        public bool IsFull => _pigeons.Count >= Capacity;
        public string Name { get; set; }

        public Loft(string name, int capacity)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            if (capacity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be positive.");
            }

            Capacity = capacity;
        }

        public bool TryAddPigeon(Pigeon pigeon)
        {
            if (pigeon == null)
            {
                throw new ArgumentNullException(nameof(pigeon));
            }

            if (IsFull)
            {
                return false;
            }

            _pigeons.Add(pigeon);
            return true;
        }

        public bool RemovePigeon(Pigeon pigeon)
        {
            if (pigeon == null)
            {
                throw new ArgumentNullException(nameof(pigeon));
            }

            return _pigeons.Remove(pigeon);
        }

        public Pigeon FindById(int id)
        {
            return _pigeons.Find(p => p.Id == id);
        }

        public void UpgradeCapacity(int additionalSlots)
        {
            if (additionalSlots <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(additionalSlots), "Must be positive.");
            }

            Capacity += additionalSlots;
        }

        public void AdvanceWeek()
        {
            foreach (Pigeon pigeon in _pigeons)
            {
                pigeon.AdvanceWeek();
            }
        }
    }
}
