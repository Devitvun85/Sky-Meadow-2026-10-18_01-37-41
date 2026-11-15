using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas), typeof(CanvasScaler))]
public class CoinCounter : MonoBehaviour
{
    public static CoinCounter Instance { get; private set; }

    [Header("Coin Settings")]
    [SerializeField, Min(1)] private int totalCoins = 5;

    private int collectedCoins;
    private Text scoreText;

    private void Awake()
    {
        // Only use one counter in the scene.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        CreateScoreUI();
        UpdateScoreText();
    }

    private void CreateScoreUI()
    {
        // Display the counter on top of the game view.
        Canvas canvas = GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;

        CanvasScaler scaler = GetComponent<CanvasScaler>();
        scaler.uiScaleMode =
            CanvasScaler.ScaleMode.ScaleWithScreenSize;

        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        // Create the text object.
        GameObject textObject = new GameObject(
            "Coin Score",
            typeof(RectTransform),
            typeof(Text),
            typeof(Outline)
        );

        textObject.transform.SetParent(transform, false);

        // Position it at the top-center of the screen.
        RectTransform rect =
            textObject.GetComponent<RectTransform>();

        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);

        rect.anchoredPosition = new Vector2(0f, -25f);
        rect.sizeDelta = new Vector2(400f, 80f);

        scoreText = textObject.GetComponent<Text>();
        scoreText.font =
            Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        scoreText.fontSize = 48;
        scoreText.fontStyle = FontStyle.Bold;
        scoreText.alignment = TextAnchor.MiddleCenter;
        scoreText.color = Color.white;
        scoreText.raycastTarget = false;

        // Dark outline makes the text easier to read.
        Outline outline = textObject.GetComponent<Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(2f, -2f);
    }

    public void AddCoin()
    {
        collectedCoins++;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        scoreText.text = $"Coins: {collectedCoins}/{totalCoins}";
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}