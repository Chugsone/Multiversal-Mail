using UnityEngine;

public class WireManager : MonoBehaviour
{
    public int totalWires = 4;
    private int connectedWires = 0;

    public void CheckCompletion()
    {
        connectedWires++;
        if (connectedWires >= totalWires)
        {
            gameObject.SetActive(false);
            Debug.Log("Wire Task Complete!");
        }
    }
}
