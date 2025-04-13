using UnityEngine;

public class FireStarter : MonoBehaviour
{
    [Header("Fire Prefabs")]
    public GameObject bigFirePrefab;
    public GameObject mediumFirePrefab;
    public GameObject smallFirePrefab;

    [Header("Settings")]
    public FireSize fireSize = FireSize.Medium;
    public float startRadius = 3.0f;
    public LayerMask flammableLayer;

    public enum FireSize { Big, Medium, Small }

    void Start()
    {
        StartFire();
    }

    void StartFire()
{
    GameObject firePrefab = ChooseFirePrefab(fireSize);
    if (firePrefab == null)
    {
        Debug.LogError("❌ [FireStarter] No fire prefab assigned!");
        return;
    }

    Debug.Log($"🔥 [FireStarter] Fire started at {transform.position}");

Collider[] hitColliders = Physics.OverlapSphere(
    transform.position, 
    startRadius, 
    -1,  // All layers
    QueryTriggerInteraction.Collide
);



    Debug.Log($"🔥 [FireStarter] Found {hitColliders.Length} potential flammables");

    foreach (var hit in hitColliders)
{
    Debug.Log($"[FireStarter] Found collider on: {hit.gameObject.name} (Layer: {LayerMask.LayerToName(hit.gameObject.layer)})");
    FlammableObject flammable = hit.GetComponent<FlammableObject>();
    if (flammable != null)
    {
        Debug.Log($"🔥 [FireStarter] Igniting {hit.gameObject.name}");
        flammable.Ignite();
    }
}

}


    GameObject ChooseFirePrefab(FireSize size)
    {
        return size switch
        {
            FireSize.Big => bigFirePrefab,
            FireSize.Medium => mediumFirePrefab,
            FireSize.Small => smallFirePrefab,
            _ => mediumFirePrefab,
        };
    }
}