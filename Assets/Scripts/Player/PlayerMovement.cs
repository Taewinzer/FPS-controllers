using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float playerHeight = 3f;

    [Header("References")]
    [SerializeField] private Transform orientation;
    [SerializeField] private WallRun wallRun;
    [SerializeField] private Transform playerBody;

    [Header("Movement")]
    public float moveSpeed = 6f;
    private float moveMultiplier = 10f;
    [SerializeField] private float airMultiplier = 0.4f;

    [Header("Move Speeds")]
    [SerializeField] private float currentSpeed = 0f;
    [SerializeField] private float crouchSpeed = 3f;
    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float accelleration = 10f;

    [Header("Jumping")]
    public int airJumps = 1;
    public float airJumpForce = 3f;
    public float jumpForce = 5f;

    [Header("Crouching")]
    [SerializeField] private float slideTime = 1f;
    private float lastSlide = 0f;
    [SerializeField] private float slideForce = 4f;

    [Header("Keybinds")]
    [SerializeField] private bool toggleSprint = false;
    [SerializeField] private bool sprint = false;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;

    [Header("States")]
    [SerializeField] private bool isCrouching = false;
    [SerializeField] private bool isSliding = false;
    [SerializeField] private Vector3 standingScale = new Vector3(1f, 1.5f, 1f);
    [SerializeField] private Vector3 crouchScale = new Vector3(1f, 0.75f, 1f);

    [Header("Camera")]
    [SerializeField] private Camera cam;
    [SerializeField] private float fov;
    [SerializeField] private int baseFov = 110;
    [SerializeField] private int fovMax = 115;
    [SerializeField] private int slideFov;
    [SerializeField] private int sprintFov;
    [SerializeField] private float fovTime;

    [Header("Drag")]
    [SerializeField] private float groundDrag = 6f;
    [SerializeField] private float airDrag = 0f;
    [SerializeField] private float slideDrag = 2f;

    private float verticalMovement;
    private float horizontalMovement;

    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;
    public bool isGrounded;
    private float groundDistance = 0.4f;

    private Vector3 moveDir;
    private Vector3 slopeMoveDir;

    private Rigidbody rb;

    private RaycastHit slopeHit;

    private string HUDstring = "";

    private void OnGUI()
    {
        GUI.Label(new Rect(5, 65, 100, 25), HUDstring);
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.5f))
        {
            if (slopeHit.normal != Vector3.up)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    IEnumerator Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        GUI.depth = 2;
        while (true)
        {
            if (Time.timeScale == 1)
            {
                yield return new WaitForSeconds(0.01f);
                HUDstring = "Speed :" + currentSpeed;
            }
            else
            {
                HUDstring = "Pause";
            }

            yield return new WaitForSeconds(0.05f);
        }
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded)
        {
            airJumps = 1;
        }

        MyInput();
        ControlDrag();
        ControlSpeed();
        FieldOfView();

        if (Input.GetKeyDown(jumpKey))
        {
            Jump();
        }

        if (Input.GetKeyDown(sprintKey) && toggleSprint)
        {
            sprint = !sprint;
        }

        slopeMoveDir = Vector3.ProjectOnPlane(moveDir, slopeHit.normal);

        currentSpeed = rb.velocity.magnitude;
    }

    void MyInput()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        moveDir = orientation.forward * verticalMovement + orientation.right * horizontalMovement;

        if (Input.GetKeyDown(crouchKey))
        {
            StartCrouch();
        }
        if (Input.GetKeyUp(crouchKey))
        {
            StopCrouch();
        }
    }

    void Jump()
    {
        if (isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
        else if (airJumps > 0 && !isGrounded && !wallRun.isWallRunning)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(transform.up * airJumpForce, ForceMode.Impulse);

            airJumps--;
        }
    }

    void StartCrouch()
    {
        isCrouching = true;
        playerBody.localScale = crouchScale;
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.75f, transform.position.z);
        MoveCamera.yOffset = -0.5f;

        if (currentSpeed >= 8f && Time.time >= lastSlide + slideTime)
        {
            rb.AddForce(moveDir.normalized * slideForce, ForceMode.Impulse);
            lastSlide = Time.time;
        }
    }

    void StopCrouch()
    {
        playerBody.localScale = standingScale;
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.75f, transform.position.z);
        MoveCamera.yOffset = 0f;
        isCrouching = false;
    }

    void ControlSpeed()
    {
        if (currentSpeed >= 4 && isCrouching && isGrounded)
        {
            isSliding = true;
        }
        else
        {
            isSliding = false;
        }

        if (Input.GetKey(sprintKey) && isGrounded || sprint && isGrounded && !isCrouching)
        {
            moveSpeed = Mathf.Lerp(moveSpeed, sprintSpeed, accelleration * Time.deltaTime);
        }
        else if (Input.GetKey(crouchKey) && isGrounded && !isSliding)
        {
            moveSpeed = Mathf.Lerp(moveSpeed, crouchSpeed, accelleration * Time.deltaTime);
        }
        else
        {
            moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, accelleration * Time.deltaTime);
        }
    }

    void ControlDrag()
    {
        if (isGrounded && !isSliding)
        {
            rb.drag = groundDrag;
        }
        else if (isSliding)
        {
            rb.drag = slideDrag;
        }
        else
        {
            rb.drag = airDrag;
        }
    }

    void FieldOfView()
    {
        if (wallRun.isWallRunning)
        {
            fov = baseFov + currentSpeed + 5;
        }
        else
        {
            fov = baseFov + currentSpeed;
        }

        if (fov > fovMax)
        {
            fov = fovMax;
        }

        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fov, fovTime);
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        if (isGrounded && !OnSlope())
        {
            rb.AddForce(moveDir.normalized * moveSpeed * moveMultiplier, ForceMode.Acceleration);
        }
        else if (isGrounded && OnSlope())
        {
            rb.AddForce(slopeMoveDir.normalized * moveSpeed * moveMultiplier, ForceMode.Acceleration);
        }
        else if (!isGrounded)
        {
            rb.AddForce(moveDir.normalized * moveSpeed * airMultiplier, ForceMode.Acceleration);
        }
    }
}
