using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPortal : MonoBehaviour
{

    public GameObject portal1;
    public GameObject portal2;
    public GameObject mainCamera;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private void Start()
    {
        GameObject portalCopy = new GameObject();
        portalCopy.transform.SetPositionAndRotation(portal1.transform.position, portal1.transform.rotation);

        GameObject cameraCopy = Instantiate(portalCopy, mainCamera.transform.position, mainCamera.transform.rotation, portalCopy.transform);

        portalCopy.transform.SetPositionAndRotation(portal2.transform.position, portal2.transform.rotation);
        Vector3 rotationPoint = portalCopy.transform.position;
        portalCopy.transform.RotateAround(rotationPoint, Vector3.up, 180);
        initialPosition = cameraCopy.transform.position;
        initialRotation = cameraCopy.transform.rotation;
        transform.SetPositionAndRotation(initialPosition, initialRotation);
        Destroy(portalCopy);
    }

    void Update()
    {
        transform.SetPositionAndRotation(
            mainCamera.transform.position + initialPosition, 
            mainCamera.transform.rotation * initialRotation);
    }
}
