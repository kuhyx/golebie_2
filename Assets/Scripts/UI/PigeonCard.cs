using PigeonRacingManager.Core;
using PigeonRacingManager.Models;
using UnityEngine;
using UnityEngine.UI;

namespace PigeonRacingManager.UI
{
    public class PigeonCard : MonoBehaviour
    {
        [SerializeField]
        private Text _nameText;

        [SerializeField]
        private Text _statsText;

        [SerializeField]
        private Text _ageText;

        [SerializeField]
        private Text _statusText;

        [SerializeField]
        private Button _selectButton;

        private Pigeon _pigeon;

        public void Setup(Pigeon pigeon)
        {
            _pigeon = pigeon;

            if (_nameText != null)
            {
                _nameText.text = $"{pigeon.Name} ({pigeon.Sex})";
            }

            if (_statsText != null)
            {
                _statsText.text = $"SPD:{pigeon.Stats.Speed} END:{pigeon.Stats.Endurance} "
                    + $"NAV:{pigeon.Stats.Navigation} HP:{pigeon.Stats.Health} "
                    + $"OVR:{pigeon.Stats.Overall}";
            }

            if (_ageText != null)
            {
                _ageText.text = $"Age: {pigeon.AgeDisplay} | {pigeon.Color}";
            }

            if (_statusText != null)
            {
                string status = pigeon.IsRaceReady ? "Ready" : "Not Ready";
                _statusText.text = $"Fatigue: {pigeon.Fatigue} | Cond: {pigeon.Condition} | {status}";
            }

            if (_selectButton != null)
            {
                _selectButton.onClick.AddListener(OnSelectClicked);
            }
        }

        private void OnSelectClicked()
        {
            if (_pigeon == null)
            {
                return;
            }

            PigeonDetailScreen.SelectedPigeon = _pigeon;
            ScreenManager.Instance.ShowScreen(ScreenType.PigeonDetail);
        }

        private void OnDestroy()
        {
            if (_selectButton != null)
            {
                _selectButton.onClick.RemoveListener(OnSelectClicked);
            }
        }
    }
}
