using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class combine : MonoBehaviour
{
    public GameObject hitbox;
    public GameObject combined;
    public GameObject torso;
    public GameObject legs;
    public bool areLegs;
    public bool areTorso;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

   
    // Update is called once per frame
    void Update()
    {
        if (CompareTag("Torso"))
        {
            areTorso = true;
        }
        if (CompareTag("legs"))
        {
            areLegs = true;
        }
    }
    public void fullbody(InputAction.CallbackContext ctx)
    {
        //Vector3 position = transform.position + Camera.main.transform.forward * 2;
        //position.y = transform.position.y;

        //Vector3 rotation = Camera.main.transform.rotation.eulerAngles;
        //rotation.x = 0;
        //rotation.z = 0;


        //Instantiate(combined, position, Quaternion.Euler(rotation));
        //GameObject hit = Instantiate(combined, position, Quaternion.Euler(rotation));
        
    }
    private void OnTriggerEnter(Collider hitbox)
    {
        if (CompareTag("Torso"))
        {
            Destroy(torso);
            Destroy(legs);
        }
        if (CompareTag("Legs"))
        {
            Destroy(torso);
            Destroy(legs);
        }
    }
}
