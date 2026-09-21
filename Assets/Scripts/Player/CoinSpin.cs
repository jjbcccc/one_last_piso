using UnityEngine;

public class CoinSpin : MonoBehaviour
{
    [Header("Roll (Z rotation)")]
    [SerializeField] private float rollMultiplier = 200f;
    [SerializeField] private float maxRollSpeed = 720f;

    [Header("Direction")]
    [SerializeField] private bool invertRoll = false;

    private Rigidbody2D rb;
    private float rotationZ;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float signedX = rb != null ? rb.linearVelocity.x : 0f;
        if (invertRoll) signedX = -signedX;

        float rollSpeed = Mathf.Clamp(signedX * rollMultiplier, -maxRollSpeed, maxRollSpeed);
        rotationZ += rollSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);
    }
}