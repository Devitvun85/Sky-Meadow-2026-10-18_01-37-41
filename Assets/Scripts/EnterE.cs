namespace EasyDoorSystem
{
    using UnityEngine;

    [RequireComponent(typeof(EasyDoor))]
    public class EnterE : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private float interactionRange = 2.5f;
        [SerializeField] private KeyCode interactKey = KeyCode.E;

        private EasyDoor door;

        private void Awake()
        {
            door = GetComponent<EasyDoor>();
            door.automaticPlayerDetection = false;
            door.DestroyOnPlayerTouch = false;
            FindPlayer();
        }

        private void Update()
        {
            if (!player) FindPlayer();
            if (!player || Vector3.Distance(player.position, door.transform.position) > interactionRange)
                return;

            if (Input.GetKeyDown(interactKey))
                door.ToggleDoor();
        }

        private void FindPlayer()
        {
            var playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject)
            {
                player = playerObject.transform;
                return;
            }

            var controller = FindAnyObjectByType<CharacterController>();
            if (controller) player = controller.transform;
        }
    }
}
