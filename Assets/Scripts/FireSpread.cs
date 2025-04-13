using UnityEngine;

public class FireSpread : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Radius in which fire will try to spread.")]
    public float spreadRadius = 5.0f;
    
    [Tooltip("Delay between each spread attempt (in seconds).")]
    public float spreadDelay = 3.0f;
    
    [Tooltip("Maximum number of spread attempts.")]
    public int maxSpreadCount = 5;

    private int currentSpreadCount = 0;
    private bool isSpreading = true;

    void Start()
    {
        Invoke(nameof(SpreadFire), spreadDelay);
    }

    void SpreadFire()
    {
        if (!isSpreading || currentSpreadCount >= maxSpreadCount)
        {
            Debug.Log($"⏹ [FireSpread] Stopping fire spread after {currentSpreadCount} generations.");
            return;
        }

        Debug.Log($"[FireSpread] Spreading fire (Generation {currentSpreadCount})...");

        // Look for nearby colliders
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, spreadRadius, -1, QueryTriggerInteraction.Collide);
        bool spreadSuccessful = false;

        foreach (var hit in hitColliders)
        {
            // Avoid re-igniting self
            if (hit.transform == transform)
                continue;

            FlammableObject flammable = hit.GetComponent<FlammableObject>();
            if (flammable != null)
            {
                Debug.Log($"🔥 [FireSpread] Igniting {hit.gameObject.name}");
                flammable.Ignite();
                spreadSuccessful = true;
            }
        }

        if (spreadSuccessful)
            currentSpreadCount++;

        Invoke(nameof(SpreadFire), spreadDelay);
    }

    // Optional: Visualize spread radius
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spreadRadius);
    }
}
