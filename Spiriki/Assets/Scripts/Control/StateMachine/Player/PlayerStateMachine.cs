using System.Collections.Generic;
using UnityEngine;
using Spiriki.Interaction;

namespace Spiriki.Control
{
    public class PlayerStateMachine : StateMachine /*Statemachine of the player. This class stores the variables that all the player states use.*/
    {
        [field: SerializeField] public float FreeLookMoveSpeed { get; private set; } /*Player speed when walking in PlayerFreeLookState.*/
        [field: SerializeField] public float FreeLookSprintSpeed { get; private set; } /*Player speed when sprinting in PlayerFreeLookState.*/
        [field: SerializeField] public float SneakSpeed { get; private set; } /*Player speed when sneaking in PlayerSneakState.*/
        [field: SerializeField] public float JumpForce { get; private set; } /*Upwards velocity applied to the player when jumping.*/
        [field: SerializeField] public float RotationSpeed { get; private set; } /*Speed at which the player rotates.*/
        [field: SerializeField] public float stealthDetectionRateMultiplier { get; private set; } /*Multiplier for how quickly the player gets spotted when sneaking.*/
        [field: SerializeField] private ChooseInteractableMethod chooseInteractableMethod = ChooseInteractableMethod.ClosestToScreenCenter; /*Defines how the statemachine should select CurrentInteractable.*/
        [field: Header("Landing")]
        [field: SerializeField] public Transform groundCheckOriginPoint { get; private set; } /*Transform point from which the player does ground-checking.*/
        [field: SerializeField] public Vector3 halfExtends { get; private set; } /*Half-extends of the box for boxcasting.*/
        [field: SerializeField] public float groundCheckDistance { get; private set; } /*Distance to the ground within which the player is grounded.*/
        [field: SerializeField] public float shouldLandCheckDistance { get; private set; } /*The distance at which the player starts landing.*/
        [field: SerializeField] public LayerMask groundCheckLayerMask { get; private set; } /*The layers the player should check for when ground-checking.*/
        public Camera MainCamera { get; private set; } /*Reference to the main camera.*/
        public InteractionTrigger InteractionTrigger { get; private set; } /*Player's interaction trigger. Sits on the player visuals GameObject. Allows for the PlayerStateMachine.Interact function to be called on animation-events.*/
        public PlayerCarrySlot PlayerCarrySlot { get; private set; } /*Player's PlayerCarrySlot. Allows the player to carry an object.*/
        public Rigidbody RB { get; private set; } /*Player Rigidbody.*/
        public CharacterController CharacterController { get; private set; } /*Player CharacterController.*/
        public Animator Animator { get; private set; } /*Player animator.*/
        public InputReader InputReader { get; private set; } /*PLayer InputReader.*/
        public List<Interactable> Interactables { get; private set; } = new List<Interactable>(); /*List of all interactables in the scene.*/
        public Interactable CurrentInteractable { get; private set; } /*Current interactable. This Interactable will be interacted with, if the player presses to interact.*/
        public bool IsGrounded { get; set; } /*Is the player grounded.*/
        public bool IsSneaking { get; set; } /*Is the player sneaking*/
        public float verticalVelocity; /*Player's vertical velocity.*/

        private void Awake() /*Assigns references to Unity components.*/
        {
            Animator = GetComponentInChildren<Animator>();
            CharacterController = GetComponent<CharacterController>();
            RB = GetComponent<Rigidbody>();
            InputReader = GetComponent<InputReader>();
            InteractionTrigger = GetComponentInChildren<InteractionTrigger>();
            PlayerCarrySlot = GetComponentInChildren<PlayerCarrySlot>();
        }

        private void Start() /*Hides the cursor, subscribes to OnInteract event, and switches to PlayerFreeLookState.*/
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            MainCamera = Camera.main;

            InteractionTrigger.OnInteract += Interact;

            SwitchState(new PlayerFreeLookState(this));
        }

        public void Interact() /*If CurrentInteractable is not null, interact with it.*/
        {
            CurrentInteractable?.Interact();
            Interactables.Remove(CurrentInteractable);
            CurrentInteractable = null;
        }

        public void UpdateCurrentInteractable() /*Calculate the value of CurrentInteractable based on chooseInteractableMethod and the interactables' positions.*/
        {
            if (Interactables.Count == 0)
            {
                CurrentInteractable = null;
                return;
            }

            Interactable closestInteractable = null;
            float closestInteractableDistanceSqr = Mathf.Infinity;

            if (chooseInteractableMethod == ChooseInteractableMethod.ClosestToPlayer)
            {
                foreach (Interactable interactable in Interactables)
                {
                    float sqrMagnitude = (interactable.GetRendererPos() - transform.position).sqrMagnitude;
                    if (sqrMagnitude < closestInteractableDistanceSqr)
                    {
                        closestInteractable = interactable;
                        closestInteractableDistanceSqr = sqrMagnitude;
                    }
                }
            }
            else if (chooseInteractableMethod == ChooseInteractableMethod.ClosestToScreenCenter)
            {
                foreach (Interactable interactable in Interactables)
                {
                    if (!interactable.GetRenderer().isVisible)
                    {
                        continue;
                    }

                    Vector2 viewPos = MainCamera.WorldToViewportPoint(interactable.GetRendererPos());
                    Vector2 toCenter = viewPos - new Vector2(0.5f, 0.5f);

                    if (toCenter.sqrMagnitude < closestInteractableDistanceSqr)
                    {
                        closestInteractable = interactable;
                        closestInteractableDistanceSqr = toCenter.sqrMagnitude;
                    }
                }
            }

            //because of the continue statement above, the old or new value could be null
            //if the Instances list only contains renderers out of sight
            if (CurrentInteractable != closestInteractable)
            {
                CurrentInteractable?.ActivateVisual(false);
                CurrentInteractable = closestInteractable;
                CurrentInteractable?.ActivateVisual(true);
            }
        }

        public void CheckGroundStatus() /*Checks if the player is grounded.*/
        {
            IsGrounded = Physics.BoxCast(groundCheckOriginPoint.position, halfExtends, Vector3.down, out RaycastHit hit, Quaternion.identity, groundCheckDistance, groundCheckLayerMask);
        }

        public void ApplyGravity() /*Applies gravity to the player.*/
        {
            if (verticalVelocity < 0f && IsGrounded)
            {
                verticalVelocity = Physics.gravity.y;
            }
            else
            {
                verticalVelocity += Physics.gravity.y * Time.deltaTime;
            }
        }

        public bool GetShouldLand() /*Checks if the player should start landing from a fall.*/
        {
            return Physics.BoxCast(groundCheckOriginPoint.position, halfExtends, Vector3.down, out RaycastHit hit, Quaternion.identity, shouldLandCheckDistance, groundCheckLayerMask);
        }

        public float GetSpottingSpeedMultiplier() /*Returns how quickly the player should be spotted, based on whether or not the player is sneaking.*/
        {
            if (IsSneaking)
            {
                return stealthDetectionRateMultiplier;
            }

            return 1f;
        }

        private void OnDrawGizmos() /*Draw gizmos to help visualize ground-checking.*/
        {
            Gizmos.color = Color.red;

            Vector3 center = groundCheckOriginPoint.position + Vector3.down * groundCheckDistance / 2;
            Vector3 size = new Vector3(halfExtends.x * 2, halfExtends.y * 2 + groundCheckDistance, halfExtends.z * 2);

            Gizmos.DrawWireCube(center, size);

            Gizmos.color = Color.blue;

            center = groundCheckOriginPoint.position + Vector3.down * shouldLandCheckDistance / 2;
            size = new Vector3(halfExtends.x * 2, halfExtends.y * 2 + shouldLandCheckDistance, halfExtends.z * 2);

            Gizmos.DrawWireCube(center, size);
        }
    }
}