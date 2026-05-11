using UnityEngine;
using UnityEngine.Events;

public class Granade : MonoBehaviour
{
    public float Timer = 3f;
    public float Radius = 5f;
    public LayerMask Mask; 
    public UnityEvent OnExplotion;

    void Start()
    {
        Invoke(nameof(OnExplode), Timer);
    }

    public void OnExplode()
    {
        Collider[] colls = Physics.OverlapSphere(transform.position, Radius, Mask);

        foreach (var coll in colls)
        {
            Destroy(coll.gameObject);
            Debug.Log("Enemigo eliminado por la granada: " + coll.name);
        }

        OnExplotion?.Invoke();

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Radius);
    }
}