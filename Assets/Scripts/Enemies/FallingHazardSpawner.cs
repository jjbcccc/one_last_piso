using System.Collections;
using UnityEngine;

public class FallingHazardSpawner : MonoBehaviour
{
    [Header("Hazard Prefabs")]
    [SerializeField] private GameObject[] hazardPrefabs;

    [Header("Camera")]
    [SerializeField] private Camera playerCamera;

    [Header("Spawn Area")]
    [Range(0f, 1f)]
    [SerializeField] private float minViewportX = 0.05f;

    [Range(0f, 1f)]
    [SerializeField] private float maxViewportX = 0.95f;

    [SerializeField] private float topOffset = 1f;

    [Header("Dog Throw Settings")]
    [SerializeField] private int minimumObjectsPerAttack = 1;
    [SerializeField] private int maximumObjectsPerAttack = 3;

    [SerializeField] private float minimumThrowDelay = 0.15f;
    [SerializeField] private float maximumThrowDelay = 0.5f;

    private void Awake()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
    }

    public void StartDogAttack()
    {
        StartCoroutine(DogAttackRoutine());
    }

    private IEnumerator DogAttackRoutine()
    {
        if (hazardPrefabs == null || hazardPrefabs.Length == 0)
        {
            Debug.LogWarning(
                "[FallingHazardSpawner] No hazard prefabs assigned."
            );

            yield break;
        }

        int objectCount = Random.Range(
            minimumObjectsPerAttack,
            maximumObjectsPerAttack + 1
        );

        Debug.Log(
            "[FallingHazardSpawner] Dog attack: " +
            objectCount +
            " object(s)."
        );

        for (int i = 0; i < objectCount; i++)
        {
            SpawnRandomHazard();

            float delay = Random.Range(
                minimumThrowDelay,
                maximumThrowDelay
            );

            yield return new WaitForSeconds(delay);
        }
    }

    private void SpawnRandomHazard()
    {
        if (playerCamera == null)
        {
            Debug.LogWarning(
                "[FallingHazardSpawner] Player Camera is missing."
            );

            return;
        }

        GameObject selectedPrefab =
            hazardPrefabs[
                Random.Range(
                    0,
                    hazardPrefabs.Length
                )
            ];

        if (selectedPrefab == null)
        {
            Debug.LogWarning(
                "[FallingHazardSpawner] " +
                "One of the hazard slots is empty."
            );

            return;
        }

        float randomX = Random.Range(
            minViewportX,
            maxViewportX
        );

        Vector3 viewportPosition = new Vector3(
            randomX,
            topOffset,
            0f
        );

        Vector3 worldPosition =
            playerCamera.ViewportToWorldPoint(
                viewportPosition
            );

        worldPosition.z = 0f;

        Instantiate(
            selectedPrefab,
            worldPosition,
            Quaternion.identity
        );

        Debug.Log(
            "[FallingHazardSpawner] Spawned: " +
            selectedPrefab.name
        );
    }
}