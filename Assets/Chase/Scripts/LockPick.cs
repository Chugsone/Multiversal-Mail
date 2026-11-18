using UnityEngine;
using UnityEngine.UI;

public class LockPick : MonoBehaviour
{
    private enum PuzzleState { Idle, Searching, Turning, Grinding, Cracked }
    private PuzzleState _state = PuzzleState.Idle;

    public GameObject lockpickCanvas;
    public RectTransform lockPick;
    public RectTransform Group;
    [Tooltip("Leave EMPTY if Canvas is Screen Space - Overlay!")]
    public Camera puzzleCamera;

    [Range(2f, 45f)] public float sweetSpotDegrees = 15f; // Widened slightly for easier testing
    [Range(40f, 180f)] public float rotationSpeed = 90f;
    public float closeCanvasAngle = 45f;

    [Range(0f, 15f)] public float wrongSpotGive = 4f;

    private float _targetAngle;
    private float _pickAngle;
    private float _cylinderAngle;

    void Start()
    {
        BeginLockpicking();
    }

    public void BeginLockpicking()
    {
        if (lockpickCanvas != null)
        {
            lockpickCanvas.SetActive(true);
        }

        _targetAngle = Random.Range(-90f, 90f);
        Debug.Log($"[LockPick] Secret Target Angle is: {_targetAngle:F1}°"); // Check Console to test!

        _pickAngle = 0f;
        _cylinderAngle = 0f;
        lockPick.localRotation = Quaternion.identity;
        Group.localRotation = Quaternion.identity;
        _state = PuzzleState.Searching;
    }

    void Update()
    {
        switch (_state)
        {
            case PuzzleState.Searching:
                TrackMouseToCanvasAngle();
                if (IsTurnHeld())
                {
                    _state = PuzzleState.Turning;
                }
                break;

            case PuzzleState.Turning:
                if (IsTurnHeld())
                {
                    RotateLockTumbler();
                }
                else
                {
                    ReturnCylinderToZero();
                }
                break;

            case PuzzleState.Grinding:
                HandlePickDamage();
                if (!IsTurnHeld())
                {
                    ReturnCylinderToZero();
                }
                break;

            case PuzzleState.Cracked:
                break;
        }
    }

    private bool IsTurnHeld()
    {
        return Input.GetAxisRaw("Horizontal") > 0 || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
    }

    private void TrackMouseToCanvasAngle()
    {
        RectTransform parentRect = lockPick.parent as RectTransform;
        if (parentRect == null) return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, Input.mousePosition, puzzleCamera, out Vector2 localPoint))
        {
            float radialAngle = Mathf.Atan2(localPoint.y, localPoint.x) * Mathf.Rad2Deg;
            _pickAngle = radialAngle - 90f;

            if (_pickAngle < -180f) _pickAngle += 360f;

            _pickAngle = Mathf.Clamp(_pickAngle, -90f, 90f);
            lockPick.localRotation = Quaternion.Euler(0f, 0f, _pickAngle);
        }
    }

    private void RotateLockTumbler()
    {
        float distanceToTarget = Mathf.Abs(_pickAngle - _targetAngle);
        float limitRotationValue = wrongSpotGive;

        if (distanceToTarget <= sweetSpotDegrees)
        {
            float proximityScale = distanceToTarget / sweetSpotDegrees;
            limitRotationValue = Mathf.Lerp(90f, wrongSpotGive, proximityScale);
        }

        _cylinderAngle = Mathf.MoveTowards(_cylinderAngle, 90f, rotationSpeed * Time.deltaTime);

        if (_cylinderAngle >= closeCanvasAngle)
        {
            CloseLockpickCanvas();
            return;
        }

        if (_cylinderAngle >= limitRotationValue)
        {
            _cylinderAngle = limitRotationValue;
            if (_cylinderAngle < 89f)
            {
                _state = PuzzleState.Grinding;
            }
        }

        if (_cylinderAngle >= 89f)
        {
            _state = PuzzleState.Cracked;
            OnLockOpenedSuccess();
            return;
        }

        Group.localRotation = Quaternion.Euler(0f, 0f, _cylinderAngle);
    }

    private void ReturnCylinderToZero()
    {
        _cylinderAngle = Mathf.MoveTowards(_cylinderAngle, 0f, rotationSpeed * 2f * Time.deltaTime);
        Group.localRotation = Quaternion.Euler(0f, 0f, _cylinderAngle);

        if (_cylinderAngle <= 0.05f)
        {
            _cylinderAngle = 0f;
            _state = PuzzleState.Searching;
        }
    }

    private void HandlePickDamage()
    {
        float canvasShakeIntensity = Random.Range(-3f, 3f);
        lockPick.localRotation = Quaternion.Euler(0f, 0f, _pickAngle + canvasShakeIntensity);
    }

    private void CloseLockpickCanvas()
    {
        _state = PuzzleState.Idle;
        if (lockpickCanvas != null)
        {
            lockpickCanvas.SetActive(false);
        }
    }

    private void OnLockOpenedSuccess()
    {
        Debug.Log("oh shiii he robbing");
        this.enabled = false;
        CloseLockpickCanvas();
    }
}
