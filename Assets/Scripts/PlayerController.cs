using FallGuyClone;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float turnSpeed = 360f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -40f;

    [Header("Camera - drag the player's child Camera here (optional)")]
    [SerializeField] private Transform cameraTransform;

    [Header("Coins")]
    [SerializeField] private int totalCoins = 5;

    [Header("Pushing")]
    [SerializeField] private float pushPower = 2f;

    [Header("Failure")]
    [SerializeField] private float fallResetHeight = -10f;

    private CharacterController controller;
    private MovingPlatformRL currentPlatform;

    private Vector3 velocity;
    private int coinsCollected;
    private Text coinScoreText;

    private bool isQuitting;
    private bool hasFailed;

    private void Start()
    {
        controller = GetComponent<CharacterController>();

        // Keep the camera looking straight ahead with the player.
        if (cameraTransform != null)
        {
            cameraTransform.localRotation = Quaternion.identity;
        }

        // Keep the cursor free since mouse is not used to look.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        CreateCoinScoreUI();
        UpdateCoinScoreUI();
    }

    private void Update()
    {
        if (isQuitting || hasFailed)
        {
            return;
        }

        // Show the failure screen when the player falls too far.
        if (transform.position.y < fallResetHeight)
        {
            ShowFailureScreen();
            return;
        }

        // A/D or Left/Right turns the player left and right.
        float horizontal = Input.GetAxisRaw("Horizontal");

        // W/S or Up/Down moves forward and backward.
        float vertical = Input.GetAxisRaw("Vertical");

        // Rotate the player (the child camera rotates with it).
        if (horizontal != 0f)
        {
            transform.Rotate(0f, horizontal * turnSpeed * Time.deltaTime, 0f);
        }

        // Move forward or backward in the direction the player is facing.
        Vector3 moveDirection = transform.forward * vertical;

        // Keep the player grounded.
        if (controller.isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        // Jump.
        if (controller.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        Vector3 finalMove =
            moveDirection * moveSpeed +
            Vector3.up * velocity.y;

        controller.Move(finalMove * Time.deltaTime);

        UpdateMovingPlatformParent();
    }

    public void CollectCoin()
    {
        coinsCollected++;
        UpdateCoinScoreUI();
    }

    private void CreateCoinScoreUI()
    {
        var canvasObject = new GameObject(
            "Coin Score HUD",
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster)
        );

        var canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 50;

        var scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);

        var bgObject = new GameObject(
            "Score Background",
            typeof(RectTransform),
            typeof(Image)
        );
        bgObject.transform.SetParent(canvasObject.transform, false);

        var bgRect = bgObject.GetComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0.5f, 1f);
        bgRect.anchorMax = new Vector2(0.5f, 1f);
        bgRect.pivot = new Vector2(0.5f, 1f);
        bgRect.anchoredPosition = new Vector2(0f, -25f);
        bgRect.sizeDelta = new Vector2(280f, 70f);

        var bgImage = bgObject.GetComponent<Image>();
        bgImage.color = new Color(0f, 0f, 0f, 0.55f);
        bgImage.raycastTarget = false;

        var textObject = new GameObject(
            "Score Text",
            typeof(RectTransform),
            typeof(Text)
        );
        textObject.transform.SetParent(bgObject.transform, false);

        var textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        coinScoreText = textObject.GetComponent<Text>();
        coinScoreText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        coinScoreText.fontSize = 36;
        coinScoreText.fontStyle = FontStyle.Bold;
        coinScoreText.alignment = TextAnchor.MiddleCenter;
        coinScoreText.color = Color.white;
        coinScoreText.raycastTarget = false;
    }

    private void UpdateCoinScoreUI()
    {
        if (coinScoreText != null)
        {
            coinScoreText.text = $"Coins: {coinsCollected}/{totalCoins}";
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        // Only push non-kinematic rigidbodies.
        if (body == null || body.isKinematic)
        {
            return;
        }

        // Do not push objects underneath the player.
        if (hit.moveDirection.y < -0.3f)
        {
            return;
        }

        Vector3 pushDirection = new Vector3(
            hit.moveDirection.x,
            0f,
            hit.moveDirection.z
        );

        Vector3 pushVelocity =
            pushDirection * moveSpeed * (pushPower * 0.5f);

        // Support Unity 6 and earlier Unity versions.
#if UNITY_6000_0_OR_NEWER
        body.linearVelocity = pushVelocity;
#else
        body.velocity = pushVelocity;
#endif
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Orb"))
        {
            Debug.Log("Collected orb: " + other.gameObject.name);
            Destroy(other.gameObject);
        }
    }

    private void UpdateMovingPlatformParent()
    {
        MovingPlatformRL platform = null;

        if (controller.isGrounded &&
            Physics.Raycast(
                controller.bounds.center,
                Vector3.down,
                out RaycastHit hit,
                controller.bounds.extents.y + 0.2f,
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore
            ))
        {
            platform =
                hit.collider.GetComponentInParent<MovingPlatformRL>();
        }

        if (platform == currentPlatform)
        {
            return;
        }

        currentPlatform = platform;

        transform.SetParent(
            platform != null ? platform.transform : null,
            true
        );
    }

    private void ShowFailureScreen()
    {
        if (hasFailed)
        {
            return;
        }

        hasFailed = true;
        controller.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Create the canvas.
        var canvasObject = new GameObject(
            "Failure Screen",
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster)
        );

        var canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        var scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode =
            CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);

        // Create the background panel.
        var panelObject = new GameObject(
            "Failure Panel",
            typeof(RectTransform),
            typeof(Image)
        );

        panelObject.transform.SetParent(
            canvasObject.transform,
            false
        );

        var panelRect = panelObject.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        panelObject.GetComponent<Image>().color =
            new Color(0f, 0f, 0f, 0.7f);

        // Create the failure message.
        var titleObject = new GameObject(
            "Failure Message",
            typeof(RectTransform),
            typeof(Text)
        );

        titleObject.transform.SetParent(
            panelObject.transform,
            false
        );

        var titleRect = titleObject.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.5f);
        titleRect.anchorMax = new Vector2(0.5f, 0.5f);
        titleRect.anchoredPosition = new Vector2(0f, 100f);
        titleRect.sizeDelta = new Vector2(800f, 120f);

        var title = titleObject.GetComponent<Text>();
        title.font =
            Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        title.text = "YOU FAILED";
        title.fontSize = 64;
        title.fontStyle = FontStyle.Bold;
        title.alignment = TextAnchor.MiddleCenter;
        title.color = Color.white;
        title.raycastTarget = false;

        // Create the restart button.
        var buttonObject = new GameObject(
            "Play Again Button",
            typeof(RectTransform),
            typeof(Image),
            typeof(Button)
        );

        buttonObject.transform.SetParent(
            panelObject.transform,
            false
        );

        var buttonRect =
            buttonObject.GetComponent<RectTransform>();

        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.anchoredPosition = new Vector2(0f, -40f);
        buttonRect.sizeDelta = new Vector2(320f, 90f);

        var buttonImage = buttonObject.GetComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.75f, 0.45f);

        var button = buttonObject.GetComponent<Button>();
        button.targetGraphic = buttonImage;

        button.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(
                SceneManager.GetActiveScene().buildIndex
            );
        });

        // Create the button label.
        var buttonLabelObject = new GameObject(
            "Label",
            typeof(RectTransform),
            typeof(Text)
        );

        buttonLabelObject.transform.SetParent(
            buttonObject.transform,
            false
        );

        var labelRect =
            buttonLabelObject.GetComponent<RectTransform>();

        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        var label = buttonLabelObject.GetComponent<Text>();
        label.font = title.font;
        label.text = "PLAY AGAIN";
        label.fontSize = 36;
        label.fontStyle = FontStyle.Bold;
        label.alignment = TextAnchor.MiddleCenter;
        label.color = Color.white;
        label.raycastTarget = false;

        // Create an EventSystem if the scene does not have one.
        if (EventSystem.current == null)
        {
            var eventSystemObject = new GameObject(
                "EventSystem",
                typeof(EventSystem)
            );

            var inputModule =
                eventSystemObject.AddComponent<InputSystemUIInputModule>();

            inputModule.AssignDefaultActions();
        }
    }

    private void OnApplicationQuit()
    {
        isQuitting = true;
    }
}