using UnityEngine;

public class ShootState : BaseState
{
    public ShootState (PlayerController player, Animator animator) : base (player, animator) {}

    public override void OnEnter()
    {
        animator.CrossFade (ShootHash, crossFadeDuration + 1f);   
        animator.SetLayerWeight(1,1);
    }

    public override void OnExit()
    {
        animator.SetLayerWeight(1, 0);
    }

    public override void FixedUpdate()
    {
        player.HandleMove();
        player.HandleShooting();

        
    }
}
