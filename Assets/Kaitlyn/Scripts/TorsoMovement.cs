using System.Collections.Generic;
using UnityEngine;

public class TorsoMovement : MonoBehaviour
{

    public CamTest camController;
    private InputSystem_Actions _controls;

    private void Awake()
    {
        _controls = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        
    }

    void Start()
    {
        _controls.Enable();

        //_controls.Player.CamSwitch.performed += ctx =>
        {
            Debug.Log("yo");
            camController.CamSwitch();
        };
    }

    
    void Update()
    {
        
    }
}
