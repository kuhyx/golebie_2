using System;
using System.Collections.Generic;
using UnityEngine;

namespace PigeonRacingManager.Core
{
    public enum ScreenType
    {
        MainMenu,
        Loft,
        PigeonDetail,
        Training,
        Race,
        Calendar,
        Market,
        RaceResult,
    }

    public class ScreenManager : MonoBehaviour
    {
        public static ScreenManager Instance { get; private set; }

        [SerializeField]
        private List<ScreenMapping> _screenMappings = new List<ScreenMapping>();

        private readonly Dictionary<ScreenType, GameObject> _screens = new Dictionary<ScreenType, GameObject>();
        private ScreenType _currentScreen;

        public ScreenType CurrentScreen => _currentScreen;
        public event Action<ScreenType> OnScreenChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            foreach (ScreenMapping mapping in _screenMappings)
            {
                if (mapping.ScreenObject != null)
                {
                    _screens[mapping.Type] = mapping.ScreenObject;
                    mapping.ScreenObject.SetActive(false);
                }
            }
        }

        public void ShowScreen(ScreenType type)
        {
            foreach (KeyValuePair<ScreenType, GameObject> screen in _screens)
            {
                screen.Value.SetActive(screen.Key == type);
            }

            _currentScreen = type;
            OnScreenChanged?.Invoke(type);
        }

        [Serializable]
        public class ScreenMapping
        {
            public ScreenType Type;
            public GameObject ScreenObject;
        }
    }
}
