using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spiriki.Control
{
    public class PlayerLandingState : PlayerBaseState /*State for when the player is landing from a fall.*/
    {
        private readonly int LandHash = Animator.StringToHash("Land"); /*Hash for the landing animation.*/
        private const float CrossfadeDuration = 0.1f; /*The amount of seconds to crossfade to the landing animation.*/
        private Vector3 momentum; /*Momentum that should be continued throughout the jump.*/
        private bool hasLanded; /*Defines whether or not the player has landed yet.*/

        public PlayerLandingState(PlayerStateMachine stateMachine, Vector3 momentum) : base(stateMachine)
        {
            this.momentum = momentum;
        }

        public override void Enter() /*Called when entering the state. Crossfade to landing animation.*/
        {
            hasLanded = false;
            stateMachine.Animator.CrossFadeInFixedTime(LandHash, CrossfadeDuration);
        }

        public override void Tick(float deltaTime) /*Called every FixedUpdate. Apply gravity, and move the player. If the landing animation is halfway done, swich to PlayerFreeLookState.*/
        {
            stateMachine.CheckGroundStatus();
            stateMachine.ApplyGravity();

            if (!hasLanded)
            {
                CheckHasLanded();
            }

            Move(momentum, deltaTime);

            if (GetNormalizedTime(stateMachine.Animator, "Land") >= 0.5f)
            {
                stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
            }
        }

        public override void Exit() /*Called when leaving the state. This function is empty. */
        {
        }

        private void CheckHasLanded() /*If the player has landed, set momentum to Vector3.zero. */
        {
            if (stateMachine.IsGrounded)
            {
                hasLanded = true;
                momentum = Vector3.zero;
            }
        }
    }
}