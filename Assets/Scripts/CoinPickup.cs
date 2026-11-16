using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    [Header("Coin Animation")]
    [SerializeField, Min(0f)] private float spinSpeed = 90f;
    [SerializeField, Min(0f)] private float bobHeight = 0.15f;
    [SerializeField, Min(0f)] private float bobSpeed = 2f;

    private bool collected;
    private Vector3 startLocalPosition;
    private float bobPhase;

    private void Awake()
    {
        startLocalPosition = transform.localPosition;
        bobPhase = Random.value * Mathf.PI * 2f;
    }

    private void Update()
    {
        if (collected)
        {
            return;
        }

        transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f, Space.Self);
        transform.localPosition =
            startLocalPosition +
            Vector3.up *
            (Mathf.Sin(Time.time * bobSpeed + bobPhase) * bobHeight);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Prevent this coin from being counted twice.
        if (collected)
        {
            return;
        }

        // Only collect when the object belongs to the Player.
        PlayerController player =
            other.GetComponentInParent<PlayerController>();

        if (player == null)
        {
            return;
        }

        if (CoinCounter.Instance == null)
        {
            Debug.LogError(
                "CoinPickup: Add a CoinCounter object to the scene.",
                this
            );

            return;
        }

        collected = true;

        // Update the counter.
        CoinCounter.Instance.AddCoin();

        // Remove the coin from the scene.
        Destroy(gameObject);
    }
}