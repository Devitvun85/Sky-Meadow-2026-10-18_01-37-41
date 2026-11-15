using FallGuyClone;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float rotationSpeed = 15f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -40f;



    [Header("Camera Zoom (Stretch)")]
    [SerializeField] private float minZoom = 2f;
    [SerializeField] private float maxZoom = 10f;
    [SerializeField] private float scrollSensitivity = 5f;
    private float currentZoomDistance;

    [Header("Lab wiring (drag your objects here)")]
    [SerializeField] private Transform cameraTransform;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isQuitting;

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
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        HandleZoom();

        // 1. Get horizontal movement direction from camera
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = (camForward * v + camRight * h).normalized;

        // 2. Rotate player to face movement direction (matches where the camera looks)
        if (moveDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // 3. Handle ground check and jumping
        if (controller.isGrounded)
        {
            if (velocity.y < 0)
            {
                velocity.y = -2f;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        // 4. Combine horizontal speed and vertical velocity into ONE Move call
        Vector3 finalMove = new Vector3(moveDir.x * moveSpeed, velocity.y, moveDir.z * moveSpeed);
        controller.Move(finalMove * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Orb"))
        {
            Debug.Log("Collected orb: " + other.gameObject.name);
            Destroy(other.gameObject);
        }
    }

    

    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            currentZoomDistance -= scroll * scrollSensitivity;
            currentZoomDistance = Mathf.Clamp(currentZoomDistance, minZoom, maxZoom);
        }
    }
}