using UnityEngine;
using System.Collections.Generic;
using Utility;
using UnityEngine.VFX;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputReader inputs;
    [SerializeField] private Rigidbody playerRB;
    [SerializeField] private GroundCheck groundChecker;
    [SerializeField] private MovementData data;
    [SerializeField] private Transform target;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform gunPoint;
    [SerializeField] private ParticleSystem hitEffect;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private TrailRenderer tracerEffect;
    [SerializeField] private ParticleSystem dashEffect;
    [SerializeField] private VisualEffect jumpEffect;

    private Camera mainCamera;
    public LayerMask planeLayer;

    StateMachine stateMachine;

    private Vector2 moveDirection;
    
    private int dashDirection;
    private float jumpVelocity;
    private  float jumpTiming;
    private readonly float rotationSpeed = 18f;
    public float speedDiff;
    public bool isShooting;
    
    private List <Timer> timers;
    private CooldownTimer jumpTimer, dashTimer;
    private CooldownTimer jumpCoolDownTimer, dashCoolDownTimer;
    private float lastTimeWasOnGround;
    private Vector3 shootDirection;
    private readonly float maxProjectileDistance = 20f;

    public int FacingSide
    {
        // returning the facing direction  of the player in world space
        get 
        {
            Vector3 perpendicularPosition = Vector3.Cross(transform.forward, Vector3.forward);
            float direction = Vector3.Dot(perpendicularPosition, transform.up);
            return direction > 0? -1 : direction < 0? 1 : 0;
        }
    }

    
    void Awake()
    {
        inputs.EnablePlayerActions();
        mainCamera = Camera.main;
        stateMachine = new StateMachine();

        //Timers set up based on the MovementData scriptable object values 
        jumpTimer = new CooldownTimer(data.jumpButtonHoldTime);
        dashTimer = new CooldownTimer(data.dashDuration);
        jumpCoolDownTimer = new CooldownTimer(data.jumpCoolDown);
        dashCoolDownTimer = new CooldownTimer (data.dashCooldown);

        // actions to performed when timers start and stop
        jumpTimer.OnTimerStop += () => { jumpCoolDownTimer.Start(); isShooting = false; jumpEffect.Stop(); };
        dashTimer.OnTimerStop += () => { dashCoolDownTimer.Start(); isShooting = false; };
        jumpTimer.OnTimerStart += () => jumpEffect.Play();
        dashTimer.OnTimerStart += () =>  dashEffect.Play();


        timers = new List<Timer> { jumpTimer, dashTimer, jumpCoolDownTimer, dashCoolDownTimer };


        // This part that handles animations will be refactored and moved to AnimationLogic Script!!!
        var locomotionState = new LocomotionState(this, animator);
        var jumpState = new JumpState(this, animator);
        var dashState = new DashState (this, animator);
        var shootState = new ShootState (this, animator);

        //Declaring conditions to transfer from one animations state to another
        At(locomotionState, jumpState, new FuncPredicate(() => jumpTimer.isRunning));
        At(locomotionState, shootState, new FuncPredicate(() => isShooting));
        At(locomotionState, dashState, new FuncPredicate(() => dashTimer.isRunning));
        At(shootState, jumpState, new FuncPredicate(() => jumpTimer.isRunning));
        Any(dashState, new FuncPredicate(() => dashTimer.isRunning));
        Any(locomotionState, new FuncPredicate(() => !isShooting && !jumpTimer.isRunning && !dashTimer.isRunning && groundChecker.isGrounded));
        
        stateMachine.SetState(locomotionState);
    } 

    void At (IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to ,condition);
    void Any (IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);


    void OnEnable()
    {
        inputs.Move += OnMove;
        inputs.Jump += OnJump;
        inputs.Dash += OnDash;  
        inputs.Shoot += OnFire;   
    }

    void OnDisable()
    {
        inputs.Move -= OnMove;
        inputs.Jump -= OnJump;
        inputs.Dash -= OnDash;
        inputs.Shoot -= OnFire;    
    }

    void Update()
    {
        //State Machine checking state every frame for transitions
        stateMachine.Update();
        HandleAiming();
        HandleTimers(); 
        
    }

    void FixedUpdate()
    {
        //Animation States also handles the movement(Jump, Dash, Move)
        stateMachine.FixedUpdate();
        HandleRotation();
        ApplyGravity();
    }

    private void HandleTimers()
    {
        foreach (var timer in timers)
        {
            timer.Tick (Time.deltaTime);
        }
    }


    private void OnMove(Vector2 direction)
    {
        moveDirection = direction;
    }

    private void OnFire(bool performed)
    {
        isShooting = performed;
    }

    private void OnJump( bool performed)
    {
        jumpTiming = Time.time;
        //                                                                     checking if player pressed the button in  a small time window after leaving the ground
        if (performed && !jumpCoolDownTimer.isRunning && !jumpTimer.isRunning && (groundChecker.isGrounded || jumpTiming - lastTimeWasOnGround <= data.coyteTime ))
        { 
            jumpTimer.Start();
            jumpCoolDownTimer.Start();
        }
        else if (!performed && jumpTimer.isRunning)
        {
            jumpTimer.Stop();
        }
    }

    
    private void OnDash(bool performed)
    {
        if (performed && !dashCoolDownTimer.isRunning && !dashTimer.isRunning)
        {
            dashDirection = FacingSide;
            dashTimer.Start();
            dashCoolDownTimer.Start();
        }
        else if (!performed && dashTimer.isRunning)
        {
            dashTimer.Stop();
        }
    }


    private void HandleAiming()
    {   //Raycast from camera to mousecursor and projecting the hit on  invis plane behind the player  
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray,out RaycastHit hitInfo, Mathf.Infinity, planeLayer))
        {
            target.position = hitInfo.point;
        }

        //Projecting the hit to players forward Axis
        Vector3 toHit = target.position - transform.position;
        Vector3 projectedPoint = Vector3.ProjectOnPlane(toHit, transform.right) + transform.position;
        shootDirection = (projectedPoint - gunPoint.position).normalized;

    }


    public void HandleShooting()
    {
        muzzleFlash.Emit(1);
        var tracer = Instantiate(tracerEffect, gunPoint.position, Quaternion.identity);
        tracer.AddPosition (gunPoint.position);

        //clamping the the mouseposition shooting point in relation with guns barrel point
        if (Vector3.Angle(gunPoint.right.normalized, shootDirection.normalized) > 1)
        {
            shootDirection =  Vector3.ProjectOnPlane(gunPoint.right, transform.right);
        }

       //Raycasting from gun barrel point to shootdirection we pointing with mousecursor
        if (Physics.Raycast( gunPoint.position, shootDirection, out RaycastHit hitInfo, Mathf.Infinity))
        {
            //if the raycasthit anything else than the plane behind the player (which is for camera raycast) , show the effects(bulletholes, impact effect, projectile trails)
            if( hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("TargetPlain")) return;
            
            hitEffect.transform.position = hitInfo.point;
            hitEffect.transform.forward = hitInfo.normal;
            hitEffect.Emit(1);
            tracer.transform.position = hitInfo.point;
        }
        else
        {
            //still showing projectile trails even if players raycast didnt hit anything (example: sky)
            tracer.transform.position = gunPoint.position + shootDirection * maxProjectileDistance;
        }
    }


    private void HandleRotation()
    {
        // player's model smooth 90 degrees roration based on the crosshair position in world space
        float targetAngle = 90 * Mathf.Sign(target.position.x - transform.position.x);
        Quaternion targetRotation = Quaternion.Euler ( new Vector3 (0, targetAngle, 0));
        Quaternion smoothRotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
         
        playerRB.MoveRotation(smoothRotation);

    }


    public void HandleMove()
    {
        // local values to calculate accel - decel based on player Inputs
         float accelRate;
         float targetSpeed = moveDirection.x * data.maxMovementSpeed;
         speedDiff = targetSpeed - playerRB.linearVelocity.x;

        if (groundChecker.isGrounded)
        {       
            //Values when he is on the ground
            lastTimeWasOnGround = Time.time;
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f)? data.groundAcceleration : data.groundDeceleration;
        }
        else
        {
            // Less control on the air, values when he is in the air , even jumping or falling from platform
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f)? data.airAcceleration : data.airDeceleration;
        }

        float accelForce = speedDiff * accelRate;
        playerRB.linearVelocity += accelForce * Time.fixedDeltaTime * Vector3.right;

        if (dashTimer.isRunning)
        {
            // pausing gravity momentarily when dashing
            playerRB.linearVelocity = new Vector3(dashDirection * data.dashPower, 0, 0);
        }
        
    }

    public void HandleJump()
    {   
        if (!jumpTimer.isRunning && groundChecker.isGrounded)
        { 
            jumpVelocity = 0f;
            jumpTimer.Stop();
            return;
        }

        if (jumpTimer.isRunning)
        {
            float launchPoint = 0.9f;
            // using big force on the first 10% of the jump
            if (jumpTimer.Progress > launchPoint)
            {
                jumpVelocity = data.jumpForce; 
            }
            else 
            { 
                // progressive lower jump force values based on wich phase of the jump player be  
                jumpVelocity += (1 - jumpTimer.Progress) * data.gravityStrength * data.smallJumpMultiplier * Time.fixedDeltaTime;
            }
        }
        else
        {
            // progressive more custom gravity values after the apex of the jump
            jumpVelocity += data.gravityStrength * data.fallMultiplier * Time.fixedDeltaTime;
        }

        playerRB.linearVelocity = new Vector3 (playerRB.linearVelocity.x, jumpVelocity, 0);
    }


    private void ApplyGravity()
    {   
        // Applying custom gravity values
        if (groundChecker.isGrounded)
        {   // handles the gravity values when player is on ground
            playerRB.linearVelocity = new Vector3(playerRB.linearVelocity.x, playerRB.linearVelocity.y + data.gravityStrength * Time.fixedDeltaTime, 0);
        }        
        else if (!groundChecker.isGrounded && !jumpTimer.isRunning )
        {   //handles the gravity values when player is just falling without jumping before
            playerRB.linearVelocity = new Vector3(playerRB.linearVelocity.x, playerRB.linearVelocity.y + data.fallMultiplier * data.gravityStrength * Time.fixedDeltaTime, 0);
        }
    }
}
