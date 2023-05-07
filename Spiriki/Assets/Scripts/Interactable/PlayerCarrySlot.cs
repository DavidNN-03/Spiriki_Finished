using UnityEngine;

namespace Spiriki.Interaction
{
    public class PlayerCarrySlot : MonoBehaviour /*This calss let's the player carry interactables.*/
    {
        [field: SerializeField] public Transform CarryTransform { get; set; } /*Transform the interactable will be set as child of.*/
        public Interactable CurrentInteractable { get; set; } /*Reference to the currently carried interactable.*/

        public bool GetHasInteractable() /*Returns whether the player is currently carrying an interactable.*/
        {
            return CurrentInteractable != null;
        }
    }
}