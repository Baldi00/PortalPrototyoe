using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PortalCamera : MonoBehaviour
{
    [SerializeField]
    private Portal portal1;
    [SerializeField]
    private Portal portal2;
    [SerializeField]
    private int iterations = 2;
    [SerializeField]
    private Material textureColorer;
    [SerializeField]
    private Color portal1Color;
    [SerializeField]
    private Color portal2Color;

    private RenderTexture portalRenderTexture1;
    private RenderTexture portalRenderTexture2;
    private Camera mainCamera;
    private Camera portalCamera;
    
    private Texture texture;
    private Material textureColorer1;
    private Material textureColorer2;

    void Awake()
    {
        portalRenderTexture1 = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
        portalRenderTexture2 = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
        portal1.Renderer.material.mainTexture = portalRenderTexture1;
        portal2.Renderer.material.mainTexture = portalRenderTexture2;
        portalCamera = GetComponent<Camera>();
        mainCamera = Camera.main;

        textureColorer1 = new Material(textureColorer);
        textureColorer2 = new Material(textureColorer);

        textureColorer1.color = portal1Color;
        textureColorer2.color = portal2Color;
    }

    void OnEnable()
    {
        RenderPipelineManager.beginCameraRendering += RenderCamera;
    }

    void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= RenderCamera;
    }

    private void RenderCamera(ScriptableRenderContext SRC, Camera camera)
    {
        portalRenderTexture1.Release();

        Graphics.Blit(texture, portalRenderTexture1, textureColorer1, 0);

        portalCamera.targetTexture = portalRenderTexture1;
        for (int i = iterations - 1; i >= 0; i--)
        {
            Transform cameraTransform = portalCamera.transform;
            cameraTransform.SetPositionAndRotation(mainCamera.transform.position, mainCamera.transform.rotation);

            for (int j = 0; j <= i; j++)
            {
                // Position the camera behind the other portal.
                Vector3 relativePos = portal1.transform.InverseTransformPoint(cameraTransform.position);
                relativePos = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativePos;
                cameraTransform.position = portal2.transform.TransformPoint(relativePos);

                // Rotate the camera to look through the other portal.
                Quaternion relativeRot = Quaternion.Inverse(portal1.transform.rotation) * cameraTransform.rotation;
                relativeRot = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativeRot;
                cameraTransform.rotation = portal2.transform.rotation * relativeRot;
            }

            // Set the camera's oblique view frustum.
            Plane p = new Plane(-portal2.transform.forward, portal2.transform.position);
            Vector4 clipPlaneWorldSpace = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
            Vector4 clipPlaneCameraSpace =
                Matrix4x4.Transpose(Matrix4x4.Inverse(portalCamera.worldToCameraMatrix)) * clipPlaneWorldSpace;

            var newMatrix = mainCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
            portalCamera.projectionMatrix = newMatrix;

            UniversalRenderPipeline.RenderSingleCamera(SRC, portalCamera);
        }

        //textureColorer2.color = portal2Color;
        //Graphics.Blit(texture, portalRenderTexture2, textureColorer2);

        portalRenderTexture2.Release();
        Graphics.Blit(texture, portalRenderTexture2, textureColorer2);
        portalCamera.targetTexture = portalRenderTexture2;
        for (int i = iterations - 1; i >= 0; i--)
        {
            Transform cameraTransform = portalCamera.transform;
            cameraTransform.SetPositionAndRotation(mainCamera.transform.position, mainCamera.transform.rotation);

            for (int j = 0; j <= i; j++)
            {
                // Position the camera behind the other portal.
                Vector3 relativePos = portal2.transform.InverseTransformPoint(cameraTransform.position);
                relativePos = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativePos;
                cameraTransform.position = portal1.transform.TransformPoint(relativePos);

                // Rotate the camera to look through the other portal.
                Quaternion relativeRot = Quaternion.Inverse(portal2.transform.rotation) * cameraTransform.rotation;
                relativeRot = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativeRot;
                cameraTransform.rotation = portal1.transform.rotation * relativeRot;
            }

            // Set the camera's oblique view frustum.
            Plane p = new Plane(-portal1.transform.forward, portal1.transform.position);
            Vector4 clipPlaneWorldSpace = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
            Vector4 clipPlaneCameraSpace =
                Matrix4x4.Transpose(Matrix4x4.Inverse(portalCamera.worldToCameraMatrix)) * clipPlaneWorldSpace;

            var newMatrix = mainCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
            portalCamera.projectionMatrix = newMatrix;

            UniversalRenderPipeline.RenderSingleCamera(SRC, portalCamera);
        }
    }
}
