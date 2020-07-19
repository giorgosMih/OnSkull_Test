using UnityEngine;

public class Movement : MonoBehaviour
{
    public Camera cam;//camera object
    public CharacterController controller;//camera controller
    public float moveSpeed = 10f;//movement speed
    public float rotationSpeed = 50f;//rotation speed

    void Update()
    {
        //camera movement
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(0f, 0f, 0f);
        
        //calculate vertical movement (forward or backwards)
        if(vertical > 0)
        {
            direction += cam.transform.forward;
        }
        else if(vertical < 0)
        {
            direction += (-cam.transform.forward);
        }

        //calculate horizontal movement (left or right)
        if (horizontal > 0)
        {
            direction += cam.transform.right;
        }
        else if (horizontal < 0)
        {

            direction += (-cam.transform.right);
        }

        //apply movement
        controller.Move(direction * moveSpeed * Time.deltaTime);

        //camera rotation (only if right click is held)
        if (Input.GetMouseButton(1))
        {
            float rotHorizontal = Input.GetAxisRaw("Mouse X");
            float rotVertical = Input.GetAxisRaw("Mouse Y");
            Vector3 rotation = new Vector3(rotVertical, rotHorizontal, 0f);//calculate new rotation

            //apply rotation
            transform.Rotate(rotation * Time.deltaTime * rotationSpeed);
        }
    }
}
