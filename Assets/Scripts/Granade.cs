using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class Granade : MonoBehaviour
{
    public float Timer;
    public float Radius;
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

        }
        OnExplotion?.Invoke();

        Destroy(gameObject);
    }
}   