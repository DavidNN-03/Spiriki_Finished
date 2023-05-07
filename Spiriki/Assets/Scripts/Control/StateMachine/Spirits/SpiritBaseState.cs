using UnityEngine;

namespace Spiriki.Control
{
    public abstract class SpiritBaseState : State /*This is the spirit base-state. All spirit states inherit from this class.*/
    {
        protected SpiritStateMachine stateMachine; /*The statemachine of the spirit. The states need this variabel, since otherwise, they would not have access to any GameObjects or components in Unity.*/

        public SpiritBaseState(SpiritStateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }
    }
}