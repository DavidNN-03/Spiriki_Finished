using UnityEngine;

namespace Spiriki.Objective
{
    public class ObjectiveCompletionArea : MonoBehaviour /*This class manages the ObjectiveCompletionArea. The player wins, if an Objective reaches an ObjectiveCompletionArea.*/
    {
        [SerializeField] private GameObject visuals; /*The GameObject that serves as the visuals of the ObjectiveCompletionArea.*/

        private void Start() /*Hide the visuals.*/
        {
            ActivateVisuals(false);
        }

        public void ActivateVisuals(bool shouldBeActive) /*Activates or disactivates the visuals.*/
        {
            visuals.SetActive(shouldBeActive);
        }
    }
}