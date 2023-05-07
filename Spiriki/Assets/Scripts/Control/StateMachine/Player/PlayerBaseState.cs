using UnityEngine;

namespace Spiriki.Control
{
    public abstract class PlayerBaseState : State /*Player base-state. All player states inherit from this class.*/
    {
        protected PlayerStateMachine stateMachine; /*The statemachine of the player. The states need this variabel, since otherwise, they would not have access to any GameObjects or components in Unity.*/

        public PlayerBaseState(PlayerStateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        protected void Move(Vector3 inputVelocity, float deltaTime) /*Moves the player in a given direction by calling stateMachine.CharacterController.Move().*/
        {
            stateMachine.CharacterController.Move((inputVelocity + Vector3.up * stateMachine.verticalVelocity) * deltaTime);
        }

        protected void FaceMovementDirection(Vector3 movement, float deltaTime) /*Rotates the player to face the movement direction.*/
        {
            stateMachine.transform.rotation = Quaternion.Slerp(
                stateMachine.transform.rotation,
                Quaternion.LookRotation(movement),
                deltaTime * stateMachine.RotationSpeed);
        }
    }
}