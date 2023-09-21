using UnityEngine;

public class PortalCamera : MonoBehaviour
{
    [SerializeField]
    private Portal portal1;
    [SerializeField]
    private Portal portal2;

    private RenderTexture portalRenderTexture;
    private Camera mainCamera;
    private Camera portalCamera;

    void Awake()
    {
        portalRenderTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
        portal1.Renderer.material.mainTexture = portalRenderTexture;
        portalCamera = GetComponent<Camera>();
        portalCamera.targetTexture = portalRenderTexture;
        mainCamera = Camera.main;
    }

    void Update()
    {
        Transform cameraTransform = portalCamera.transform;

        // Position the camera behind the other portal.
        Vector3 relativePos = portal1.transform.InverseTransformPoint(mainCamera.transform.position);
        relativePos = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativePos;
        cameraTransform.position = portal2.transform.TransformPoint(relativePos);

        // Rotate the camera to look through the other portal.
        Quaternion relativeRot = Quaternion.Inverse(portal1.transform.rotation) * mainCamera.transform.rotation;
        relativeRot = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativeRot;
        cameraTransform.rotation = portal2.transform.rotation * relativeRot;

        // Set the camera's oblique view frustum.
        Plane p = new Plane(-portal2.transform.forward, portal2.transform.position);
        Vector4 clipPlaneWorldSpace = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
        Vector4 clipPlaneCameraSpace =
            Matrix4x4.Transpose(Matrix4x4.Inverse(portalCamera.worldToCameraMatrix)) * clipPlaneWorldSpace;

        var newMatrix = mainCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
        portalCamera.projectionMatrix = newMatrix;
    }
}
