using UnityEngine;
using UnityEngine.AI;

public class BaseEnemy : MonoBehaviour
{
    public float range = 5f;
    public float speed = 3f;

    private Transform player;
    private bool playerInRange = false;
    private NavMeshAgent agent;

    void Start()
    {
        CapsuleCollider col = GetComponent<CapsuleCollider>();
        if (col != null)
        {
            col.radius = range;
            col.isTrigger = true;
        }
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (playerInRange && player != null)
        {
            Vector3 dir = (player.position - transform.position).normalized;
            transform.position += dir * speed * Time.deltaTime;
            agent.SetDestination(player.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
            playerInRange = true;
            print("Player detectado");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            player = null;
            print("Player salió del rango");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);

        if (playerInRange && player != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }
}
