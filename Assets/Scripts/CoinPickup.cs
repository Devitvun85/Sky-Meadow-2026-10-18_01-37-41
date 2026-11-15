using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    private bool collected;

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