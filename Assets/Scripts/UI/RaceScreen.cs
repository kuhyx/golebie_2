using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PigeonRacingManager.Core;
using PigeonRacingManager.Models;
using PigeonRacingManager.Systems;
using UnityEngine;
using UnityEngine.UI;

namespace PigeonRacingManager.UI
{
    public class RaceScreen : MonoBehaviour
    {
        public static Race SelectedRace { get; set; }

        [SerializeField]
        private Text _raceNameText;

        [SerializeField]
        private Text _eventLogText;

        [SerializeField]
        private Text _resultsText;

        [SerializeField]
        private ScrollRect _eventScrollRect;

        [SerializeField]
        private Button _startRaceButton;

        [SerializeField]
        private Button _viewResultsButton;

        [SerializeField]
        private Button _backButton;

        [SerializeField]
        private RectTransform _raceProgressPanel;

        [SerializeField]
        private Text _raceTimerText;

        [SerializeField]
        private float _eventDisplayDelay = 0.3f;

        private RaceSimulationRun _lastSimulation;
        private readonly StringBuilder _eventLog = new StringBuilder();
        private readonly List<RaceBarEntry> _raceBars = new List<RaceBarEntry>();

        private static readonly Color PlayerBarColor = new Color(0.9f, 0.75f, 0.1f, 1f);
        private static readonly Color RivalBarColor = new Color(0.3f, 0.5f, 0.8f, 1f);
        private static readonly Color BarBgColor = new Color(0.15f, 0.15f, 0.2f, 1f);
        private static readonly Color PlayerLabelColor = new Color(1f, 0.9f, 0.3f, 1f);
        private static readonly Color RivalLabelColor = new Color(0.7f, 0.8f, 0.9f, 1f);

        private void OnEnable()
        {
            RefreshUI();

            if (_startRaceButton != null)
            {
                _startRaceButton.onClick.AddListener(OnStartRaceClicked);
            }

            if (_viewResultsButton != null)
            {
                _viewResultsButton.onClick.AddListener(OnViewResultsClicked);
                _viewResultsButton.gameObject.SetActive(false);
            }

            if (_backButton != null)
            {
                _backButton.onClick.AddListener(OnBackClicked);
            }
        }

        private void OnDisable()
        {
            if (_startRaceButton != null)
            {
                _startRaceButton.onClick.RemoveListener(OnStartRaceClicked);
            }

            if (_viewResultsButton != null)
            {
                _viewResultsButton.onClick.RemoveListener(OnViewResultsClicked);
            }

            if (_backButton != null)
            {
                _backButton.onClick.RemoveListener(OnBackClicked);
            }

            StopAllCoroutines();
            ClearProgressBars();
        }

        public void RefreshUI()
        {
            if (SelectedRace == null)
            {
                return;
            }

            if (_raceNameText != null)
            {
                _raceNameText.text = $"{SelectedRace.Name} ({SelectedRace.Distance}, {SelectedRace.DistanceKm}km)";
            }

            if (_eventLogText != null)
            {
                _eventLogText.text = string.Empty;
            }

            if (_resultsText != null)
            {
                _resultsText.text = string.Empty;
            }

            if (_raceTimerText != null)
            {
                _raceTimerText.text = string.Empty;
            }

            _eventLog.Clear();
            ClearProgressBars();

            bool canRace = !SelectedRace.IsCompleted;
            if (_startRaceButton != null)
            {
                _startRaceButton.gameObject.SetActive(canRace);
            }
        }

        private void OnStartRaceClicked()
        {
            if (SelectedRace == null || GameManager.Instance == null || GameManager.Instance.State == null)
            {
                return;
            }

            List<Pigeon> readyPigeons = new List<Pigeon>();
            foreach (Pigeon pigeon in GameManager.Instance.State.PlayerLoft.Pigeons)
            {
                if (pigeon.IsRaceReady)
                {
                    readyPigeons.Add(pigeon);
                }
            }

            if (readyPigeons.Count == 0)
            {
                if (_eventLogText != null)
                {
                    _eventLogText.text = "No pigeons are race-ready!";
                }

                return;
            }

            if (_startRaceButton != null)
            {
                _startRaceButton.gameObject.SetActive(false);
            }

            _lastSimulation = GameManager.Instance.State.RunRace(SelectedRace, readyPigeons);
            GameManager.Instance.NotifyMoneyChanged();

            CreateProgressBars();
            StartCoroutine(DisplayRaceCoroutine());
        }

        private void CreateProgressBars()
        {
            ClearProgressBars();

            if (_raceProgressPanel == null || _lastSimulation == null)
            {
                return;
            }

            IReadOnlyDictionary<int, string> names = _lastSimulation.ParticipantNames;
            HashSet<int> playerIds = _lastSimulation.PlayerPigeonIds;

            // Sort: player pigeons first, then rivals
            List<int> sortedIds = names.Keys
                .OrderByDescending(id => playerIds.Contains(id))
                .ThenBy(id => names[id])
                .ToList();

            int count = sortedIds.Count;
            float usableHeight = 0.88f;
            float barHeight = Mathf.Min(usableHeight / count, 0.06f);
            float spacing = (count > 1) ? (usableHeight - (barHeight * count)) / (count - 1) : 0f;
            float maxTrackWidth = 0.9f;

            for (int i = 0; i < count; i++)
            {
                int pigeonId = sortedIds[i];
                bool isPlayer = playerIds.Contains(pigeonId);
                float yTop = usableHeight - (i * (barHeight + spacing));
                float yBottom = yTop - barHeight;

                // Name label (left side)
                GameObject labelGo = new GameObject($"Label_{pigeonId}", typeof(RectTransform));
                labelGo.transform.SetParent(_raceProgressPanel, false);
                RectTransform labelRT = labelGo.GetComponent<RectTransform>();
                labelRT.anchorMin = new Vector2(0f, yBottom);
                labelRT.anchorMax = new Vector2(0.14f, yTop);
                labelRT.sizeDelta = Vector2.zero;
                Text label = labelGo.AddComponent<Text>();
                label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                label.fontSize = count > 12 ? 11 : 13;
                label.color = isPlayer ? PlayerLabelColor : RivalLabelColor;
                label.alignment = TextAnchor.MiddleRight;
                label.text = names[pigeonId];
                if (isPlayer)
                {
                    label.fontStyle = FontStyle.Bold;
                }

                // Track background
                GameObject bgGo = new GameObject($"TrackBg_{pigeonId}", typeof(RectTransform));
                bgGo.transform.SetParent(_raceProgressPanel, false);
                RectTransform bgRT = bgGo.GetComponent<RectTransform>();
                bgRT.anchorMin = new Vector2(0.15f, yBottom);
                bgRT.anchorMax = new Vector2(0.15f + maxTrackWidth, yTop);
                bgRT.sizeDelta = Vector2.zero;
                bgGo.AddComponent<Image>().color = BarBgColor;

                // Progress bar (fill)
                GameObject barGo = new GameObject($"Bar_{pigeonId}", typeof(RectTransform));
                barGo.transform.SetParent(bgGo.transform, false);
                RectTransform barRT = barGo.GetComponent<RectTransform>();
                barRT.anchorMin = Vector2.zero;
                barRT.anchorMax = new Vector2(0f, 1f);
                barRT.sizeDelta = Vector2.zero;
                barGo.AddComponent<Image>().color = isPlayer ? PlayerBarColor : RivalBarColor;

                _raceBars.Add(new RaceBarEntry(pigeonId, barRT, isPlayer));
            }
        }

        private void ClearProgressBars()
        {
            foreach (RaceBarEntry entry in _raceBars)
            {
                if (entry.BarRect != null && entry.BarRect.parent != null)
                {
                    Destroy(entry.BarRect.parent.gameObject);
                }
            }

            _raceBars.Clear();

            if (_raceProgressPanel != null)
            {
                // Destroy labels and track backgrounds
                for (int i = _raceProgressPanel.childCount - 1; i >= 0; i--)
                {
                    Transform child = _raceProgressPanel.GetChild(i);
                    if (child.name.StartsWith("Label_") || child.name.StartsWith("TrackBg_"))
                    {
                        Destroy(child.gameObject);
                    }
                }
            }
        }

        private void UpdateProgressBars(Dictionary<int, float> snapshot)
        {
            if (snapshot == null)
            {
                return;
            }

            foreach (RaceBarEntry entry in _raceBars)
            {
                if (snapshot.TryGetValue(entry.PigeonId, out float progress))
                {
                    entry.BarRect.anchorMax = new Vector2(progress, 1f);
                }
            }
        }

        private IEnumerator DisplayRaceCoroutine()
        {
            foreach (RaceEvent raceEvent in _lastSimulation.Events)
            {
                string timeStr = FormatTime(raceEvent.TimeInRace);

                if (_raceTimerText != null)
                {
                    _raceTimerText.text = $"Race Time: {timeStr}";
                }

                UpdateProgressBars(raceEvent.ProgressSnapshot);

                _eventLog.AppendLine($"[{timeStr}] {raceEvent.Message}");

                if (_eventLogText != null)
                {
                    _eventLogText.text = _eventLog.ToString();
                }

                if (_eventScrollRect != null)
                {
                    Canvas.ForceUpdateCanvases();
                    _eventScrollRect.verticalNormalizedPosition = 0f;
                }

                yield return new WaitForSeconds(_eventDisplayDelay);
            }

            if (_viewResultsButton != null)
            {
                _viewResultsButton.gameObject.SetActive(true);
            }
        }

        private void OnViewResultsClicked()
        {
            if (_lastSimulation == null)
            {
                return;
            }

            RaceResultScreen.LastSimulation = _lastSimulation;
            RaceResultScreen.LastRace = SelectedRace;
            ScreenManager.Instance.ShowScreen(ScreenType.RaceResult);
        }

        private void OnBackClicked()
        {
            ScreenManager.Instance.ShowScreen(ScreenType.Loft);
        }

        private static string FormatTime(float seconds)
        {
            int hours = (int)(seconds / 3600f);
            int minutes = (int)((seconds % 3600f) / 60f);
            int secs = (int)(seconds % 60f);
            return hours > 0
                ? $"{hours}h{minutes:D2}m{secs:D2}s"
                : $"{minutes}m{secs:D2}s";
        }

        private class RaceBarEntry
        {
            public int PigeonId { get; }
            public RectTransform BarRect { get; }
            public bool IsPlayer { get; }

            public RaceBarEntry(int pigeonId, RectTransform barRect, bool isPlayer)
            {
                PigeonId = pigeonId;
                BarRect = barRect;
                IsPlayer = isPlayer;
            }
        }
    }
}
