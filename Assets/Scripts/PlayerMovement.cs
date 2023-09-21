using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
	public const float MIN_MOVEMENT_THRESHOLD = 0.1f;

	[SerializeField]
	private float cameraHorizontalSpeed = 5f;
	[SerializeField]
	private float cameraVerticalSpeed = 1f;
	[SerializeField]
	private float maxCameraAngle = 45f;
	[SerializeField]
	private float minCameraAngle = -45f;
	[SerializeField]
	private float walkingSpeed = 3;

	private Camera cam;
	private CharacterController characterController;

	private float currentCameraXRotation = 0.0f;
	private float currentCameraYRotation = 0.0f;
	private float playerPositionHorizontalShift = 0.0f;
	private float playerPositionVerticalShift = 0.0f;

	void Awake()
	{
		cam = Camera.main;
		characterController = GetComponent<CharacterController>();
	}

	void Update()
	{
		SetNextCameraRotation(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        SetNextPlayerPosition(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
		UpdateCameraRotation();
		UpdatePlayerPosition();
	}

	/// <summary>
	/// Sets the next camera rotation based on the player inputs
	/// </summary>
	/// <param name="mouseXInput">The x coordinate of the mouse position [-1, 1]</param>
	/// <param name="mouseYInput">The y coordinate of the mouse position [-1, 1]</param>
	public void SetNextCameraRotation(float mouseXInput, float mouseYInput)
	{
		currentCameraYRotation += mouseXInput * cameraHorizontalSpeed;
		currentCameraXRotation -= mouseYInput * cameraVerticalSpeed;
		currentCameraXRotation = Mathf.Clamp(currentCameraXRotation, minCameraAngle, maxCameraAngle);
	}

	/// <summary>
	/// Sets the next player position based on the player inputs
	/// </summary>
	/// <param name="horizontalMovement">The horizontal shift [-1, 1]</param>
	/// <param name="verticalMovement">The vertical shift [-1, 1]</param>
	public void SetNextPlayerPosition(float horizontalMovement, float verticalMovement)
	{
		playerPositionHorizontalShift = horizontalMovement * walkingSpeed;
		playerPositionVerticalShift = verticalMovement * walkingSpeed;
	}

	/// <summary>
	/// Updates the player position based on the set values from the input manager
	/// </summary>
	private void UpdatePlayerPosition()
	{
		characterController.Move(
			(transform.right * playerPositionHorizontalShift +
			transform.forward * playerPositionVerticalShift +
			Physics.gravity) * Time.deltaTime);
	}

	/// <summary>
	/// Updates the camera rotation based on the set values from the input manager
	/// </summary>
	private void UpdateCameraRotation()
	{
		transform.eulerAngles = new Vector3(0, currentCameraYRotation, 0.0f);
		cam.transform.eulerAngles = new Vector3(currentCameraXRotation, currentCameraYRotation, 0.0f);
	}
}