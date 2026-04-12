using System;
using System.Collections.Generic;

namespace PigeonRacingManager.Models
{
    public enum RaceDistance
    {
        Short,
        Medium,
        Long,
    }

    public enum RaceEventType
    {
        RaceStarted,
        PositionUpdate,
        PigeonFinished,
        RaceCompleted,
    }

    [Serializable]
    public class RaceEvent
    {
        public RaceEventType EventType { get; }
        public string Message { get; }
        public float TimeInRace { get; }
        public int PigeonId { get; }
        public Dictionary<int, float> ProgressSnapshot { get; }

        public RaceEvent(RaceEventType eventType, string message, float timeInRace, int pigeonId = -1,
            Dictionary<int, float> progressSnapshot = null)
        {
            EventType = eventType;
            Message = message ?? throw new ArgumentNullException(nameof(message));
            TimeInRace = timeInRace;
            PigeonId = pigeonId;
            ProgressSnapshot = progressSnapshot;
        }
    }

    [Serializable]
    public class RaceResult
    {
        public int PigeonId { get; }
        public string PigeonName { get; }
        public int Position { get; }
        public float FinishTime { get; }
        public float SpeedMps { get; }

        public RaceResult(int pigeonId, string pigeonName, int position, float finishTime, float speedMps)
        {
            PigeonId = pigeonId;
            PigeonName = pigeonName ?? throw new ArgumentNullException(nameof(pigeonName));
            Position = position;
            FinishTime = finishTime;
            SpeedMps = speedMps;
        }
    }

    [Serializable]
    public class Race
    {
        public string Name { get; }
        public RaceDistance Distance { get; }
        public int DistanceKm { get; }
        public int SeasonWeek { get; }
        public int PrizeMoney { get; }
        public List<RaceResult> Results { get; }
        public bool IsCompleted { get; private set; }

        public Race(string name, RaceDistance distance, int distanceKm, int seasonWeek, int prizeMoney)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Distance = distance;
            DistanceKm = distanceKm;
            SeasonWeek = seasonWeek;
            PrizeMoney = prizeMoney;
            Results = new List<RaceResult>();
        }

        public void MarkCompleted(List<RaceResult> results)
        {
            if (results == null)
            {
                throw new ArgumentNullException(nameof(results));
            }

            Results.Clear();
            Results.AddRange(results);
            IsCompleted = true;
        }

        public static int GetTypicalDistanceKm(RaceDistance distance)
        {
            switch (distance)
            {
                case RaceDistance.Short:
                    return 160;
                case RaceDistance.Medium:
                    return 500;
                case RaceDistance.Long:
                    return 900;
                default:
                    throw new ArgumentOutOfRangeException(nameof(distance));
            }
        }
    }
}
