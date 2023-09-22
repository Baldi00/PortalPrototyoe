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

        portal1.FrontPortal.material.mainTexture = portalRenderTexture1;
        portal1.BehindPortal.material.mainTexture = portalRenderTexture1;
        portal2.FrontPortal.material.mainTexture = portalRenderTexture2;
        portal2.BehindPortal.material.mainTexture = portalRenderTexture2;

        portalCamera = GetComponent<Camera>();
        mainCamera = Camera.main;

        textureColorer1 = new Material(textureColorer);
        textureColorer2 = new Material(textureColorer);

        textureColorer1.color = portal1.Color;
        textureColorer2.color = portal2.Color;
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
        RenderPortal(portal1, portal2, portalRenderTexture1, textureColorer1, SRC);
        RenderPortal(portal2, portal1, portalRenderTexture2, textureColorer2, SRC);
    }

    private void RenderPortal(Portal inPortal, Portal outPortal, RenderTexture portalRenderTexture,
        Material textureColorer, ScriptableRenderContext SRC)
    {
        portalRenderTexture.Release();
        portalCamera.targetTexture = portalRenderTexture;

        // Behind color
        Graphics.Blit(texture, portalRenderTexture, textureColorer, 0);
        
        for (int i = iterations - 1; i >= 0; i--)
        {
            Transform cameraTransform = portalCamera.transform;
            cameraTransform.SetPositionAndRotation(mainCamera.transform.position, mainCamera.transform.rotation);

            for (int j = 0; j <= i; j++)
            {
                // Position the camera behind the other portal.
                Vector3 relativePos = inPortal.transform.InverseTransformPoint(cameraTransform.position);
                relativePos = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativePos;
                cameraTransform.position = outPortal.transform.TransformPoint(relativePos);

                // Rotate the camera to look through the other portal.
                Quaternion relativeRot = Quaternion.Inverse(inPortal.transform.rotation) * cameraTransform.rotation;
                relativeRot = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativeRot;
                cameraTransform.rotation = outPortal.transform.rotation * relativeRot;
            }

            // Set the camera's oblique view frustum.
            var p = new Plane(-outPortal.transform.forward, outPortal.transform.position);
            var clipPlaneWorldSpace = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
            var clipPlaneCameraSpace = 
                Matrix4x4.Transpose(Matrix4x4.Inverse(portalCamera.worldToCameraMatrix)) * clipPlaneWorldSpace;

            var newMatrix = mainCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
            portalCamera.projectionMatrix = newMatrix;

            UniversalRenderPipeline.RenderSingleCamera(SRC, portalCamera);
        }
    }
}
