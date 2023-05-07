using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spiriki.Objective;

namespace Spiriki.Control
{
    public class SpiritRunToCompletionAreaState : SpiritBaseState /*State for when the spirit is running to an ObjectiveCompletionArea.*/
    {
        private readonly int SpeedHash = Animator.StringToHash("speed"); /*Hash for the speed parameter.*/
        private readonly int FreeBlendTree = Animator.StringToHash("FreeBlendTree"); /*Hash for the free blend-tree.*/
        private const float CrossFadeDuration = 0.4f; /*The amount of seconds to crossfade to the free blend-tree.*/
        private const float AnimatorDampTime = 0.1f; /*Animation damp time. Defines how long it takes to change to change the speed parameter. A high value means that the parameter will change more slowly.*/
        ObjectiveCompletionArea CompletionArea;

        public SpiritRunToCompletionAreaState(SpiritStateMachine stateMachine, ObjectiveCompletionArea completionArea) : base(stateMachine)
        {
            this.CompletionArea = completionArea;
        }

        public override void Enter() /*Called when entering the state. Enables the navesh agent and crossfades to the free blend-tree.*/
        {
            stateMachine.Agent.enabled = true;
            stateMachine.Agent.stoppingDistance = stateMachine.CompletionAreaStoppingDistance;
            stateMachine.Agent.SetDestination(CompletionArea.transform.position);

            stateMachine.Animator.CrossFadeInFixedTime(FreeBlendTree, CrossFadeDuration);
        }

        public override void Tick(float deltaTime) /*Called every FixedUpdate. Sets the navmesh agent destination to the ObjectiveCompletionArea's position.*/
        {
            stateMachine.Agent.SetDestination(CompletionArea.transform.position);

            if (stateMachine.Agent.velocity.sqrMagnitude <= 0.1f)
            {
                stateMachine.Animator.SetFloat(SpeedHash, 0f, AnimatorDampTime, deltaTime);
            }
            else
            {
                stateMachine.Animator.SetFloat(SpeedHash, 1f, AnimatorDampTime, deltaTime);
            }
        }

        public override void Exit() /*Called when exiting the state. This function is empty.*/
        {
        }
    }
}