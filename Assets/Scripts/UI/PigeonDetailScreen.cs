using PigeonRacingManager.Core;
using PigeonRacingManager.Models;
using PigeonRacingManager.Systems;
using UnityEngine;
using UnityEngine.UI;

namespace PigeonRacingManager.UI
{
    public class PigeonDetailScreen : MonoBehaviour
    {
        public static Pigeon SelectedPigeon { get; set; }

        [SerializeField]
        private Text _nameText;

        [SerializeField]
        private Text _detailsText;

        [SerializeField]
        private Text _statsText;

        [SerializeField]
        private Text _recordText;

        [SerializeField]
        private Text _trainingResultText;

        [SerializeField]
        private Button _trainSpeedButton;

        [SerializeField]
        private Button _trainEnduranceButton;

        [SerializeField]
        private Button _trainNavigationButton;

        [SerializeField]
        private Button _restButton;

        [SerializeField]
        private Button _backButton;

        private void OnEnable()
        {
            RefreshUI();

            if (_trainSpeedButton != null)
            {
                _trainSpeedButton.onClick.AddListener(() => OnTrainClicked(TrainingType.Speed));
            }

            if (_trainEnduranceButton != null)
            {
                _trainEnduranceButton.onClick.AddListener(() => OnTrainClicked(TrainingType.Endurance));
            }

            if (_trainNavigationButton != null)
            {
                _trainNavigationButton.onClick.AddListener(() => OnTrainClicked(TrainingType.Navigation));
            }

            if (_restButton != null)
            {
                _restButton.onClick.AddListener(() => OnTrainClicked(TrainingType.Rest));
            }

            if (_backButton != null)
            {
                _backButton.onClick.AddListener(OnBackClicked);
            }
        }

        private void OnDisable()
        {
            if (_trainSpeedButton != null)
            {
                _trainSpeedButton.onClick.RemoveAllListeners();
            }

            if (_trainEnduranceButton != null)
            {
                _trainEnduranceButton.onClick.RemoveAllListeners();
            }

            if (_trainNavigationButton != null)
            {
                _trainNavigationButton.onClick.RemoveAllListeners();
            }

            if (_restButton != null)
            {
                _restButton.onClick.RemoveAllListeners();
            }

            if (_backButton != null)
            {
                _backButton.onClick.RemoveAllListeners();
            }
        }

        public void RefreshUI()
        {
            if (SelectedPigeon == null)
            {
                return;
            }

            Pigeon pigeon = SelectedPigeon;

            if (_nameText != null)
            {
                _nameText.text = pigeon.Name;
            }

            if (_detailsText != null)
            {
                _detailsText.text = $"Sex: {pigeon.Sex} | Color: {pigeon.Color} | Age: {pigeon.AgeDisplay}";
            }

            if (_statsText != null)
            {
                _statsText.text = $"Speed: {pigeon.Stats.Speed}\n"
                    + $"Endurance: {pigeon.Stats.Endurance}\n"
                    + $"Navigation: {pigeon.Stats.Navigation}\n"
                    + $"Health: {pigeon.Stats.Health}\n"
                    + $"Overall: {pigeon.Stats.Overall}\n\n"
                    + $"Fatigue: {pigeon.Fatigue} | Condition: {pigeon.Condition}";
            }

            if (_recordText != null)
            {
                _recordText.text = $"Races: {pigeon.RacesCompleted} | Wins: {pigeon.RacesWon}";
            }
        }

        private void OnTrainClicked(TrainingType type)
        {
            if (SelectedPigeon == null || GameManager.Instance == null || GameManager.Instance.State == null)
            {
                return;
            }

            TrainingResult result = GameManager.Instance.State.TrainPigeon(SelectedPigeon, type);

            if (_trainingResultText != null)
            {
                _trainingResultText.text = result.Message;
            }

            RefreshUI();
        }

        private void OnBackClicked()
        {
            ScreenManager.Instance.ShowScreen(ScreenType.Loft);
        }
    }
}
