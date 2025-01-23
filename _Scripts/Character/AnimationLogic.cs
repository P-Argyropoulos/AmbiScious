using UnityEngine;

public class AnimationLogic : MonoBehaviour
{
    
    [SerializeField] private Animator animator;

    private Rigidbody playerRB;
    private PlayerController playerController;

    private readonly int Speed = Animator.StringToHash("Speed");
    
    void Start()
    {
        playerRB = GetComponent<Rigidbody>();
        playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        UpdateAnimator();
    }

    private void UpdateAnimator()
    {  
        animator.SetFloat(Speed, playerController.FacingSide * playerRB.linearVelocity.x);
    }

}
