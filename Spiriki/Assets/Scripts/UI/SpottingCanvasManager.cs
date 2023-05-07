using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Spiriki.UI
{
    public class SpottingCanvasManager : MonoBehaviour /*This class handles the UI showing if the player is being spotted.*/
    {
        [SerializeField] private float screenMargin; /*The margin of the markers to the screen.*/
        [SerializeField] private RectTransform canvas; /*RectTransform of the Canvas.*/
        [SerializeField] private Vector3 pointerOffset; /*The offset applied to the indicators.*/
        [SerializeField] private Image prefab; /*The prefab that is instantiated for every enemy.*/
        private List<GameObject> enemies; /*List of every enemy in the scene.*/
        private Dictionary<GameObject, Image> dict; /*Dictionary of all the enemies and their corresponding spotting-indicator.*/
        private Vector3 canvasCenter; /*Center point of the canvas.*/

        private void Start() /*Find all enemies in the scene, create and fill the dictionary, and calculate the center of the screen.*/
        {
            enemies = new List<GameObject>();
            dict = new Dictionary<GameObject, Image>();
            canvasCenter = new Vector3(canvas.rect.width / 2f, canvas.rect.height / 2f, 0f) * canvas.localScale.x;

            foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                enemies.Add(enemy);
                dict.Add(enemy, Instantiate(prefab, transform));
            }
        }

        public void UpdatePointer(GameObject enemy, float normalizedProgress, Color color) /*Update the indicator of a given enemy to a given value and color.*/
        {
            if (!dict.TryGetValue(enemy, out Image pointer)) return;

            if (normalizedProgress <= 0f)
            {
                pointer.gameObject.SetActive(false);
                return;
            }

            pointer.gameObject.SetActive(true);


            pointer.fillAmount = normalizedProgress;
            pointer.color = color;

            Vector3 pointerPos = Camera.main.WorldToScreenPoint(enemy.transform.position) + pointerOffset;

            if (pointerPos.x >= 0f && pointerPos.x <= canvas.rect.width * canvas.localScale.x &&
                            pointerPos.y >= 0f && pointerPos.y <= canvas.rect.height * canvas.localScale.x &&
                            pointerPos.z >= 0f) //if the pointer is naturally within the bounds of the screen.
            {
                pointerPos.z = 0f;

                pointerPos.x = Mathf.Max(pointerPos.x, screenMargin);
                pointerPos.x = Mathf.Min(pointerPos.x, canvas.rect.width * canvas.localScale.x - screenMargin);

                pointerPos.y = Mathf.Max(pointerPos.y, screenMargin);
                pointerPos.y = Mathf.Min(pointerPos.y, canvas.rect.height * canvas.localScale.x - screenMargin);
            }
            else if (pointerPos.z >= 0f) /*If the pointer is outside the screen.*/
            {
                pointerPos = OutOfRangePos(pointerPos);
            }
            else /*If the player is looking away from the enemy.*/
            {
                pointerPos *= -1f;
                pointerPos = OutOfRangePos(pointerPos);
            }

            pointer.transform.position = pointerPos;
        }

        private Vector3 OutOfRangePos(Vector3 pointerPos) /*Handles pointers outside the bounds of the screen.*/
        {
            pointerPos.z = 0f;

            pointerPos -= canvasCenter;

            float divX = (canvas.rect.width / 2f - screenMargin) / Mathf.Abs(pointerPos.x);
            float divY = (canvas.rect.height / 2f - screenMargin) / Mathf.Abs(pointerPos.y);

            if (divX < divY)
            {
                float angle = Vector3.SignedAngle(Vector3.right, pointerPos, Vector3.forward);
                pointerPos.x = Mathf.Sign(pointerPos.x) * (canvas.rect.width * 0.5f - screenMargin) * canvas.localScale.x;
                pointerPos.y = Mathf.Tan(Mathf.Deg2Rad * angle) * pointerPos.x;
            }

            else
            {
                float angle = Vector3.SignedAngle(Vector3.up, pointerPos, Vector3.forward);

                pointerPos.y = Mathf.Sign(pointerPos.y) * (canvas.rect.height / 2f - screenMargin) * canvas.localScale.y;
                pointerPos.x = -Mathf.Tan(Mathf.Deg2Rad * angle) * pointerPos.y;
            }

            pointerPos += canvasCenter;
            return pointerPos;
        }
    }
}