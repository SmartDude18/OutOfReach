using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] PlayerData data;
    [SerializeField] Transform view;
    [SerializeField] Animator animator;
    [SerializeField] float rayLength;
    [SerializeField] LayerMask groundLayerMask;
    [SerializeField] private PlayerDataBroadcast dataSystem;
    [SerializeField] private string[] deathTags = new string[] { };
    [SerializeField] private GameManager gameManager;

    CharacterController controller;
    InputAction moveAction;
    InputAction jumpAction;
    InputAction sprintAction;

    Vector2 movementInput = Vector2.zero;
    Vector3 velocity = Vector3.zero;

    bool isSprinting = false;
    bool onGround = true;

    float groundedTimer = 0f;
    const float groundedGrace = 0.1f;
    public int playerDeaths { get; private set; }

    public Transform View { get => view; set => view = value; }

    private List<GameObject> activeFlags = new List<GameObject>();

    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        moveAction.performed += OnMove;
        moveAction.canceled += OnMove;

        sprintAction = InputSystem.actions.FindAction("Sprint");
        sprintAction.performed += OnSprint;
        sprintAction.canceled += OnSprint;

        jumpAction = InputSystem.actions.FindAction("Jump");
        jumpAction.performed += OnJump;
        jumpAction.canceled += OnJump;

        controller = GetComponent<CharacterController>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (controller.isGrounded)
            groundedTimer = groundedGrace;
        else
            groundedTimer -= Time.deltaTime;

        onGround = groundedTimer > 0;

        if (onGround && velocity.y < 0)
            velocity.y = -2f;

        dataSystem.PlayerGrounded(onGround);

        Vector3 movement = new Vector3(movementInput.x, 0, movementInput.y);
        movement = Quaternion.AngleAxis(view.rotation.eulerAngles.y, Vector3.up) * movement;

        Vector3 acceleration = Vector3.zero;
        acceleration.x = movement.x * data.acceleration;
        acceleration.z = movement.z * data.acceleration;

        if (!onGround) acceleration *= 0.1f;

        

        Vector3 XZVelocity = new Vector3(velocity.x, 0, velocity.z);
        if(XZVelocity.magnitude <= 0)
        {
            gameManager.UpdateInvisibleLevel(true);
        }else if(velocity.magnitude > 0)
        {
            gameManager.UpdateInvisibleLevel(false);
        }

        XZVelocity += acceleration * Time.deltaTime;
        XZVelocity = Vector3.ClampMagnitude(XZVelocity, isSprinting ? data.sprintSpeed : data.speed);

        velocity.x = XZVelocity.x;
        velocity.z = XZVelocity.z;

        if (movement.sqrMagnitude <= 0 && onGround)
        {
            float drag = 10;
            velocity.x = Mathf.MoveTowards(velocity.x, 0, drag * Time.deltaTime);
            velocity.z = Mathf.MoveTowards(velocity.z, 0, drag * Time.deltaTime);
        }

        transform.rotation = Quaternion.Euler(0, view.eulerAngles.y, 0);

        velocity.y += data.gravity * Time.deltaTime;

        if (onGround)
            velocity.y = Mathf.Max(velocity.y, -2f);

        controller.Move(velocity * Time.deltaTime);

        animator.SetFloat("Speed", new Vector3(velocity.x, 0, velocity.z).magnitude);
        animator.SetFloat("YVelocity", controller.velocity.y);
        animator.SetBool("OnGround", onGround);
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        movementInput = ctx.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Performed) isSprinting = true;
        else if (ctx.phase == InputActionPhase.Canceled) isSprinting = false;
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Performed && onGround)
        {
            velocity.y = Mathf.Sqrt(-2 * data.gravity * data.jumpHeight);
            dataSystem.PlayerJump();
            animator.SetTrigger("Jump");
        }
    }
    public void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (deathTags.Contains(hit.gameObject.tag))
        {
            playerDeaths++;
            controller.enabled = false;
            transform.position = gameManager.spawnPoint;
            controller.enabled = true;
            gameManager.UpdateWinSign();
            dataSystem.PlayerDies(hit.gameObject.tag);
        }

        var rb = hit.collider.attachedRigidbody;
        if (rb == null || rb.isKinematic || hit.moveDirection.y < -0.3f) return;
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
        rb.linearVelocity = pushDir * data.pushForce;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Touched");
        Debug.Log(other.gameObject.tag.ToString());
        switch (other.gameObject.tag.ToString())
        {
            case "Checkpoint":
                gameManager.UpdateSpawnpoint(false);

                SetCheckpointActive(other.gameObject, 1, false);
                SetCheckpointActive(other.gameObject, 2, true);
                activeFlags.Add(other.gameObject);
                break;
            case "Restart":
                Debug.Log("we in");
                gameManager.UpdateSpawnpoint(true);

                foreach(GameObject flag in activeFlags)
                {
                    SetCheckpointActive(flag, 1, true);
                    SetCheckpointActive(flag, 2, false);
                }
                activeFlags.Remove(other.gameObject);
                controller.enabled = false;
                transform.position = gameManager.spawnPoint;
                controller.enabled = true;
                gameManager.UpdateWinSign();

                break;
        }
    }


    private void SetCheckpointActive(GameObject other, int childIndex, bool active)
    {
        other.transform.GetChild(other.gameObject.transform.childCount - childIndex).gameObject.SetActive(active);
    }
}