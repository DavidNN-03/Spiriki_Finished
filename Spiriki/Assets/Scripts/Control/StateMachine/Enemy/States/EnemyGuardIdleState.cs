using UnityEngine;

namespace Spiriki.Control
{
    public class EnemyGuardIdleState : EnemyBaseState /*Enemy state for when standing at waypoint.*/
    {
        private readonly int IdleHash = Animator.StringToHash("Idle"); /*Hash for the enemy idle animation.*/
        private const float CrossFadeDuration = 0.1f; /*Seconds it takes to crossfade to the idle animation.*/
        private float time; /*Amount of seconds enemy has spent at current waypoint.*/

        public EnemyGuardIdleState(EnemyStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter() /*Called when entering this state. Resets time and angent path. Also, enter idle animation.*/
        {
            time = 0f;

            stateMachine.Agent.ResetPath();
            stateMachine.Agent.velocity = Vector3.zero;

            stateMachine.Animator.CrossFadeInFixedTime(IdleHash, CrossFadeDuration);
        }

        public override void Tick(float deltaTime) /*Called every FixedUpdate. Rotates the enemy and attempts to spot player.*/
        {
            stateMachine.AttemptSpotPlayer(deltaTime);

            Vector3 desiredForward = stateMachine.PatrolPath.GetWaypoint(stateMachine.CurrentGuardPointIndex).forward;

            stateMachine.transform.forward = Vector3.Slerp(stateMachine.transform.forward,
                                                            desiredForward,
                                                            Time.deltaTime * stateMachine.RotationSpeed);

            if (time >= stateMachine.GuardIdleTime)
            {
                int nextIndex = stateMachine.PatrolPath.GetNextIndex(stateMachine.CurrentGuardPointIndex);

                stateMachine.CurrentGuardPointIndex = nextIndex;
                stateMachine.SwitchState(new EnemyGuardMovingState(stateMachine));
                return;
            }

            time += Time.deltaTime;
        }

        public override void Exit() /*Called when exiting state. This function is empty.*/
        {
        }
    }
}