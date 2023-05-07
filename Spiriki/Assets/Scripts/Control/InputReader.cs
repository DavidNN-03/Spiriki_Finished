using UnityEngine;
using UnityEngine.InputSystem;
using System;

namespace Spiriki.Control
{
    public class InputReader : MonoBehaviour, Controls.IPlayerActions /*Handles input-related events.*/
    {
        public Vector2 MovementValue { get; private set; } /*How does the pplayer want to move. Based on WASD.*/
        public event Action InteractEvent; /*Event for interaction.*/
        public event Action JumpEvent; /*Event for jumping.*/
        public event Action<bool> SprintEvent;  /*Event for sprinting.*/
        public event Action<bool> SneakEvent; /*Event for sneaking.*/
        private Controls controls; /*Reference to the InputActions from the Unity InputSystem.*/

        private void Awake() /*Initialize and enable controls.*/
        {
            controls = new Controls();
            controls.Player.SetCallbacks(this);
            controls.Enable();
        }

        private void OnDestroy() /*Disable controls.*/
        {
            controls.Disable();
        }

        public void OnMove(InputAction.CallbackContext context) /*When the player wants to move, update MovementValue.*/
        {
            MovementValue = context.ReadValue<Vector2>();
        }

        public void OnLook(InputAction.CallbackContext context) { } /*When the player looks around. This function is empty.*/

        public void OnJump(InputAction.CallbackContext context) /*When the player wants to jump, invoke JumpEvent, if it has any subscribers.*/
        {
            {
                if (!context.performed) return;

                JumpEvent?.Invoke();
            }
        }

        public void OnInteract(InputAction.CallbackContext context) /*When the player wants to interact, invoke InteractEvent, if it has any subscribers.*/
        {
            if (!context.performed) return;

            InteractEvent?.Invoke();
        }

        public void OnSprint(InputAction.CallbackContext context) /*When the player wants to sprint, invoke SprintEvent, if it has any subscribers.*/
        {
            SprintEvent?.Invoke(context.performed);
        }

        public bool GetSprint() /*Returns if the player wants to sprint.*/
        {
            return controls.Player.Sprint.phase == InputActionPhase.Started;
        }

        public void OnSneak(InputAction.CallbackContext context) /*When the player wants to sneak, invoke SneakEvent, if it has any subscribers.*/
        {
            SneakEvent.Invoke(context.performed);
        }
    }
}