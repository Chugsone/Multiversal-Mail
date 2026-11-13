using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class movement : MonoBehaviour
{
    public float accelerate = 200f;
    public float topSpeed = 14;
    public Rigidbody rb;
    public bool IsGrounded;
    public bool canjump;
    public float jumpforce = 10f;
    public bool clamping;
    public bool MovementSwitch;
    public bool Running;
    public bool Walk;
    public Vector3 movementtttt;
    private float moveHorizontal;
    private float moveVertical;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MovementSwitch = true;
       rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        rb.AddForce(movementtttt * accelerate);
        if (clamping) 
        { 
          rb.linearVelocity = new Vector3
          (
             Mathf.Clamp(rb.linearVelocity.x, -topSpeed, topSpeed),
             rb.linearVelocity.y,
             Mathf.Clamp(rb.linearVelocity.z, -topSpeed, topSpeed)
          );
        }

    }
    public void Movement(InputAction.CallbackContext ctx)
    {
        if (MovementSwitch == true)
        {
            moveHorizontal = ctx.ReadValue<Vector2>().x;
            moveVertical = ctx.ReadValue<Vector2>().y;

            movementtttt = Camera.main.transform.right * moveHorizontal + Camera.main.transform.forward * moveVertical;
          movementtttt.y = 0;
        
        }
    }
    public void jump (InputAction.CallbackContext ctx)
    {
        if(IsGrounded == true) 
        { 
            rb.AddForce(Vector3.up * jumpforce, ForceMode.Impulse);
            IsGrounded = false;
            StartCoroutine(canjumpcooldown());
        }
    } 
    public void RunningTillNot(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Running = true;
            Walk = false;

        }
        else
        {
            Walk = true;
            Running = false;
        }
        if (Running == true)
        {
            topSpeed = 20;
        }
        else if (IsGrounded == false)
        {
            topSpeed =17;
        }
        if (Walk == true)
        {
            topSpeed = 14;
        }
        else if (IsGrounded == false)
        {
            topSpeed = 12;
        }
    }
    void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Ground"))
        {
            IsGrounded = true;

        }
    }
    IEnumerator canjumpcooldown()
    {
        canjump = false;
        yield return new WaitForSeconds(1f);
        canjump = true;
    }
}
