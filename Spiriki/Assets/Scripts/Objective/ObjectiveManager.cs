using UnityEngine;
using Spiriki.SceneHandling;

namespace Spiriki.Objective
{
    public class ObjectiveManager : MonoBehaviour /*Manages the games objective.*/
    {
        [SerializeField] private int nextSceneIndex; /*Index of the scene that should be loaded if the player wins.*/
        [SerializeField] private PlayerSpottedReaction playerSpottedReaction; /*What should happen if the player is spotted.*/
        [SerializeField] private int playerSpottedLoadSceneIndex; /*Index of the scene that should be loaded, if the player is spotted, and playerSpottedReaction is PlayerSpottedReaction.ReloadScene.*/
        public ObjectiveCompletionArea[] CompletionAreas { get; private set; } /*Array of all ObjectiveCompletionAreas in scene.*/

        private void Start() /*Finds all ObjectiveCompletionAreas in scene.*/
        {
            CompletionAreas = FindObjectsOfType<ObjectiveCompletionArea>();
        }

        public void ActivateAreaVisuals(bool shouldBeActive) /*Activates the visuals of all ObjectiveCompletionAreas.*/
        {
            foreach (ObjectiveCompletionArea area in CompletionAreas)
            {
                area.ActivateVisuals(shouldBeActive);
            }
        }

        public void Win() /*If the player wins, load next scene.*/
        {
            SceneHandler.instance.LoadScene(nextSceneIndex);
        }

        public void Lose() /*If the player loses, either reload the current scene, or load a given scene.*/
        {
            if (playerSpottedReaction == PlayerSpottedReaction.ReloadScene)
            {
                SceneHandler.instance.ReloadScene();
            }
            else if (playerSpottedReaction == PlayerSpottedReaction.LoadScene)
            {
                SceneHandler.instance.LoadScene(playerSpottedLoadSceneIndex);
            }
        }
    }
}