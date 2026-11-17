using UnityEngine;
using UnityEngine.UI;

public class LockPick : MonoBehaviour
{
    private enum PuzzleState { Idle, Searching, Turning, Grinding, Cracked }
    private PuzzleState _state = PuzzleState.Idle;

    public RectTransform lockPick;
    public RectTransform Group;
    public Camera puzzleCamera;

    [Range(2f, 20f)] public float sweetSpotDegrees = 8f;
    [Range(40f, 180f)] public float rotationSpeed = 90f;

    [Header("Partial Give Settings")]
    [Tooltip("How many degrees the lock will turn when completely wrong, just to show it's not totally stuck.")]
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
        _targetAngle = Random.Range(-90f, 90f);

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
                DetectTurnInput();
                break;

            case PuzzleState.Turning:
                RotateLockTumbler();
                break;

            case PuzzleState.Grinding:
                HandlePickDamage();
                DetectTurnRelease();
                break;

            case PuzzleState.Cracked:
                break;
        }
    }

    private void TrackMouseToCanvasAngle()
    {
        RectTransform parentRect = lockPick.parent as RectTransform;

        if (parentRect == null) return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            Input.mousePosition,
            puzzleCamera,
            out Vector2 localPoint))
        {
            float radialAngle = Mathf.Atan2(localPoint.y, localPoint.x) * Mathf.Rad2Deg;

            _pickAngle = radialAngle - 90f;
            _pickAngle = Mathf.Clamp(_pickAngle, -90f, 90f);

            lockPick.localRotation = Quaternion.Euler(0f, 0f, _pickAngle);
        }
    }

    private void DetectTurnInput()
    {
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            _state = PuzzleState.Turning;
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

    private void HandlePickDamage()
    {
        float canvasShakeIntensity = Random.Range(-3f, 3f);
        lockPick.localRotation = Quaternion.Euler(0f, 0f, _pickAngle + canvasShakeIntensity);

        DetectTurnRelease();
    }

    private void DetectTurnRelease()
    {
        if (Input.GetAxisRaw("Horizontal") <= 0)
        {
            _cylinderAngle = Mathf.MoveTowards(_cylinderAngle, 0f, rotationSpeed * 2f * Time.deltaTime);
            Group.localRotation = Quaternion.Euler(0f, 0f, _cylinderAngle);

            if (_cylinderAngle <= 0.05f)
            {
                _state = PuzzleState.Searching;
            }
        }
    }

    private void OnLockOpenedSuccess()
    {
        Debug.Log("oh shiii he robbing");
        this.enabled = false;
        transform.root.gameObject.SetActive(false);
    }
}
