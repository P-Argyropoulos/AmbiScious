using System;
using UnityEngine;

[CreateAssetMenu]
public class MovementData : ScriptableObject
{

   [Header("Gravity")]
   [HideInInspector] public float gravityStrength; //Gravity forced applied 
   public float fallMultiplier; //Gravity  multiplier on Big Jumps
   public float smallJumpMultiplier; // Gravity multiplier on Small Jumps
   public float maxFallSpeed; //the max clamped velocity when falling

   [Space(20)]

   [Header("Run")]
   [Range(1f, 100f)] public float maxMovementSpeed; // max speed that player can have
   [Range(.25f, 50f)] public float groundAcceleration; // the rate of movement speed change on Ground (lower to bigger value)
   [Range(.25f, 50f)] public float groundDeceleration; // the rate of movement speed change on Ground ( bigger to lower value)
   [Range(.25f, 50f)]public float airAcceleration; // 
   [Range(.25f, 50f)]public float airDeceleration;

   [Space(20)]

   [Header("Jump")]

   public float jumpHeight;//  height of players jump
   public float jumpTimeToApex; // the time it takes for player to reach the jumpHeight
   public float jumpForce;// the force upwards applied to player for the jump
   public float jumpCoolDown;
   public float jumpButtonHoldTime;

   [Space(20)]

   [Header("Dash")]
   public float dashPower;
   public float dashDuration;
   public float dashCooldown;

   [Space(20)]

   [Header("Features")]
   [Range(0f, 1f)]public float coyteTime;
   [Range(0f, 1f)]public float jumpBufferTime;
   

   private void OnValidate()
   {
      // gravityStrength calculation formula
      gravityStrength = -(2 * jumpHeight) / Mathf.Pow(jumpTimeToApex, 2);

      // jumpForce calculation
      jumpForce = 2 * jumpHeight / jumpTimeToApex;
   }
}

