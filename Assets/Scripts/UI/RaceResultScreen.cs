using System.Text;
using PigeonRacingManager.Core;
using PigeonRacingManager.Models;
using PigeonRacingManager.Systems;
using UnityEngine;
using UnityEngine.UI;

namespace PigeonRacingManager.UI
{
    public class RaceResultScreen : MonoBehaviour
    {
        public static RaceSimulationRun LastSimulation { get; set; }
        public static Race LastRace { get; set; }

        [SerializeField]
        private Text _headerText;

        [SerializeField]
        private Text _resultsText;

        [SerializeField]
        private Text _prizesText;

        [SerializeField]
        private Button _backButton;

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
            if (LastSimulation == null || LastRace == null)
            {
                return;
            }

            if (_headerText != null)
            {
                _headerText.text = $"{LastRace.Name} - Results";
            }

            if (_resultsText != null)
            {
                StringBuilder sb = new StringBuilder();
                foreach (RaceResult result in LastSimulation.Results)
                {
                    string medal = result.Position switch
                    {
                        1 => "[1st]",
                        2 => "[2nd]",
                        3 => "[3rd]",
                        _ => $"[{result.Position}th]",
                    };
                    sb.AppendLine($"{medal} {result.PigeonName} - {result.FinishTime:F1}s "
                        + $"({result.SpeedMps:F1} m/s)");
                }

                _resultsText.text = sb.ToString();
            }

            if (_prizesText != null)
            {
                _prizesText.text = $"Prize Pool: ${LastRace.PrizeMoney:N0}\n"
                    + $"1st: ${LastRace.PrizeMoney:N0} | 2nd: ${LastRace.PrizeMoney / 2:N0} "
                    + $"| 3rd: ${LastRace.PrizeMoney / 4:N0}";
            }
        }

        private void OnBackClicked()
        {
            ScreenManager.Instance.ShowScreen(ScreenType.Loft);
        }
    }
}
