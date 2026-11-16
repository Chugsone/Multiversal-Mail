using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class CamTest : MonoBehaviour
{
    public Transform playr;
    public Transform firstPersonCam;

    public CinemachineCamera firstPerson;
    public CinemachineCamera sideScroller;
    private CinemachineBrain brain;

    void Awake()
    {
        brain = Camera.main.gameObject.GetComponent<CinemachineBrain>();
    }

    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    public void CamSwitch()
    {
        Debug.Log("gurt");
        if(firstPerson != null && sideScroller != null)
        {
            if(CinemachineCore.IsLive(firstPerson))
            {
                firstPerson.Priority = 0;
                sideScroller.Priority = 10;
            }
            else if (CinemachineCore.IsLive(sideScroller))
            {
                firstPerson.Priority = 10;
                sideScroller.Priority = 0;
            }
        }
    }
}
