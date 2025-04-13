using UnityEngine;
using UnityEngine.AI;

public class NPCMovement : MonoBehaviour
{
    public Transform target;
    private NavMeshAgent agent;
    private float nextUpdateTime;
    public float updateRate = 1f; // раз в 1 секунду

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent не найден!");
            return;
        }
        UpdateDestination();
    }

    void Update()
    {
        // Периодически пересчитываем путь
        if (Time.time >= nextUpdateTime)
        {
            UpdateDestination();
            nextUpdateTime = Time.time + updateRate;
        }

        // Дополнительная логика поворота, если нужно
        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(agent.velocity);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, 5f * Time.deltaTime);
        }
    }

    void UpdateDestination()
    {
        if (target == null) return;
        agent.SetDestination(target.position);

        if (agent.pathStatus == NavMeshPathStatus.PathPartial)
        {
            Debug.LogWarning($"{name}: путь частичный!");
        }
        else if (agent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            Debug.LogError($"{name}: путь не найден (PathInvalid)!");
        }
    }
}
