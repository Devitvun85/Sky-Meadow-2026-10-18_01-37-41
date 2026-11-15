using FallGuyClone;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float rotationSpeed = 15f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -40f;

    [Header("Failure")]
    [SerializeField] private float fallResetHeight = -10f;



    [Header("Mouse Look")]
[SerializeField] private float mouseSensitivity = 2f;
[SerializeField, Range(1f, 89f)] private float maxLookAngle = 85f;

private float cameraPitch;

    [Header("Lab wiring (drag your objects here)")]
    [SerializeField] private Transform cameraTransform;

    private CharacterController controller;
    private MovingPlatformRL currentPlatform;
    private Vector3 velocity;
    private bool isQuitting;
    private bool hasFailed;

    private void OnApplicationQuit()
    {
        isQuitting = true;
    }


    [Header("Pushing")]
    [SerializeField] private float pushPower = 2.0f;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        // Check if the object has a rigidbody and is not kinematic
        if (body == null || body.isKinematic)
        {
            return;
        }

        // Prevent pushing objects that are stacked below the player (e.g., standing on a box)
        if (hit.moveDirection.y < -0.3f)
        {
            return;
        }

        // Calculate push direction horizontally (ignore vertical slope hits)
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0f, hit.moveDirection.z);

        // Apply force to the rigidbody based on movement speed and push power
        body.linearVelocity = pushDir * moveSpeed * (pushPower * 0.5f);
    }
 private void Start()
{
    controller = GetComponent<CharacterController>();

    if (cameraTransform == null)
    {
        Debug.LogError(
            "Assign the player's child Camera to Camera Transform.",
            this
        );

        enabled = false;
        return;
    }

    cameraPitch = 0f;
    cameraTransform.localRotation = Quaternion.identity;

    SetCursorLocked(true);
}

private void Update()
{
    if (isQuitting || hasFailed)
        return;

    // Keep your existing falling/failure behavior.
    if (transform.position.y < fallResetHeight)
    {
        ShowFailureScreen();
        return;
    }

    // Escape releases the cursor.
    // Clicking in the Game view captures it again.
    if (Input.GetKeyDown(KeyCode.Escape))
    {
        SetCursorLocked(false);
    }
    else if (Cursor.lockState != CursorLockMode.Locked &&
             Input.GetMouseButtonDown(0))
    {
        SetCursorLocked(true);
    }

    bool canControl = Cursor.lockState == CursorLockMode.Locked;

    if (canControl)
    {
        HandleMouseLook();
    }

    // Movement comes from the keyboard, not the mouse.
    float horizontal = canControl
        ? Input.GetAxisRaw("Horizontal")
        : 0f;

    float vertical = canControl
        ? Input.GetAxisRaw("Vertical")
        : 0f;

    // Move relative to the player's facing direction.
    // A/D strafe instead of turning the player.
    Vector3 moveDirection =
        transform.right * horizontal +
        transform.forward * vertical;

    // Prevent faster diagonal movement.
    moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

    // Grounding and jumping.
    if (controller.isGrounded && velocity.y < 0f)
    {
        velocity.y = -2f;
    }

    if (canControl &&
        controller.isGrounded &&
        Input.GetKeyDown(KeyCode.Space))
    {
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    // Gravity continues even when the cursor is unlocked.
    velocity.y += gravity * Time.deltaTime;

    Vector3 finalMove =
        moveDirection * moveSpeed +
        Vector3.up * velocity.y;

    controller.Move(finalMove * Time.deltaTime);

    // Keep your moving-platform support.
    UpdateMovingPlatformParent();
}

private void HandleMouseLook()
{
    float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
    float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

    // Left/right: turn the player.
    transform.Rotate(0f, mouseX, 0f, Space.World);

    // Up/down: tilt only the camera.
    cameraPitch -= mouseY;
    cameraPitch = Mathf.Clamp(
        cameraPitch,
        -maxLookAngle,
        maxLookAngle
    );

    cameraTransform.localRotation =
        Quaternion.Euler(cameraPitch, 0f, 0f);
}

private void SetCursorLocked(bool locked)
{
    Cursor.lockState = locked
        ? CursorLockMode.Locked
        : CursorLockMode.None;

    Cursor.visible = !locked;
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
            Physics.Raycast(controller.bounds.center, Vector3.down, out RaycastHit hit,
                controller.bounds.extents.y + 0.2f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            platform = hit.collider.GetComponentInParent<MovingPlatformRL>();
        }

        if (platform == currentPlatform) return;

        currentPlatform = platform;
        transform.SetParent(platform != null ? platform.transform : null, true);
    }

    private void ShowFailureScreen()
    {
        hasFailed = true;
        controller.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        var canvasObject = new GameObject("Failure Screen", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        var scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);

        var panelObject = new GameObject("Failure Panel", typeof(RectTransform), typeof(Image));
        panelObject.transform.SetParent(canvasObject.transform, false);
        var panelRect = panelObject.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        panelObject.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.7f);

        var titleObject = new GameObject("Failure Message", typeof(RectTransform), typeof(Text));
        titleObject.transform.SetParent(panelObject.transform, false);
        var titleRect = titleObject.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.5f);
        titleRect.anchorMax = new Vector2(0.5f, 0.5f);
        titleRect.anchoredPosition = new Vector2(0f, 100f);
        titleRect.sizeDelta = new Vector2(800f, 120f);
        var title = titleObject.GetComponent<Text>();
        title.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        title.text = "YOU FAILED";
        title.fontSize = 64;
        title.fontStyle = FontStyle.Bold;
        title.alignment = TextAnchor.MiddleCenter;
        title.color = Color.white;

        var buttonObject = new GameObject("Play Again Button", typeof(RectTransform), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(panelObject.transform, false);
        var buttonRect = buttonObject.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.anchoredPosition = new Vector2(0f, -40f);
        buttonRect.sizeDelta = new Vector2(320f, 90f);
        buttonObject.GetComponent<Image>().color = new Color(0.2f, 0.75f, 0.45f);
        buttonObject.GetComponent<Button>().onClick.AddListener(() =>
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex));

        var buttonLabelObject = new GameObject("Label", typeof(RectTransform), typeof(Text));
        buttonLabelObject.transform.SetParent(buttonObject.transform, false);
        var labelRect = buttonLabelObject.GetComponent<RectTransform>();
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

        if (EventSystem.current == null)
        {
            var eventSystem = new GameObject("EventSystem", typeof(EventSystem));
            var inputModule = eventSystem.AddComponent<InputSystemUIInputModule>();
            inputModule.AssignDefaultActions();
        }
    }
}