using System;
using PigeonRacingManager.Models;

namespace PigeonRacingManager.Systems
{
    public class TrainingSystem
    {
        private readonly Random _rng;

        public TrainingSystem(Random rng)
        {
            _rng = rng ?? throw new ArgumentNullException(nameof(rng));
        }

        public TrainingResult Train(Pigeon pigeon, TrainingType type)
        {
            if (pigeon == null)
            {
                throw new ArgumentNullException(nameof(pigeon));
            }

            if (type == TrainingType.Rest)
            {
                return ApplyRest(pigeon);
            }

            if (pigeon.Fatigue >= 90)
            {
                return new TrainingResult(type, 0, 0, $"{pigeon.Name} is too fatigued to train. Let them rest.");
            }

            int currentStat = GetStatValue(pigeon.Stats, type);
            int gain = CalculateGain(currentStat, pigeon.Fatigue);
            int fatigueGain = CalculateFatigueGain(type, pigeon.Stats.Health);

            ApplyStatGain(pigeon.Stats, type, gain);
            pigeon.AddFatigue(fatigueGain);

            string message = FormatTrainingMessage(pigeon.Name, type, gain, fatigueGain);
            return new TrainingResult(type, gain, fatigueGain, message);
        }

        private TrainingResult ApplyRest(Pigeon pigeon)
        {
            int recovery = 20 + (pigeon.Stats.Health / 5) + _rng.Next(0, 10);
            int oldFatigue = pigeon.Fatigue;
            pigeon.ReduceFatigue(recovery);
            int actualRecovery = oldFatigue - pigeon.Fatigue;

            return new TrainingResult(
                TrainingType.Rest,
                0,
                -actualRecovery,
                $"{pigeon.Name} rested and recovered {actualRecovery} fatigue.");
        }

        private int CalculateGain(int currentStat, int fatigue)
        {
            double diminishingFactor = 1.0 - (currentStat / 120.0);
            double fatiguePenalty = 1.0 - (fatigue / 150.0);
            int baseGain = 2 + _rng.Next(0, 4);
            int gain = (int)Math.Round(baseGain * diminishingFactor * fatiguePenalty);
            return Math.Max(0, gain);
        }

        private int CalculateFatigueGain(TrainingType type, int health)
        {
            int baseFatigue;
            switch (type)
            {
                case TrainingType.Speed:
                    baseFatigue = 15;
                    break;
                case TrainingType.Endurance:
                    baseFatigue = 20;
                    break;
                case TrainingType.Navigation:
                    baseFatigue = 10;
                    break;
                default:
                    baseFatigue = 0;
                    break;
            }

            double healthReduction = health / 200.0;
            int fatigue = (int)Math.Round(baseFatigue * (1.0 - healthReduction));
            return Math.Max(1, fatigue + _rng.Next(-2, 3));
        }

        private static int GetStatValue(PigeonStats stats, TrainingType type)
        {
            switch (type)
            {
                case TrainingType.Speed:
                    return stats.Speed;
                case TrainingType.Endurance:
                    return stats.Endurance;
                case TrainingType.Navigation:
                    return stats.Navigation;
                default:
                    return 0;
            }
        }

        private static void ApplyStatGain(PigeonStats stats, TrainingType type, int gain)
        {
            switch (type)
            {
                case TrainingType.Speed:
                    stats.Speed += gain;
                    break;
                case TrainingType.Endurance:
                    stats.Endurance += gain;
                    break;
                case TrainingType.Navigation:
                    stats.Navigation += gain;
                    break;
            }
        }

        private static string FormatTrainingMessage(string name, TrainingType type, int gain, int fatigueGain)
        {
            if (gain == 0)
            {
                return $"{name} trained {type} but made no progress. Consider varying the routine.";
            }

            return $"{name} trained {type}: +{gain} stat, +{fatigueGain} fatigue.";
        }
    }
}
