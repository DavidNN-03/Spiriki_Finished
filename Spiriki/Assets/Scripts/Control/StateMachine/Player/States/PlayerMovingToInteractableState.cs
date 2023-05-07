using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Spiriki.Control
{
    public class PlayerMovingToInteractableState : PlayerBaseState /*State for when the player is moving to interactable.*/
    {
        private NavMeshAgent agent; /*Reference to the navmesh agent.*/

        public PlayerMovingToInteractableState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter() /*Called when entering the state. Enable agent and set destination to current interactable's position.*/
        {
            //stateMachine.RB.velocity = Vector3.zero;
            agent = stateMachine.GetComponent<NavMeshAgent>();
            agent.enabled = true;
            agent.SetDestination(stateMachine.CurrentInteractable.GetPlayerTargetTransform().position);
        }

        public override void Tick(float deltaTime) /*Called every FixedUpdate. If the player has reached the interactable, switch to PlayerInteractingState.*/
        {
            stateMachine.CheckGroundStatus();
            stateMachine.ApplyGravity();

            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                stateMachine.SwitchState(new PlayerInteractingState(stateMachine));
            }
        }

        public override void Exit() /*Called when exiting the state. Disables the navmesh agent.*/
        {
            agent.enabled = false;
        }
    }
}