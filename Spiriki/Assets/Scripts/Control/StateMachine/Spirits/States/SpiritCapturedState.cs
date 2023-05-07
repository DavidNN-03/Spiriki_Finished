using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spiriki.Control
{
    public class SpiritCapturedState : SpiritBaseState /*State for when the spirit is captured.*/
    {
        private readonly int CapturedHash = Animator.StringToHash("Captured"); /*Hash for the captured animation.*/
        private const float CrossFadeDuration = 0.1f; /*The amount of seconds to crossfade to the captured animation.*/

        public SpiritCapturedState(SpiritStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter() /*CAlled when entering the state. Subscribes to the onCageOpened event. Crossfades to the captured animation.*/
        {
            stateMachine.Agent.enabled = false;
            stateMachine.Cage.onCageOpened += HandleCageOpened;

            stateMachine.Animator.CrossFadeInFixedTime(CapturedHash, CrossFadeDuration);
        }

        public override void Tick(float deltaTime) /*Called every FixedUpdate. This function is empty.*/
        {
        }

        public override void Exit() /*Called when exitinng the state. Unsubscribes from the onCageOpened event.*/
        {
            stateMachine.Cage.onCageOpened -= HandleCageOpened;
        }

        private void HandleCageOpened() /*When the cage is opened, switch to SpiritFollowPlayerState.*/
        {
            stateMachine.SwitchState(new SpiritFollowPlayerState(stateMachine));
        }
    }
}