using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Wire : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler
{
    public string wireColor; // e.g., "Red", "Blue", "Yellow", "Pink"
    public Color visualColor = Color.red;
    public RectTransform wireLine; // Drag child WireLine# here
    public WireManager wireManager;

    private Canvas parentCanvas;
    private RectTransform canvasRect;
    private bool isConnected = false;

    void Start()
    {
        parentCanvas = GetComponentInParent<Canvas>();
        canvasRect = parentCanvas.GetComponent<RectTransform>();

        GetComponent<Image>().color = visualColor;
        if (wireLine != null) wireLine.GetComponent<Image>().color = visualColor;

        ResetWire();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isConnected) return;
        UpdateWire(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isConnected) return;
        UpdateWire(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isConnected) return;
        ResetWire();
    }

    public void ConnectToPort(RectTransform portTransform)
    {
        isConnected = true;

        // Snap wire end directly to the center of the target EndPort#
        Vector3 worldCenter = portTransform.position;
        Vector2 localStart = wireLine.parent.InverseTransformPoint(wireLine.position);
        Vector2 localEnd = wireLine.parent.InverseTransformPoint(worldCenter);

        StretchLine(localEnd - localStart);
        wireManager.CheckCompletion();
    }

    private void UpdateWire(Vector2 screenPosition)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPosition,
            parentCanvas.worldCamera,
            out Vector2 localMousePos
        );

        Vector2 startPos = canvasRect.InverseTransformPoint(wireLine.position);
        StretchLine(localMousePos - startPos);
    }

    private void StretchLine(Vector2 direction)
    {
        wireLine.sizeDelta = new Vector2(direction.magnitude, wireLine.sizeDelta.y);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        wireLine.localRotation = Quaternion.Euler(0, 0, angle);
    }

    private void ResetWire()
    {
        wireLine.sizeDelta = new Vector2(0, wireLine.sizeDelta.y);
        wireLine.localRotation = Quaternion.identity;
    }
}
