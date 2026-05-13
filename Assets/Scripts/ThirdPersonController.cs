using Sirenix.OdinInspector;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ThirdPersonController : MonoBehaviour
{
    [FoldoutGroup("References")]
    public InputSystem_Actions playerInputsMap;
    [FoldoutGroup("References")]
    private CharacterController charMovement;
    [FoldoutGroup("References")]
    public CinemachineCamera camNormal;
    [FoldoutGroup("References")]
    public CinemachineCamera camAiming;
    [FoldoutGroup("References")]
    public LineRenderer visualLaserShot;
    [FoldoutGroup("References")]
    public Transform gunMuzzlePoint;

    [FoldoutGroup("VFX System")]
    public ParticleSystem vfxMuzzleFlash;
    [FoldoutGroup("VFX System")]
    public ParticleSystem vfxFootsteps;
    [FoldoutGroup("VFX System")]
    public GameObject vfxImpactPrefab;

    [FoldoutGroup("Turret System")]
    public GameObject deployableTurretPrefab;
    [FoldoutGroup("Turret System")]
    public Transform turretDropPosition;

    [FoldoutGroup("Controller Stats")]
    public float speedMove = 5f;
    [FoldoutGroup("Controller Stats")]
    public float speedRotate = 200f;
    [FoldoutGroup("Controller Stats")]
    public float velVertical = 0;
    [FoldoutGroup("Controller Stats")]
    public float powerJump = 10;
    [FoldoutGroup("Controller Stats")]
    public float powerPush = 4;

    [FoldoutGroup("Dash Settings")]
    private bool stateDashing;
    [FoldoutGroup("Dash Settings")]
    public float forceDash;
    [FoldoutGroup("Dash Settings")]
    public float timeDash = 0.2f;
    [FoldoutGroup("Dash Settings")]
    private float timerDash;

    [FoldoutGroup("Animator Settings")]
    [SerializeField] private CinemachineImpulseSource impulseCamSource;
    [FoldoutGroup("Animator Settings")]
    public UnityEvent OnShootAction;

    private Vector2 axisInputMove;

    private void Awake()
    {
        playerInputsMap = new InputSystem_Actions();
        charMovement = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        playerInputsMap.Enable();

        playerInputsMap.Player.Move.performed += ctx => axisInputMove = ctx.ReadValue<Vector2>();
        playerInputsMap.Player.Move.canceled += ctx => axisInputMove = Vector2.zero;
        playerInputsMap.Player.Jump.performed += ExecJump;
        playerInputsMap.Player.Sprint.performed += ExecDash;

        playerInputsMap.Player.Attack.performed += ExecFireWeapon;

        playerInputsMap.Player.Interact.performed += ExecDeployTurret;
    }

    private void OnDisable()
    {
        playerInputsMap.Disable();
    }

    private void Update()
    {
        HandlePlayerMovement();
    }

    private void HandlePlayerMovement()
    {
        Vector3 directionMove = transform.forward * axisInputMove.y + transform.right * axisInputMove.x;
        directionMove *= speedMove;

        if (charMovement.isGrounded && velVertical < 0)
        {
            velVertical = -2f;
        }

        velVertical += Physics.gravity.y * Time.deltaTime;
        directionMove.y = velVertical;

        if (stateDashing)
        {
            directionMove = transform.forward * forceDash * (timerDash / timeDash);
            timerDash -= Time.deltaTime;

            if (timerDash <= 0)
            {
                stateDashing = false;
            }
        }

        charMovement.Move(directionMove * Time.deltaTime);

        if (charMovement.isGrounded && axisInputMove.magnitude > 0.1f && !stateDashing)
        {
            if (vfxFootsteps != null && !vfxFootsteps.isPlaying)
                vfxFootsteps.Play();
        }
        else
        {
            if (vfxFootsteps != null && vfxFootsteps.isPlaying)
                vfxFootsteps.Stop();
        }
    }

    private void ExecJump(InputAction.CallbackContext context)
    {
        if (!charMovement.isGrounded) return;
        velVertical = powerJump;
    }

    private void ExecDash(InputAction.CallbackContext context)
    {
        stateDashing = true;
        timerDash = timeDash;
    }

    private void ExecFireWeapon(InputAction.CallbackContext context)
    {
        OnShootAction?.Invoke();
        if (impulseCamSource != null) impulseCamSource.GenerateImpulse();

        if (vfxMuzzleFlash != null)
            vfxMuzzleFlash.Play();

        if (Physics.Raycast(camAiming.transform.position, camAiming.transform.forward, out RaycastHit hitData, 100f))
        {
            StartCoroutine(DisplayShotTrajectory(gunMuzzlePoint.position, hitData.point));

            if (vfxImpactPrefab != null)
            {
                GameObject instImpact = Instantiate(vfxImpactPrefab, hitData.point, Quaternion.LookRotation(hitData.normal));
                Destroy(instImpact, 2f);
            }
        }
        else
        {
            Vector3 maxDistPoint = camAiming.transform.position + camAiming.transform.forward * 100f;
            StartCoroutine(DisplayShotTrajectory(gunMuzzlePoint.position, maxDistPoint));
        }
    }

    private IEnumerator DisplayShotTrajectory(Vector3 originPoint, Vector3 destPoint)
    {
        if (visualLaserShot != null)
        {
            visualLaserShot.enabled = true;
            visualLaserShot.SetPosition(0, originPoint);
            visualLaserShot.SetPosition(1, destPoint);

            yield return new WaitForSeconds(0.05f);

            visualLaserShot.enabled = false;
        }
    }

    private void ExecDeployTurret(InputAction.CallbackContext context)
    {
        if (deployableTurretPrefab != null && turretDropPosition != null)
        {
            Instantiate(deployableTurretPrefab, turretDropPosition.position, turretDropPosition.rotation);
        }
    }
}