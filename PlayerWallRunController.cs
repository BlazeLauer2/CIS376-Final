using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerWallRunController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float jumpForce = 10f;

    [Header("Wall Running")]
    public float wallRunForce = 10f;
    public float wallRunDuration = 1.5f;
    public float wallCheckDistance = 1f;
    public float wallStickForce = 1f;

    public float gravity = -10f;

    public LayerMask groundLayer;
    public LayerMask wallLayer;

    private Rigidbody rb;
    private bool isGrounded;
    private bool isWallRunning = false;
    private bool isNearWall = false;
    private float wallRunTimer;
    private Vector3 wallNormal;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        HandleGroundCheck();
        HandleWallCheck();

        if (isNearWall && !isGrounded)
        {
            StartWallRun();
        }
        else if (!isNearWall || isGrounded)
        {
            StopWallRun();
        }

        if (isWallRunning)
        {
            wallRunTimer -= Time.deltaTime;
            if (wallRunTimer <= 0 || !isNearWall)
            {
                StopWallRun();
            }

            if (Input.GetButtonDown("Jump"))
            {
                JumpOffWall();
            }
        }
        else
        {
            HandleMovement();
        }
    }

    void HandleGroundCheck()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f, groundLayer);
    }

    // Check if the player is near a wall
    void HandleWallCheck()
    {
        isNearWall = false;
        RaycastHit hit;
        float radius = 0.3f; // tweak this to match your character's width

        // Check left
        if (Physics.SphereCast(transform.position, radius, -transform.right, out hit, wallCheckDistance, wallLayer))
        {
            isNearWall = true;
            wallNormal = hit.normal;
        }
        // Check right
        else if (Physics.SphereCast(transform.position, radius, transform.right, out hit, wallCheckDistance, wallLayer))
        {
            isNearWall = true;
            wallNormal = hit.normal;
        }
    }


    // Handle player movement and jumping
    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 moveDir = transform.right * moveX + transform.forward * moveZ;
        Vector3 velocity = new Vector3(moveDir.x * moveSpeed, rb.velocity.y, moveDir.z * moveSpeed);
        rb.velocity = velocity;

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        }
    }

    // Gravity adjustments for wall running
    void StartWallRun()
    {
        if (isWallRunning) return;

        isWallRunning = true;
        wallRunTimer = wallRunDuration;
        rb.useGravity = false;
    }

    void StopWallRun()
    {
        isWallRunning = false;
        rb.useGravity = true;
    }

    void JumpOffWall()
    {
        StopWallRun();
        Vector3 jumpDirection = wallNormal + Vector3.up;
        float wallJumpMultiplier = 1.5f;
        rb.velocity = jumpDirection.normalized * jumpForce * wallJumpMultiplier;
    }

    void FixedUpdate()
    {
        HandleGroundCheck();
        HandleWallCheck();

        if (isWallRunning)
        {
            // Still apply stick force so the player clings to the wall
            Vector3 stickForce = -wallNormal * wallStickForce;
            rb.AddForce(stickForce, ForceMode.Force);

            // Let player control movement manually
            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");

            Vector3 moveDir = transform.right * moveX + transform.forward * moveZ;
            Vector3 currentVelocity = rb.velocity;
            Vector3 newVelocity = new Vector3(moveDir.x * moveSpeed, currentVelocity.y, moveDir.z * moveSpeed);

            rb.velocity = newVelocity;
        }
        else
        {
            // Apply gravity when not wall running
            if (!isGrounded)
            {
                rb.AddForce(Vector3.up * gravity, ForceMode.Acceleration);
            }
        }
    }
}