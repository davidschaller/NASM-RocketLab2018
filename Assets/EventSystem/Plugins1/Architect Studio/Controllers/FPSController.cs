using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    public RotationAxes axes = RotationAxes.MouseXAndY;
    public float jumpSpeed = 8.0f;

    public float speed = 3.0f;
    public float fastSpeed = 5.0f;

    public float gravity = 5.0f;

    private Vector3 moveDirection = Vector3.zero;
    private bool grounded = false;

    void Start()
    {
        // Make the rigid body not change rotation
        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().freezeRotation = true;
    }

    public float rotateSpeed = 300;

    void FixedUpdate()
    {
        if (CameraController.Locked())
            return;

        if (grounded)
        {
            // We are grounded, so recalculate movedirection directly from axes
            moveDirection = new Vector3(0, 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);

            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                moveDirection *= fastSpeed;
            else
                moveDirection *= speed;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            moveDirection.y += jumpSpeed;
        }

        // Apply gravity
        moveDirection.y -= gravity * Time.deltaTime;

        // Move the controller
        CharacterController controller = GetComponent(typeof(CharacterController)) as CharacterController;
        CollisionFlags flags = controller.Move(moveDirection * Time.deltaTime);
        grounded = (flags & CollisionFlags.CollidedBelow) != 0;

        transform.Rotate(0, Input.GetAxis("Horizontal") * rotateSpeed * Time.deltaTime, 0);

        if (!Input.GetMouseButton(0))
            return;

        if (axes == RotationAxes.MouseXAndY)
        {
            transform.Rotate(0, Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime, 0);
        }
    }
}