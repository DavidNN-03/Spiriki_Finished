using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spiriki.Control
{
    public class EnemyGuardMovingState : EnemyBaseState /*Enemy state for when standing at waypoint.*/
    {
        private readonly int WalkHash = Animator.StringToHash("Walking"); /*Hash for the walking animation.*/
        private const float CrossFadeDuration = 0.1f; /*Seconds it takes to crossfade to the idle animation.*/

        public EnemyGuardMovingState(EnemyStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter() /*Called when entering state. Switch to walking animation and set navmesh agent destination to current waypoint.*/
        {
            stateMachine.Animator.CrossFadeInFixedTime(WalkHash, CrossFadeDuration);

            stateMachine.Agent.SetDestination(stateMachine.PatrolPath.GetWaypoint(stateMachine.CurrentGuardPointIndex).position);
        }

        public override void Tick(float deltaTime) /*Called every fixed update. Attempts to spot player.*/
        {
            stateMachine.AttemptSpotPlayer(deltaTime);

            //stateMachine.Animator.SetFloat(MoveSpeedHash, 1f, AnimatorDampTime, deltaTime);

            if (stateMachine.Agent.remainingDistance <= stateMachine.Agent.stoppingDistance)
            {
                stateMachine.SwitchState(new EnemyGuardIdleState(stateMachine));
            }
        }

        public override void Exit() /*Called when exiting state. This function is empty.*/
        {
        }
    }
}