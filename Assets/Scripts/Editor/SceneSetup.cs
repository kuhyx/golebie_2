using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PigeonRacingManager.Editor
{
    public static class SceneSetup
    {
        [MenuItem("Tools/Setup Game Scene")]
        public static void SetupScene()
        {
            // Create EventSystem
            GameObject esGo = new GameObject("EventSystem");
            esGo.AddComponent<EventSystem>();

            // Use Input System module if available, otherwise fallback
            System.Type inputSystemModule = System.Type.GetType(
                "UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
            if (inputSystemModule != null)
            {
                esGo.AddComponent(inputSystemModule);
            }
            else
            {
                esGo.AddComponent<StandaloneInputModule>();
            }

            // Create main Canvas
            GameObject canvasGo = new GameObject("Canvas");
            Canvas canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasGo.AddComponent<GraphicRaycaster>();

            // ===== MAIN MENU SCREEN =====
            GameObject mainMenu = CreatePanel("MainMenuScreen", canvasGo.transform);
            CreateText("TitleText", mainMenu.transform, 0.7f, 0.9f, 0.1f, 0.9f, 48, TextAnchor.MiddleCenter);
            CreateInputField(mainMenu.transform);
            CreateButton("StartButton", mainMenu.transform, 0.35f, 0.45f, 0.35f, 0.65f, "Start Game");
            mainMenu.AddComponent<UI.MainMenuScreen>();

            // ===== LOFT SCREEN =====
            GameObject loftScreen = CreatePanel("LoftScreen", canvasGo.transform);
            CreateText("HeaderText", loftScreen.transform, 0.92f, 1f, 0f, 0.7f, 28, TextAnchor.MiddleLeft);
            CreateText("MoneyText", loftScreen.transform, 0.92f, 1f, 0.7f, 1f, 28, TextAnchor.MiddleRight);
            CreateScrollView("PigeonListScroll", loftScreen.transform, 0.12f, 0.9f);
            CreateButton("CalendarButton", loftScreen.transform, 0f, 0.1f, 0f, 0.33f, "Calendar");
            CreateButton("MarketButton", loftScreen.transform, 0f, 0.1f, 0.34f, 0.66f, "Market");
            CreateButton("AdvanceWeekButton", loftScreen.transform, 0f, 0.1f, 0.67f, 1f, "Next Week");
            loftScreen.AddComponent<UI.LoftScreen>();

            // ===== PIGEON DETAIL SCREEN =====
            GameObject detailScreen = CreatePanel("PigeonDetailScreen", canvasGo.transform);
            CreateText("NameText", detailScreen.transform, 0.88f, 0.98f, 0.05f, 0.95f, 36, TextAnchor.MiddleCenter);
            CreateText("DetailsText", detailScreen.transform, 0.78f, 0.86f, 0.05f, 0.95f, 22, TextAnchor.MiddleCenter);
            CreateText("StatsText", detailScreen.transform, 0.4f, 0.76f, 0.05f, 0.95f, 24, TextAnchor.UpperLeft);
            CreateText("RecordText", detailScreen.transform, 0.32f, 0.4f, 0.05f, 0.95f, 22, TextAnchor.MiddleLeft);
            CreateText("TrainingResultText", detailScreen.transform, 0.22f, 0.3f, 0.05f, 0.95f, 20, TextAnchor.MiddleCenter);
            CreateButton("TrainSpeedButton", detailScreen.transform, 0.12f, 0.2f, 0.02f, 0.24f, "Train Speed");
            CreateButton("TrainEnduranceButton", detailScreen.transform, 0.12f, 0.2f, 0.26f, 0.48f, "Train Endurance");
            CreateButton("TrainNavigationButton", detailScreen.transform, 0.12f, 0.2f, 0.52f, 0.74f, "Train Navigation");
            CreateButton("RestButton", detailScreen.transform, 0.12f, 0.2f, 0.76f, 0.98f, "Rest");
            CreateButton("BackButton", detailScreen.transform, 0.02f, 0.1f, 0.35f, 0.65f, "Back to Loft");
            detailScreen.AddComponent<UI.PigeonDetailScreen>();

            // ===== CALENDAR SCREEN =====
            GameObject calendarScreen = CreatePanel("CalendarScreen", canvasGo.transform);
            CreateText("HeaderText", calendarScreen.transform, 0.92f, 1f, 0f, 1f, 28, TextAnchor.MiddleCenter);
            CreateScrollView("RaceListScroll", calendarScreen.transform, 0.1f, 0.9f);
            CreateButton("BackButton", calendarScreen.transform, 0f, 0.08f, 0.35f, 0.65f, "Back");
            calendarScreen.AddComponent<UI.CalendarScreen>();

            // ===== RACE SCREEN =====
            GameObject raceScreen = CreatePanel("RaceScreen", canvasGo.transform);
            CreateText("RaceNameText", raceScreen.transform, 0.92f, 1f, 0f, 1f, 30, TextAnchor.MiddleCenter);
            CreateRaceProgressPanel(raceScreen.transform);
            GameObject raceScroll = CreateScrollView("EventLogScroll", raceScreen.transform, 0.15f, 0.45f);
            Transform raceContent = raceScroll.GetComponent<ScrollRect>().content.transform;
            CreateEventLogText(raceContent);
            CreateButton("StartRaceButton", raceScreen.transform, 0.02f, 0.12f, 0.05f, 0.45f, "Start Race!");
            CreateButton("ViewResultsButton", raceScreen.transform, 0.02f, 0.12f, 0.55f, 0.95f, "View Results");
            CreateButton("BackButton", raceScreen.transform, 0.02f, 0.12f, 0.35f, 0.65f, "Back");
            raceScreen.AddComponent<UI.RaceScreen>();

            // ===== RACE RESULT SCREEN =====
            GameObject resultScreen = CreatePanel("RaceResultScreen", canvasGo.transform);
            CreateText("HeaderText", resultScreen.transform, 0.88f, 0.98f, 0f, 1f, 32, TextAnchor.MiddleCenter);
            CreateText("ResultsText", resultScreen.transform, 0.25f, 0.85f, 0.05f, 0.95f, 22, TextAnchor.UpperLeft);
            CreateText("PrizesText", resultScreen.transform, 0.1f, 0.22f, 0.05f, 0.95f, 22, TextAnchor.MiddleCenter);
            CreateButton("BackButton", resultScreen.transform, 0.02f, 0.08f, 0.35f, 0.65f, "Back to Loft");
            resultScreen.AddComponent<UI.RaceResultScreen>();

            // ===== MARKET SCREEN =====
            GameObject marketScreen = CreatePanel("MarketScreen", canvasGo.transform);
            CreateText("HeaderText", marketScreen.transform, 0.88f, 0.98f, 0f, 1f, 28, TextAnchor.MiddleCenter);
            CreateText("PigeonInfoText", marketScreen.transform, 0.5f, 0.85f, 0.1f, 0.9f, 24, TextAnchor.MiddleCenter);
            CreateText("PriceText", marketScreen.transform, 0.4f, 0.48f, 0.1f, 0.9f, 28, TextAnchor.MiddleCenter);
            CreateText("FeedbackText", marketScreen.transform, 0.3f, 0.38f, 0.1f, 0.9f, 22, TextAnchor.MiddleCenter);
            CreateButton("RerollButton", marketScreen.transform, 0.15f, 0.25f, 0.1f, 0.45f, "Next Pigeon");
            CreateButton("BuyButton", marketScreen.transform, 0.15f, 0.25f, 0.55f, 0.9f, "Buy");
            CreateButton("BackButton", marketScreen.transform, 0.02f, 0.1f, 0.35f, 0.65f, "Back");
            marketScreen.AddComponent<UI.MarketScreen>();

            // ===== PREFABS =====
            CreatePigeonCardPrefab(canvasGo.transform);
            CreateRaceEntryPrefab(canvasGo.transform);

            // ===== SCREEN MANAGER on GameManager =====
            GameObject gmGo = GameObject.Find("GameManager");
            if (gmGo != null)
            {
                gmGo.AddComponent<Core.ScreenManager>();
            }

            Debug.Log("Scene setup complete!");
            EditorUtility.SetDirty(canvasGo);
        }

        private static GameObject CreatePanel(string name, Transform parent)
        {
            GameObject panel = new GameObject(name, typeof(RectTransform));
            panel.transform.SetParent(parent, false);
            RectTransform rt = panel.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;
            Image img = panel.AddComponent<Image>();
            img.color = new Color(0.12f, 0.12f, 0.15f, 1f);
            return panel;
        }

        private static GameObject CreateText(
            string name,
            Transform parent,
            float ancMinY,
            float ancMaxY,
            float ancMinX,
            float ancMaxX,
            int fontSize,
            TextAnchor anchor)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(ancMinX, ancMinY);
            rt.anchorMax = new Vector2(ancMaxX, ancMaxY);
            rt.sizeDelta = Vector2.zero;
            rt.offsetMin = new Vector2(20, 0);
            rt.offsetMax = new Vector2(-20, 0);
            Text text = go.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = fontSize;
            text.color = Color.white;
            text.alignment = anchor;
            return go;
        }

        private static GameObject CreateButton(
            string name,
            Transform parent,
            float ancMinY,
            float ancMaxY,
            float ancMinX,
            float ancMaxX,
            string label)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(ancMinX, ancMinY);
            rt.anchorMax = new Vector2(ancMaxX, ancMaxY);
            rt.sizeDelta = Vector2.zero;
            rt.offsetMin = new Vector2(10, 5);
            rt.offsetMax = new Vector2(-10, -5);
            Image img = go.AddComponent<Image>();
            img.color = new Color(0.2f, 0.4f, 0.7f, 1f);
            go.AddComponent<Button>();

            GameObject textGo = new GameObject("Text", typeof(RectTransform));
            textGo.transform.SetParent(go.transform, false);
            RectTransform trt = textGo.GetComponent<RectTransform>();
            trt.anchorMin = Vector2.zero;
            trt.anchorMax = Vector2.one;
            trt.sizeDelta = Vector2.zero;
            Text txt = textGo.AddComponent<Text>();
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            txt.fontSize = 24;
            txt.color = Color.white;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.text = label;
            return go;
        }

        private static GameObject CreateScrollView(string name, Transform parent, float ancMinY, float ancMaxY)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, ancMinY);
            rt.anchorMax = new Vector2(1, ancMaxY);
            rt.sizeDelta = Vector2.zero;
            rt.offsetMin = new Vector2(10, 5);
            rt.offsetMax = new Vector2(-10, -5);
            go.AddComponent<Image>().color = new Color(0.08f, 0.08f, 0.1f, 1f);
            go.AddComponent<Mask>().showMaskGraphic = true;
            ScrollRect scrollRect = go.AddComponent<ScrollRect>();
            scrollRect.horizontal = false;

            GameObject viewport = new GameObject("Viewport", typeof(RectTransform));
            viewport.transform.SetParent(go.transform, false);
            RectTransform vrt = viewport.GetComponent<RectTransform>();
            vrt.anchorMin = Vector2.zero;
            vrt.anchorMax = Vector2.one;
            vrt.sizeDelta = Vector2.zero;

            GameObject content = new GameObject("Content", typeof(RectTransform));
            content.transform.SetParent(viewport.transform, false);
            RectTransform crt = content.GetComponent<RectTransform>();
            crt.anchorMin = new Vector2(0, 1);
            crt.anchorMax = new Vector2(1, 1);
            crt.pivot = new Vector2(0.5f, 1);
            crt.sizeDelta = Vector2.zero;
            VerticalLayoutGroup vlg = content.AddComponent<VerticalLayoutGroup>();
            vlg.childControlWidth = true;
            vlg.childControlHeight = true;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;
            vlg.spacing = 5;
            vlg.padding = new RectOffset(10, 10, 10, 10);
            content.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            scrollRect.content = crt;
            scrollRect.viewport = vrt;
            return go;
        }

        private static void CreateInputField(Transform parent)
        {
            GameObject inputGo = new GameObject("PlayerNameInput", typeof(RectTransform));
            inputGo.transform.SetParent(parent, false);
            RectTransform irt = inputGo.GetComponent<RectTransform>();
            irt.anchorMin = new Vector2(0.3f, 0.5f);
            irt.anchorMax = new Vector2(0.7f, 0.58f);
            irt.sizeDelta = Vector2.zero;
            inputGo.AddComponent<Image>().color = new Color(0.2f, 0.2f, 0.25f, 1f);
            InputField inputField = inputGo.AddComponent<InputField>();

            GameObject inputTextGo = new GameObject("Text", typeof(RectTransform));
            inputTextGo.transform.SetParent(inputGo.transform, false);
            RectTransform itrt = inputTextGo.GetComponent<RectTransform>();
            itrt.anchorMin = Vector2.zero;
            itrt.anchorMax = Vector2.one;
            itrt.sizeDelta = Vector2.zero;
            itrt.offsetMin = new Vector2(10, 0);
            itrt.offsetMax = new Vector2(-10, 0);
            Text inputText = inputTextGo.AddComponent<Text>();
            inputText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            inputText.fontSize = 24;
            inputText.color = Color.white;
            inputText.alignment = TextAnchor.MiddleCenter;
            inputField.textComponent = inputText;

            GameObject placeholderGo = new GameObject("Placeholder", typeof(RectTransform));
            placeholderGo.transform.SetParent(inputGo.transform, false);
            RectTransform prt = placeholderGo.GetComponent<RectTransform>();
            prt.anchorMin = Vector2.zero;
            prt.anchorMax = Vector2.one;
            prt.sizeDelta = Vector2.zero;
            prt.offsetMin = new Vector2(10, 0);
            prt.offsetMax = new Vector2(-10, 0);
            Text phText = placeholderGo.AddComponent<Text>();
            phText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            phText.fontSize = 24;
            phText.color = new Color(0.6f, 0.6f, 0.6f, 0.5f);
            phText.alignment = TextAnchor.MiddleCenter;
            phText.text = "Enter your name...";
            phText.fontStyle = FontStyle.Italic;
            inputField.placeholder = phText;
        }

        private static void CreateEventLogText(Transform content)
        {
            GameObject go = new GameObject("EventLogText", typeof(RectTransform));
            go.transform.SetParent(content, false);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(0.5f, 1);
            rt.sizeDelta = new Vector2(0, 0);
            LayoutElement le = go.AddComponent<LayoutElement>();
            le.flexibleWidth = 1;
            ContentSizeFitter csf = go.AddComponent<ContentSizeFitter>();
            csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            Text txt = go.AddComponent<Text>();
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            txt.fontSize = 18;
            txt.color = new Color(0.8f, 1f, 0.8f, 1f);
        }

        private static void CreateRaceProgressPanel(Transform parent)
        {
            GameObject panel = new GameObject("RaceProgressPanel", typeof(RectTransform));
            panel.transform.SetParent(parent, false);
            RectTransform rt = panel.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.02f, 0.48f);
            rt.anchorMax = new Vector2(0.98f, 0.9f);
            rt.sizeDelta = Vector2.zero;
            Image bg = panel.AddComponent<Image>();
            bg.color = new Color(0.08f, 0.08f, 0.1f, 1f);

            // Finish line label
            GameObject finishLabel = new GameObject("FinishLabel", typeof(RectTransform));
            finishLabel.transform.SetParent(panel.transform, false);
            RectTransform flrt = finishLabel.GetComponent<RectTransform>();
            flrt.anchorMin = new Vector2(0.92f, 0.9f);
            flrt.anchorMax = new Vector2(1f, 1f);
            flrt.sizeDelta = Vector2.zero;
            Text finishText = finishLabel.AddComponent<Text>();
            finishText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            finishText.fontSize = 14;
            finishText.color = new Color(1f, 1f, 1f, 0.5f);
            finishText.text = "FINISH";
            finishText.alignment = TextAnchor.MiddleCenter;

            // Finish line visual
            GameObject finishLine = new GameObject("FinishLine", typeof(RectTransform));
            finishLine.transform.SetParent(panel.transform, false);
            RectTransform frt = finishLine.GetComponent<RectTransform>();
            frt.anchorMin = new Vector2(0.92f, 0f);
            frt.anchorMax = new Vector2(0.922f, 0.9f);
            frt.sizeDelta = Vector2.zero;
            Image finishImg = finishLine.AddComponent<Image>();
            finishImg.color = new Color(1f, 1f, 1f, 0.4f);

            // Race timer text
            CreateText("RaceTimerText", panel.transform, 0.9f, 1f, 0f, 0.5f, 16, TextAnchor.MiddleLeft);
        }

        private static void CreatePigeonCardPrefab(Transform canvasTransform)
        {
            GameObject card = new GameObject("PigeonCardPrefab", typeof(RectTransform));
            card.transform.SetParent(canvasTransform, false);
            card.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 120);
            card.AddComponent<Image>().color = new Color(0.18f, 0.18f, 0.22f, 1f);
            card.AddComponent<Button>();
            LayoutElement le = card.AddComponent<LayoutElement>();
            le.preferredHeight = 120;
            le.flexibleWidth = 1;

            AddCardText("NameText", card.transform, 0.7f, 0.95f, 0.02f, 0.98f, 22, new Color(0.9f, 0.8f, 0.3f, 1f), FontStyle.Bold);
            AddCardText("StatsText", card.transform, 0.35f, 0.7f, 0.02f, 0.98f, 18, Color.white, FontStyle.Normal);
            AddCardText("AgeText", card.transform, 0.05f, 0.35f, 0.02f, 0.5f, 16, new Color(0.7f, 0.7f, 0.7f, 1f), FontStyle.Normal);
            AddCardText("StatusText", card.transform, 0.05f, 0.35f, 0.5f, 0.98f, 16, new Color(0.7f, 0.7f, 0.7f, 1f), FontStyle.Normal);

            card.AddComponent<UI.PigeonCard>();
            card.SetActive(false);
        }

        private static void AddCardText(
            string name,
            Transform parent,
            float ancMinY,
            float ancMaxY,
            float ancMinX,
            float ancMaxX,
            int fontSize,
            Color color,
            FontStyle style)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(ancMinX, ancMinY);
            rt.anchorMax = new Vector2(ancMaxX, ancMaxY);
            rt.sizeDelta = Vector2.zero;
            Text txt = go.AddComponent<Text>();
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            txt.fontSize = fontSize;
            txt.color = color;
            txt.fontStyle = style;
        }

        private static void CreateRaceEntryPrefab(Transform canvasTransform)
        {
            GameObject entry = new GameObject("RaceEntryPrefab", typeof(RectTransform));
            entry.transform.SetParent(canvasTransform, false);
            entry.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 50);
            entry.AddComponent<Image>().color = new Color(0.15f, 0.2f, 0.3f, 1f);
            entry.AddComponent<Button>();
            LayoutElement le = entry.AddComponent<LayoutElement>();
            le.preferredHeight = 50;
            le.flexibleWidth = 1;

            GameObject textGo = new GameObject("Text", typeof(RectTransform));
            textGo.transform.SetParent(entry.transform, false);
            RectTransform retrt = textGo.GetComponent<RectTransform>();
            retrt.anchorMin = Vector2.zero;
            retrt.anchorMax = Vector2.one;
            retrt.sizeDelta = Vector2.zero;
            retrt.offsetMin = new Vector2(15, 0);
            retrt.offsetMax = new Vector2(-15, 0);
            Text reTxt = textGo.AddComponent<Text>();
            reTxt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            reTxt.fontSize = 18;
            reTxt.color = Color.white;

            entry.SetActive(false);
        }
    }
}
