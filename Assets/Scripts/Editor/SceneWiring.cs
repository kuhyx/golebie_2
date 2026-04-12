using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace PigeonRacingManager.Editor
{
    public static class SceneWiring
    {
        [MenuItem("Tools/Wire UI References")]
        public static void WireReferences()
        {
            GameObject canvas = GameObject.Find("Canvas");
            if (canvas == null)
            {
                Debug.LogError("Canvas not found! Run 'Setup Game Scene' first.");
                return;
            }

            WireMainMenu(canvas.transform);
            WireLoftScreen(canvas.transform);
            WirePigeonDetailScreen(canvas.transform);
            WireCalendarScreen(canvas.transform);
            WireRaceScreen(canvas.transform);
            WireRaceResultScreen(canvas.transform);
            WireMarketScreen(canvas.transform);
            WireScreenManager(canvas.transform);

            Debug.Log("All UI references wired successfully!");
        }

        private static void WireMainMenu(Transform canvas)
        {
            Transform panel = canvas.Find("MainMenuScreen");
            if (panel == null)
            {
                return;
            }

            UI.MainMenuScreen comp = panel.GetComponent<UI.MainMenuScreen>();
            if (comp == null)
            {
                return;
            }

            SerializedObject so = new SerializedObject(comp);
            SetProp(so, "_titleText", FindChild<Text>(panel, "TitleText"));
            SetProp(so, "_playerNameInput", FindChild<InputField>(panel, "PlayerNameInput"));
            SetProp(so, "_startButton", FindChild<Button>(panel, "StartButton"));
            so.ApplyModifiedProperties();
        }

        private static void WireLoftScreen(Transform canvas)
        {
            Transform panel = canvas.Find("LoftScreen");
            if (panel == null)
            {
                return;
            }

            UI.LoftScreen comp = panel.GetComponent<UI.LoftScreen>();
            if (comp == null)
            {
                return;
            }

            SerializedObject so = new SerializedObject(comp);
            SetProp(so, "_headerText", FindChild<Text>(panel, "HeaderText"));
            SetProp(so, "_moneyText", FindChild<Text>(panel, "MoneyText"));

            Transform scroll = panel.Find("PigeonListScroll");
            if (scroll != null)
            {
                Transform content = scroll.Find("Viewport/Content");
                SetProp(so, "_pigeonListContent", content);
            }

            // Pigeon card prefab
            Transform cardPrefab = canvas.Find("PigeonCardPrefab");
            if (cardPrefab != null)
            {
                so.FindProperty("_pigeonCardPrefab").objectReferenceValue = cardPrefab.gameObject;
            }

            SetProp(so, "_calendarButton", FindChild<Button>(panel, "CalendarButton"));
            SetProp(so, "_marketButton", FindChild<Button>(panel, "MarketButton"));
            SetProp(so, "_advanceWeekButton", FindChild<Button>(panel, "AdvanceWeekButton"));
            so.ApplyModifiedProperties();
        }

        private static void WirePigeonDetailScreen(Transform canvas)
        {
            Transform panel = canvas.Find("PigeonDetailScreen");
            if (panel == null)
            {
                return;
            }

            UI.PigeonDetailScreen comp = panel.GetComponent<UI.PigeonDetailScreen>();
            if (comp == null)
            {
                return;
            }

            SerializedObject so = new SerializedObject(comp);
            SetProp(so, "_nameText", FindChild<Text>(panel, "NameText"));
            SetProp(so, "_detailsText", FindChild<Text>(panel, "DetailsText"));
            SetProp(so, "_statsText", FindChild<Text>(panel, "StatsText"));
            SetProp(so, "_recordText", FindChild<Text>(panel, "RecordText"));
            SetProp(so, "_trainingResultText", FindChild<Text>(panel, "TrainingResultText"));
            SetProp(so, "_trainSpeedButton", FindChild<Button>(panel, "TrainSpeedButton"));
            SetProp(so, "_trainEnduranceButton", FindChild<Button>(panel, "TrainEnduranceButton"));
            SetProp(so, "_trainNavigationButton", FindChild<Button>(panel, "TrainNavigationButton"));
            SetProp(so, "_restButton", FindChild<Button>(panel, "RestButton"));
            SetProp(so, "_backButton", FindChild<Button>(panel, "BackButton"));
            so.ApplyModifiedProperties();
        }

        private static void WireCalendarScreen(Transform canvas)
        {
            Transform panel = canvas.Find("CalendarScreen");
            if (panel == null)
            {
                return;
            }

            UI.CalendarScreen comp = panel.GetComponent<UI.CalendarScreen>();
            if (comp == null)
            {
                return;
            }

            SerializedObject so = new SerializedObject(comp);
            SetProp(so, "_headerText", FindChild<Text>(panel, "HeaderText"));

            Transform scroll = panel.Find("RaceListScroll");
            if (scroll != null)
            {
                Transform content = scroll.Find("Viewport/Content");
                SetProp(so, "_raceListContent", content);
            }

            // Race entry prefab
            Transform entryPrefab = canvas.Find("RaceEntryPrefab");
            if (entryPrefab != null)
            {
                so.FindProperty("_raceEntryPrefab").objectReferenceValue = entryPrefab.gameObject;
            }

            SetProp(so, "_backButton", FindChild<Button>(panel, "BackButton"));
            so.ApplyModifiedProperties();
        }

        private static void WireRaceScreen(Transform canvas)
        {
            Transform panel = canvas.Find("RaceScreen");
            if (panel == null)
            {
                return;
            }

            UI.RaceScreen comp = panel.GetComponent<UI.RaceScreen>();
            if (comp == null)
            {
                return;
            }

            SerializedObject so = new SerializedObject(comp);
            SetProp(so, "_raceNameText", FindChild<Text>(panel, "RaceNameText"));

            Transform scroll = panel.Find("EventLogScroll");
            if (scroll != null)
            {
                so.FindProperty("_eventScrollRect").objectReferenceValue = scroll.GetComponent<ScrollRect>();
                Transform content = scroll.Find("Viewport/Content");
                if (content != null)
                {
                    SetProp(so, "_eventLogText", FindChild<Text>(content, "EventLogText"));
                }
            }

            SetProp(so, "_startRaceButton", FindChild<Button>(panel, "StartRaceButton"));
            SetProp(so, "_viewResultsButton", FindChild<Button>(panel, "ViewResultsButton"));
            SetProp(so, "_backButton", FindChild<Button>(panel, "BackButton"));

            Transform progressPanel = panel.Find("RaceProgressPanel");
            if (progressPanel != null)
            {
                so.FindProperty("_raceProgressPanel").objectReferenceValue =
                    progressPanel.GetComponent<RectTransform>();
                SetProp(so, "_raceTimerText", FindChild<Text>(progressPanel, "RaceTimerText"));
            }

            so.ApplyModifiedProperties();
        }

        private static void WireRaceResultScreen(Transform canvas)
        {
            Transform panel = canvas.Find("RaceResultScreen");
            if (panel == null)
            {
                return;
            }

            UI.RaceResultScreen comp = panel.GetComponent<UI.RaceResultScreen>();
            if (comp == null)
            {
                return;
            }

            SerializedObject so = new SerializedObject(comp);
            SetProp(so, "_headerText", FindChild<Text>(panel, "HeaderText"));
            SetProp(so, "_resultsText", FindChild<Text>(panel, "ResultsText"));
            SetProp(so, "_prizesText", FindChild<Text>(panel, "PrizesText"));
            SetProp(so, "_backButton", FindChild<Button>(panel, "BackButton"));
            so.ApplyModifiedProperties();
        }

        private static void WireMarketScreen(Transform canvas)
        {
            Transform panel = canvas.Find("MarketScreen");
            if (panel == null)
            {
                return;
            }

            UI.MarketScreen comp = panel.GetComponent<UI.MarketScreen>();
            if (comp == null)
            {
                return;
            }

            SerializedObject so = new SerializedObject(comp);
            SetProp(so, "_headerText", FindChild<Text>(panel, "HeaderText"));
            SetProp(so, "_pigeonInfoText", FindChild<Text>(panel, "PigeonInfoText"));
            SetProp(so, "_priceText", FindChild<Text>(panel, "PriceText"));
            SetProp(so, "_feedbackText", FindChild<Text>(panel, "FeedbackText"));
            SetProp(so, "_rerollButton", FindChild<Button>(panel, "RerollButton"));
            SetProp(so, "_buyButton", FindChild<Button>(panel, "BuyButton"));
            SetProp(so, "_backButton", FindChild<Button>(panel, "BackButton"));
            so.ApplyModifiedProperties();
        }

        private static void WireScreenManager(Transform canvas)
        {
            // Wire prefab internal references first
            WirePigeonCardPrefab(canvas);

            GameObject gmGo = GameObject.Find("GameManager");
            if (gmGo == null)
            {
                return;
            }

            Core.ScreenManager sm = gmGo.GetComponent<Core.ScreenManager>();
            if (sm == null)
            {
                return;
            }

            SerializedObject so = new SerializedObject(sm);
            SerializedProperty mappings = so.FindProperty("_screenMappings");
            mappings.ClearArray();

            AddScreenMapping(mappings, (int)Core.ScreenType.MainMenu, canvas.Find("MainMenuScreen"));
            AddScreenMapping(mappings, (int)Core.ScreenType.Loft, canvas.Find("LoftScreen"));
            AddScreenMapping(mappings, (int)Core.ScreenType.PigeonDetail, canvas.Find("PigeonDetailScreen"));
            AddScreenMapping(mappings, (int)Core.ScreenType.Training, null);
            AddScreenMapping(mappings, (int)Core.ScreenType.Race, canvas.Find("RaceScreen"));
            AddScreenMapping(mappings, (int)Core.ScreenType.Calendar, canvas.Find("CalendarScreen"));
            AddScreenMapping(mappings, (int)Core.ScreenType.Market, canvas.Find("MarketScreen"));
            AddScreenMapping(mappings, (int)Core.ScreenType.RaceResult, canvas.Find("RaceResultScreen"));

            so.ApplyModifiedProperties();
        }

        private static void AddScreenMapping(SerializedProperty array, int typeIndex, Transform screen)
        {
            array.InsertArrayElementAtIndex(array.arraySize);
            SerializedProperty element = array.GetArrayElementAtIndex(array.arraySize - 1);
            element.FindPropertyRelative("Type").enumValueIndex = typeIndex;
            element.FindPropertyRelative("ScreenObject").objectReferenceValue = screen != null ? screen.gameObject : null;
        }

        private static void WirePigeonCardPrefab(Transform canvas)
        {
            Transform cardPrefab = canvas.Find("PigeonCardPrefab");
            if (cardPrefab == null)
            {
                return;
            }

            UI.PigeonCard comp = cardPrefab.GetComponent<UI.PigeonCard>();
            if (comp == null)
            {
                return;
            }

            SerializedObject so = new SerializedObject(comp);
            SetProp(so, "_nameText", FindChild<Text>(cardPrefab, "NameText"));
            SetProp(so, "_statsText", FindChild<Text>(cardPrefab, "StatsText"));
            SetProp(so, "_ageText", FindChild<Text>(cardPrefab, "AgeText"));
            SetProp(so, "_statusText", FindChild<Text>(cardPrefab, "StatusText"));

            // The card itself has a Button component used for selection
            Button selectButton = cardPrefab.GetComponent<Button>();
            if (selectButton != null)
            {
                so.FindProperty("_selectButton").objectReferenceValue = selectButton;
            }

            so.ApplyModifiedProperties();
        }

        private static T FindChild<T>(Transform parent, string childName)
            where T : Component
        {
            Transform child = parent.Find(childName);
            return child != null ? child.GetComponent<T>() : null;
        }

        private static void SetProp(SerializedObject so, string propName, UnityEngine.Object value)
        {
            SerializedProperty prop = so.FindProperty(propName);
            if (prop != null && value != null)
            {
                prop.objectReferenceValue = value;
            }
        }
    }
}
