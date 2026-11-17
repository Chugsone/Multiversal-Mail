using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
public class kick : MonoBehaviour
{
    public GameObject kickhit;
    public float kickdamg = 5;
    public Rigidbody rb;
    public float scale = 1;
    public bool CanAttack;
    public bool attacking;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CanAttack = true;
        
    }


    // Update is called once per frame
    void Update()
    {
        
    }
    public void hit(InputAction.CallbackContext ctx)
    {
        Vector3 position = transform.position + Camera.main.transform.forward * 2;
        position.y = transform.position.y;

        Vector3 rotation = Camera.main.transform.rotation.eulerAngles;
        rotation.x = 0;
        rotation.z = 0;

        
        Instantiate(kickhit, position, Quaternion.Euler(rotation));
        GameObject hit = Instantiate(kickhit, position, Quaternion.Euler(rotation));
        hit.transform.SetParent(transform);
        hit.transform.localScale = new Vector3(scale, scale, scale);
        attacking = true;
        
        
        StartCoroutine(TimeTillReset());

    }
    IEnumerator TimeTillReset()
    {
        
        if (attacking == true)
        {
            CanAttack = false;
            yield return new WaitForSeconds(3);
            CanAttack = true;
        }
        attacking = false;
    }
}
