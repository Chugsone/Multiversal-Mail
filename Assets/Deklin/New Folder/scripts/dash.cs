using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class dash : MonoBehaviour
{
    public bool candash = true;
    public bool dashing;
    public float dashSpeed = 100f;
    public float dashcooldowntime = 18f;
    public float hitwall = 3f;
    public GameObject player;
    public Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void dodash(InputAction.CallbackContext ctx)
    {
        if (candash == true)
        {
            if (GetComponent<movement>().IsGrounded == false && ctx.performed)
            {
                Vector3 dashDirection = Camera.main.transform.forward;
                rb.AddForce(dashDirection * dashSpeed, ForceMode.VelocityChange);
                dashing = true;
                StartCoroutine(movementWallCooldown());
                StartCoroutine(dashcooldown());

            }
        }
    }
    IEnumerator dashcooldown()
    {
        candash = false;

        yield return new WaitForSeconds(3);

        candash = true;
    }
    IEnumerator movementWallCooldown()
    {
        dashing = false;
        GetComponent<movement>().MovementSwitch = false;

        yield return new WaitForSeconds(3);

        GetComponent<movement>().MovementSwitch = true;

    }
}