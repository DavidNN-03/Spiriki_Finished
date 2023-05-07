using UnityEngine;

namespace Spiriki.Control
{
    public class PlayerJumpingState : PlayerBaseState /*State for when the player is jumping.*/
    {
        private readonly int JumpHash = Animator.StringToHash("InAir"); /*Hash for the in-air animation.*/
        private const float CrossfadeDuration = 0.1f; /*The amount of seconds to crossfade to the jump animation.*/
        private Vector3 momentum; /*Momentum that should be continued throughout the jump.*/

        public PlayerJumpingState(PlayerStateMachine stateMachine, Vector3 momentum) : base(stateMachine)
        {
            this.momentum = momentum;
        }

        public override void Enter() /*Called when entering the state. Crossfades to the jump animation.*/
        {
            stateMachine.verticalVelocity += stateMachine.JumpForce;

            momentum.y = 0f;

            stateMachine.Animator.CrossFadeInFixedTime(JumpHash, CrossfadeDuration);
        }

        public override void Tick(float deltaTime) /*Called every FixedUpdate. Moves the player, and if the player is falling, switch to PlayerFallingState.*/
        {
            stateMachine.CheckGroundStatus();
            stateMachine.ApplyGravity();

            Move(momentum, deltaTime);

            if (momentum != Vector3.zero)
            {
                FaceMovementDirection(momentum, deltaTime);
            }

            if (stateMachine.GetShouldLand())
            {
                //stateMachine.SwitchState(new PlayerLandingState(stateMachine));
            }

            if (stateMachine.CharacterController.velocity.y <= 0f)
            {
                stateMachine.SwitchState(new PlayerFallingState(stateMachine, momentum));
            }
        }

        public override void Exit() /*Called when exiting state. This function is empty.*/
        {
        }
    }
}