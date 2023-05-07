using Spiriki.Objective;
using UnityEngine;

namespace Spiriki.Control
{
    public class EnemySuspicionState : EnemyBaseState /*Enemy state for when looking for player.*/
    {
        private readonly int MoveSpeedHash = Animator.StringToHash("MoveSpeed"); /*Hash for the parameter "MoveSpeed".*/
        private readonly int GuardBlendTreeHash = Animator.StringToHash("Locomotion"); /*Hash for the locomotion blendtree.*/
        private const float CrossFadeDuration = 0.1f; /*Seconds it takes to crossfade to the locomotion blend tree.*/
        private const float AnimatorDampTime = 0.1f; /*Animation damp time. Defines how long it takes to change to change the MoveSpeed parameter. A high value means that the parameter will change more slowly.*/
        private float spottingProgressNormalized; /*Defines how close the enemy is to having fully spotted the player. 
        When entering this state, it is set to 0. If it gets to 1 or above, the player is spotted.*/

        public EnemySuspicionState(EnemyStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter() /*Called when entering state. Sets spottingProgressionNormalized to 0, fades to Guard blend-tree. Also, sets navmesh agent's destination to player's current position.*/
        {
            spottingProgressNormalized = 0f;

            stateMachine.Animator.SetFloat(MoveSpeedHash, 0f);

            stateMachine.Animator.CrossFadeInFixedTime(GuardBlendTreeHash, CrossFadeDuration);

            stateMachine.Agent.SetDestination(stateMachine.Player.transform.position);
        }

        public override void Tick(float deltaTime) /*This function is called every FixedUpdate. Enemy will try and spot the player. If the player is spotted, which happens if spottingProgressNormalized is more than or equal to 1, the player loses.*/
        {
            stateMachine.Animator.SetFloat(MoveSpeedHash, 1f, AnimatorDampTime, deltaTime);

            if (stateMachine.CheckSpotPlayer())
            {
                float spottingChange = stateMachine.CalculateSpottingChange(deltaTime);

                spottingProgressNormalized += spottingChange;
            }
            else
            {
                spottingProgressNormalized -= stateMachine.SpottingProgressLossSpeed * deltaTime;
            }

            if (spottingProgressNormalized >= 1f)
            {
                stateMachine.ObjectiveManager.Lose();
            }
            else if (spottingProgressNormalized <= 0f)
            {
                stateMachine.SwitchState(new EnemyGuardMovingState(stateMachine));
            }

            stateMachine.SpottingCanvasManager.UpdatePointer(stateMachine.gameObject, spottingProgressNormalized, Color.red);
        }

        public override void Exit() /*This function is called when exitting the state. This function is empty.*/
        {
        }
    }
}