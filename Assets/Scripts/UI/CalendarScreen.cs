using System.Collections.Generic;
using PigeonRacingManager.Core;
using PigeonRacingManager.Models;
using UnityEngine;
using UnityEngine.UI;

namespace PigeonRacingManager.UI
{
    public class CalendarScreen : MonoBehaviour
    {
        [SerializeField]
        private Transform _raceListContent;

        [SerializeField]
        private GameObject _raceEntryPrefab;

        [SerializeField]
        private Text _headerText;

        [SerializeField]
        private Button _backButton;

        private readonly List<GameObject> _entries = new List<GameObject>();

        private void OnEnable()
        {
            RefreshUI();

            if (_backButton != null)
            {
                _backButton.onClick.AddListener(OnBackClicked);
            }
        }

        private void OnDisable()
        {
            if (_backButton != null)
            {
                _backButton.onClick.RemoveListener(OnBackClicked);
            }
        }

        public void RefreshUI()
        {
            if (GameManager.Instance == null || GameManager.Instance.State == null)
            {
                return;
            }

            Systems.GameState state = GameManager.Instance.State;
            Season season = state.CurrentSeason;

            if (_headerText != null)
            {
                _headerText.text = $"Season {season.Year} Calendar - Week {season.CurrentWeek}/{Season.WeeksPerSeason}";
            }

            ClearEntries();

            if (_raceEntryPrefab == null || _raceListContent == null)
            {
                return;
            }

            foreach (Race race in season.Races)
            {
                GameObject entry = Instantiate(_raceEntryPrefab, _raceListContent);
                entry.SetActive(true);
                Text entryText = entry.GetComponentInChildren<Text>();
                if (entryText != null)
                {
                    string status = race.IsCompleted ? "[DONE]" : "[UPCOMING]";
                    bool isCurrent = race.SeasonWeek == season.CurrentWeek;
                    string marker = isCurrent ? " <<< THIS WEEK" : string.Empty;
                    entryText.text = $"Week {race.SeasonWeek}: {race.Name} ({race.Distance}, {race.DistanceKm}km) "
                        + $"- Prize: ${race.PrizeMoney:N0} {status}{marker}";
                }

                Button raceButton = entry.GetComponent<Button>();
                if (raceButton != null)
                {
                    Race capturedRace = race;
                    raceButton.onClick.AddListener(() => OnRaceEntryClicked(capturedRace));
                    raceButton.interactable = !race.IsCompleted && race.SeasonWeek == season.CurrentWeek;
                }

                _entries.Add(entry);
            }

            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(
                _raceListContent.GetComponent<RectTransform>());
        }

        private void OnRaceEntryClicked(Race race)
        {
            RaceScreen.SelectedRace = race;
            ScreenManager.Instance.ShowScreen(ScreenType.Race);
        }

        private void ClearEntries()
        {
            foreach (GameObject entry in _entries)
            {
                Destroy(entry);
            }

            _entries.Clear();
        }

        private void OnBackClicked()
        {
            ScreenManager.Instance.ShowScreen(ScreenType.Loft);
        }
    }
}
