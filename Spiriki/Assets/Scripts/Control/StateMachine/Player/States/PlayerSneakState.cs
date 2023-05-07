using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spiriki.Control
{
    public class PlayerSneakState : PlayerBaseState /*State for when the player is sneaking.*/
    {
        private readonly int SneakSpeedHash = Animator.StringToHash("SneakSpeed"); /*Hash for the parameter SneakSpeed.*/
        private readonly int SneakBlendTreeHash = Animator.StringToHash("SneakBlendTree"); /*Hash for the sneaking blend-tree.*/
        private const float CrossFadeDuration = 0.1f; /*The amount of seconds to crossfade to the sneak animation.*/
        private const float AnimatorDampTime = 0.1f; /*Animation damp time. Defines how long it takes to change to change the FreeLookSpeed parameter. A high value means that the parameter will change more slowly.*/

        public PlayerSneakState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter() /*Called when entering state. Subscribes to Jump-, and Sneakevent. Also, crossfades to sneaking blend-tree.*/
        {
            stateMachine.IsSneaking = true;
            stateMachine.InputReader.JumpEvent += HandleJump;
            stateMachine.InputReader.SneakEvent += HandleSneak;

            stateMachine.Animator.SetFloat(SneakSpeedHash, 0f);

            stateMachine.Animator.CrossFadeInFixedTime(SneakBlendTreeHash, CrossFadeDuration);
        }

        public override void Tick(float deltaTime) /*Called every FixedUpdate. Applies gravity and moves the player. Also, if the player is not grounded, switch to PlayerFallingState.*/
        {
            stateMachine.CheckGroundStatus();
            stateMachine.ApplyGravity();

            if (!stateMachine.IsGrounded)
            {
                stateMachine.SwitchState(new PlayerFallingState(stateMachine, stateMachine.RB.velocity));
                return;
            }

            stateMachine.UpdateCurrentInteractable();

            Vector3 movement = CalculateMovement();

            Move(movement * stateMachine.SneakSpeed, deltaTime);

            if (stateMachine.InputReader.MovementValue == Vector2.zero)
            {
                stateMachine.Animator.SetFloat(SneakSpeedHash, 0f, AnimatorDampTime, deltaTime);
                return;
            }
            else
            {
                stateMachine.Animator.SetFloat(SneakSpeedHash, 1f, AnimatorDampTime, deltaTime);
            }

            FaceMovementDirection(movement, deltaTime);
        }

        public override void Exit() /*Called when exiting the state. Unsubscribes from Jump- and SneakEvent.*/
        {
            stateMachine.IsSneaking = false;
            stateMachine.InputReader.JumpEvent -= HandleJump;
            stateMachine.InputReader.SneakEvent -= HandleSneak;
        }

        private void HandleSneak(bool shouldSneak) /*Sets if the player should sneak. If notm switch to PlayerFreeLookState.*/
        {
            if (shouldSneak) return;

            stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
        }

        private void HandleJump() /*If the player is grounded, switch to PlayerEnterJumpState.*/
        {
            if (!stateMachine.IsGrounded) return;

            stateMachine.SwitchState(new PlayerEnterJumpState(stateMachine, stateMachine.RB.velocity));
        }

        private Vector3 CalculateMovement() /*Calculate which direction the player should move in.*/
        {
            Vector3 forward = stateMachine.MainCamera.transform.forward;
            Vector3 right = stateMachine.MainCamera.transform.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            return forward * stateMachine.InputReader.MovementValue.y + right * stateMachine.InputReader.MovementValue.x;
        }
    }
}