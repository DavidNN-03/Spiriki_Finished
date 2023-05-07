using System;
using UnityEngine;

namespace Spiriki.Interaction
{
    public class InteractionTrigger : MonoBehaviour /*This class is used as a messenger between the PlayerStateMachine and the player's animator. The PlayerStateMachine subscribes to the OnInteract event, which is invoked as a result of animation events.*/
    {
        public event Action OnInteract; /*This event is invoked from the player's interaction animations.*/

        //Animaton event
        public void Interact() /*Invokes OnInteract if it has subscribers.*/
        {
            OnInteract?.Invoke();
        }
    }
}