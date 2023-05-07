using UnityEngine;

namespace Spiriki.Control
{
    public class PlayerFallingState : PlayerBaseState /*State for when the player is falling.*/
    {
        private readonly int FallHash = Animator.StringToHash("InAir"); /*Hash for the in-air animation.*/
        private Vector3 momentum; /*The momentum that should be continued throughout the fall.*/
        private const float CrossfadeDuration = 0.1f; /*The amount of seconds to crossfade to the falling animation.*/

        public PlayerFallingState(PlayerStateMachine stateMachine, Vector3 momentum) : base(stateMachine)
        {
            this.momentum = momentum;
        }

        public override void Enter() /*Called when entering this state. Crossfade to the falling animation.*/
        {
            momentum.y = 0f;

            stateMachine.Animator.CrossFadeInFixedTime(FallHash, CrossfadeDuration);
        }

        public override void Tick(float deltaTime) /*Called every FixedUpdate. Apply gravity and move the player. Check if the player should switch to the PlayerLandingState.*/
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
                stateMachine.SwitchState(new PlayerLandingState(stateMachine, momentum));
            }
        }

        public override void Exit() /*Called when eciting the state. This function is empty.*/
        {
        }
    }
}