using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spiriki.Control
{
    public class PlayerEnterJumpState : PlayerBaseState /*The player state for when the player is getting ready to jump.*/
    {
        private readonly int JumpHash = Animator.StringToHash("Jump"); /*Hash for the jump animation.*/
        private Vector3 momentum; /*The momentum that should be continued throughout the jump.*/
        private const float CrossfadeDuration = 0.1f; /*The amount of seconds to crossfade to the jump animation.*/

        public PlayerEnterJumpState(PlayerStateMachine stateMachine, Vector3 momentum) : base(stateMachine)
        {
            this.momentum = momentum;
            this.momentum.y = 0f;
        }

        public override void Enter() /*Called when entering this state. Crossdade to jumping animation.*/
        {
            stateMachine.Animator.CrossFadeInFixedTime(JumpHash, CrossfadeDuration);
        }

        public override void Tick(float deltaTime) /*Called every FixedUpdate. Checks if the jumping animation is done. If so, switch state to PlayerJumpingState.*/
        {
            stateMachine.CheckGroundStatus();
            stateMachine.ApplyGravity();

            Move(Vector3.zero, deltaTime);

            if (GetNormalizedTime(stateMachine.Animator, "Jump") >= 1f)
            {
                stateMachine.SwitchState(new PlayerJumpingState(stateMachine, momentum));
            }
        }

        public override void Exit() /*Called when exiting state. This function is empty.*/
        {
        }
    }
}