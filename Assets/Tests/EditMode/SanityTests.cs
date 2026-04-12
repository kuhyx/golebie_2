using System;
using System.Collections.Generic;
using NUnit.Framework;
using PigeonRacingManager.Models;
using PigeonRacingManager.Systems;

namespace PigeonRacingManager.Tests.EditMode
{
    [TestFixture]
    public class PigeonStatsTests
    {
        [Test]
        public void Constructor_DefaultValues_AllFifty()
        {
            var stats = new PigeonStats();
            Assert.That(stats.Speed, Is.EqualTo(50));
            Assert.That(stats.Endurance, Is.EqualTo(50));
            Assert.That(stats.Navigation, Is.EqualTo(50));
            Assert.That(stats.Health, Is.EqualTo(50));
        }

        [Test]
        public void Constructor_CustomValues_SetsCorrectly()
        {
            var stats = new PigeonStats(70, 80, 60, 90);
            Assert.That(stats.Speed, Is.EqualTo(70));
            Assert.That(stats.Endurance, Is.EqualTo(80));
            Assert.That(stats.Navigation, Is.EqualTo(60));
            Assert.That(stats.Health, Is.EqualTo(90));
        }

        [Test]
        public void Speed_ClampedToMax()
        {
            var stats = new PigeonStats();
            stats.Speed = 150;
            Assert.That(stats.Speed, Is.EqualTo(PigeonStats.MaxStat));
        }

        [Test]
        public void Speed_ClampedToMin()
        {
            var stats = new PigeonStats();
            stats.Speed = -10;
            Assert.That(stats.Speed, Is.EqualTo(PigeonStats.MinStat));
        }

        [Test]
        public void Endurance_ClampedToMax()
        {
            var stats = new PigeonStats();
            stats.Endurance = 200;
            Assert.That(stats.Endurance, Is.EqualTo(PigeonStats.MaxStat));
        }

        [Test]
        public void Endurance_ClampedToMin()
        {
            var stats = new PigeonStats();
            stats.Endurance = -5;
            Assert.That(stats.Endurance, Is.EqualTo(PigeonStats.MinStat));
        }

        [Test]
        public void Navigation_ClampedToMax()
        {
            var stats = new PigeonStats();
            stats.Navigation = 999;
            Assert.That(stats.Navigation, Is.EqualTo(PigeonStats.MaxStat));
        }

        [Test]
        public void Navigation_ClampedToMin()
        {
            var stats = new PigeonStats();
            stats.Navigation = -1;
            Assert.That(stats.Navigation, Is.EqualTo(PigeonStats.MinStat));
        }

        [Test]
        public void Health_ClampedToMax()
        {
            var stats = new PigeonStats();
            stats.Health = 101;
            Assert.That(stats.Health, Is.EqualTo(PigeonStats.MaxStat));
        }

        [Test]
        public void Health_ClampedToMin()
        {
            var stats = new PigeonStats();
            stats.Health = -100;
            Assert.That(stats.Health, Is.EqualTo(PigeonStats.MinStat));
        }

        [Test]
        public void Overall_CalculatesWeightedAverage()
        {
            var stats = new PigeonStats(100, 100, 100, 100);
            Assert.That(stats.Overall, Is.EqualTo(100));
        }

        [Test]
        public void Overall_AllZero_IsZero()
        {
            var stats = new PigeonStats(0, 0, 0, 0);
            Assert.That(stats.Overall, Is.EqualTo(0));
        }

        [Test]
        public void Clone_ReturnsDistinctCopy()
        {
            var original = new PigeonStats(70, 80, 60, 90);
            PigeonStats clone = original.Clone();

            Assert.That(clone.Speed, Is.EqualTo(70));
            clone.Speed = 99;
            Assert.That(original.Speed, Is.EqualTo(70));
        }
    }

    [TestFixture]
    public class PigeonTests
    {
        [SetUp]
        public void SetUp()
        {
            Pigeon.ResetIdCounter();
        }

        [Test]
        public void Constructor_SetsAllProperties()
        {
            var stats = new PigeonStats(60, 70, 55, 80);
            var pigeon = new Pigeon("TestBird", PigeonSex.Male, PigeonColor.Blue, 30, stats);

            Assert.That(pigeon.Name, Is.EqualTo("TestBird"));
            Assert.That(pigeon.Sex, Is.EqualTo(PigeonSex.Male));
            Assert.That(pigeon.Color, Is.EqualTo(PigeonColor.Blue));
            Assert.That(pigeon.AgeInWeeks, Is.EqualTo(30));
            Assert.That(pigeon.Fatigue, Is.EqualTo(0));
            Assert.That(pigeon.Condition, Is.EqualTo(100));
        }

        [Test]
        public void Constructor_NullName_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new Pigeon(null, PigeonSex.Male, PigeonColor.Blue, 30, new PigeonStats()));
        }

        [Test]
        public void Constructor_NullStats_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new Pigeon("Test", PigeonSex.Male, PigeonColor.Blue, 30, null));
        }

        [Test]
        public void IsRaceReady_YoungPigeon_ReturnsFalse()
        {
            var pigeon = new Pigeon("Young", PigeonSex.Male, PigeonColor.Blue, 20, new PigeonStats());
            Assert.That(pigeon.IsRaceReady, Is.False);
        }

        [Test]
        public void IsRaceReady_HealthyAdult_ReturnsTrue()
        {
            var pigeon = new Pigeon("Adult", PigeonSex.Male, PigeonColor.Blue, 30, new PigeonStats());
            Assert.That(pigeon.IsRaceReady, Is.True);
        }

        [Test]
        public void IsRaceReady_Fatigued_ReturnsFalse()
        {
            var pigeon = new Pigeon("Tired", PigeonSex.Male, PigeonColor.Blue, 30, new PigeonStats());
            pigeon.AddFatigue(80);
            Assert.That(pigeon.IsRaceReady, Is.False);
        }

        [Test]
        public void IsRaceReady_LowCondition_ReturnsFalse()
        {
            var pigeon = new Pigeon("Weak", PigeonSex.Male, PigeonColor.Blue, 30, new PigeonStats());
            pigeon.ReduceCondition(80);
            Assert.That(pigeon.IsRaceReady, Is.False);
        }

        [Test]
        public void AdvanceWeek_ReducesFatigue()
        {
            var pigeon = new Pigeon("Test", PigeonSex.Male, PigeonColor.Blue, 30, new PigeonStats());
            pigeon.AddFatigue(50);
            int oldFatigue = pigeon.Fatigue;
            pigeon.AdvanceWeek();
            Assert.That(pigeon.Fatigue, Is.LessThan(oldFatigue));
        }

        [Test]
        public void AdvanceWeek_IncreasesAge()
        {
            var pigeon = new Pigeon("Test", PigeonSex.Male, PigeonColor.Blue, 30, new PigeonStats());
            pigeon.AdvanceWeek();
            Assert.That(pigeon.AgeInWeeks, Is.EqualTo(31));
        }

        [Test]
        public void AdvanceWeek_RecoverCondition()
        {
            var pigeon = new Pigeon("Test", PigeonSex.Male, PigeonColor.Blue, 30, new PigeonStats());
            pigeon.ReduceCondition(30);
            int oldCondition = pigeon.Condition;
            pigeon.AdvanceWeek();
            Assert.That(pigeon.Condition, Is.GreaterThan(oldCondition));
        }

        [Test]
        public void AddFatigue_ClampsToHundred()
        {
            var pigeon = new Pigeon("Test", PigeonSex.Male, PigeonColor.Blue, 30, new PigeonStats());
            pigeon.AddFatigue(200);
            Assert.That(pigeon.Fatigue, Is.EqualTo(100));
        }

        [Test]
        public void ReduceCondition_ClampsToZero()
        {
            var pigeon = new Pigeon("Test", PigeonSex.Male, PigeonColor.Blue, 30, new PigeonStats());
            pigeon.ReduceCondition(200);
            Assert.That(pigeon.Condition, Is.EqualTo(0));
        }

        [Test]
        public void RecordRaceResult_Win_IncrementsBoth()
        {
            var pigeon = new Pigeon("Test", PigeonSex.Male, PigeonColor.Blue, 30, new PigeonStats());
            pigeon.RecordRaceResult(true);
            Assert.That(pigeon.RacesCompleted, Is.EqualTo(1));
            Assert.That(pigeon.RacesWon, Is.EqualTo(1));
        }

        [Test]
        public void RecordRaceResult_Loss_IncrementsOnlyCompleted()
        {
            var pigeon = new Pigeon("Test", PigeonSex.Male, PigeonColor.Blue, 30, new PigeonStats());
            pigeon.RecordRaceResult(false);
            Assert.That(pigeon.RacesCompleted, Is.EqualTo(1));
            Assert.That(pigeon.RacesWon, Is.EqualTo(0));
        }

        [Test]
        public void AgeDisplay_UnderOneYear()
        {
            var pigeon = new Pigeon("Test", PigeonSex.Male, PigeonColor.Blue, 30, new PigeonStats());
            Assert.That(pigeon.AgeDisplay, Is.EqualTo("30w"));
        }

        [Test]
        public void AgeDisplay_OverOneYear()
        {
            var pigeon = new Pigeon("Test", PigeonSex.Male, PigeonColor.Blue, 60, new PigeonStats());
            Assert.That(pigeon.AgeDisplay, Is.EqualTo("1y 8w"));
        }

        [Test]
        public void UniqueIds_AreSequential()
        {
            var a = new Pigeon("A", PigeonSex.Male, PigeonColor.Blue, 30, new PigeonStats());
            var b = new Pigeon("B", PigeonSex.Female, PigeonColor.Red, 40, new PigeonStats());
            Assert.That(b.Id, Is.EqualTo(a.Id + 1));
        }
    }

    [TestFixture]
    public class LoftTests
    {
        [SetUp]
        public void SetUp()
        {
            Pigeon.ResetIdCounter();
        }

        [Test]
        public void Constructor_SetsNameAndCapacity()
        {
            var loft = new Loft("My Loft", 10);
            Assert.That(loft.Name, Is.EqualTo("My Loft"));
            Assert.That(loft.Capacity, Is.EqualTo(10));
            Assert.That(loft.Count, Is.EqualTo(0));
        }

        [Test]
        public void Constructor_NullName_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new Loft(null, 10));
        }

        [Test]
        public void Constructor_ZeroCapacity_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Loft("Test", 0));
        }

        [Test]
        public void Constructor_NegativeCapacity_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Loft("Test", -1));
        }

        [Test]
        public void TryAddPigeon_Success()
        {
            var loft = new Loft("Test", 5);
            var pigeon = new Pigeon("Bird", PigeonSex.Male, PigeonColor.Blue, 30, new PigeonStats());
            bool result = loft.TryAddPigeon(pigeon);
            Assert.That(result, Is.True);
            Assert.That(loft.Count, Is.EqualTo(1));
        }

        [Test]
        public void TryAddPigeon_NullPigeon_Throws()
        {
            var loft = new Loft("Test", 5);
            Assert.Throws<ArgumentNullException>(() => loft.TryAddPigeon(null));
        }

        [Test]
        public void TryAddPigeon_Full_ReturnsFalse()
        {
            var loft = new Loft("Test", 1);
            var first = new Pigeon("A", PigeonSex.Male, PigeonColor.Blue, 30, new PigeonStats());
            var second = new Pigeon("B", PigeonSex.Female, PigeonColor.Red, 30, new PigeonStats());
            loft.TryAddPigeon(first);
            bool result = loft.TryAddPigeon(second);
            Assert.That(result, Is.False);
        }

        [Test]
        public void RemovePigeon_Existing_ReturnsTrue()
        {
            var loft = new Loft("Test", 5);
            var pigeon = new Pigeon("Bird", PigeonSex.Male, PigeonColor.Blue, 30, new PigeonStats());
            loft.TryAddPigeon(pigeon);
            bool result = loft.RemovePigeon(pigeon);
            Assert.That(result, Is.True);
            Assert.That(loft.Count, Is.EqualTo(0));
        }

        [Test]
        public void RemovePigeon_NullPigeon_Throws()
        {
            var loft = new Loft("Test", 5);
            Assert.Throws<ArgumentNullException>(() => loft.RemovePigeon(null));
        }

        [Test]
        public void FindById_Existing_ReturnsPigeon()
        {
            var loft = new Loft("Test", 5);
            var pigeon = new Pigeon("Bird", PigeonSex.Male, PigeonColor.Blue, 30, new PigeonStats());
            loft.TryAddPigeon(pigeon);
            Pigeon found = loft.FindById(pigeon.Id);
            Assert.That(found, Is.SameAs(pigeon));
        }

        [Test]
        public void FindById_NonExisting_ReturnsNull()
        {
            var loft = new Loft("Test", 5);
            Assert.That(loft.FindById(999), Is.Null);
        }

        [Test]
        public void UpgradeCapacity_IncreasesCapacity()
        {
            var loft = new Loft("Test", 5);
            loft.UpgradeCapacity(3);
            Assert.That(loft.Capacity, Is.EqualTo(8));
        }

        [Test]
        public void UpgradeCapacity_ZeroOrNegative_Throws()
        {
            var loft = new Loft("Test", 5);
            Assert.Throws<ArgumentOutOfRangeException>(() => loft.UpgradeCapacity(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => loft.UpgradeCapacity(-1));
        }

        [Test]
        public void IsFull_WhenFull_ReturnsTrue()
        {
            var loft = new Loft("Test", 1);
            loft.TryAddPigeon(new Pigeon("A", PigeonSex.Male, PigeonColor.Blue, 30, new PigeonStats()));
            Assert.That(loft.IsFull, Is.True);
        }

        [Test]
        public void AdvanceWeek_AllPigeonsAge()
        {
            var loft = new Loft("Test", 5);
            var pigeon = new Pigeon("A", PigeonSex.Male, PigeonColor.Blue, 30, new PigeonStats());
            loft.TryAddPigeon(pigeon);
            loft.AdvanceWeek();
            Assert.That(pigeon.AgeInWeeks, Is.EqualTo(31));
        }
    }

    [TestFixture]
    public class PlayerProfileTests
    {
        [Test]
        public void Constructor_SetsNameAndMoney()
        {
            var player = new PlayerProfile("Tester", 5000);
            Assert.That(player.PlayerName, Is.EqualTo("Tester"));
            Assert.That(player.Money, Is.EqualTo(5000));
            Assert.That(player.Reputation, Is.EqualTo(0));
        }

        [Test]
        public void Constructor_NullName_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new PlayerProfile(null, 1000));
        }

        [Test]
        public void Constructor_NegativeMoney_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new PlayerProfile("Test", -1));
        }

        [Test]
        public void TrySpend_Sufficient_ReturnsTrue()
        {
            var player = new PlayerProfile("Test", 1000);
            bool result = player.TrySpend(500);
            Assert.That(result, Is.True);
            Assert.That(player.Money, Is.EqualTo(500));
        }

        [Test]
        public void TrySpend_Insufficient_ReturnsFalse()
        {
            var player = new PlayerProfile("Test", 100);
            bool result = player.TrySpend(500);
            Assert.That(result, Is.False);
            Assert.That(player.Money, Is.EqualTo(100));
        }

        [Test]
        public void TrySpend_NegativeAmount_Throws()
        {
            var player = new PlayerProfile("Test", 1000);
            Assert.Throws<ArgumentOutOfRangeException>(() => player.TrySpend(-1));
        }

        [Test]
        public void Earn_AddsToMoney()
        {
            var player = new PlayerProfile("Test", 1000);
            player.Earn(500);
            Assert.That(player.Money, Is.EqualTo(1500));
        }

        [Test]
        public void Earn_NegativeAmount_Throws()
        {
            var player = new PlayerProfile("Test", 1000);
            Assert.Throws<ArgumentOutOfRangeException>(() => player.Earn(-1));
        }

        [Test]
        public void AddReputation_Positive()
        {
            var player = new PlayerProfile("Test", 1000);
            player.AddReputation(5);
            Assert.That(player.Reputation, Is.EqualTo(5));
        }

        [Test]
        public void AddReputation_NeverGoesNegative()
        {
            var player = new PlayerProfile("Test", 1000);
            player.AddReputation(-100);
            Assert.That(player.Reputation, Is.EqualTo(0));
        }
    }

    [TestFixture]
    public class RaceModelTests
    {
        [Test]
        public void Constructor_SetsProperties()
        {
            var race = new Race("Test Race", RaceDistance.Medium, 500, 15, 1000);
            Assert.That(race.Name, Is.EqualTo("Test Race"));
            Assert.That(race.Distance, Is.EqualTo(RaceDistance.Medium));
            Assert.That(race.DistanceKm, Is.EqualTo(500));
            Assert.That(race.SeasonWeek, Is.EqualTo(15));
            Assert.That(race.PrizeMoney, Is.EqualTo(1000));
            Assert.That(race.IsCompleted, Is.False);
        }

        [Test]
        public void Constructor_NullName_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new Race(null, RaceDistance.Short, 160, 10, 500));
        }

        [Test]
        public void MarkCompleted_SetsResults()
        {
            var race = new Race("Test", RaceDistance.Short, 160, 10, 500);
            var results = new List<RaceResult>
            {
                new RaceResult(1, "Bird", 1, 3600f, 20f),
            };
            race.MarkCompleted(results);
            Assert.That(race.IsCompleted, Is.True);
            Assert.That(race.Results.Count, Is.EqualTo(1));
        }

        [Test]
        public void MarkCompleted_NullResults_Throws()
        {
            var race = new Race("Test", RaceDistance.Short, 160, 10, 500);
            Assert.Throws<ArgumentNullException>(() => race.MarkCompleted(null));
        }

        [Test]
        public void GetTypicalDistanceKm_Short()
        {
            Assert.That(Race.GetTypicalDistanceKm(RaceDistance.Short), Is.EqualTo(160));
        }

        [Test]
        public void GetTypicalDistanceKm_Medium()
        {
            Assert.That(Race.GetTypicalDistanceKm(RaceDistance.Medium), Is.EqualTo(500));
        }

        [Test]
        public void GetTypicalDistanceKm_Long()
        {
            Assert.That(Race.GetTypicalDistanceKm(RaceDistance.Long), Is.EqualTo(900));
        }

        [Test]
        public void RaceEvent_Constructor_NullMessage_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new RaceEvent(RaceEventType.RaceStarted, null, 0f));
        }

        [Test]
        public void RaceResult_Constructor_NullName_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new RaceResult(1, null, 1, 3600f, 20f));
        }
    }

    [TestFixture]
    public class SeasonTests
    {
        [Test]
        public void Constructor_InitializesCorrectly()
        {
            var races = new List<Race>
            {
                new Race("Race 1", RaceDistance.Short, 160, 12, 500),
            };
            var season = new Season(1, races);
            Assert.That(season.Year, Is.EqualTo(1));
            Assert.That(season.CurrentWeek, Is.EqualTo(1));
            Assert.That(season.IsOver, Is.False);
        }

        [Test]
        public void Constructor_NullRaces_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new Season(1, null));
        }

        [Test]
        public void IsRaceSeason_BeforeSeason_ReturnsFalse()
        {
            var season = new Season(1, new List<Race>());
            Assert.That(season.IsRaceSeason, Is.False);
        }

        [Test]
        public void IsRaceSeason_DuringSeason_ReturnsTrue()
        {
            var season = new Season(1, new List<Race>());
            for (int i = 0; i < Season.RaceSeasonStart - 1; i++)
            {
                season.AdvanceWeek();
            }

            Assert.That(season.IsRaceSeason, Is.True);
        }

        [Test]
        public void AdvanceWeek_IncrementsWeek()
        {
            var season = new Season(1, new List<Race>());
            season.AdvanceWeek();
            Assert.That(season.CurrentWeek, Is.EqualTo(2));
        }

        [Test]
        public void AdvanceWeek_DoesNotExceedMax()
        {
            var season = new Season(1, new List<Race>());
            for (int i = 0; i < 60; i++)
            {
                season.AdvanceWeek();
            }

            Assert.That(season.IsOver, Is.True);
        }

        [Test]
        public void GetRaceForWeek_ExistingRace()
        {
            var race = new Race("Race 1", RaceDistance.Short, 160, 12, 500);
            var season = new Season(1, new List<Race> { race });
            Assert.That(season.GetRaceForWeek(12), Is.SameAs(race));
        }

        [Test]
        public void GetRaceForWeek_NoRace_ReturnsNull()
        {
            var season = new Season(1, new List<Race>());
            Assert.That(season.GetRaceForWeek(5), Is.Null);
        }

        [Test]
        public void CompletedRaceCount_TracksCorrectly()
        {
            var race = new Race("Race 1", RaceDistance.Short, 160, 12, 500);
            var season = new Season(1, new List<Race> { race });
            Assert.That(season.CompletedRaceCount(), Is.EqualTo(0));
            race.MarkCompleted(new List<RaceResult>());
            Assert.That(season.CompletedRaceCount(), Is.EqualTo(1));
        }

        [Test]
        public void TotalRaceCount_ReturnsCorrect()
        {
            var races = new List<Race>
            {
                new Race("A", RaceDistance.Short, 160, 10, 500),
                new Race("B", RaceDistance.Medium, 500, 20, 1000),
            };
            var season = new Season(1, races);
            Assert.That(season.TotalRaceCount(), Is.EqualTo(2));
        }
    }

    [TestFixture]
    public class TrainingSystemTests
    {
        private TrainingSystem _trainingSystem;

        [SetUp]
        public void SetUp()
        {
            Pigeon.ResetIdCounter();
            _trainingSystem = new TrainingSystem(new Random(42));
        }

        [Test]
        public void Constructor_NullRng_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new TrainingSystem(null));
        }

        [Test]
        public void Train_NullPigeon_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _trainingSystem.Train(null, TrainingType.Speed));
        }

        [Test]
        public void Train_Speed_IncreasesSpeedStat()
        {
            var pigeon = new Pigeon("Test", PigeonSex.Male, PigeonColor.Blue, 30, new PigeonStats(30, 30, 30, 80));
            int oldSpeed = pigeon.Stats.Speed;
            TrainingResult result = _trainingSystem.Train(pigeon, TrainingType.Speed);
            Assert.That(pigeon.Stats.Speed, Is.GreaterThanOrEqualTo(oldSpeed));
            Assert.That(result.Type, Is.EqualTo(TrainingType.Speed));
        }

        [Test]
        public void Train_Endurance_IncreasesEnduranceStat()
        {
            var pigeon = new Pigeon("Test", PigeonSex.Male, PigeonColor.Blue, 30, new PigeonStats(30, 30, 30, 80));
            int oldEndurance = pigeon.Stats.Endurance;
            _trainingSystem.Train(pigeon, TrainingType.Endurance);
            Assert.That(pigeon.Stats.Endurance, Is.GreaterThanOrEqualTo(oldEndurance));
        }

        [Test]
        public void Train_Navigation_IncreasesNavigationStat()
        {
            var pigeon = new Pigeon("Test", PigeonSex.Male, PigeonColor.Blue, 30, new PigeonStats(30, 30, 30, 80));
            int oldNav = pigeon.Stats.Navigation;
            _trainingSystem.Train(pigeon, TrainingType.Navigation);
            Assert.That(pigeon.Stats.Navigation, Is.GreaterThanOrEqualTo(oldNav));
        }

        [Test]
        public void Train_AddsFatigue()
        {
            var pigeon = new Pigeon("Test", PigeonSex.Male, PigeonColor.Blue, 30, new PigeonStats(30, 30, 30, 80));
            _trainingSystem.Train(pigeon, TrainingType.Speed);
            Assert.That(pigeon.Fatigue, Is.GreaterThan(0));
        }

        [Test]
        public void Train_TooFatigued_NoGain()
        {
            var pigeon = new Pigeon("Test", PigeonSex.Male, PigeonColor.Blue, 30, new PigeonStats(30, 30, 30, 80));
            pigeon.AddFatigue(95);
            int oldSpeed = pigeon.Stats.Speed;
            TrainingResult result = _trainingSystem.Train(pigeon, TrainingType.Speed);
            Assert.That(pigeon.Stats.Speed, Is.EqualTo(oldSpeed));
            Assert.That(result.StatGain, Is.EqualTo(0));
        }

        [Test]
        public void Train_Rest_ReducesFatigue()
        {
            var pigeon = new Pigeon("Test", PigeonSex.Male, PigeonColor.Blue, 30, new PigeonStats(30, 30, 30, 80));
            pigeon.AddFatigue(50);
            int oldFatigue = pigeon.Fatigue;
            TrainingResult result = _trainingSystem.Train(pigeon, TrainingType.Rest);
            Assert.That(pigeon.Fatigue, Is.LessThan(oldFatigue));
            Assert.That(result.Type, Is.EqualTo(TrainingType.Rest));
        }

        [Test]
        public void Train_ReturnsNonNullMessage()
        {
            var pigeon = new Pigeon("Test", PigeonSex.Male, PigeonColor.Blue, 30, new PigeonStats(30, 30, 30, 80));
            TrainingResult result = _trainingSystem.Train(pigeon, TrainingType.Speed);
            Assert.That(result.Message, Is.Not.Null.And.Not.Empty);
        }
    }

    [TestFixture]
    public class TrainingResultTests
    {
        [Test]
        public void Constructor_NullMessage_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new TrainingResult(TrainingType.Speed, 5, 10, null));
        }

        [Test]
        public void Constructor_SetsProperties()
        {
            var result = new TrainingResult(TrainingType.Speed, 3, 8, "Good session.");
            Assert.That(result.Type, Is.EqualTo(TrainingType.Speed));
            Assert.That(result.StatGain, Is.EqualTo(3));
            Assert.That(result.FatigueGain, Is.EqualTo(8));
            Assert.That(result.Message, Is.EqualTo("Good session."));
        }
    }

    [TestFixture]
    public class RaceSimulatorTests
    {
        private RaceSimulator _simulator;

        [SetUp]
        public void SetUp()
        {
            Pigeon.ResetIdCounter();
            _simulator = new RaceSimulator(new Random(42));
        }

        [Test]
        public void Constructor_NullRng_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new RaceSimulator(null));
        }

        [Test]
        public void SimulateRace_NullRace_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _simulator.SimulateRace(null, new List<Pigeon>()));
        }

        [Test]
        public void SimulateRace_NullParticipants_Throws()
        {
            var race = new Race("Test", RaceDistance.Short, 160, 10, 500);
            Assert.Throws<ArgumentNullException>(() =>
                _simulator.SimulateRace(race, null));
        }

        [Test]
        public void SimulateRace_EmptyParticipants_Throws()
        {
            var race = new Race("Test", RaceDistance.Short, 160, 10, 500);
            Assert.Throws<ArgumentException>(() =>
                _simulator.SimulateRace(race, new List<Pigeon>()));
        }

        [Test]
        public void SimulateRace_ProducesResults()
        {
            var race = new Race("Test", RaceDistance.Short, 160, 10, 500);
            var pigeons = new List<Pigeon>
            {
                new Pigeon("A", PigeonSex.Male, PigeonColor.Blue, 30, new PigeonStats(70, 70, 70, 80)),
                new Pigeon("B", PigeonSex.Female, PigeonColor.Red, 30, new PigeonStats(60, 60, 60, 80)),
            };

            RaceSimulationRun sim = _simulator.SimulateRace(race, pigeons);

            Assert.That(sim.Results.Count, Is.EqualTo(2));
            Assert.That(sim.Events.Count, Is.GreaterThan(0));
            Assert.That(sim.Results[0].Position, Is.EqualTo(1));
            Assert.That(sim.Results[1].Position, Is.EqualTo(2));
        }

        [Test]
        public void SimulateRace_MediumDistance_ProducesResults()
        {
            var race = new Race("Test", RaceDistance.Medium, 500, 10, 1000);
            var pigeons = new List<Pigeon>
            {
                new Pigeon("A", PigeonSex.Male, PigeonColor.Blue, 30, new PigeonStats(70, 70, 70, 80)),
            };

            RaceSimulationRun sim = _simulator.SimulateRace(race, pigeons);
            Assert.That(sim.Results.Count, Is.EqualTo(1));
        }

        [Test]
        public void SimulateRace_LongDistance_ProducesResults()
        {
            var race = new Race("Test", RaceDistance.Long, 900, 10, 2000);
            var pigeons = new List<Pigeon>
            {
                new Pigeon("A", PigeonSex.Male, PigeonColor.Blue, 30, new PigeonStats(70, 70, 70, 80)),
            };

            RaceSimulationRun sim = _simulator.SimulateRace(race, pigeons);
            Assert.That(sim.Results.Count, Is.EqualTo(1));
        }

        [Test]
        public void SimulateRace_HasStartAndEndEvents()
        {
            var race = new Race("Test", RaceDistance.Short, 160, 10, 500);
            var pigeons = new List<Pigeon>
            {
                new Pigeon("A", PigeonSex.Male, PigeonColor.Blue, 30, new PigeonStats(70, 70, 70, 80)),
            };

            RaceSimulationRun sim = _simulator.SimulateRace(race, pigeons);

            Assert.That(sim.Events[0].EventType, Is.EqualTo(RaceEventType.RaceStarted));
            Assert.That(sim.Events[sim.Events.Count - 1].EventType, Is.EqualTo(RaceEventType.RaceCompleted));
        }

        [Test]
        public void GenerateRivalPigeons_CorrectCount()
        {
            List<Pigeon> rivals = _simulator.GenerateRivalPigeons(5, 50);
            Assert.That(rivals.Count, Is.EqualTo(5));
        }
    }

    [TestFixture]
    public class SeasonGeneratorTests
    {
        [Test]
        public void Constructor_NullRng_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new SeasonGenerator(null));
        }

        [Test]
        public void GenerateSeason_ProducesRaces()
        {
            var gen = new SeasonGenerator(new Random(42));
            Season season = gen.GenerateSeason(1);
            Assert.That(season.Year, Is.EqualTo(1));
            Assert.That(season.Races.Count, Is.GreaterThan(0));
        }

        [Test]
        public void GenerateSeason_RacesWithinSeasonWindow()
        {
            var gen = new SeasonGenerator(new Random(42));
            Season season = gen.GenerateSeason(1);
            foreach (Race race in season.Races)
            {
                Assert.That(race.SeasonWeek, Is.GreaterThanOrEqualTo(Season.RaceSeasonStart));
                Assert.That(race.SeasonWeek, Is.LessThanOrEqualTo(Season.RaceSeasonEnd + 5));
            }
        }
    }

    [TestFixture]
    public class PigeonGeneratorTests
    {
        [SetUp]
        public void SetUp()
        {
            Pigeon.ResetIdCounter();
        }

        [Test]
        public void Constructor_NullRng_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new PigeonGenerator(null));
        }

        [Test]
        public void GenerateRandom_ProducesValidPigeon()
        {
            var gen = new PigeonGenerator(new Random(42));
            Pigeon pigeon = gen.GenerateRandom(30, 70);
            Assert.That(pigeon.Name, Is.Not.Null.And.Not.Empty);
            Assert.That(pigeon.Stats.Speed, Is.InRange(30, 70));
            Assert.That(pigeon.Stats.Endurance, Is.InRange(30, 70));
        }

        [Test]
        public void GenerateStarterFlock_CorrectCount()
        {
            var gen = new PigeonGenerator(new Random(42));
            List<Pigeon> flock = gen.GenerateStarterFlock(6);
            Assert.That(flock.Count, Is.EqualTo(6));
        }
    }

    [TestFixture]
    public class GameStateTests
    {
        [SetUp]
        public void SetUp()
        {
            Pigeon.ResetIdCounter();
        }

        [Test]
        public void Constructor_InitializesCorrectly()
        {
            var state = new GameState("TestPlayer", new Random(42));
            Assert.That(state.Player.PlayerName, Is.EqualTo("TestPlayer"));
            Assert.That(state.Player.Money, Is.EqualTo(5000));
            Assert.That(state.PlayerLoft.Count, Is.EqualTo(6));
            Assert.That(state.CurrentSeason, Is.Not.Null);
        }

        [Test]
        public void TrainPigeon_NullPigeon_Throws()
        {
            var state = new GameState("Test", new Random(42));
            Assert.Throws<ArgumentNullException>(() =>
                state.TrainPigeon(null, TrainingType.Speed));
        }

        [Test]
        public void TrainPigeon_ReturnsResult()
        {
            var state = new GameState("Test", new Random(42));
            Pigeon pigeon = state.PlayerLoft.Pigeons[0];
            TrainingResult result = state.TrainPigeon(pigeon, TrainingType.Speed);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Message, Is.Not.Null);
        }

        [Test]
        public void RunRace_NullRace_Throws()
        {
            var state = new GameState("Test", new Random(42));
            Assert.Throws<ArgumentNullException>(() =>
                state.RunRace(null, new List<Pigeon>()));
        }

        [Test]
        public void RunRace_NullPigeons_Throws()
        {
            var state = new GameState("Test", new Random(42));
            var race = new Race("Test", RaceDistance.Short, 160, 10, 500);
            Assert.Throws<ArgumentNullException>(() =>
                state.RunRace(race, null));
        }

        [Test]
        public void RunRace_ProducesResults()
        {
            var state = new GameState("Test", new Random(42));
            var race = new Race("Test", RaceDistance.Short, 160, 10, 500);
            var selected = new List<Pigeon> { state.PlayerLoft.Pigeons[0] };
            RaceSimulationRun sim = state.RunRace(race, selected);
            Assert.That(sim.Results.Count, Is.GreaterThan(0));
            Assert.That(race.IsCompleted, Is.True);
        }

        [Test]
        public void AdvanceWeek_ProgressesSeason()
        {
            var state = new GameState("Test", new Random(42));
            int weekBefore = state.CurrentSeason.CurrentWeek;
            state.AdvanceWeek();
            Assert.That(state.CurrentSeason.CurrentWeek, Is.EqualTo(weekBefore + 1));
        }

        [Test]
        public void AdvanceWeek_SeasonRollover()
        {
            var state = new GameState("Test", new Random(42));
            for (int i = 0; i < 53; i++)
            {
                state.AdvanceWeek();
            }

            Assert.That(state.CurrentSeason.Year, Is.EqualTo(2));
        }

        [Test]
        public void GenerateMarketPigeon_ReturnsValidPigeon()
        {
            var state = new GameState("Test", new Random(42));
            Pigeon pigeon = state.GenerateMarketPigeon();
            Assert.That(pigeon, Is.Not.Null);
            Assert.That(pigeon.Name, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void TryBuyPigeon_NullPigeon_Throws()
        {
            var state = new GameState("Test", new Random(42));
            Assert.Throws<ArgumentNullException>(() =>
                state.TryBuyPigeon(null, 100));
        }

        [Test]
        public void TryBuyPigeon_CanAfford_AddsToLoft()
        {
            var state = new GameState("Test", new Random(42));
            Pigeon pigeon = state.GenerateMarketPigeon();
            bool result = state.TryBuyPigeon(pigeon, 100);
            Assert.That(result, Is.True);
            Assert.That(state.PlayerLoft.Count, Is.EqualTo(7));
        }

        [Test]
        public void TryBuyPigeon_CannotAfford_ReturnsFalse()
        {
            var state = new GameState("Test", new Random(42));
            Pigeon pigeon = state.GenerateMarketPigeon();
            bool result = state.TryBuyPigeon(pigeon, 99999);
            Assert.That(result, Is.False);
        }

        [Test]
        public void RaceSimulationRun_AddEvent_NullThrows()
        {
            var run = new RaceSimulationRun();
            Assert.Throws<ArgumentNullException>(() => run.AddEvent(null));
        }

        [Test]
        public void RaceSimulationRun_SetResults_NullThrows()
        {
            var run = new RaceSimulationRun();
            Assert.Throws<ArgumentNullException>(() => run.SetResults(null));
        }
    }
}

