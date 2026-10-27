using UnityEngine;

public class Rotator : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(15f * Time.deltaTime, 30f * Time.deltaTime, 45f * Time.deltaTime);
    }
}
