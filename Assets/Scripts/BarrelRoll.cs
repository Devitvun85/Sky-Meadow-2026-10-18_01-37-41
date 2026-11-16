using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BarrelRoll : MonoBehaviour
{
    [Header("Rolling Settings")]
    [SerializeField] private float rollForce = 10f; // Speed of the roll

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Apply torque (rotational force) along the local X or Z axis to make it roll forward
        // Depending on how your barrel model is oriented, change transform.right to transform.forward
        rb.AddTorque(transform.right * rollForce, ForceMode.Acceleration);
    }
}