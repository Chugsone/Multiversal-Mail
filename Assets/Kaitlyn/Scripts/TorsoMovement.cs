using System.Collections.Generic;
using UnityEngine;

public class TorsoMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [Space(3f)]
    public float walkSpeed = 5f;
    public float slopeLimit;
    private Rigidbody _rb;

    [Space(7f)]
    [Header("Ground Check Settings")]
    [Space(3f)]
    public float groundCheckDistance = 6f;
    public LayerMask groundLayer;

    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private Vector3 _groundCheckSize;
    private bool _isGrounded;
    private bool _isMovingDownSlope;

    [Space(7f)]
    [Header("Grapple Settings")]
    [Space(3f)]
    public LayerMask grappleable;
    public float maxGrappleDistance;
    public float grappleDelay;
    [Space(4f)]
    public float grappleCooldown;
    private float grappleCDTimer;
    
    private bool _grappling;
    private Vector3 _grapplePoint;

    public LineRenderer lr;
   
    [Space(7f)]
    public CamTest _camController;
    private InputSystem_Actions _controls;
    private Vector2 _moveInput;
    private Vector3 _movementVelocity;
    public Vector3 LastDirection { get; private set; }
    public Vector3 LookingDirection { get; private set; }

    Vector3 testr;

    private void Awake()
    {
        _controls = new InputSystem_Actions();
        _rb = GetComponent<Rigidbody>();
        LastDirection = transform.forward;


        _controls.Player.CamSwitch.performed += ctx =>
        {    
            _camController.CamSwitch();
        };

        #region Walking
        float deadzoneAngle = _camController.horizontalDeadzoneAngle;
        _controls.Player.Move.performed += ctx =>
        {
            _moveInput = ctx.ReadValue<Vector2>();
            Quaternion camRotation = Quaternion.Euler(0f, _camController.transform.eulerAngles.y, 0f);
            transform.rotation = camRotation;
            _camController._resetXRotation = 0f;
            _camController.SetDefaultRotation();
            _camController.horizontalDeadzoneAngle = 0f; //so when you look around while walking youll automatically rotate
        };
        _controls.Player.Move.canceled += ctx =>
        {
            _moveInput = Vector2.zero;
            _rb.linearVelocity = Vector2.zero;
            _camController.horizontalDeadzoneAngle = deadzoneAngle; //reset the deadzone angle when you stop moving
        };
        #endregion

        _controls.Player.Grapple.performed += ctx =>
        {    
            StartGrapple();
        };
    }

    void OnEnable()
    {
        _controls.Enable();
    }

    private void OnDisable()
    {
        _controls.Disable();
    }

    void Start()
    {

    }
    
    void Update()
    {
        if(grappleCDTimer > 0)
        {
            grappleCDTimer -= Time.deltaTime;
        }

        GameObject camera = _camController.gameObject;
        _camController._MouseInput = _controls.Player.Look.ReadValue<Vector2>();

        Vector3 forward = camera.transform.forward; // directions based off cam position :man_juggling:
        Vector3 right = camera.transform.right;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = (forward * _moveInput.y) + (right * _moveInput.x);
        _isMovingDownSlope = IsMovingDownSlope(moveDirection);
        Vector3 adjustedMoveDirection = AdjustMovementToSlope(moveDirection);
        adjustedMoveDirection = adjustedMoveDirection.normalized;
        adjustedMoveDirection.y = 0;

        #region direction

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            LastDirection = new Vector3(moveDirection.x, 0f, moveDirection.z);
        }

        LookingDirection = forward.normalized;
        #endregion

        if (_moveInput.magnitude > 0.01f)
        {
            _movementVelocity.x = adjustedMoveDirection.x * (walkSpeed);
            _movementVelocity.z = adjustedMoveDirection.z * (walkSpeed);
        }
        else
        {
            _movementVelocity.x = 0;
            _movementVelocity.z = 0;         
        }

        Vector3 finalMovement = (_movementVelocity * Time.deltaTime);

        testr = finalMovement;

        _rb.linearVelocity = _movementVelocity;
    }

    private void FixedUpdate()
    {
        _isGrounded = Physics.CheckBox(_groundCheckPoint.position, _groundCheckSize / 2, _groundCheckPoint.rotation, groundLayer);
    }

    private void LateUpdate()
    {
        if (_grappling)
        {
            lr.SetPosition(0, transform.position);
        }
    }
    
    private Vector3 AdjustMovementToSlope(Vector3 inputDir)
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, groundCheckDistance, groundLayer))
        {
            float slopeAngle = Vector3.Angle(Vector3.up, hit.normal);
            if (slopeAngle <= slopeLimit)
            {
                return Vector3.ProjectOnPlane(inputDir, hit.normal).normalized;
            }
            else
            {
                return Vector3.zero;
            }
        }

        return inputDir;
    }

    public bool IsMovingDownSlope(Vector3 inputDir)
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, groundCheckDistance, groundLayer))
        {
            Vector3 groundNormal = hit.normal;

            if (Mathf.Approximately(groundNormal.y, 1f))
            {
                return false;
            }

            Vector3 downhillDirection = Vector3.ProjectOnPlane(Vector3.down, groundNormal).normalized;

            Vector3 moveDir = new Vector3(inputDir.x, 0, inputDir.z).normalized;

            float dotProduct = Vector3.Dot(moveDir, downhillDirection);

            return dotProduct > 0.05f;
        }
        return false;
    }

    public void StartGrapple()
    {
        if (grappleCDTimer > 0) return;
        GameObject camera = _camController.gameObject;

        _grappling = true;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, camera.transform.forward, out hit, maxGrappleDistance, grappleable))
        {
            _grapplePoint = hit.point;

            Invoke(nameof(ExecuteGrapple), grappleDelay);
        }
        else
        {
            _grapplePoint = camera.transform.position + camera.transform.forward * maxGrappleDistance;
            Invoke(nameof(StopGrapple), grappleDelay);
        }

        lr.enabled = true;
        lr.SetPosition(1, _grapplePoint);
    }

    public void ExecuteGrapple()
    {

    }

    public void StopGrapple()
    {
        _grappling = false;

        grappleCDTimer = grappleCooldown;

        lr.enabled = false;
    }

    private void OnDrawGizmos()
    {
        Matrix4x4 originalMatrix = Gizmos.matrix;

        Gizmos.matrix = _groundCheckPoint.localToWorldMatrix;

        Gizmos.color = Color.green; //groundcheck
        Gizmos.DrawCube(Vector3.zero, _groundCheckSize);

        Gizmos.matrix = originalMatrix;

        Gizmos.color = Color.red; //groundcheck ray
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);

        Gizmos.color = Color.blue; //direction lines
        Gizmos.DrawLine(transform.position, transform.position + LastDirection * 2f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + LookingDirection * 2);
    }
}
