using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Wire : MonoBehaviour,
    IPointerDownHandler,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    public string wireColor;
    public Color visualColor = Color.red;
    public RectTransform wireLine;
    public WireManager wireManager;

    private RectTransform lineParent;
    private CanvasGroup canvasGroup;
    private bool isConnected;

    private void Awake()
    {
        if (wireLine == null)
        {
            Debug.LogError($"{name}: WireLine is not assigned.");
            enabled = false;
            return;
        }

        lineParent = wireLine.parent as RectTransform;

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        Image wireImage = GetComponent<Image>();
        if (wireImage != null)
            wireImage.color = visualColor;

        Image lineImage = wireLine.GetComponent<Image>();
        if (lineImage != null)
        {
            lineImage.color = visualColor;
            lineImage.raycastTarget = false;
        }

        ResetWire();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isConnected)
            UpdateWire(eventData.position, eventData.pressEventCamera);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isConnected)
            return;

        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isConnected)
            return;

        UpdateWire(eventData.position, eventData.pressEventCamera);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        if (!isConnected)
            ResetWire();
    }

    private void UpdateWire(Vector2 screenPosition, Camera eventCamera)
    {
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                lineParent,
                screenPosition,
                eventCamera,
                out Vector2 mousePosition))
        {
            return;
        }

        Vector2 startPosition =
            lineParent.InverseTransformPoint(wireLine.position);

        StretchLine(mousePosition - startPosition);
    }

    public void ConnectToPort(RectTransform portTransform)
    {
        if (isConnected)
            return;

        Vector2 startPosition =
            lineParent.InverseTransformPoint(wireLine.position);

        Vector2 portCenter =
            lineParent.InverseTransformPoint(portTransform.position);

        Vector2 direction = portCenter - startPosition;

        StretchLine(direction);

        isConnected = true;

        Debug.Log($"{name} snapped to {portTransform.name}");

        if (wireManager != null)
            wireManager.CheckCompletion();
    }

    private void StretchLine(Vector2 direction)
    {
        wireLine.sizeDelta = new Vector2(
            direction.magnitude,
            wireLine.sizeDelta.y
        );

        wireLine.localRotation = Quaternion.Euler(
            0f,
            0f,
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg
        );
    }

    private void ResetWire()
    {
        wireLine.sizeDelta = new Vector2(
            0f,
            wireLine.sizeDelta.y
        );

        wireLine.localRotation = Quaternion.identity;
    }
}