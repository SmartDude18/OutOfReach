using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{

    //inspector items - objects
    [SerializeField]
    private InputActionMap actions;
    [SerializeField]
    private GameObject foot,cam;
    [SerializeField]
    private LayerMask groundLayer;
    [SerializeField]
    private PlayerDataBroadcast dataSystem;

    //inspector items - settings
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float jumpHeight;
    [SerializeField]
    private float groundCheckDistance;


    //internal objects
    InputAction moveAction;
    InputAction jumpAction;
    private Rigidbody body;
    bool hasJumped;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        body = transform.GetComponent<Rigidbody>();

        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");

        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //read input
        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        bool isGrounded = groundCheck();

        //broadcast system
        dataSystem.PlayerMove(moveValue.magnitude > 0,moveValue.magnitude);
        dataSystem.PlayerGrounded(isGrounded);

        //movement
        transform.rotation = Quaternion.Euler(transform.rotation.x, cam.transform.eulerAngles.y, transform.rotation.z);
        body.AddForce(((transform.forward * moveValue.y) + (transform.right * moveValue.x))* moveSpeed, ForceMode.Force);

        
        //jump system
        //has jumped is used to make the movement off of 1 frame of jump force, making the jump more consistent
        //could be messed with in theory by pressing quickly while still in ground check range, but for how small it is... good luck
        if (jumpAction.IsPressed() && isGrounded && !hasJumped)
        {
            hasJumped = true;
            jump();
        }
        else if(!jumpAction.IsPressed() && isGrounded && hasJumped)
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
}
