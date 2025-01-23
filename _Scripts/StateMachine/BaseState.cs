using UnityEngine;

public abstract class BaseState : IState
{
    protected PlayerController player;
    protected Animator animator;

    protected BaseState (PlayerController player, Animator animator)
    {
        this.player = player;
        this.animator = animator;
    }

    protected static readonly int LocomotionHash = Animator.StringToHash("LocoMotion");
    protected static readonly int JumpHash = Animator.StringToHash("Jump");
    protected static readonly int DashHash = Animator.StringToHash("Dash");
    protected static readonly int ShootHash = Animator.StringToHash("Shoot");


    protected const float crossFadeDuration = 0.1f;


    public virtual void OnEnter()
    {}

    public virtual void Update()
    {}

    public virtual void FixedUpdate()
    {}

    public virtual void OnExit()
    {}
}
