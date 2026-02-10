using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{

    //inspector items - objects
    [SerializeField]
    private InputActionMap actions;
    [SerializeField]
    private GameObject foot, cam;
    [SerializeField]
    private LayerMask groundLayer;
    [SerializeField]
    private PlayerDataBroadcast dataSystem;

    //inspector items - settings
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float maxSpeed;
    [SerializeField]
    private float jumpHeight;
    [SerializeField]
    private float groundCheckDistance;
    [SerializeField]
    private Vector2 camSensitivity;
    [SerializeField]
    private float camMaxAngle, camMinAngle;


    //internal objects
    InputAction moveAction;
    InputAction jumpAction;
    InputAction camAction;
    private Rigidbody body;
    bool hasJumped;
    private float camX = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        body = transform.GetComponent<Rigidbody>();
        body.maxLinearVelocity = maxSpeed;

        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        camAction = InputSystem.actions.FindAction("Look");

        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //read input
        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        Vector2 lookValue = camAction.ReadValue<Vector2>();
        bool isGrounded = groundCheck();

        //broadcast system
        dataSystem.PlayerMove(moveValue.magnitude > 0, moveValue.magnitude);
        dataSystem.PlayerGrounded(isGrounded);

        //transform.rotation = Quaternion.Euler(transform.rotation.x, cam.transform.eulerAngles.y, transform.rotation.z);
        updateCamera(lookValue);
        Vector3 movePower = ((transform.forward * moveValue.y) + (transform.right * moveValue.x)) * moveSpeed;
        if (isGrounded)
        {
            //movePower = ((-getGround().transform.forward * moveValue.y) + (-getGround().transform.right * moveValue.x)) * moveSpeed;
            movePower = movePower.ProjectOntoPlane(getGround().normal);
        }
        //movement

        body.AddForce(movePower, ForceMode.Force);


        //jump system
        //has jumped is used to make the movement off of 1 frame of jump force, making the jump more consistent
        //could be messed with in theory by pressing quickly while still in ground check range, but for how small it is... good luck
        if (jumpAction.IsPressed() && isGrounded && !hasJumped)
        {
            hasJumped = true;
            jump();
        }
        else if (!jumpAction.IsPressed() && isGrounded && hasJumped)
        {
            hasJumped = false;
        }
    }

    private void jump()
    {
        dataSystem.PlayerJump();
        body.AddForce(transform.up * jumpHeight, ForceMode.Impulse);
    }

    private bool groundCheck()
    {
        RaycastHit hit;
        return (Physics.Raycast(foot.transform.position, Vector3.down, out hit, groundCheckDistance, groundLayer));
    }

    private RaycastHit getGround()
    {
        RaycastHit hit;
        Physics.Raycast(foot.transform.position, Vector3.down, out hit, groundCheckDistance, groundLayer);
        return hit;
    }

    private void updateCamera(Vector2 lookValue)
    {
        transform.Rotate(new Vector3(0, lookValue.x * camSensitivity.x, 0));
        camX -= lookValue.y * camSensitivity.y;
        camX = Mathf.Clamp(camX, camMinAngle, camMaxAngle);
        cam.transform.localRotation = Quaternion.Euler(camX, cam.transform.localRotation.y , cam.transform.localRotation.z);
    }
}
