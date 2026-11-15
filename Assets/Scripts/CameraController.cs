using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;     // drag the Player here

    [Header("Orbit & Sensitivity")]
    [SerializeField] private float mouseSensitivity = 3f;
    [SerializeField] private float fixedPitch = 20f; // Fixed vertical height angle (change this to tilt higher or lower)
    [SerializeField] private float followSharpness = 15f;

    [Header("Zoom")]
    [SerializeField] private float distance = 10f;
    [SerializeField] private float minZoom = 6f;
    [SerializeField] private float maxZoom = 18f;
    [SerializeField] private float scrollSensitivity = 5f;

    private float yaw = 0f;

    void Start()
    {
        // Initialize yaw rotation from current camera rotation
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 1. Read ONLY horizontal mouse input (Mouse X) to orbit left/right
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;

        // 2. Handle Zoom via Mouse ScrollWheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            distance -= scroll * scrollSensitivity;
            distance = Mathf.Clamp(distance, minZoom, maxZoom);
        }

        // 3. Calculate target rotation using the fixed pitch (disabling vertical look)
        Quaternion rotation = Quaternion.Euler(fixedPitch, yaw, 0f);
        Vector3 targetPosition = target.position + new Vector3(0f, 1.5f, 0f); // Look slightly above feet
        Vector3 desiredPosition = targetPosition - (rotation * Vector3.forward * distance);

        // 4. Smoothly move and rotate the camera
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSharpness * Time.deltaTime);
        transform.rotation = rotation;
    }
}