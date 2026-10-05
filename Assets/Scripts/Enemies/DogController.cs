using UnityEngine;

public class DogController : MonoBehaviour
{
    [Header("Dog Visuals")]
    [SerializeField] private GameObject dogLeftVisual;
    [SerializeField] private GameObject dogRightVisual;

    [Header("Camera")]
    [SerializeField] private Camera playerCamera;

    [Header("Hazard Spawner")]
    [SerializeField] private FallingHazardSpawner hazardSpawner;

    [Header("Corner Position (viewport 0..1)")]
    [Tooltip("How far from the left/right edge the dog sits when fully visible.")]
    [Range(0f, 0.3f)]
    [SerializeField] private float cornerOffsetX = 0.1f;

    [Tooltip("How far from the top edge the dog sits.")]
    [Range(0f, 0.3f)]
    [SerializeField] private float cornerOffsetY = 0.1f;

    [Header("Slide Distance")]
    [Tooltip("How far off-screen the dog starts before sliding in (in viewport units).")]
    [Range(0.01f, 0.2f)]
    [SerializeField] private float slideDistance = 0.08f;

    [Header("Timing")]
    [SerializeField] private float slideInDuration = 0.2f;
    [SerializeField] private float slideOutDuration = 0.15f;
    [SerializeField] private float minAppearTime = 2f;
    [SerializeField] private float maxAppearTime = 5f;
    [SerializeField] private float visibleTime = 1.5f;

    // Internal state
    private float appearTimer;
    private float visibleTimer;
    private bool dogVisible;
    private bool dogOnLeft;
    private float slideTimer;
    private float currentSlideT;   // 0 = off-screen, 1 = fully in place
    private bool slidingIn;

    private void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        dogVisible = false;
        HideBothDogs();
        SetNextAppearTime();
    }

    private void Update()
    {
        if (!dogVisible)
        {
            appearTimer -= Time.deltaTime;
            if (appearTimer <= 0f)
                ShowDog();
            return;
        }

        slideTimer += Time.deltaTime;

        if (slidingIn)
        {
            currentSlideT = slideInDuration > 0f
                ? Mathf.Clamp01(slideTimer / slideInDuration)
                : 1f;

            if (currentSlideT >= 1f)
            {
                slidingIn = false;
                visibleTimer = visibleTime;
            }
        }
        else
        {
            visibleTimer -= Time.deltaTime;

            if (visibleTimer <= 0f)
            {
                // Begin slide-out
                currentSlideT = slideOutDuration > 0f
                    ? 1f - Mathf.Clamp01(slideTimer / slideOutDuration)
                    : 0f;

                if (currentSlideT <= 0f)
                {
                    HideDogImmediate();
                }
            }
        }
    }

    private void LateUpdate()
    {
        if (!dogVisible) return;
        if (playerCamera == null) return;

        UpdateVisibleDogPosition();
    }

    private void ShowDog()
    {
        if (playerCamera == null) return;
        if (dogLeftVisual == null || dogRightVisual == null) return;

        dogOnLeft = Random.value < 0.5f;
        HideBothDogs();

        GameObject activeDog = dogOnLeft ? dogLeftVisual : dogRightVisual;
        activeDog.SetActive(true);

        Debug.Log(dogOnLeft
            ? "[DogController] Left-facing dog appeared."
            : "[DogController] Right-facing dog appeared.");

        slideTimer = 0f;
        currentSlideT = 0f;
        slidingIn = true;
        dogVisible = true;

        UpdateVisibleDogPosition();

        if (hazardSpawner != null)
            hazardSpawner.StartDogAttack();
    }

    private void HideDogImmediate()
    {
        dogVisible = false;
        HideBothDogs();
        SetNextAppearTime();
    }

    private void HideBothDogs()
    {
        if (dogLeftVisual != null) dogLeftVisual.SetActive(false);
        if (dogRightVisual != null) dogRightVisual.SetActive(false);
    }

    private void SetNextAppearTime()
    {
        appearTimer = Random.Range(minAppearTime, maxAppearTime);
    }

    /// <summary>
    /// Positions the dog in the correct corner. currentSlideT controls
    /// how far the dog slides in from just off the screen edge.
    /// </summary>
    private void UpdateVisibleDogPosition()
    {
        // Start (off-screen) X and final (resting) X for each side.
        // Start is only slideDistance beyond the resting spot — a SHORT slide.
        float finalX = dogOnLeft ? cornerOffsetX : 1f - cornerOffsetX;
        float startX = dogOnLeft ? -slideDistance : 1f + slideDistance;

        // Interpolate between start and final based on slide progress
        float viewportX = Mathf.Lerp(startX, finalX, currentSlideT);

        // Y position — always near the top
        float viewportY = 1f - cornerOffsetY;

        Vector3 viewportPosition = new Vector3(viewportX, viewportY, 0f);
        Vector3 worldPosition = playerCamera.ViewportToWorldPoint(viewportPosition);
        worldPosition.z = 0f;

        GameObject activeDog = dogOnLeft ? dogLeftVisual : dogRightVisual;
        if (activeDog != null)
            activeDog.transform.position = worldPosition;
    }
}