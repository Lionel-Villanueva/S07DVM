using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("Configuración de Ataque")]
    public float detectionRadius = 10f;
    public float fireRate = 1f;
    public LayerMask enemyLayer;

    [Header("Referencias")]
    [Tooltip("Asigna aquí el objeto hijo que debe rotar. Si lo dejas vacío, rotará toda la torreta entera.")]
    public Transform turretHead;

    private float fireTimer;
    private Transform currentTarget;

    void Start()
    {
        fireTimer = 4f / fireRate;
    }

    void Update()
    {
        fireTimer -= Time.deltaTime;

        currentTarget = FindClosestEnemy();

        if (currentTarget != null)
        {
            AimAtTarget(currentTarget);

            if (fireTimer <= 0f)
            {
                Shoot(currentTarget);
                fireTimer = 1f / fireRate;
            }
        }
    }

    private Transform FindClosestEnemy()
    {
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);
        Transform closestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (Collider enemy in enemiesInRange)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < minDistance)
            {
                minDistance = distanceToEnemy;
                closestEnemy = enemy.transform;
            }
        }

        return closestEnemy;
    }

    private void AimAtTarget(Transform target)
    {
        Vector3 directionToTarget = target.position - transform.position;
        directionToTarget.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);

        if (turretHead != null)
        {
            turretHead.rotation = Quaternion.Slerp(turretHead.rotation, lookRotation, Time.deltaTime * 5f);
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    private void Shoot(Transform target)
    {
        Debug.Log("La torreta ha eliminado a: " + target.name);

        Destroy(target.gameObject);

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}