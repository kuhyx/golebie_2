using System;
using System.Collections.Generic;
using PigeonRacingManager.Models;

namespace PigeonRacingManager.Systems
{
    public class GameState
    {
        public PlayerProfile Player { get; }
        public Loft PlayerLoft { get; }
        public Season CurrentSeason { get; private set; }

        private readonly SeasonGenerator _seasonGenerator;
        private readonly RaceSimulator _raceSimulator;
        private readonly TrainingSystem _trainingSystem;
        private readonly PigeonGenerator _pigeonGenerator;
        private readonly Random _rng;

        public GameState(string playerName, Random rng)
        {
            _rng = rng ?? throw new ArgumentNullException(nameof(rng));
            Player = new PlayerProfile(playerName, 5000);
            PlayerLoft = new Loft($"{playerName}'s Loft", 12);

            _seasonGenerator = new SeasonGenerator(_rng);
            _raceSimulator = new RaceSimulator(_rng);
            _trainingSystem = new TrainingSystem(_rng);
            _pigeonGenerator = new PigeonGenerator(_rng);

            List<Pigeon> starters = _pigeonGenerator.GenerateStarterFlock(6);
            foreach (Pigeon p in starters)
            {
                PlayerLoft.TryAddPigeon(p);
            }

            CurrentSeason = _seasonGenerator.GenerateSeason(1);
        }

        public TrainingResult TrainPigeon(Pigeon pigeon, TrainingType type)
        {
            if (pigeon == null)
            {
                throw new ArgumentNullException(nameof(pigeon));
            }

            return _trainingSystem.Train(pigeon, type);
        }

        public RaceSimulationRun RunRace(Race race, List<Pigeon> selectedPigeons)
        {
            if (race == null)
            {
                throw new ArgumentNullException(nameof(race));
            }

            if (selectedPigeons == null)
            {
                throw new ArgumentNullException(nameof(selectedPigeons));
            }

            int rivalCount = 8 + _rng.Next(0, 9);
            int avgSkill = CalculateAverageSkill(selectedPigeons);
            List<Pigeon> rivals = _raceSimulator.GenerateRivalPigeons(rivalCount, avgSkill);

            var allParticipants = new List<Pigeon>(selectedPigeons);
            allParticipants.AddRange(rivals);

            RaceSimulationRun simulation = _raceSimulator.SimulateRace(race, allParticipants);

            foreach (Pigeon p in selectedPigeons)
            {
                simulation.PlayerPigeonIds.Add(p.Id);
            }

            race.MarkCompleted(new List<RaceResult>(simulation.Results));

            foreach (RaceResult result in simulation.Results)
            {
                Pigeon playerPigeon = PlayerLoft.FindById(result.PigeonId);
                if (playerPigeon != null)
                {
                    bool won = result.Position == 1;
                    playerPigeon.RecordRaceResult(won);
                    playerPigeon.AddFatigue(CalculateRaceFatigue(race.Distance));
                    playerPigeon.ReduceCondition(10 + _rng.Next(0, 11));

                    int prize = CalculatePrize(race.PrizeMoney, result.Position);
                    if (prize > 0)
                    {
                        Player.Earn(prize);
                    }

                    int repGain = CalculateReputationGain(result.Position);
                    Player.AddReputation(repGain);
                }
            }

            return simulation;
        }

        public void AdvanceWeek()
        {
            CurrentSeason.AdvanceWeek();
            PlayerLoft.AdvanceWeek();

            if (CurrentSeason.IsOver)
            {
                int nextYear = CurrentSeason.Year + 1;
                CurrentSeason = _seasonGenerator.GenerateSeason(nextYear);
            }
        }

        public Pigeon GenerateMarketPigeon()
        {
            return _pigeonGenerator.GenerateRandom(35, 75);
        }

        public bool TryBuyPigeon(Pigeon pigeon, int price)
        {
            if (pigeon == null)
            {
                throw new ArgumentNullException(nameof(pigeon));
            }

            if (PlayerLoft.IsFull)
            {
                return false;
            }

            if (!Player.TrySpend(price))
            {
                return false;
            }

            return PlayerLoft.TryAddPigeon(pigeon);
        }

        private static int CalculateAverageSkill(List<Pigeon> pigeons)
        {
            if (pigeons.Count == 0)
            {
                return 50;
            }

            int total = 0;
            foreach (Pigeon p in pigeons)
            {
                total += p.Stats.Overall;
            }

            return total / pigeons.Count;
        }

        private static int CalculateRaceFatigue(RaceDistance distance)
        {
            switch (distance)
            {
                case RaceDistance.Short:
                    return 25;
                case RaceDistance.Medium:
                    return 40;
                case RaceDistance.Long:
                    return 60;
                default:
                    return 30;
            }
        }

        private static int CalculatePrize(int totalPrize, int position)
        {
            switch (position)
            {
                case 1:
                    return totalPrize;
                case 2:
                    return (int)(totalPrize * 0.5);
                case 3:
                    return (int)(totalPrize * 0.25);
                default:
                    return 0;
            }
        }

        private static int CalculateReputationGain(int position)
        {
            switch (position)
            {
                case 1:
                    return 10;
                case 2:
                    return 5;
                case 3:
                    return 2;
                default:
                    return 0;
            }
        }
    }
}
