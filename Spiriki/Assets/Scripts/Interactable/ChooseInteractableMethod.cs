namespace Spiriki.Interaction
{
    public enum ChooseInteractableMethod /*Defines how the PlayerStateMachine should decide the value of CurrentInteractable.*/
    {
        ClosestToPlayer, /*Select interactable closest to the player.*/
        ClosestToScreenCenter /*Select interactable closest to the center of the screen.*/
    }
}