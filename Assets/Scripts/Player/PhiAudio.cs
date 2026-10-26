using UnityEngine;

[RequireComponent(typeof(PhiController))]
public class PhiAudio : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource jumpHideSource;
    [SerializeField] private AudioSource rollingSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip hideSound;
    [SerializeField] private AudioClip rollingSound;

    [Header("Rolling Settings")]
    [SerializeField] private float minPitch = 0.8f;
    [SerializeField] private float maxPitch = 1.2f;

    private PhiController phiController;
    private Rigidbody2D rb;

    private void Awake()
    {
        phiController = GetComponent<PhiController>();
        rb = GetComponent<Rigidbody2D>();

        // Setup rolling loop
        if (rollingSource != null && rollingSound != null)
        {
            rollingSource.clip = rollingSound;
            rollingSource.loop = true;
        }
    }

    private void OnEnable()
    {
        // Subscribe to the events from PhiController
        if (phiController != null)
        {
            phiController.OnJumped += PlayJumpSound;
            phiController.OnHideToggled += PlayHideSound;
        }
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks or errors
        if (phiController != null)
        {
            phiController.OnJumped -= PlayJumpSound;
            phiController.OnHideToggled -= PlayHideSound;
        }
    }

    private void Update()
    {
        HandleRollingSFX();
    }

    private void HandleRollingSFX()
    {
        if (rollingSource == null || rb == null) return;

        // Check if we are moving horizontally and NOT hiding
        bool isMoving = Mathf.Abs(rb.linearVelocity.x) > 0.5f;
        bool shouldPlayRoll = isMoving && !phiController.IsHiding;

        if (shouldPlayRoll)
        {
            if (!rollingSource.isPlaying)
                rollingSource.Play();

            // Adjust pitch based on speed to make the loop sound natural
            float speedPercent = Mathf.Clamp01(Mathf.Abs(rb.linearVelocity.x) / 6f);
            rollingSource.pitch = Mathf.Lerp(minPitch, maxPitch, speedPercent);
        }
        else
        {
            if (rollingSource.isPlaying)
                rollingSource.Stop();
        }
    }

    // Called via the OnJumped event
    private void PlayJumpSound()
    {
        if (jumpHideSource != null && jumpSound != null)
            jumpHideSource.PlayOneShot(jumpSound);
    }

    // Called via the OnHideToggled event
    private void PlayHideSound()
    {
        if (jumpHideSource != null && hideSound != null)
            jumpHideSource.PlayOneShot(hideSound);
    }
}