using UnityEngine;

namespace Spiriki.Control
{
    public abstract class StateMachine : MonoBehaviour /*The parent-class of all statemachines.*/
    {
        private State currentState; /*The current state of the statemachine.*/

        private void FixedUpdate() /*If currentState is not null, call currentState.Tick().*/
        {
            currentState?.Tick(Time.deltaTime);
        }

        public void SwitchState(State newState) /*Switch to a new given state.*/
        {
            currentState?.Exit();

            currentState = newState;

            currentState?.Enter();
        }
    }
}