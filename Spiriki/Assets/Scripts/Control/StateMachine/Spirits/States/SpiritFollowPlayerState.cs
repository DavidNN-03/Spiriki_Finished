using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spiriki.Objective;

namespace Spiriki.Control
{
    public class SpiritFollowPlayerState : SpiritBaseState /*State for when the spirit is following the player.*/
    {
        private readonly int SpeedHash = Animator.StringToHash("speed"); /*Hash for the speed parameter.*/
        private readonly int FreeBlendTree = Animator.StringToHash("FreeBlendTree"); /*Hash for the free blend-tree.*/
        private const float CrossFadeDuration = 0.4f; /*The amount of seconds to crossfade to the free blend-tree.*/
        private const float AnimatorDampTime = 0.1f; /*Animation damp time. Defines how long it takes to change to change the soeed parameter. A high value means that the parameter will change more slowly.*/

        public SpiritFollowPlayerState(SpiritStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter() /*Called when entering the state. Moves the spirit outside the cage, enables the navmesh agent, sets the agent's destination to the player.
        Activates the ObjectiveCompletionArea's visual. Also, crossfades to the free blend-tree.*/
        {
            //stateMachine.transform.position = stateMachine.Cage.SpiritFreedTransform.position;

            stateMachine.Agent.enabled = true;
            stateMachine.Agent.stoppingDistance = stateMachine.FollowPlayerStoppingDistance;

            stateMachine.ObjectiveManager.ActivateAreaVisuals(true);

            stateMachine.Animator.CrossFadeInFixedTime(FreeBlendTree, CrossFadeDuration);
        }

        public override void Tick(float deltaTime) /*Called every FixedUpdate. Checks the distance from the spirit to the ObjectiveCompletionArea. Sets the agent's destination to the player's position.*/
        {
            CheckDistanceToCompletionArea();

            stateMachine.Agent.SetDestination(stateMachine.Player.transform.position);

            Debug.Log(stateMachine.Agent.velocity.sqrMagnitude);
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

        private void CheckDistanceToCompletionArea() /*Checks the distance from the spirit to the nearest ObjectiveCompletionArea. If the distance is less than stateMachine.RunToCompletionAreaDistance, switch to SpiritRunToCompletionAreaState.*/
        {
            float closestDistance = Mathf.Infinity;
            ObjectiveCompletionArea closestArea = null;

            foreach (ObjectiveCompletionArea area in stateMachine.ObjectiveManager.CompletionAreas)
            {
                float distance = Vector3.Distance(stateMachine.transform.position, area.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestArea = area;
                }
            }

            if (closestDistance <= stateMachine.RunToCompletionAreaDistance)
            {
                stateMachine.SwitchState(new SpiritRunToCompletionAreaState(stateMachine, closestArea));
            }
        }
    }
}