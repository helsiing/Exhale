using DG.Tweening;
using Exhale.Scripts.External.ServiceLocators;
using Exhale.Scripts.Services;
using Exhale.Utils;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Exhale.Board
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Camera camera;
        
        [BoxGroup("Pan")]
        [SerializeField] private float panSpeed;
        [BoxGroup("Pan")]
        [SerializeField] private float mousePanSpeed;

        [BoxGroup("Zoom")]
        [SerializeField] private float zoomSpeed;
        [BoxGroup("Zoom")]
        [SerializeField] private float minFOV = 20f;
        [BoxGroup("Zoom")]
        [SerializeField] private float maxFOV = 80f;

        [BoxGroup("Rotate")]
        [SerializeField] private float rotationSpeed = 100f; 
        [BoxGroup("Rotate")]
        [SerializeField] private float maxTiltAngle = 75f;
        [BoxGroup("Rotate")]
        [SerializeField] private float minTiltAngle = 15f;
        
        private InputAction panInputKeyboard;
        private InputAction rotateInputMouse;
        private InputAction zoomInputMouse;
        private InputAction panInputMouse;
        private InputAction rotateInputKeyboard; // New action for Q/E rotation
        private CameraControlActions cameraActions;
        private Vector3 mouseVelocity = Vector3.zero;
        private readonly ServiceReference<ICameraService> cameraService = new ();


        private void Awake()
        {
            cameraActions = new CameraControlActions();
        }
        
        private void OnEnable()
        {
            cameraActions.Camera.Enable();

            panInputKeyboard = cameraActions.Camera.Pan_Keyboard;
            panInputMouse = cameraActions.Camera.Pan_Mouse;
            rotateInputMouse = cameraActions.Camera.Rotate;
            rotateInputKeyboard = cameraActions.Camera.Rotate_Keyboard;
            zoomInputMouse = cameraActions.Camera.Zoom_Mouse;
            
            rotateInputMouse.started += RotateCamera;
            zoomInputMouse.performed += ZoomCamera;
        }

        private void OnDisable()
        {
            rotateInputMouse.performed -= RotateCamera;
            zoomInputMouse.performed -= ZoomCamera;
            
            cameraActions.Camera.Disable();
        }

        private void FixedUpdate()
        {
            HandleKeyboardPanning();
            HandleMousePanning();
            
            HandleKeyboardRotation(); // Handle Q/E rotation
            //ClampCameraPosition();
        }   
        
        public void CenterAtPosition(int2 position)
        {
            var worldPosition = BoardHelper.HexToWorldPosition(position);
            
            transform.DOMove(worldPosition, .5f)
                .SetEase(Ease.OutQuad); // Smooth movement with easing
        }
        
        private void HandleKeyboardPanning()
        {
            Vector2 input = panInputKeyboard.ReadValue<Vector2>();
            if (input.sqrMagnitude <= 0.01f) return;

            Vector3 forward = Vector3.ProjectOnPlane(GetCameraForward(), Vector3.up).normalized;
            Vector3 right   = Vector3.ProjectOnPlane(GetCameraRight(), Vector3.up).normalized;

            Vector3 targetPos = transform.position + (right * input.x + forward * input.y) * panSpeed * Time.deltaTime;

            transform.position = Vector3.Lerp(transform.position, targetPos, 0.2f);
        }

        private void HandleMousePanning()
        {
            if (!Mouse.current.middleButton.isPressed)
                return;

            Vector2 mouseDelta = panInputMouse.ReadValue<Vector2>();
            if (mouseDelta.sqrMagnitude < 0.1f)
                return;

            Vector3 forward = Vector3.ProjectOnPlane(GetCameraForward(), Vector3.up).normalized;
            Vector3 right   = Vector3.ProjectOnPlane(GetCameraRight(), Vector3.up).normalized;

            Vector3 direction = (-mouseDelta.x * right) + (-mouseDelta.y * forward);

            Vector3 targetPos = transform.position + direction * mousePanSpeed * Time.deltaTime;

            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPos,
                ref mouseVelocity,
                0.1f      // smooth time — tweak to taste
            );
        }

        private void HandleKeyboardRotation()
        {
            float rotateInput = rotateInputKeyboard.ReadValue<float>(); // Get input from Q and E keys

            if (Mathf.Abs(rotateInput) > 0.1f)
            {
                // Apply yaw rotation based on Q/E input
                Vector3 currentEuler = transform.rotation.eulerAngles;
                currentEuler.y += rotateInput * rotationSpeed * Time.deltaTime;
                transform.rotation = Quaternion.Euler(currentEuler);
            }
        }

        private void RotateCamera(InputAction.CallbackContext context)
        {
            if (!Mouse.current.rightButton.isPressed)
            {
                return;
            }

            Vector2 rotationDelta = context.ReadValue<Vector2>();
            float yaw = rotationDelta.x * rotationSpeed * Time.deltaTime;
            float pitch = -rotationDelta.y * rotationSpeed * Time.deltaTime;

            Vector3 currentEuler = transform.rotation.eulerAngles;
            currentEuler.y += yaw;
            currentEuler.x = Mathf.Clamp(currentEuler.x + pitch, minTiltAngle, maxTiltAngle);

            transform.rotation = Quaternion.Euler(currentEuler);
        }

        private void ZoomCamera(InputAction.CallbackContext context)
        {
            float scrollDelta = context.ReadValue<Vector2>().y;
            if (Mathf.Abs(scrollDelta) < 0.1f)
                return;

            float targetFOV = camera.fieldOfView - scrollDelta * zoomSpeed;

            // Clamp limits
            targetFOV = Mathf.Clamp(targetFOV, minFOV, maxFOV);

            camera.fieldOfView = targetFOV;
        }

        /*private void ClampCameraPosition()
        {
            float currentHeight = camera.transform.localPosition.y;
            float clampedHeight = Mathf.Clamp(currentHeight, minZoom, maxZoom);

            Vector3 localPosition = camera.transform.localPosition;
            localPosition.y = clampedHeight;
            camera.transform.localPosition = localPosition;
        }*/

        private Vector3 GetCameraForward()
        {
            Vector3 forward = camera.transform.forward;
            forward.y = 0f;
            return forward.normalized;
        }

        private Vector3 GetCameraRight()
        {
            Vector3 right = camera.transform.right;
            right.y = 0f;
            return right.normalized;
        }
    }
}
