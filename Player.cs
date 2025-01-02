using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(CharacterController), typeof(Animator))]
public class Player : MonoBehaviour
{
    #region Components
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private PlayerInputs playerInputs;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private CinemachineCamera playerCinemachineCamera;
    [SerializeField] private CinemachineOrbitalFollow playerCinemachineOrbitalCamera;
    #endregion

    #region HiddenVariables
    private const string gameManagerTag = "GameManager";
    private static readonly int animGrounded = Animator.StringToHash("grounded");
    private static readonly int animIdle = Animator.StringToHash("idle");
    private static readonly int animMoveForward = Animator.StringToHash("moveForward");
    private static readonly int animMoveBack = Animator.StringToHash("moveBack");
    private static readonly int animMoveLeft = Animator.StringToHash("moveLeft");
    private static readonly int animMoveRight = Animator.StringToHash("moveRight");
    private static readonly int animRunning = Animator.StringToHash("running");
    private static readonly int animJumping = Animator.StringToHash("jumping");
    private static readonly int animFalling = Animator.StringToHash("falling");
    private float coyoteTimer;
    private int jumpCount;
    private float timeToApex;
    private float initialJumpVelocity;
    private float previousYVelocity;
    RaycastHit groundTypeCheck;
    private float gravity; // calculated during jump variable setup
    private Vector3 appliedGravity; // Gravity to be applied, inside it's own Vector to avoid applying camera rotation to gravity value
    private Vector3 cameraForward;
    private Vector3 cameraRight;
    private Vector3 moveDirection;
    private bool cameraSaved;
    private bool cameraPositionSaved;
    private Vector2 savedCameraFreePosition;
    private float lerpElapsedTime = 0f; // Tracks elapsed time for LERP
    private const float lerpDuration = 0.2f; // Duration for LERP
    #endregion

    [Header("---- States ------------------------------------------------------------")]
    [Header("Movement")]
    [Space(5)]
    public bool looking;
    public bool movingForward;
    public bool movingLeft;
    public bool movingRight;
    public bool movingBack;
    public bool running;
    public bool jump;
    public bool jumping;
    public bool falling;
    [Space(5)]
    [Header("General")]
    [Space(5)]
    public bool grounded;
    public bool onStairs;
    public bool onSlope;
    public bool idle;
    public bool noCoyote;
    public bool cameraFree;
    [Space(5)]
    [Header("Effects")]
    [Space(5)]
    public bool canMove = true;
    public bool canLook = true;
    [Header("---- Variables ------------------------------------------------------------")]
    [Space(5)]
    [Tooltip("The speed the player walks at")]
    [SerializeField] private float walkSpeed = 1.3f;
    [Tooltip("The speed the player runs at")]
    [SerializeField] private float runSpeed = 2f;
    [Tooltip("The grace after walking off ledges before jumping becomes impossible")]
    [SerializeField] private float coyoteTime = 0.2f;    
    [Tooltip("Can be edited along with maxJumpTime if you wish to adjust height of jumping")]
    [SerializeField] private float maxJumpHeight = 1f;
    [Tooltip("Can be edited along with maxJumpHeight if you wish to adjust height of jumping")]
    [SerializeField] private float maxJumpTime = 0.35f;
    [Header("---- Gravity ------------------------------------------------------------")]
    [Space(5)]
    [Tooltip("The gravity to apply when on a flat surface, this gets set according to the physics engine in Awake()")]
    [SerializeField] private float groundedGravity = -9.81f;    
    [Tooltip("The gravity to apply when not on flat surfaces or player is on stairs to counter bobbing")]
    [SerializeField] private float slopeForce = -30f;
    [Tooltip("Speed to fall (not jump falling, as Velocity Verlet deals with that)")]
    [SerializeField] private float fallSpeed = 2.5f;
    [Header("---- Ground ------------------------------------------------------------")]
    [Space(5)]
    [Tooltip("The environment layer so player collides with scenery")]
    [SerializeField] private LayerMask groundLayer;    
    [Tooltip("How large the ground detection sphere is, try not to tweak this as it's finely tuned")]
    [SerializeField] private float groundCheckSphereRadius = 0.15f;
    [Tooltip("The distance downward for the RayCast for checking if player is on a slope")]
    [SerializeField] private float groundTypeRayDistance = 1.2f;

    private void Awake()
    {
        groundedGravity = Physics.gravity.y;
        coyoteTimer = coyoteTime;        
    }

    private void Start()
    {
        gameManager = GameObject.FindWithTag(gameManagerTag).GetComponent<GameManager>();
        if (TryGetComponent(out playerInputs)) { }
        if (TryGetComponent(out playerTransform)) { }
        if (TryGetComponent(out playerAnimator)){}
        if (TryGetComponent(out characterController))
        {
            characterController.center = new Vector3(0, 0.904f, 0);
            characterController.radius = 0.36f;
            characterController.height = 1.66f;
        }

        playerCameraTransform = Camera.main.transform;

        if (playerCinemachineCamera)
        {
            gameManager.options.PopulateCamera(playerCinemachineCamera);
        }
        else
        {
            Debug.Log("Please reference player's Cinemachine cameras.");
        }

        System.GC.Collect();
    }

    private void Update()
    {
        HandleCamera();
        GravityAndGround();
        if (canMove)
        {
            Move();            
            Jump();
        }        
        Animate();
    }

    private void HandleCamera()
    {

        if (canLook)
        {
            playerCinemachineOrbitalCamera.HorizontalAxis.Value += playerInputs.lookInput.x * gameManager.options.mouseHorizontalSensitivity * Time.deltaTime;

            if (gameManager.options.invertMouseVertical)
            {
                playerCinemachineOrbitalCamera.VerticalAxis.Value += playerInputs.lookInput.y * gameManager.options.mouseVerticalSensitivity * Time.deltaTime;
            }
            else
            {
                playerCinemachineOrbitalCamera.VerticalAxis.Value -= playerInputs.lookInput.y * gameManager.options.mouseVerticalSensitivity * Time.deltaTime;
            }
        }

        cameraForward = Vector3.ProjectOnPlane(playerCameraTransform.forward, Vector3.up).normalized; // Get the forward direction of camera        
        cameraRight = Vector3.ProjectOnPlane(playerCameraTransform.right, Vector3.up).normalized; // Get the right direction camera


        if (!cameraFree && cameraSaved)// LERP camera back to default after you exit free look
        {
            if (!cameraPositionSaved)
            {
                savedCameraFreePosition.x = playerCinemachineOrbitalCamera.HorizontalAxis.Value;
                savedCameraFreePosition.y = playerCinemachineOrbitalCamera.VerticalAxis.Value;
                cameraPositionSaved = true;
            }

            lerpElapsedTime += Time.deltaTime;            
            playerCinemachineOrbitalCamera.HorizontalAxis.Value = Mathf.Lerp(savedCameraFreePosition.x, 0f, Mathf.Clamp01(lerpElapsedTime / lerpDuration)); // LERP for smooth camera transition to default position
            playerCinemachineOrbitalCamera.VerticalAxis.Value = Mathf.Lerp(savedCameraFreePosition.y, 17.5f, Mathf.Clamp01(lerpElapsedTime / lerpDuration));

            if (lerpElapsedTime >= lerpDuration)
            {
                lerpElapsedTime = 0f;
                cameraPositionSaved = false;
                cameraSaved = false;
            }
        }
    }

    private void Move()
    {    
        if (!cameraSaved)
        {
            playerTransform.rotation = Quaternion.Euler(0, playerCameraTransform.rotation.eulerAngles.y, 0);
        }

        if (appliedGravity.y != 0f) // Apply Gravity
        {
            characterController.Move(appliedGravity * Time.deltaTime);
        }

        if (playerInputs.moveInputIn3D.x != 0f || playerInputs.moveInputIn3D.z != 0f)
        {
            moveDirection = cameraFree ? playerTransform.forward * playerInputs.moveInputIn3D.z + playerTransform.right * playerInputs.moveInputIn3D.x : cameraForward * playerInputs.moveInputIn3D.z + cameraRight * playerInputs.moveInputIn3D.x;

            if (running)
            {
                characterController.Move(moveDirection * runSpeed * Time.deltaTime);
            }
            else
            {
                characterController.Move(moveDirection * walkSpeed * Time.deltaTime);
            }
        }
    }

    private void Jump()
    {
        // DONT MESS WITH ANY OF THIS AND KEEP BEFORE JUMPING CODE, USED FOR VELOCITY VERLET FOR PERFECT JUMPS
        timeToApex = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
        // ------------------------------------------------------------------------------------------

        if (jump)
        {
            if (jumpCount != 1)
            {
                jumpCount = 1;
                jump = false;
            }
        }

        if (!jumping)
        {
            if (grounded && jump && jumpCount == 1f && !falling || !grounded && jump && coyoteTimer > 0f && jumpCount == 1f && !falling)
            {
                grounded = false;
                jumping = true;
                noCoyote = true;
                falling = false;
                jumpCount = 0;
                appliedGravity.y = 0f;

                if (idle)
                {
                    appliedGravity.y += initialJumpVelocity; // Jump height when stood still
                }
                else if (movingForward || movingBack || movingLeft || movingRight)
                {
                    appliedGravity.y += initialJumpVelocity * 1.2f; // Jump height when running
                }
                else
                {
                    appliedGravity.y += initialJumpVelocity * 1.15f; // Jump height when walking
                }
            }
        }
    }

    private void GravityAndGround()
    {
        if (!jumping && !falling || jumping && appliedGravity.y < 0 || falling && appliedGravity.y < 0)
        {
            if (Physics.CheckSphere(playerTransform.position, groundCheckSphereRadius, groundLayer))// Ground Check
            {
                grounded = true;
                falling = false;
                jumping = false;
                coyoteTimer = coyoteTime;
            }
            else
            {
                grounded = false;
            }

            // TO DETECT SLOPES AND STAIRS WHICH REQUIRES HARDER GRAVITY TO STOP BOBBING DOWN INCLINES
            if (Physics.Raycast(transform.TransformPoint(characterController.center), transform.TransformDirection(Vector3.down), out groundTypeCheck, groundTypeRayDistance, groundLayer))
            {
                if (groundTypeCheck.transform.gameObject.CompareTag(Strings.slopeTag))
                {
                    onStairs = true;
                    onSlope = false;
                }
                else if (groundTypeCheck.normal != Vector3.up)
                {
                    //Debug.Log($"{slopeCheck.collider.gameObject.name} -> {slopeCheck.normal}");
                    onSlope = true;
                }
                else if (groundTypeCheck.normal == Vector3.up)
                {
                    onSlope = false;
                    onStairs = false;
                }
            }
            else
            {
                onSlope = false;
                onStairs = false;
            }
        }

        // GRAVITY --------------------------------------------------------------------
        if (jumping)
        {           
            previousYVelocity = appliedGravity.y;
            appliedGravity.y = appliedGravity.y + (gravity * Time.deltaTime);
            appliedGravity.y = (previousYVelocity + appliedGravity.y) * 0.5f;
        }
        else if (falling && !jumping) // FALLING, SLOWLY APPLY GRAVITY
        {
            appliedGravity.y -= Time.deltaTime * fallSpeed;
        }
        else if (!grounded && !falling && !jumping) // FALLING, SLOWLY APPLY GRAVITY, THIS IS FOR WHEN THE PLAYER SPAWNS IN MID AIR
        {
            appliedGravity.y -= Time.deltaTime * fallSpeed;
        }
        else if (onSlope || onStairs) // IF ON SLOPE OR STAIRS APPLY MORE GRAVITY TO STOP BOUNCING
        {
            appliedGravity.y = slopeForce;
        }
        else
        {
            appliedGravity.y = groundedGravity;
        }
    }

    private void Animate()
    {
        if (!movingForward && !movingBack && !movingRight && !movingLeft && !jumping && grounded)
        {
            idle = true;
        }
        else if (movingForward && movingBack && movingRight && movingLeft && !jumping && grounded)
        {
            idle = true;
        }
        else
        {
            idle = false;
        }

        playerAnimator.SetBool(animGrounded, grounded);
        playerAnimator.SetBool(animIdle, idle);
        playerAnimator.SetBool(animMoveForward, movingForward);
        playerAnimator.SetBool(animMoveBack, movingBack);
        playerAnimator.SetBool(animMoveLeft, movingLeft);
        playerAnimator.SetBool(animMoveRight, movingRight);
        playerAnimator.SetBool(animRunning, running);
        playerAnimator.SetBool(animJumping, jumping);
        playerAnimator.SetBool(animFalling, falling);
    }
}