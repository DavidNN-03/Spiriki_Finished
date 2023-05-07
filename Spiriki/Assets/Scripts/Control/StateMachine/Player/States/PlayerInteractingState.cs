using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spiriki.Control
{
    public class PlayerInteractingState : PlayerBaseState /*State for when the player is interacting with interactable.*/
    {
        private readonly int FreeLookSpeedHash = Animator.StringToHash("FreeLookSpeed"); /*Hash for the parameter FreeLookSpeed.*/
        private readonly int FreeLookBlendTree = Animator.StringToHash("FreeLookBlendTree"); /*Hash for the free-look blend-tree.*/
        private int InteractionHash; /*Hash for the interaction animation.*/
        private const float CrossFadeDuration = 0.1f; /*The amount of seconds to crossfade to the interaction animation.*/
        private const float AnimatorDampTime = 0.1f; /*Animation damp time. Defines how long it takes to change to change the FreeLookSpeed parameter. 
        A high value means that the parameter will change more slowly.*/

        public PlayerInteractingState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter() /*Called when entering the state. Snaps player to a given position and rotation. Gets value for InteractionHash from interactable, and crossfades to the interaction animation.*/
        {
            Transform playerTargetTransform = stateMachine.CurrentInteractable.GetPlayerTargetTransform();

            if (playerTargetTransform != null)
            {
                stateMachine.transform.position = playerTargetTransform.position;
                stateMachine.transform.forward = playerTargetTransform.forward;
            }

            InteractionHash = Animator.StringToHash(stateMachine.CurrentInteractable.GetAnimationName());

            stateMachine.Animator.SetFloat(FreeLookSpeedHash, 0f);

            stateMachine.Animator.CrossFadeInFixedTime(InteractionHash, CrossFadeDuration);
        }

        public override void Tick(float deltaTime) /*Called every FixedUpdate. If the interaction animation is finished, return to PlayerFreeLookState.*/
        {
            stateMachine.CheckGroundStatus();
            stateMachine.ApplyGravity();

            if (GetNormalizedTime(stateMachine.Animator, "Interaction") >= 1f)
            {
                stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
            }
        }

        public override void Exit() /*Called when exiting the state. This function is empty.*/
        {
        }
    }
}