using UnityEngine;

public class returnBarrel : MonoBehaviour
{
    [SerializeField] private float returnHeight = -10f;

    private Rigidbody barrelBody;
    private Vector3 startPosition;
    private Quaternion startRotation;

    private void Awake()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        barrelBody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        float currentHeight = barrelBody != null
            ? barrelBody.position.y
            : transform.position.y;

        if (currentHeight > returnHeight)
        {
            return;
        }

        if (barrelBody != null)
        {
            barrelBody.position = startPosition;
            barrelBody.rotation = startRotation;
#if UNITY_6000_0_OR_NEWER
            barrelBody.linearVelocity = Vector3.zero;
#else
            barrelBody.velocity = Vector3.zero;
#endif
            barrelBody.angularVelocity = Vector3.zero;
        }
        else
        {
            transform.SetPositionAndRotation(startPosition, startRotation);
        }
    }
}