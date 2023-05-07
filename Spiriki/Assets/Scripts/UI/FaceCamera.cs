using UnityEngine;

namespace Spiriki.UI
{
    public class FaceCamera : MonoBehaviour
    {
        [SerializeField] private FaceCameraMode mode; /*How should the GameObject behave in relationship to the camera.*/

        private void LateUpdate() /*Rotate the GameObject based on the selected Mode.*/
        {
            switch (mode)
            {
                case FaceCameraMode.LookAtInverted:
                    Vector3 dirFromCamera = transform.position - Camera.main.transform.position;
                    transform.LookAt(transform.position + dirFromCamera);
                    break;
                case FaceCameraMode.CameraForward:
                    transform.forward = Camera.main.transform.forward;
                    break;
            }
        }
    }
}