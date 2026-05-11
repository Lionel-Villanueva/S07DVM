using UnityEngine;
using UnityEngine.InputSystem;

public class TurretBuilder : MonoBehaviour
{
    [Header("Referencias de Input")]
    public InputSystem_Actions inputs;

    [Header("Configuración de la Torreta")]
    public GameObject turretPrefab;
    public LayerMask groundLayer;
    public float buildRange = 10f;

    private Camera mainCamera;

    private void Awake()
    {
        if (inputs == null)
            inputs = new InputSystem_Actions();

        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("No se encontró una Main Camera en la escena. Asegúrate de que tu cámara tenga el Tag 'MainCamera'.");
        }
    }

    private void OnEnable()
    {
        inputs.Enable();
        inputs.Player.PlaceTurret.performed += OnPlaceTurretInput;
    }

    private void OnDisable()
    {
        inputs.Player.PlaceTurret.performed -= OnPlaceTurretInput;
        inputs.Disable();
    }

    private void OnPlaceTurretInput(InputAction.CallbackContext context)
    {
        if (mainCamera != null)
        {
            TryBuildTurret();
        }
    }

    private void TryBuildTurret()
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, buildRange, groundLayer))
        {
            Instantiate(turretPrefab, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
            Debug.Log("Torreta colocada con la cámara de Cinemachine.");
        }
    }
}