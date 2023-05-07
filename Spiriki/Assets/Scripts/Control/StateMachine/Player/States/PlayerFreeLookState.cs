using UnityEngine;
using UnityEngine.AI;

namespace Spiriki.Control
{
    public class PlayerFreeLookState : PlayerBaseState /*State for when the player is simply moving and looking around.*/
    {
        private readonly int FreeLookSpeedHash = Animator.StringToHash("FreeLookSpeed"); /*Hash for the parameter FreeLookSpeed.*/
        private readonly int FreeLookBlendTree = Animator.StringToHash("FreeLookBlendTree"); /*Hash for the free-look blend-tree.*/
        private const float CrossFadeDuration = 0.4f; /*The amount of seconds to crossfade to the free-look blend-tree.*/
        private const float AnimatorDampTime = 0.1f; /*Animation damp time. Defines how long it takes to change to change the FreeLookSpeed parameter. A high value means that the parameter will change more slowly.*/
        private bool sprint; /*Defines if the player is sprinting.*/
        NavMeshAgent agent; /*Reference to the navmesh agent.*/

        public PlayerFreeLookState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter() /*Called when entering the state. Subscribe to Jump-, Interact-, Sprint-, and SneakEvent. Crossfade to the free.look blend-tree.*/
        {
            stateMachine.InputReader.JumpEvent += HandleJump;
            stateMachine.InputReader.InteractEvent += HandleInteract;
            stateMachine.InputReader.SprintEvent += HandleSprint;
            stateMachine.InputReader.SneakEvent += HandleSneak;

            stateMachine.Animator.SetFloat(FreeLookSpeedHash, 0f);

            stateMachine.Animator.CrossFadeInFixedTime(FreeLookBlendTree, CrossFadeDuration);

            sprint = stateMachine.InputReader.GetSprint();

            agent = stateMachine.GetComponent<NavMeshAgent>();
        }

        public override void Tick(float deltaTime) /*Called every FixedUpdate. Apply gravity and move the player. If the player is not grounded, switch to PlayerFallingState.*/
        {
            stateMachine.CheckGroundStatus();
            stateMachine.ApplyGravity();

            if (!stateMachine.IsGrounded)
            {
                stateMachine.SwitchState(new PlayerFallingState(stateMachine, stateMachine.CharacterController.velocity));
                return;
            }

            stateMachine.UpdateCurrentInteractable();

            Vector3 movement = CalculateMovement();

            float speed = stateMachine.FreeLookMoveSpeed;

            if (sprint)
            {
                speed = stateMachine.FreeLookSprintSpeed;
            }

            Move(movement * speed, deltaTime);

            if (stateMachine.InputReader.MovementValue == Vector2.zero)
            {
                stateMachine.Animator.SetFloat(FreeLookSpeedHash, 0f, AnimatorDampTime, deltaTime);
                return;
            }
            else if (!sprint)
            {
                stateMachine.Animator.SetFloat(FreeLookSpeedHash, 0.5f, AnimatorDampTime, deltaTime);
            }
            else
            {
                stateMachine.Animator.SetFloat(FreeLookSpeedHash, 1f, AnimatorDampTime, deltaTime);
            }

            FaceMovementDirection(movement, deltaTime);
        }

        public override void Exit() /*Called when exiting this state. Unsubscribes from the events, that were subscribed to in Enter().*/
        {
            stateMachine.InputReader.JumpEvent -= HandleJump;
            stateMachine.InputReader.InteractEvent -= HandleInteract;
            stateMachine.InputReader.SprintEvent -= HandleSprint;
            stateMachine.InputReader.SneakEvent -= HandleSneak;
        }

        private void HandleJump() /*If the player is grounded, switch to PlayerEnterJumpState.*/
        {
            if (!stateMachine.IsGrounded) return;

            stateMachine.SwitchState(new PlayerEnterJumpState(stateMachine, /*stateMachine.RB.velocity*/ stateMachine.CharacterController.velocity));
        }

        private void HandleInteract() /*If the player has something to interact with, switch to PlayerMovingToInteractableState.*/
        {
            //stateMachine.Interact();
            if (stateMachine.CurrentInteractable == null) return;

            if (!stateMachine.CurrentInteractable.GetRequiresKey() || stateMachine.PlayerCarrySlot.GetHasInteractable())
            {
                stateMachine.SwitchState(new PlayerMovingToInteractableState(stateMachine));
            }
        }

        private void HandleSprint(bool _sprint) /*Sets sprint to a given value.*/
        {
            sprint = _sprint;
        }

        private void HandleSneak(bool shouldSneak) /*If passed a true value, switch to PlayerSneakState.*/
        {
            if (!shouldSneak) return;

            stateMachine.SwitchState(new PlayerSneakState(stateMachine));
        }

        private Vector3 CalculateMovement() /*Calculates the direction the player should move in.*/
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