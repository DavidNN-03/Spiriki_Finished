using System;
using UnityEngine;

namespace Spiriki.Interaction
{
    public class Cage : Interactable /*Controls the cage the spirit is locked in.*/
    {
        [SerializeField] private string animationName; /*Name of the animation that should play when opening the cage.*/
        [SerializeField] private Transform keyParentTransform; /*The transform that should be set as the parent of the key GameObject.*/
        public event Action onCageOpened; /*Event that will be invoked when the cage is opened.*/
        private Animator animator; /*Reference to the cage's animator.*/

        private void Awake() /*Get the animator.*/
        {
            animator = GetComponent<Animator>();
        }

        public override void Interact() /*If the player has a key, insert the key into the lock, play the opening animation, and disable the UI.*/
        {
            PlayerCarrySlot carrySlot = FindObjectOfType<PlayerCarrySlot>();

            if (!carrySlot.GetHasInteractable()) return;

            carrySlot.CurrentInteractable.IsInteractable = false;
            Transform keyTransform = carrySlot.CurrentInteractable.transform;

            keyTransform.parent = keyParentTransform;
            keyTransform.forward = keyParentTransform.forward;
            keyTransform.localPosition = Vector3.zero;

            carrySlot.CurrentInteractable = null;

            IsInteractable = false;
            animator.Play(animationName);

            ActivateVisual(false);
        }

        //animation event
        public void CageOpened() /*Invoke onCageOpened when an animation event is invoked.*/
        {
            onCageOpened?.Invoke();
        }
    }
}