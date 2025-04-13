using UnityEngine;
using System.Collections;

public class FlammableObject : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Delay between catching fire and the fire effect actually starting.")]
    public float ignitionDelay = 1.0f;

    [Tooltip("The fire prefab to instantiate when ignited.")]
    public GameObject firePrefab;
    
    [Tooltip("Offset from the object's position where the fire appears.")]
    public Vector3 fireOffset = new Vector3(0, 1f, 0);

    [Tooltip("Time in seconds to gradually grow the fire scale after ignition.")]
    public float fireGrowthDuration = 0.01f;
    
    [Tooltip("Target scale for the fire effect.")]
    public Vector3 targetFireScale = new Vector3(2f, 2f, 2f);

    [Header("Extinguishing")]
    [Tooltip("Set to true when the fire is being actively extinguished (e.g. by a fire extinguisher).")]
    public bool isBeingExtinguished = false;

    [Tooltip("Time in seconds to completely extinguish the fire (scale -> 0).")]
    public float extinguishDuration = 1.0f;

    [Header("Debug")]
    [SerializeField] private bool isBurning = false;
    [SerializeField] private bool effectStarted = false; // Tracks whether the fire effect has been spawned
    private GameObject fireInstance;

    /// <summary>
    /// Call this method to ignite the object.
    /// After a delay, the fire effect appears.
    /// </summary>
    public void Ignite()
    {
        if (isBurning)
            return;

        isBurning = true;
        Debug.Log($"🔥 [FlammableObject] {gameObject.name} is catching fire...");

        // Start the ignition delay coroutine.
        StartCoroutine(DelayedIgnite());
    }

    /// <summary>
    /// Waits for ignitionDelay seconds, then instantiates the fire effect and starts its initial growth.
    /// </summary>
    private IEnumerator DelayedIgnite()
    {
        yield return new WaitForSeconds(ignitionDelay);
        Debug.Log($"🔥 [FlammableObject] {gameObject.name} is now on fire!");

        if (firePrefab != null)
        {
            // Instantiate the fire effect as a child of the object.
            fireInstance = Instantiate(firePrefab, transform.position + fireOffset, Quaternion.identity, transform);
            Debug.Log($"🔥🔥 [FlammableObject] Fire spawned at {fireInstance.transform.position}");

            // If the fire does not have a BoxCollider, add one (for interactions such as with a fire extinguisher)
            BoxCollider boxCol = fireInstance.GetComponent<BoxCollider>();
            if (boxCol == null)
            {
                boxCol = fireInstance.AddComponent<BoxCollider>();
                boxCol.isTrigger = true;
                // Optionally, adjust collider size:
                // boxCol.size = new Vector3(1f, 1f, 1f);
            }

            // Start the coroutine to initially grow the fire.
            StartCoroutine(GrowFire(fireInstance, targetFireScale, fireGrowthDuration));
        }
        else
        {
            Debug.LogError($"❌ [FlammableObject] {gameObject.name} - No firePrefab assigned!");
        }

        effectStarted = true;
    }

    /// <summary>
    /// Gradually increases the scale of the fire to the target scale over the specified duration.
    /// </summary>
    private IEnumerator GrowFire(GameObject fire, Vector3 targetScale, float duration)
    {
        Vector3 initialScale = fire.transform.localScale;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            fire.transform.localScale = Vector3.Lerp(initialScale, targetScale, timer / duration);
            yield return null;
        }
        fire.transform.localScale = targetScale;
    }

void Update()
{
    if (!isBurning || !effectStarted || fireInstance == null)
        return;

    // Если огонь тушится, внешний механизм должен уменьшать его масштаб.
    if (isBeingExtinguished)
    {
        fireInstance.transform.localScale = Vector3.Lerp(fireInstance.transform.localScale, Vector3.zero, Time.deltaTime * 3f);

        if (fireInstance.transform.localScale.magnitude <= 0.2f)
        {
            Destroy(fireInstance);
            Debug.Log($"🔥 [FlammableObject] Fire extinguished on {gameObject.name}");
            isBurning = false;
            effectStarted = false;
            isBeingExtinguished = false;
        }
    }
}


    // Debug Gizmo: shows where the fire will be spawned.
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position + fireOffset, 0.2f);
    }
}
