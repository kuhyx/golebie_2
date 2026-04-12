using UnityEngine;
using UnityEngine.UI;

namespace PigeonRacingManager.UI
{
    public class MainMenuScreen : MonoBehaviour
    {
        [SerializeField]
        private InputField _playerNameInput;

        [SerializeField]
        private Button _startButton;

        [SerializeField]
        private Text _titleText;

        private void Start()
        {
            if (_startButton != null)
            {
                _startButton.onClick.AddListener(OnStartClicked);
            }

            if (_titleText != null)
            {
                _titleText.text = "Pigeon Racing Manager";
            }
        }

        private void OnStartClicked()
        {
            string playerName = (_playerNameInput != null && !string.IsNullOrWhiteSpace(_playerNameInput.text))
                ? _playerNameInput.text.Trim()
                : "Player";

            Core.GameManager.Instance.StartNewGame(playerName);
            Core.ScreenManager.Instance.ShowScreen(Core.ScreenType.Loft);
        }

        private void OnDestroy()
        {
            if (_startButton != null)
            {
                _startButton.onClick.RemoveListener(OnStartClicked);
            }
        }
    }
}
