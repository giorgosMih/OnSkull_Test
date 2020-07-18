using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public Camera cam;
    public CharacterController controller;
    public float moveSpeed = 10f;
    public float rotationSpeed = 50f;
    public Vector3 camForward;

    void Update()
    {
        //movement
        camForward = cam.transform.forward;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, 0f);

        if(vertical > 0)
        {
            direction += camForward;
        }
        else if(vertical < 0)
        {
            camForward = -camForward;
            direction += camForward;
        }

        /*if (direction.magnitude >= 0.1f) 
        {
            controller.Move(direction * moveSpeed * Time.deltaTime);
        }*/

        controller.Move(direction * moveSpeed * Time.deltaTime);

        //camera rotation
        if (Input.GetMouseButton(1))
        {
            float rotHorizontal = Input.GetAxisRaw("Mouse X");
            float rotVertical = Input.GetAxisRaw("Mouse Y");
            Vector3 rotation = new Vector3(rotVertical, rotHorizontal, 0f);

            transform.Rotate(rotation * Time.deltaTime * rotationSpeed);
        }
    }
}
