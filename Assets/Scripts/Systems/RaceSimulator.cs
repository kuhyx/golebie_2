using System;
using System.Collections.Generic;
using PigeonRacingManager.Models;

namespace PigeonRacingManager.Systems
{
    public class RaceSimulator
    {
        private readonly Random _rng;

        public RaceSimulator(Random rng)
        {
            _rng = rng ?? throw new ArgumentNullException(nameof(rng));
        }

        public RaceSimulationRun SimulateRace(Race race, List<Pigeon> participants)
        {
            if (race == null)
            {
                throw new ArgumentNullException(nameof(race));
            }

            if (participants == null)
            {
                throw new ArgumentNullException(nameof(participants));
            }

            if (participants.Count == 0)
            {
                throw new ArgumentException("Need at least one participant.", nameof(participants));
            }

            var simulation = new RaceSimulationRun();
            float distanceMeters = race.DistanceKm * 1000f;

            simulation.AddEvent(new RaceEvent(
                RaceEventType.RaceStarted,
                $"The {race.Name} ({race.DistanceKm}km) has begun! {participants.Count} pigeons released!",
                0f));

            var pigeonProgress = new Dictionary<int, float>();
            var pigeonSpeeds = new Dictionary<int, float>();
            var pigeonNames = new Dictionary<int, string>();
            var finished = new List<RaceResult>();

            foreach (Pigeon p in participants)
            {
                pigeonProgress[p.Id] = 0f;
                pigeonSpeeds[p.Id] = CalculateBaseSpeed(p, race.Distance);
                pigeonNames[p.Id] = p.Name;
            }

            simulation.SetParticipantNames(pigeonNames);

            float timeStep = 60f;
            float currentTime = 0f;
            int updateInterval = GetUpdateInterval(race.Distance);
            int stepCount = 0;

            while (finished.Count < participants.Count)
            {
                currentTime += timeStep;
                stepCount++;

                foreach (Pigeon p in participants)
                {
                    if (pigeonProgress[p.Id] >= distanceMeters)
                    {
                        continue;
                    }

                    float speed = CalculateCurrentSpeed(
                        pigeonSpeeds[p.Id], p, pigeonProgress[p.Id], distanceMeters, currentTime);
                    pigeonProgress[p.Id] += speed * timeStep;

                    if (pigeonProgress[p.Id] >= distanceMeters)
                    {
                        pigeonProgress[p.Id] = distanceMeters;
                        int position = finished.Count + 1;
                        float speedMps = distanceMeters / currentTime;
                        var result = new RaceResult(p.Id, p.Name, position, currentTime, speedMps);
                        finished.Add(result);

                        simulation.AddEvent(new RaceEvent(
                            RaceEventType.PigeonFinished,
                            $"{p.Name} finishes in position {position}! Time: {FormatTime(currentTime)}",
                            currentTime,
                            p.Id,
                            CaptureProgressSnapshot(pigeonProgress, distanceMeters)));
                    }
                }

                if ((stepCount % updateInterval) == 0 && finished.Count < participants.Count)
                {
                    string update = GeneratePositionUpdate(participants, pigeonProgress, distanceMeters, currentTime);
                    simulation.AddEvent(new RaceEvent(
                        RaceEventType.PositionUpdate,
                        update,
                        currentTime,
                        -1,
                        CaptureProgressSnapshot(pigeonProgress, distanceMeters)));
                }

                if (currentTime > 86400f)
                {
                    break;
                }
            }

            simulation.AddEvent(new RaceEvent(
                RaceEventType.RaceCompleted,
                $"The {race.Name} is complete! Winner: {finished[0].PigeonName}!",
                currentTime));

            simulation.SetResults(finished);
            return simulation;
        }

        public List<Pigeon> GenerateRivalPigeons(int count, int skillLevel)
        {
            var rivals = new List<Pigeon>();
            string[] names =
            {
                "Burza", "Strzala", "Wicher", "Piorun", "Bialy",
                "Szary", "Orzel", "Sokol", "Chmura", "Grom",
                "Iskra", "Perla", "Rudy", "Cichy", "Lotnik",
                "Dymek", "Krol", "Hetman", "Goniec", "Zwiad",
            };

            for (int i = 0; i < count; i++)
            {
                int baseSkill = Math.Max(20, Math.Min(85, skillLevel + _rng.Next(-15, 16)));
                var stats = new PigeonStats(
                    baseSkill + _rng.Next(-10, 11),
                    baseSkill + _rng.Next(-10, 11),
                    baseSkill + _rng.Next(-10, 11),
                    60 + _rng.Next(0, 31));

                string name = names[_rng.Next(names.Length)];
                var sex = (_rng.Next(2) == 0) ? PigeonSex.Male : PigeonSex.Female;
                var color = (PigeonColor)_rng.Next(Enum.GetValues(typeof(PigeonColor)).Length);
                int age = 52 + _rng.Next(0, 156);

                var rival = new Pigeon($"{name} #{i + 1}", sex, color, age, stats);
                rivals.Add(rival);
            }

            return rivals;
        }

        private float CalculateBaseSpeed(Pigeon pigeon, RaceDistance distance)
        {
            float speedFactor = pigeon.Stats.Speed / 100f;
            float enduranceFactor = pigeon.Stats.Endurance / 100f;
            float navFactor = pigeon.Stats.Navigation / 100f;

            float baseSpeedMps;
            switch (distance)
            {
                case RaceDistance.Short:
                    baseSpeedMps = 18f + (speedFactor * 8f) + (navFactor * 2f);
                    break;
                case RaceDistance.Medium:
                    baseSpeedMps = 16f + (speedFactor * 5f) + (enduranceFactor * 4f) + (navFactor * 3f);
                    break;
                case RaceDistance.Long:
                    baseSpeedMps = 14f + (enduranceFactor * 6f) + (navFactor * 4f) + (speedFactor * 2f);
                    break;
                default:
                    baseSpeedMps = 18f;
                    break;
            }

            float conditionMod = pigeon.Condition / 100f;
            float fatigueMod = 1f - (pigeon.Fatigue / 200f);

            return baseSpeedMps * conditionMod * fatigueMod;
        }

        private float CalculateCurrentSpeed(
            float baseSpeed, Pigeon pigeon, float progress, float totalDistance, float time)
        {
            float progressRatio = progress / totalDistance;
            float enduranceDrop = (1f - (pigeon.Stats.Endurance / 120f)) * progressRatio * 0.3f;
            float randomVariance = 1f + (((float)_rng.NextDouble() - 0.5f) * 0.1f);
            return baseSpeed * (1f - enduranceDrop) * randomVariance;
        }

        private static int GetUpdateInterval(RaceDistance distance)
        {
            switch (distance)
            {
                case RaceDistance.Short:
                    return 5;
                case RaceDistance.Medium:
                    return 10;
                case RaceDistance.Long:
                    return 15;
                default:
                    return 10;
            }
        }

        private static string GeneratePositionUpdate(
            List<Pigeon> participants, Dictionary<int, float> progress, float totalDistance, float time)
        {
            var sorted = new List<Pigeon>(participants);
            sorted.Sort((a, b) => progress[b.Id].CompareTo(progress[a.Id]));

            string leader = sorted[0].Name;
            float leaderPct = (progress[sorted[0].Id] / totalDistance) * 100f;

            return $"[{FormatTime(time)}] {leader} leads at {leaderPct:F0}% of the course.";
        }

        private static Dictionary<int, float> CaptureProgressSnapshot(
            Dictionary<int, float> progress, float totalDistance)
        {
            var snapshot = new Dictionary<int, float>();
            foreach (KeyValuePair<int, float> kvp in progress)
            {
                snapshot[kvp.Key] = Math.Max(0f, Math.Min(1f, kvp.Value / totalDistance));
            }

            return snapshot;
        }

        private static string FormatTime(float seconds)
        {
            int hours = (int)(seconds / 3600);
            int minutes = (int)((seconds % 3600) / 60);
            int secs = (int)(seconds % 60);

            if (hours > 0)
            {
                return $"{hours}h {minutes}m {secs}s";
            }

            return $"{minutes}m {secs}s";
        }
    }

    public class RaceSimulationRun
    {
        private readonly List<RaceEvent> _events = new List<RaceEvent>();
        private List<RaceResult> _results = new List<RaceResult>();
        private readonly Dictionary<int, string> _participantNames = new Dictionary<int, string>();
        private readonly HashSet<int> _playerPigeonIds = new HashSet<int>();

        public IReadOnlyList<RaceEvent> Events => _events;
        public IReadOnlyList<RaceResult> Results => _results;
        public IReadOnlyDictionary<int, string> ParticipantNames => _participantNames;
        public HashSet<int> PlayerPigeonIds => _playerPigeonIds;

        public void AddEvent(RaceEvent raceEvent)
        {
            if (raceEvent == null)
            {
                throw new ArgumentNullException(nameof(raceEvent));
            }

            _events.Add(raceEvent);
        }

        public void SetResults(List<RaceResult> results)
        {
            _results = results ?? throw new ArgumentNullException(nameof(results));
        }

        public void SetParticipantNames(Dictionary<int, string> names)
        {
            _participantNames.Clear();
            foreach (KeyValuePair<int, string> kvp in names)
            {
                _participantNames[kvp.Key] = kvp.Value;
            }
        }
    }
}
