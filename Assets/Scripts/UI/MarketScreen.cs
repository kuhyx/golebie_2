using PigeonRacingManager.Core;
using PigeonRacingManager.Models;
using UnityEngine;
using UnityEngine.UI;

namespace PigeonRacingManager.UI
{
    public class MarketScreen : MonoBehaviour
    {
        [SerializeField]
        private Text _headerText;

        [SerializeField]
        private Text _pigeonInfoText;

        [SerializeField]
        private Text _priceText;

        [SerializeField]
        private Text _feedbackText;

        [SerializeField]
        private Button _rerollButton;

        [SerializeField]
        private Button _buyButton;

        [SerializeField]
        private Button _backButton;

        private Pigeon _currentPigeon;
        private int _currentPrice;

        private void OnEnable()
        {
            GenerateNewOffer();

            if (_rerollButton != null)
            {
                _rerollButton.onClick.AddListener(GenerateNewOffer);
            }

            if (_buyButton != null)
            {
                _buyButton.onClick.AddListener(OnBuyClicked);
            }

            if (_backButton != null)
            {
                _backButton.onClick.AddListener(OnBackClicked);
            }
        }

        private void OnDisable()
        {
            if (_rerollButton != null)
            {
                _rerollButton.onClick.RemoveListener(GenerateNewOffer);
            }

            if (_buyButton != null)
            {
                _buyButton.onClick.RemoveListener(OnBuyClicked);
            }

            if (_backButton != null)
            {
                _backButton.onClick.RemoveListener(OnBackClicked);
            }
        }

        private void GenerateNewOffer()
        {
            if (GameManager.Instance == null || GameManager.Instance.State == null)
            {
                return;
            }

            _currentPigeon = GameManager.Instance.State.GenerateMarketPigeon();
            _currentPrice = CalculatePrice(_currentPigeon);

            if (_headerText != null)
            {
                _headerText.text = $"Market - Your Funds: ${GameManager.Instance.State.Player.Money:N0}";
            }

            if (_pigeonInfoText != null)
            {
                _pigeonInfoText.text = $"{_currentPigeon.Name} ({_currentPigeon.Sex}, {_currentPigeon.Color})\n"
                    + $"Age: {_currentPigeon.AgeDisplay}\n"
                    + $"SPD:{_currentPigeon.Stats.Speed} END:{_currentPigeon.Stats.Endurance} "
                    + $"NAV:{_currentPigeon.Stats.Navigation} HP:{_currentPigeon.Stats.Health}\n"
                    + $"Overall: {_currentPigeon.Stats.Overall}";
            }

            if (_priceText != null)
            {
                _priceText.text = $"Price: ${_currentPrice:N0}";
            }

            if (_feedbackText != null)
            {
                _feedbackText.text = string.Empty;
            }
        }

        private void OnBuyClicked()
        {
            if (_currentPigeon == null || GameManager.Instance == null || GameManager.Instance.State == null)
            {
                return;
            }

            bool success = GameManager.Instance.State.TryBuyPigeon(_currentPigeon, _currentPrice);

            if (success)
            {
                if (_feedbackText != null)
                {
                    _feedbackText.text = $"Bought {_currentPigeon.Name}!";
                }

                GameManager.Instance.NotifyMoneyChanged();
                GenerateNewOffer();
            }
            else if (GameManager.Instance.State.PlayerLoft.IsFull)
            {
                if (_feedbackText != null)
                {
                    _feedbackText.text = "Your loft is full!";
                }
            }
            else
            {
                if (_feedbackText != null)
                {
                    _feedbackText.text = "Not enough money!";
                }
            }
        }

        private void OnBackClicked()
        {
            ScreenManager.Instance.ShowScreen(ScreenType.Loft);
        }

        private static int CalculatePrice(Pigeon pigeon)
        {
            return 200 + (pigeon.Stats.Overall * 10);
        }
    }
}
