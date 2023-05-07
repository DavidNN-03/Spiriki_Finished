using Spiriki.SceneHandling;
using UnityEngine;

namespace Spiriki.UI
{
    public class MainMenu : MonoBehaviour /*Class that manages the main menu.*/
    {
        private void Start() /*Show the cursor.*/
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        //Unity event on buttons in main menu scene
        public void LoadScene(int index) /*This functon is called by a Unity-event on a button in the main menu.*/
        {
            SceneHandler.instance.LoadScene(index);
        }
    }
}