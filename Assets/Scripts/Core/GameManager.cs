using System;
using PigeonRacingManager.Systems;
using UnityEngine;

namespace PigeonRacingManager.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField]
        private string _playerName = "Player";

        [SerializeField]
        private int _randomSeed = -1;

        public GameState State { get; private set; }

        public event Action OnWeekAdvanced;
        public event Action OnSeasonChanged;
        public event Action OnMoneyChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            StartNewGame(_playerName);

            if (ScreenManager.Instance != null)
            {
                ScreenManager.Instance.ShowScreen(ScreenType.Loft);
            }
        }

        public void StartNewGame(string playerName)
        {
            string name = string.IsNullOrWhiteSpace(playerName) ? _playerName : playerName;
            System.Random rng = (_randomSeed >= 0) ? new System.Random(_randomSeed) : new System.Random();
            State = new GameState(name, rng);
        }

        public void AdvanceWeek()
        {
            if (State == null)
            {
                return;
            }

            int oldYear = State.CurrentSeason.Year;
            State.AdvanceWeek();

            OnWeekAdvanced?.Invoke();

            if (State.CurrentSeason.Year != oldYear)
            {
                OnSeasonChanged?.Invoke();
            }
        }

        public void NotifyMoneyChanged()
        {
            OnMoneyChanged?.Invoke();
        }
    }
}
