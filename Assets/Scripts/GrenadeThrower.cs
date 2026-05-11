using UnityEngine;
using UnityEngine.InputSystem;

public class GrenadeThrower : MonoBehaviour
{
    public InputSystem_Actions inputs;
    [Header("Configuración")]
    public GameObject granadePrefab;
    public Transform throwPoint; 
    public float throwForce = 15f; 

    private void Awake()
    {
        if (inputs == null)
            inputs = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        inputs.Enable();
        inputs.Player.ThrowGranade.performed += OnThrowGranade;
    }

    private void OnDisable()
    {
        inputs.Player.ThrowGranade.performed -= OnThrowGranade;
        inputs.Disable();
    }

    private void OnThrowGranade(InputAction.CallbackContext context)
    {
        LanzarGranada();
    }

    private void LanzarGranada()
    {
        if (granadePrefab != null && throwPoint != null)
        {
            GameObject granade = Instantiate(granadePrefab, throwPoint.position, throwPoint.rotation);

            Rigidbody rb = granade.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(throwPoint.forward * throwForce, ForceMode.Impulse);
            }
        }
    }
}