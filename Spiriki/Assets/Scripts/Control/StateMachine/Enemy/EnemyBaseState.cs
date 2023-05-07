using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spiriki.Control
{
    public abstract class EnemyBaseState : State /*This is the enemy base-state. All enemy states inherit from this class.*/
    {
        protected EnemyStateMachine stateMachine; /*The statemachine of the enemy. The states need this variabel, since otherwise, they would not have access to any GameObjects or components in Unity.*/

        public EnemyBaseState(EnemyStateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }
    }
}