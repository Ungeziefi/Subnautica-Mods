using UnityEngine;

namespace Ungeziefi.Cuddlefish_Renamer;

public class FaceCamera : MonoBehaviour
{
    private Transform cameraTransform;

    private void Start()
    {
        cameraTransform = MainCamera.camera.transform;
    }

    private void LateUpdate()
    {
        if (cameraTransform)
            transform.rotation = Quaternion.LookRotation(transform.position - cameraTransform.position);
    }
}