using System.Collections.Generic;
using PigeonRacingManager.Core;
using PigeonRacingManager.Models;
using UnityEngine;
using UnityEngine.UI;

namespace PigeonRacingManager.UI
{
    public class LoftScreen : MonoBehaviour
    {
        [SerializeField]
        private Transform _pigeonListContent;

        [SerializeField]
        private GameObject _pigeonCardPrefab;

        [SerializeField]
        private Text _headerText;

        [SerializeField]
        private Text _moneyText;

        [SerializeField]
        private Button _calendarButton;

        [SerializeField]
        private Button _marketButton;

        [SerializeField]
        private Button _advanceWeekButton;

        private readonly List<GameObject> _cards = new List<GameObject>();

        private void OnEnable()
        {
            RefreshUI();

            if (_calendarButton != null)
            {
                _calendarButton.onClick.AddListener(OnCalendarClicked);
            }

            if (_marketButton != null)
            {
                _marketButton.onClick.AddListener(OnMarketClicked);
            }

            if (_advanceWeekButton != null)
            {
                _advanceWeekButton.onClick.AddListener(OnAdvanceWeekClicked);
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnWeekAdvanced += RefreshUI;
            }
        }

        private void OnDisable()
        {
            if (_calendarButton != null)
            {
                _calendarButton.onClick.RemoveListener(OnCalendarClicked);
            }

            if (_marketButton != null)
            {
                _marketButton.onClick.RemoveListener(OnMarketClicked);
            }

            if (_advanceWeekButton != null)
            {
                _advanceWeekButton.onClick.RemoveListener(OnAdvanceWeekClicked);
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnWeekAdvanced -= RefreshUI;
            }
        }

        public void RefreshUI()
        {
            if (GameManager.Instance == null || GameManager.Instance.State == null)
            {
                return;
            }

            Systems.GameState state = GameManager.Instance.State;

            if (_headerText != null)
            {
                _headerText.text = $"Year {state.CurrentSeason.Year} - Week {state.CurrentSeason.CurrentWeek}";
            }

            if (_moneyText != null)
            {
                _moneyText.text = $"${state.Player.Money:N0}";
            }

            ClearCards();
            PopulatePigeonCards(state.PlayerLoft);
        }

        private void PopulatePigeonCards(Loft loft)
        {
            if (_pigeonCardPrefab == null || _pigeonListContent == null)
            {
                return;
            }

            foreach (Pigeon pigeon in loft.Pigeons)
            {
                GameObject card = Instantiate(_pigeonCardPrefab, _pigeonListContent);
                card.SetActive(true);
                PigeonCard pigeonCard = card.GetComponent<PigeonCard>();
                if (pigeonCard != null)
                {
                    pigeonCard.Setup(pigeon);
                }

                _cards.Add(card);
            }

            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(
                _pigeonListContent.GetComponent<RectTransform>());
        }

        private void ClearCards()
        {
            foreach (GameObject card in _cards)
            {
                Destroy(card);
            }

            _cards.Clear();
        }

        private void OnCalendarClicked()
        {
            ScreenManager.Instance.ShowScreen(ScreenType.Calendar);
        }

        private void OnMarketClicked()
        {
            ScreenManager.Instance.ShowScreen(ScreenType.Market);
        }

        private void OnAdvanceWeekClicked()
        {
            GameManager.Instance.AdvanceWeek();
        }
    }
}
