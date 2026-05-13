using UnityEngine;

public class AutoTurret : MonoBehaviour
{
    public float detectionRadius = 15f;
    public float turnSpeed = 5f;
    public LayerMask enemyLayerMask;

    private Transform currentTarget;

    private void Update()
    {
        SearchForTargets();
        AimAtTarget();
    }

    private void SearchForTargets()
    {
        Collider[] entitiesInRange = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayerMask);
        float closestDist = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (Collider entity in entitiesInRange)
        {
            float distToEntity = Vector3.Distance(transform.position, entity.transform.position);
            if (distToEntity < closestDist)
            {
                closestDist = distToEntity;
                closestEnemy = entity.transform;
            }
        }

        currentTarget = closestEnemy;
    }

    private void AimAtTarget()
    {
        if (currentTarget != null)
        {
            Vector3 directionToFace = (currentTarget.position - transform.position).normalized;
            directionToFace.y = 0;

            if (directionToFace != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToFace);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}