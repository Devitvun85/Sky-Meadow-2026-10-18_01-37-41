namespace EasyDoorSystem
{
    using UnityEngine;
    using UnityEngine.Events;

    [RequireComponent(typeof(AudioSource))]
    public class EasyDoor : MonoBehaviour
    {
        public enum MovementType { Rotation, Position, Both }

        [Header("Door Settings")]
        [SerializeField] private MovementType movementType = MovementType.Rotation;
        [SerializeField] private float movementSpeed = 2f;
        [SerializeField] private float rotationSpeed = 2f;
        [Tooltip("Automatically close after specified time (0 = no auto-close)")]
        [SerializeField] private float autoCloseDelay = 0f;
        [SerializeField] public bool automaticPlayerDetection = false;
        [SerializeField] private float detectionRange = 3f;

        [Header("Break on Touch")]
        [SerializeField] private bool destroyOnPlayerTouch;
        [SerializeField] private GameObject breakEffect;
        [SerializeField] private int fragmentColumns = 3;
        [SerializeField] private int fragmentRows = 4;
        [SerializeField] private float fragmentForce = 4f;
        [SerializeField] private float fragmentLifetime = 3f;

        [Header("Transform Targets")]
        [SerializeField] private Vector3 closedRotation;
        [SerializeField] private Vector3 openedRotation;
        [SerializeField] private Vector3 closedPosition;
        [SerializeField] private Vector3 openedPosition;

        [Header("Audio")]
        [SerializeField] private AudioClip doorOpenSound;
        [SerializeField] private AudioClip doorCloseSound;
        [SerializeField][Range(0, 1)] private float audioVolume = 0.8f;

        [Header("Events")]
        public UnityEvent OnDoorOpening;
        public UnityEvent OnDoorClosed;

        public bool IsOpen { get; private set; }
        public bool IsMoving { get; private set; }
        public bool DestroyOnPlayerTouch
        {
            get => destroyOnPlayerTouch;
            set => destroyOnPlayerTouch = value;
        }

        private AudioSource audioSource;
        private Transform playerTransform;
        private Collider[] playerColliders;
        private Collider[] doorColliders;
        private Renderer[] doorRenderers;
        private Coroutine movementCoroutine;
        private bool hasBroken;

        public GameObject Player;
        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.spatialBlend = 1f; // 3D sound by default
            audioSource.playOnAwake = false;
            doorColliders = GetComponentsInChildren<Collider>();
            doorRenderers = GetComponentsInChildren<Renderer>();

            if (automaticPlayerDetection || destroyOnPlayerTouch)
                FindPlayer();
        }

        private void FindPlayer()
        {
            if (!Player)
                Player = GameObject.FindGameObjectWithTag("Player");

            if (!Player)
            {
                var controller = FindAnyObjectByType<CharacterController>();
                if (controller) Player = controller.gameObject;
            }

            if (Player)
            {
                playerTransform = Player.transform;
                playerColliders = Player.GetComponentsInChildren<Collider>();
            }
        }

        private void Update()
        {
            if ((automaticPlayerDetection || destroyOnPlayerTouch) && !playerTransform)
                FindPlayer();

            if (destroyOnPlayerTouch && playerTransform && IsPlayerTouchingDoor())
            {
                BreakDoor();
                return;
            }

            if (!automaticPlayerDetection || !playerTransform) return;

            float distance = Vector3.Distance(transform.position, playerTransform.position);
            if (distance <= detectionRange && !IsOpen)
            {
                OpenDoor();
            }
            else if (distance > detectionRange && IsOpen)
            {
                CloseDoor();
            }
        }

        private bool IsPlayerTouchingDoor()
        {
            if (playerColliders == null) return false;

            foreach (var playerCollider in playerColliders)
            {
                if (!playerCollider || !playerCollider.enabled) continue;

                foreach (var doorCollider in doorColliders)
                {
                    if (doorCollider && doorCollider.enabled &&
                        doorCollider.bounds.Intersects(playerCollider.bounds))
                        return true;
                }

                foreach (var doorRenderer in doorRenderers)
                {
                    if (doorRenderer && doorRenderer.enabled &&
                        doorRenderer.bounds.Intersects(playerCollider.bounds))
                        return true;
                }
            }

            return false;
        }

        private void BreakDoor()
        {
            if (hasBroken) return;
            hasBroken = true;

            if (breakEffect)
                Instantiate(breakEffect, transform.position, transform.rotation);

            SpawnDoorFragments();
            Destroy(gameObject);
        }

        private void SpawnDoorFragments()
        {
            var meshFilter = GetComponentInChildren<MeshFilter>();
            var sourceRenderer = meshFilter
                ? meshFilter.GetComponent<Renderer>()
                : GetComponentInChildren<Renderer>();
            if (!sourceRenderer) return;

            Transform pieceSpace = meshFilter ? meshFilter.transform : sourceRenderer.transform;
            Bounds bounds = meshFilter && meshFilter.sharedMesh
                ? meshFilter.sharedMesh.bounds
                : new Bounds(Vector3.zero, sourceRenderer.bounds.size);
            int columns = Mathf.Max(1, fragmentColumns);
            int rows = Mathf.Max(1, fragmentRows);
            Vector3 pieceSize = new Vector3(
                bounds.size.x / columns,
                bounds.size.y / rows,
                Mathf.Max(bounds.size.z, 0.08f));
            Vector3 scale = pieceSpace.lossyScale;

            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    Vector3 localPosition = new Vector3(
                        bounds.min.x + pieceSize.x * (column + 0.5f),
                        bounds.min.y + pieceSize.y * (row + 0.5f),
                        bounds.center.z);
                    var fragment = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    fragment.name = "Door Fragment";
                    fragment.transform.SetPositionAndRotation(
                        pieceSpace.TransformPoint(localPosition), pieceSpace.rotation);
                    fragment.transform.localScale = new Vector3(
                        pieceSize.x * Mathf.Abs(scale.x),
                        pieceSize.y * Mathf.Abs(scale.y),
                        pieceSize.z * Mathf.Abs(scale.z));
                    fragment.GetComponent<Renderer>().sharedMaterial = sourceRenderer.sharedMaterial;

                    var body = fragment.AddComponent<Rigidbody>();
                    Vector3 direction = fragment.transform.position - transform.position + Vector3.up * 0.5f;
                    if (direction.sqrMagnitude > 0.001f)
                        body.AddForce(direction.normalized * fragmentForce, ForceMode.Impulse);
                    body.AddTorque(Random.insideUnitSphere * fragmentForce, ForceMode.Impulse);
                    Destroy(fragment, fragmentLifetime);
                }
            }
        }

        public void ToggleDoor()
        {
            if (IsOpen) CloseDoor();
            else OpenDoor();
        }

        public void OpenDoor()
        {
            if (IsOpen || IsMoving) return;

            MoveDoor(openedPosition, openedRotation, true);
            PlaySound(doorOpenSound);
            OnDoorOpening.Invoke();
        }

        public void CloseDoor()
        {
            if (!IsOpen || IsMoving) return;

            MoveDoor(closedPosition, closedRotation, false);
            PlaySound(doorCloseSound);
            OnDoorClosed.Invoke();
        }

        private void MoveDoor(Vector3 targetPosition, Vector3 targetRotation, bool opening)
        {
            if (movementCoroutine != null)
                StopCoroutine(movementCoroutine);

            movementCoroutine = StartCoroutine(AnimateDoor(
                movementType != MovementType.Rotation ? targetPosition : transform.localPosition,
                movementType != MovementType.Position ? targetRotation : transform.localEulerAngles,
                opening
            ));
        }

        private System.Collections.IEnumerator AnimateDoor(Vector3 targetPos, Vector3 targetRot, bool opening)
        {
            IsMoving = true;
            Quaternion startRot = transform.localRotation;
            Vector3 startPos = transform.localPosition;
            Quaternion targetQuaternion = Quaternion.Euler(targetRot);

            float progress = 0;
            while (progress < 1)
            {
                progress += Time.deltaTime * Mathf.Max(movementSpeed, rotationSpeed);

                if (movementType != MovementType.Position)
                {
                    transform.localRotation = Quaternion.Slerp(startRot, targetQuaternion, progress * rotationSpeed);
                }

                if (movementType != MovementType.Rotation)
                {
                    transform.localPosition = Vector3.Lerp(startPos, targetPos, progress * movementSpeed);
                }

                yield return null;
            }

            // Ensure final positions are exact
            if (movementType != MovementType.Position)
                transform.localRotation = targetQuaternion;

            if (movementType != MovementType.Rotation)
                transform.localPosition = targetPos;

            IsOpen = opening;
            IsMoving = false;

            if (autoCloseDelay > 0 && IsOpen)
                Invoke(nameof(CloseDoor), autoCloseDelay);
        }

        private void PlaySound(AudioClip clip)
        {
            if (!clip || !audioSource) return;

            audioSource.Stop();
            audioSource.clip = clip;
            audioSource.volume = audioVolume;
            audioSource.Play();
        }

        public void SaveCurrentState(bool saveAsOpen)
        {
            if (saveAsOpen)
            {
                openedRotation = transform.localEulerAngles;
                openedPosition = transform.localPosition;
            }
            else
            {
                closedRotation = transform.localEulerAngles;
                closedPosition = transform.localPosition;
            }
        }

        // Gizmos for detection visualization
        private void OnDrawGizmosSelected()
        {
            if (automaticPlayerDetection)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(transform.position, detectionRange);
            }
        }
    }
}