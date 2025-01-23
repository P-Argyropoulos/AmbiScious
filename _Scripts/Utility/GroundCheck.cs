using UnityEngine;

namespace Utility
{
    public class GroundCheck : MonoBehaviour
    {   
        [SerializeField] private float groundDistance;
        [SerializeField] private LayerMask groundLayer;
        public bool isGrounded { get; private set; }
        
        void Update()
        {
            isGrounded = Physics.CheckSphere(transform.position, groundDistance, groundLayer);
        }
    }
}