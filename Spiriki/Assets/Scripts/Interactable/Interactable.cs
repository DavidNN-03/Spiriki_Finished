using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spiriki.Interaction
{
    public abstract class Interactable : MonoBehaviour /*Parent-class of all interactables.*/
    {
        [SerializeField] private string playerInteractionAnimationName; /*Name of the animation that the player character should play when interacting.*/
        [SerializeField] private Renderer interactableRenderer; /*The interactable GameObject's renderer.*/
        [SerializeField] private GameObject uIParent; /*Parent of the interactable's UI.*/
        [SerializeField] private Transform playerPosition; /*Where the player should be standing when interacting.*/
        [SerializeField] public bool requiresKey = false; /*Whether interacting with this interactable requires a key.*/
        public bool IsInteractable { get; set; } = true; /*Whether or not this interactable can currently be interacted with.*/

        private void Start() /*Disable the UI.*/
        {
            ActivateVisual(false);
        }

        public void ActivateVisual(bool isActive) /*Enables or disables the UI.*/
        {
            uIParent.SetActive(isActive);
        }

        public Transform GetPlayerTargetTransform() /*Returns the position, the player should be at when interacting.*/
        {
            return playerPosition;
        }

        public Renderer GetRenderer() /*Returns the interactables renderer.*/
        {
            return interactableRenderer;
        }

        public Vector3 GetRendererPos() /*Returns position of the interactable's renderer.*/
        {
            return interactableRenderer.gameObject.transform.position;
        }

        public string GetAnimationName() /*Returns the name of the animation the player should play when interacting.*/
        {
            return playerInteractionAnimationName;
        }

        public bool GetRequiresKey() /*Returns whether the player needs a key to interact with this interactable.*/
        {
            return requiresKey;
        }

        public abstract void Interact(); /*Abstract function for when interacting with interactable.*/
    }
}