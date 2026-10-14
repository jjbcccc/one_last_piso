using System;
using UnityEngine;

public class DustPowerUp : MonoBehaviour
{
    [SerializeField] private float duration = 5f;
    [SerializeField] private GameObject dustIcon;

    private float timer;
    private bool active;

    public bool IsActive => active;
    public event Action OnActivated;
    public event Action OnExpired;

    private void Start()
    {
        // Icon starts hidden
        if (dustIcon != null)
            dustIcon.SetActive(false);
    }

    public void Activate()
    {
        // If already active, refresh the timer instead of restarting
        timer = duration;

        if (!active)
        {
            active = true;
            if (dustIcon != null) dustIcon.SetActive(true);
            Debug.Log("[DustPowerUp] Activated");
            OnActivated?.Invoke();
        }
        else
        {
            Debug.Log("[DustPowerUp] Refreshed timer");
        }
    }

    private void Update()
    {
        if (!active) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            active = false;
            if (dustIcon != null) dustIcon.SetActive(false);
            Debug.Log("[DustPowerUp] Expired");
            OnExpired?.Invoke();
        }
    }
}