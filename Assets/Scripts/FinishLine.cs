using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class FinishLine : MonoBehaviour
{
    private bool hasWon = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasWon) return;

        // Check if the object entering the trigger is the player
        if (other.CompareTag("Player") || other.GetComponent<CharacterController>() != null)
        {
            hasWon = true;

            // Optional: Freeze the player controller so they can't move anymore
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.enabled = false;
            }

            // Unlock and show cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Generate the Victory Screen UI dynamically
            ShowVictoryScreen();
        }
    }

    private void ShowVictoryScreen()
    {
        // 1. Create Canvas
        var canvasObject = new GameObject(
            "Victory Screen",
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster)
        );

        var canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        var scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);

        // 2. Create Dark Background Panel
        var panelObject = new GameObject(
            "Victory Panel",
            typeof(RectTransform),
            typeof(Image)
        );

        panelObject.transform.SetParent(canvasObject.transform, false);

        var panelRect = panelObject.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        panelObject.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.75f);

        // 3. Create Victory Message Title ("VICTORY!")
        CreateLabel(
            panelObject.transform,
            "Victory Message",
            "VICTORY!",
            64,
            new Vector2(0f, 100f),
            new Vector2(800f, 120f),
            new Color(1f, 0.85f, 0f) // Gold/Yellow text
        );

        // 4. Create Restart / Next Level Button
        var buttonObject = new GameObject(
            "Restart Button",
            typeof(RectTransform),
            typeof(Image),
            typeof(Button)
        );

        buttonObject.transform.SetParent(panelObject.transform, false);

        var buttonRect = buttonObject.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.anchoredPosition = new Vector2(0f, -40f);
        buttonRect.sizeDelta = new Vector2(320f, 90f);

        var buttonImage = buttonObject.GetComponent<Image>();
        buttonImage.color = new Color(0.15f, 0.6f, 0.9f); // Nice Blue Button

        var button = buttonObject.GetComponent<Button>();
        button.targetGraphic = buttonImage;
        button.onClick.AddListener(RestartLevel);

        // Button Text Label
        CreateLabel(
            buttonObject.transform,
            "Label",
            "PLAY AGAIN",
            36,
            Vector2.zero,
            new Vector2(320f, 90f),
            Color.white
        );

        // 5. Ensure EventSystem exists for UI clicks
        if (EventSystem.current == null)
        {
            var eventSystemObject = new GameObject("EventSystem", typeof(EventSystem));
            var inputModule = eventSystemObject.AddComponent<InputSystemUIInputModule>();
            inputModule.AssignDefaultActions();
        }
    }

    private void CreateLabel(
        Transform parent,
        string objectName,
        string content,
        int fontSize,
        Vector2 position,
        Vector2 size,
        Color textColor)
    {
        var labelObject = new GameObject(
            objectName,
            typeof(RectTransform),
            typeof(Text)
        );

        labelObject.transform.SetParent(parent, false);

        var rect = labelObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        var label = labelObject.GetComponent<Text>();
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        label.text = content;
        label.fontSize = fontSize;
        label.fontStyle = FontStyle.Bold;
        label.alignment = TextAnchor.MiddleCenter;
        label.color = textColor;
        label.raycastTarget = false;
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}