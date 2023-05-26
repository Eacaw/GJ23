using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // Have set default walk and run speed so it looks about right with the animations
    public float jumpHeight = 15.0f;
    public float cameraHeight = 15.0f;
    public float cameraDistance = 7.5f;
    public bool invertCameraXAxis = false;
    public bool invertCameraYAxis = false;
    public float rotationSpeed = 120.0f;
    public Camera mainCamera;
    private Animator anim;

    private float moveSpeed;
    private float walkSpeed = 10.0f;
    private float runSpeed = 16.0f;
    private bool inputDisabled = false;
    private float gravity = 5f;

    public bool hasBackpack = false;
    public bool hasKey = false;
    public bool hasAmulet = false;

    private Vector3 cameraTargetPosition;
    Transform cameraTransform;

    Collider playerCollider;
    Rigidbody playerRigidBody;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        anim = GetComponent<Animator>();
        playerCollider = GetComponent<Collider>();
        playerRigidBody = GetComponent<Rigidbody>();
        moveSpeed = walkSpeed;
        cameraTransform = mainCamera.transform;
        // Set the camera target position to the initial camera position
        cameraTargetPosition = cameraTransform.position;
    }

    void Update()
    {
        // Disable player move inputs if game is stopped
        inputDisabled = Time.timeScale == 0f;

        // Calculate the camera's forward and right directions
        Vector3 cameraForward = mainCamera.transform.forward;
        cameraForward.y = 0.0f;
        cameraForward.Normalize();
        Vector3 cameraRight = mainCamera.transform.right;

        // Get player movement
        Vector3 movement = new Vector3(
            Input.GetAxis("Horizontal"),
            0.0f,
            Input.GetAxis("Vertical")
        );

        if (!inputDisabled)
        {
            MovePlayer(movement, cameraForward, cameraRight);
            SetAnimation(movement);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                TeleportPlayer();
            }
        }

        SetCamera();
    }

    void Jump()
    {
        // If the player is jumping or falling, you can't jump
        if (playerRigidBody.velocity.y <= 0 && playerRigidBody.velocity.y > -0.25f)
        {
            anim.SetTrigger("isJumping");
            playerRigidBody.AddForce(Vector3.up * jumpHeight, ForceMode.VelocityChange);
        }
    }

    void MovePlayer(Vector3 movement, Vector3 cameraForward, Vector3 cameraRight)
    {
        if (movement != Vector3.zero)
        {
            Vector3 movementDirection = cameraForward * movement.z + cameraRight * movement.x;
            movementDirection.Normalize();

            bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

            float speed = isRunning ? runSpeed : walkSpeed;

            playerRigidBody.velocity = new Vector3(movementDirection.x * speed, playerRigidBody.velocity.y, movementDirection.z * speed);
            playerRigidBody.AddForce(Vector3.down * gravity, ForceMode.Acceleration);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(movementDirection),
                rotationSpeed * Time.deltaTime
            );
        }
        else
        {
            playerRigidBody.velocity = new Vector3(0, playerRigidBody.velocity.y, 0);
            playerRigidBody.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
        }
    }

    void SetAnimation(Vector3 movement)
    {
        if (movement != Vector3.zero)
        {
            anim.SetBool("isWalking", true);

            if (Input.GetKey(KeyCode.LeftShift))
            {
                anim.SetBool("isRunning", true);
                moveSpeed = runSpeed;
            }
            else
            {
                anim.SetBool("isRunning", false);
                moveSpeed = walkSpeed;
            }
        }
        else
        {
            anim.SetBool("isRunning", false);
            anim.SetBool("isWalking", false);
        }
    }

    void SetCamera()
    {
        float cameraXOffset = cameraDistance;
        float cameraYOffset = cameraDistance;

        // Inverting the camera axis
        if (invertCameraXAxis)
        {
            cameraXOffset = -cameraDistance;
        }
        if (invertCameraYAxis)
        {
            cameraYOffset = -cameraDistance;
        }

        // Set the camera position and target
        cameraTransform.position = new Vector3(
            transform.position.x - cameraXOffset,
            transform.position.y + cameraHeight,
            transform.position.z + cameraYOffset
        );
        cameraTransform.LookAt(transform.position);
    }

    void TeleportPlayer()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            SceneManager.LoadScene(2);
        }
        else
        {
            SceneManager.LoadScene(1);
        }
    }
}
