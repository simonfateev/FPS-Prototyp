using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Transform playerCamera = null;
    [SerializeField] float mouseSens = 4.5f;
    [SerializeField] float walkSpeed = 8f;
    [SerializeField] float gravity = -13f;
    [SerializeField] float jumpHeight = 1.0f;
    [SerializeField] [Range(0.0f, 5.0f)] float moveSmoothTime = 0.3f;
    [SerializeField] [Range(0.0f, 5.0f)] float mouseSmoothTime = 0.03f;


    [SerializeField] bool lockCursor = true;
    [SerializeField] bool playerIsGrounded = true;

    float cameraPitch = 0.0f;
    float velocityY = 0.0f;
    CharacterController controller = null;

    Vector2 currentDir = Vector2.zero;
    Vector2 currentDirVelocity = Vector2.zero;

    Vector2 currentMouseDelta = Vector2.zero;
    Vector2 currentMouseDeltaVelocity = Vector2.zero;

   

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if(lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        UpdateMouseLook();
        UpdateMovement();
    }


    void UpdateMouseLook()
    {
        Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, mouseSmoothTime);

        cameraPitch -= currentMouseDelta.y * mouseSens;

        cameraPitch = Mathf.Clamp(cameraPitch, -90.0f, 90.0f);

        playerCamera.localEulerAngles = Vector3.right * cameraPitch;

        transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSens);
    }


    public void UpdateMovement()
    {
        Vector2 targetDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        targetDir.Normalize();

        currentDir = Vector2.SmoothDamp(currentDir, targetDir, ref currentDirVelocity, moveSmoothTime);

        if (controller.isGrounded)
            velocityY = 0.0f;

        playerIsGrounded = controller.isGrounded;
        if (playerIsGrounded && velocityY < 0)
        {
            velocityY = 0f;
        }

        Vector3 velocity = (transform.forward * currentDir.y + transform.right * currentDir.x) * walkSpeed + Vector3.up * velocityY;

        if (Input.GetKey(KeyCode.Space) && playerIsGrounded)
        {
            velocityY += Mathf.Sqrt(jumpHeight * -3.0f * gravity);
        }

        controller.Move(velocity * Time.deltaTime);
        velocityY += gravity * Time.deltaTime;
    }
}
