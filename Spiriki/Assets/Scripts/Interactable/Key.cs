using UnityEngine;

namespace Spiriki.Interaction
{
    public class Key : Interactable /*Key that lets the player open the cage.*/
    {
        [SerializeField] private AudioSource audioSource; /*Reference to the key's AudioSource.*/

        public override void Interact() /*If the player is not already carrying something, play SFX, teleport to the player's mouth, and disable the UI.*/
        {
            PlayerCarrySlot carrySlot = FindObjectOfType<PlayerCarrySlot>();

            if (carrySlot.GetHasInteractable()) return;

            audioSource.Play();
            carrySlot.CurrentInteractable = this;
            transform.parent = carrySlot.CarryTransform;
            transform.localPosition = Vector3.zero;
            transform.forward = transform.parent.forward;

            ActivateVisual(false);
        }
    }
}