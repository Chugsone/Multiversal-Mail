using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class dash : MonoBehaviour
{
    public bool candash = true;
    public bool dashing;
    public float dashSpeed = 600f;
    public float dashcooldowntime = 18f;
    public float hitwall = 3f;
    public GameObject player;
    public Rigidbody rb;
    public GameObject trail;
    public ParticleSystem Funnalpartical;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        trail.SetActive(false);
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
                dashing = true;
                Vector3 dashDirection = Camera.main.transform.forward;
                dashDirection.y = 0;
                if (dashing)
                {
                    trail.SetActive(true);
                    Funnalpartical.Emit(20);
                    
                    Vector3 position = transform.position + Camera.main.transform.forward * 2;
                    position.y = transform.position.y;

                    Vector3 rotation = Camera.main.transform.rotation.eulerAngles;
                    rotation.x = 0;
                    rotation.z = 0;
                }

                rb.AddForce(dashDirection * dashSpeed, ForceMode.VelocityChange);
                dashing = true;
                StartCoroutine(movementWallCooldown());
                StartCoroutine(dashcooldown());
              
                dashDirection.x = 10;
                dashDirection.z = 10;
                
            }
        }
    }
    IEnumerator dashcooldown()
    {
        dashing = false;
        candash = false;

        yield return new WaitForSeconds(3);

        candash = true;
    }
    IEnumerator movementWallCooldown()
    {
        
        GetComponent<movement>().MovementSwitch = false;

        yield return new WaitForSeconds(0.50f);
        
        

        dashing = false;
        GetComponent<movement>().MovementSwitch = true;
        yield return new WaitForSeconds(0.50f);
        trail.SetActive(false);

    }
}
   
 