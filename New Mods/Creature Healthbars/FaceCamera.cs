using UnityEngine;

namespace Ungeziefi.Creature_Healthbars;

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