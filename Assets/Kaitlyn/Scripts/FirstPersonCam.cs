using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FirstPersonCam : MonoBehaviour
{
    [Header("Target Refs")]
    [Space(3f)]
    public Transform playerBody;
    public TorsoMovement torso;
    public Image crosshair;

    public Transform firstPersonCam;
    [Space(5f)]
    public CinemachineCamera firstPerson;
    public CinemachineCamera sideScroller;
    private CinemachineBrain brain;
    [Space(5f)]


    [Header("Camera Settings")]
    [Space(3f)]
    [Range(1f, 100f)]
    public float verticalMouseSensitivity = 10f;
    [Range(1f, 100f)]
    public float horizontalMouseSensitivity = 10f;

    [Space(3f)]
    [Tooltip("How far to the left or right you can look before the player's body turns.")]
    public float horizontalDeadzoneAngle = 80f;
    [Tooltip("How far down you can look.")]
    public float verticalMinAngle = 56f;
    [Tooltip("How far up you can look.")]
    public float verticalMaxAngle = 90f;

    [Space(4f)]
    public bool isLocked;

    private float _yRotation = 0f; // left and right
    private float _xRotation = 0f; // up and down
    [HideInInspector] public float _resetXRotation = 0f;
    [HideInInspector] public Vector2 _MouseInput;

    void Awake()
    {
        brain = Camera.main.gameObject.GetComponent<CinemachineBrain>();
        torso = FindAnyObjectByType<TorsoMovement>();
    }

    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        if (isLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        _xRotation = playerBody.eulerAngles.y;

        if (crosshair != null)
        {
            if (torso.canMove)
            {
                crosshair.enabled = true;
            }
            else
            {
                crosshair.enabled = false;
            }
        }
    }


    void Update()
    {
        if (isLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (playerBody == null || firstPersonCam == null) return;

        float mouseX = _MouseInput.x * horizontalMouseSensitivity * Time.deltaTime;
        float mouseY = _MouseInput.y * verticalMouseSensitivity * Time.deltaTime;

        //cause 3d sucks ahh x is up and down and y is left and right
        _yRotation -= mouseY;
        _xRotation += mouseX;
        _yRotation = Mathf.Clamp(_yRotation, -verticalMaxAngle, verticalMinAngle);

        float currentAngle = Mathf.Clamp(playerBody.rotation.eulerAngles.y, -horizontalDeadzoneAngle, horizontalDeadzoneAngle);
        if (torso._isClimbing)
        {
            _xRotation = Mathf.Clamp(_xRotation, -horizontalDeadzoneAngle, horizontalDeadzoneAngle);
        }
        else
        {
            if (_xRotation > horizontalDeadzoneAngle)
            {
                float excessRotation = _xRotation - horizontalDeadzoneAngle;
                playerBody.Rotate(Vector3.up, excessRotation);
                _xRotation = horizontalDeadzoneAngle;
            }
            else if (_xRotation < -horizontalDeadzoneAngle)
            {
                float excessRotation = _xRotation + horizontalDeadzoneAngle;
                playerBody.Rotate(Vector3.up, excessRotation);
                _xRotation = -horizontalDeadzoneAngle;
            }
        }      

        firstPersonCam.localRotation = Quaternion.Euler(_yRotation, _xRotation, 0f);
    }

    public void CamSwitch()
    {
        if(firstPerson != null && sideScroller != null)
        {
            if(CinemachineCore.IsLive(firstPerson))
            {
                firstPerson.Priority = 0;
                sideScroller.Priority = 10;

                torso.canMove = false;
                if(crosshair != null)
                {
                    crosshair.enabled = false;
                }
            }
            else if (CinemachineCore.IsLive(sideScroller))
            {
                firstPerson.Priority = 10;
                sideScroller.Priority = 0;

                torso.canMove = true;
                if(crosshair != null)
                {
                    crosshair.enabled = true;
                }
            }
        }
    }

    public void SetDefaultRotation()
    {
        if (playerBody != null)
        {
            _xRotation = _resetXRotation;
        }
    }
}
