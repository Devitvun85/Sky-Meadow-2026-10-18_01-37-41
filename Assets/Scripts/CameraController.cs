using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;     // drag the Player here

    [Header("Orbit & Sensitivity")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float fixedPitch = 25f;
    [SerializeField] private float followSharpness = 80f;
    [SerializeField, Range(10f, 65f)] private float minPitch = 15f;
    [SerializeField, Range(10f, 80f)] private float maxPitch = 65f;

    [Header("Zoom")]
    [SerializeField] private float distance = 7f;
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 50f;
    [SerializeField] private float scrollSensitivity = 5f;

    private float yaw = 0f;
    private float pitch;

    void Start()
    {
        // Initialize yaw rotation from current camera rotation
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = fixedPitch;
    }

    void LateUpdate()
    {
        if (target == null) return;

        if (Time.timeScale > 0f)
        {
            yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0f)
            {
                distance -= scroll * scrollSensitivity;
                distance = Mathf.Clamp(distance, minZoom, maxZoom);
            }
        }

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 targetPosition = target.position + new Vector3(0f, 1f, 0f);
        Vector3 desiredPosition = targetPosition - (rotation * Vector3.forward * distance);

        float followAmount =
            1f - Mathf.Exp(-followSharpness * Time.unscaledDeltaTime);
        transform.position =
            Vector3.Lerp(transform.position, desiredPosition, followAmount);
        transform.rotation = rotation;
    }
}