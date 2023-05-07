using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Spiriki.SceneHandling
{
    public class SceneHandler : MonoBehaviour /*This class is responsible for switching scenes.*/
    {
        [SerializeField] private float fadeOutTime = 1f; /*Amount of seconds it takes to fade to black.*/
        [SerializeField] private float fadeInTime = 2f; /*Amount of seconds it takes to fade back in.*/
        [SerializeField] private float startFadeWaitTime = 0.5f; /*Amount of seconds before the screen starts to fade out,*/
        [SerializeField] private float fadeWaitTime = 0.5f; /*The amount of seconds the screen remains black.*/
        [SerializeField] private CanvasGroup canvasGroup; /*Reference to the CanvasGroup used to fade in and out.*/
        private Coroutine currentActiveFade = null; /*The current active fade in or out.*/
        public event Action onSceneLoaded; /*This event is invoked when a new scene is loaded.*/
        public static SceneHandler instance; /*This class is a singleton. This variable is a reference to the correct instance. All other instances will be destroyed.*/
        private bool isLoadingScene; /*If a new scene is currently being loaded.*/

        private void Awake() /*Destroy this instance, if it is not the first instance. Hide the CanvasGroup.*/
        {
            if (instance == null)
            {
                instance = this;
            }

            if (instance == this)
            {
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            canvasGroup.alpha = 0;
            isLoadingScene = false;
        }

        public void LoadScene(int index) /*If the given build-index is valid, start Transition.*/
        {
            if (index < 0)
            {
                Debug.LogError("Scene index must be a positive integer!");
                return;
            }
            if (isLoadingScene)
            {
                Debug.LogError("Cannot load new scene while already loading new scene");
                return;
            }

            StartCoroutine(Transition(index));
        }

        public void ReloadScene() /*Reload the current scene.*/
        {
            LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private IEnumerator Transition(int index) /*Fade out, load new scene, and fade back in.*/
        {
            isLoadingScene = true;

            yield return new WaitForSeconds(startFadeWaitTime);

            yield return FadeOut(fadeOutTime);

            yield return SceneManager.LoadSceneAsync(index);

            yield return new WaitForSeconds(fadeWaitTime);

            FadeIn(fadeInTime);

            currentActiveFade = null;
            isLoadingScene = false;
            onSceneLoaded?.Invoke();
        }

        private Coroutine FadeOut(float time) /*Calls Fade to fade out.*/
        {
            return Fade(1, 0, FindObjectsOfType<AudioSource>(), time);
        }

        private Coroutine FadeIn(float time) /*Calls Fade to fade in.*/
        {
            return Fade(0, 1, FindObjectsOfType<AudioSource>(), time);
        }

        private Coroutine Fade(float alphaTarget, float audioTarget, AudioSource[] audioSources, float time) /*If there is already a fade happening, stop it. Start FadeRoutine.*/
        {
            if (currentActiveFade != null)
            {
                StopCoroutine(currentActiveFade);
            }

            currentActiveFade = StartCoroutine(FadeRoutine(alphaTarget, audioTarget, audioSources, time));
            return currentActiveFade;
        }

        private IEnumerator FadeRoutine(float alphaTarget, float audioTarget, AudioSource[] audioSources, float time) /*Moves CanvasGroup's alpha and the given AudioSources' volume toward given values in a given amount of time.*/
        {
            while (!Mathf.Approximately(canvasGroup.alpha, alphaTarget))
            {
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, alphaTarget, Time.deltaTime / time);
                foreach (AudioSource audioSource in audioSources)
                {
                    audioSource.volume = Mathf.MoveTowards(audioSource.volume, audioTarget, Time.deltaTime / time);
                }
                yield return null;
            }

            canvasGroup.alpha = alphaTarget;
            foreach (AudioSource audioSource in audioSources)
            {
                audioSource.volume = audioTarget;
            }
        }
    }
}