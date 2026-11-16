using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WirePort : MonoBehaviour, IDropHandler
{
    public string portColor;

    public Color normalColor;
    public Color connectedColor;
    public Color wrongColor;

    private Image portImage;
    private bool isOccupied;

    private void Awake()
    {
        portImage = GetComponent<Image>();

        if (portImage == null)
        {
            Debug.LogError($"{name} needs an Image component.");
            return;
        }

        portImage.color = normalColor;
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log($"{name}: OnDrop received");

        if (isOccupied)
            return;

        if (eventData.pointerDrag == null)
        {
            Debug.Log("No pointerDrag object.");
            return;
        }

        Wire wire =
            eventData.pointerDrag.GetComponentInParent<Wire>();

        if (wire == null)
        {
            Debug.Log("No Wire component found on dragged object.");
            return;
        }

        if (wire.wireColor.Trim().ToLower() !=
            portColor.Trim().ToLower())
        {
            portImage.color = wrongColor;
            Debug.Log("Wrong wire color.");
            return;
        }

        isOccupied = true;
        portImage.color = connectedColor;

        wire.ConnectToPort(
            GetComponent<RectTransform>()
        );
    }
}