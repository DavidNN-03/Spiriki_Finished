using UnityEngine;

namespace Spiriki.Objective
{
    public class Objective : MonoBehaviour /*This class is the objective. The player wins, if the GameObject reaches a ObjectiveCompletionArea.*/
    {
        private void OnTriggerEnter(Collider other) /*When this GameObject's collider collides with another collider, if it is an ObjectiveCompletionArea, the player wins.*/
        {
            if (other.TryGetComponent<ObjectiveCompletionArea>(out ObjectiveCompletionArea objectiveCompletionArea))
            {
                FindObjectOfType<ObjectiveManager>().Win();
            }
        }
    }
}