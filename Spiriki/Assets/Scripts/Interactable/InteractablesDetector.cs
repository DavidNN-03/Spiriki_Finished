using UnityEngine;
using Spiriki.Interaction;

namespace Spiriki.Control
{
    public class InteractablesDetector : MonoBehaviour /*This class keeps track of all the interactables within the GameObjects collider.*/
    {
        [SerializeField] private PlayerStateMachine stateMachine; /*Reference to the PlayerStateMachine the instance belongs to.*/

        private void OnTriggerEnter(Collider other) /*When this GameObject's collider collides with another GameObject's, check if the other GameObject is an interactable. If so, add it to stateMachine.Interactables.*/
        {
            if (!other.TryGetComponent<Interactable>(out Interactable interactable)) return;

            if (!interactable.IsInteractable) return;

            stateMachine.Interactables.Add(interactable);
        }

        private void OnTriggerExit(Collider other) /*When a collider leaves this GameObject's collider, if the other GameObject is an interactable, remove it from stateMachine.Interactables.*/
        {
            if (!other.TryGetComponent<Interactable>(out Interactable interactable)) return;

            interactable.ActivateVisual(false);
            stateMachine.Interactables.Remove(interactable);
        }
    }
}
