using UnityEngine;
using UnityEngine.InputSystem;

public class JoystickCameraRotator : MonoBehaviour, IJoyStickCameraRotator
{
    public Transform followTarget;
    [SerializeField] Transform cameraTransform;
    [SerializeField] float minRotationAroundX = -80;
    [SerializeField] float maxRotationAroundX = 80;
    [SerializeField] float keyboardRotationSpeed = 15f;
    [SerializeField] float gamepadRotationSpeed = 300f;

    public float rotationAroundX { private get; set; }
    public float rotationAroundY { private get; set; }
    LockOnRotator lockOnRotator;
    bool rotationEnabled = true;
    float rotationSpeedMultiplier = 1;

    private void Start()
    {
        lockOnRotator = this.GetComponent<LockOnRotator>(true);
        rotationAroundX = followTarget.rotation.x;
        rotationAroundY = followTarget.rotation.y;

        InputManager.editor.ToogleCameraRotator.performed += dummy => ToggleRotation();
    }



    private void Update()
    {
        if (!lockOnRotator.lockOn && rotationEnabled)
        {
            RotateByJoyStick(InputManager.gameplay.RotateCamera.ReadValue<Vector2>());
            RotateByJoyStick(InputManager.death.RotateCamera.ReadValue<Vector2>());
        }
        UpdateCursorState();
    }


    void RotateByJoyStick(Vector2 rotationInput)
    {
        if (rotationInput.sqrMagnitude == 0)
            return;

        float rotationSpeed = InputManager.isUsingKeyboard 
            ? keyboardRotationSpeed : gamepadRotationSpeed;
        float rotationIncrement = rotationSpeedMultiplier * rotationSpeed * Time.deltaTime;
        
        rotationAroundX -= rotationInput.y * rotationIncrement;
        rotationAroundY += rotationInput.x * rotationIncrement;
        rotationAroundX = Mathf.Clamp(rotationAroundX, minRotationAroundX, maxRotationAroundX);

        followTarget.localRotation = Quaternion.Euler(rotationAroundX, rotationAroundY, 0);
    }
    

    void ToggleRotation()
    {
        rotationEnabled = !rotationEnabled;
    }


    void UpdateCursorState()
    {
        bool keyboardAndUI = InputManager.isUsingKeyboard && InputManager.UI.enabled;
        bool locked = rotationEnabled && !keyboardAndUI;
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
    }

    public void SetRotationSpeedMultiplier(float multiplier)
    {
        rotationSpeedMultiplier = multiplier;
    }

}

