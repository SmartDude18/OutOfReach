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
    [Range(1.0f, 100)]
    private float camSensitivity;
    [SerializeField]
    private float camMaxAngle, camMinAngle;


    //internal objects
    InputAction moveAction;
    InputAction jumpAction;
    InputAction camAction;
    InputAction sprintAction;

    Vector2 lookValue;
    private Rigidbody body;
    bool hasJumped;
    private float camX = 0;
    private float yRotation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        body = transform.GetComponent<Rigidbody>();

        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        camAction = InputSystem.actions.FindAction("Look");
        sprintAction = InputSystem.actions.FindAction("Sprint");

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        lookValue = camAction.ReadValue<Vector2>();
        yRotation += lookValue.x * (camSensitivity * 0.1f);
        camX -= lookValue.y * (camSensitivity * 0.1f);
        camX = Mathf.Clamp(camX, camMinAngle, camMaxAngle);

        transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        bool isGrounded = groundCheck();

        dataSystem.PlayerMove(moveValue.magnitude > 0, moveValue.magnitude);
        dataSystem.PlayerGrounded(isGrounded);

        Vector3 forward = Quaternion.Euler(0, yRotation, 0) * Vector3.forward;
        Vector3 right = Quaternion.Euler(0, yRotation, 0) * Vector3.right;

        float currentSpeed = sprintAction.IsPressed() ? moveSpeed * 2.0f : moveSpeed;
        Vector3 movePower = ((forward * moveValue.y) + (right * moveValue.x)) * currentSpeed;



        if (isGrounded && !hasJumped)
        {
            movePower = Vector3.ProjectOnPlane(movePower, getGround().normal);
            Vector3 newVelocity = movePower;
            newVelocity.y = body.linearVelocity.y;
            body.linearVelocity = newVelocity;
        }
        else if (!isGrounded)
        {
            body.AddForce(movePower, ForceMode.Force);

            // Clamp only horizontal velocity in air, leave Y alone
            Vector3 flat = new Vector3(body.linearVelocity.x, 0, body.linearVelocity.z);
            if (flat.magnitude > maxSpeed)
            {
                flat = flat.normalized * maxSpeed;
                body.linearVelocity = new Vector3(flat.x, body.linearVelocity.y, flat.z);
            }
        }

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


    public void LateUpdate()
    {
        cam.transform.localRotation = Quaternion.Euler(camX, yRotation, 0);
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

}
