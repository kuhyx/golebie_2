using System;

namespace PigeonRacingManager.Models
{
    public enum TrainingType
    {
        Speed,
        Endurance,
        Navigation,
        Rest,
    }

    [Serializable]
    public class TrainingResult
    {
        public TrainingType Type { get; }
        public int StatGain { get; }
        public int FatigueGain { get; }
        public string Message { get; }

        public TrainingResult(TrainingType type, int statGain, int fatigueGain, string message)
        {
            Type = type;
            StatGain = statGain;
            FatigueGain = fatigueGain;
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }
    }
}
