using UnityEngine;

namespace Ungeziefi.Creature_Healthbars
{
    public class FaceCamera : MonoBehaviour
    {
        private Transform cameraTransform;

        void Start() => cameraTransform = MainCamera.camera.transform;

        void LateUpdate()
        {
            if (cameraTransform != null)
                transform.rotation = Quaternion.LookRotation(transform.position - cameraTransform.position);
        }
    }
}