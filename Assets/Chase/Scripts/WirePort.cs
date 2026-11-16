using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class WirePort : MonoBehaviour, IDropHandler
{
    public string wireColor; // Must match the corresponding Port# string
    public Color visualColor = Color.red;
    public Image wireLineEnd; // Drag child WireLineEnd# here
    private bool isOccupied = false;

    void Start()
    {
        GetComponent<Image>().color = visualColor;
        if (wireLineEnd != null) wireLineEnd.color = visualColor;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (isOccupied || eventData.pointerDrag == null) return;

        Wire draggedWire = eventData.pointerDrag.GetComponent<Wire>();
        if (draggedWire != null && draggedWire.wireColor == this.wireColor)
        {
            isOccupied = true;
            draggedWire.ConnectToPort(GetComponent<RectTransform>());
        }
    }
}
