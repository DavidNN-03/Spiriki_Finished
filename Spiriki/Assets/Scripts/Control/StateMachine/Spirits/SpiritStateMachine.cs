using System.Collections;
using System.Collections.Generic;
using Spiriki.Interaction;
using Spiriki.Objective;
using UnityEngine;
using UnityEngine.AI;

namespace Spiriki.Control
{
    public class SpiritStateMachine : StateMachine /*Statemachine of the spirit. This class stores the variables that all the spirit states use.*/
    {

        [field: SerializeField] public float RunToCompletionAreaDistance { get; private set; } /*The distance at which the spirit switches to SpiritRunToCompletionArea.*/
        [field: SerializeField] public float FollowPlayerStoppingDistance { get; private set; } /*The navmesh agent's stopping distance for when the spirit is following the player.*/
        [field: SerializeField] public float CompletionAreaStoppingDistance { get; private set; } /*The navmesh agent's stopping distance for when running to ObjectiveCompletionArea.*/
        [field: SerializeField] public Cage Cage { get; private set; } /*The cage the spirit starts off locked in.*/
        public Animator Animator { get; private set; } /*Spirit's animator.*/
        public NavMeshAgent Agent { get; private set; } /*Spirit's navmesh agent.*/
        public ObjectiveManager ObjectiveManager { get; private set; } /*Reference to the ObjectiveManager.*/
        public Transform Player { get; private set; } /*Player's transform.*/

        private void Awake() /*Assigns values to Animator, Agent, and Player.*/
        {
            Animator = GetComponentInChildren<Animator>();
            Agent = GetComponent<NavMeshAgent>();
            ObjectiveManager = FindObjectOfType<ObjectiveManager>();
            Player = GameObject.FindWithTag("Player").transform;
        }

        private void Start() /*Switches to SpiritCapturedState.*/
        {
            SwitchState(new SpiritCapturedState(this));
        }
    }
}