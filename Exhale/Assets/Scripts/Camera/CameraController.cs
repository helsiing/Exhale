using System.Collections;
using DG.Tweening;
using ECS.Scripts.Managers;
using Exhale.Utils;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Exhale.Board
{
    [RequireComponent(typeof(Rigidbody))]
    public class CameraController : MonoBehaviour
    {
        [Header("Panning Settings")]
        [SerializeField] private float panForce = 100f;
        [SerializeField] private float maxPanSpeed = 10f;

        [Header("Mouse Drag Settings")]
        [SerializeField] private float mousePanSpeed = 0.5f;

        [Header("Zoom Settings")]
        [SerializeField] private float zoomForce = 300f;
        [SerializeField] private float minZoom = 5f;
        [SerializeField] private float maxZoom = 50f;

        [Header("Rotation Settings")]
        [SerializeField] private float rotationSpeed = 100f; // Speed for Q/E and mouse rotation
        [SerializeField] private float maxTiltAngle = 75f;
        [SerializeField] private float minTiltAngle = 15f;

        private Rigidbody rb;
        [SerializeField] private Transform cameraTransform;

        private InputAction panInputKeyboard;
        private InputAction rotateInputMouse;
        private InputAction zoomInputMouse;
        private InputAction panInputMouse;
        private InputAction rotateInputKeyboard; // New action for Q/E rotation

        private CameraControlActions cameraActions;

        
        //TODO: refactor this into a service
        private BoardEventManager boardEventManager;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.linearDamping = 5f;
            rb.angularDamping = 5f;

            cameraActions = new CameraControlActions();
        }
        
        private void Start()
        {
            BoardEventManager.OnBoardInitialized += OnBoardInitialized;
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
            BoardEventManager.OnBoardInitialized -= OnBoardInitialized;
            
            cameraActions.Camera.Disable();
        }

        private void FixedUpdate()
        {
            HandleKeyboardPanning();
            HandleMousePanning();
            
            HandleKeyboardRotation(); // Handle Q/E rotation
            ClampCameraPosition();
        }   
        
        private void OnBoardInitialized(int2 startPosition)
        {
            var startWorldPosition = BoardHelper.HexToWorldPosition(startPosition);
            
            transform.DOMove(startWorldPosition, .5f)
                .SetEase(Ease.OutQuad); // Smooth movement with easing

            // ✅ Rotate the camera to look at the target smoothly
            //transform.DOLookAt(startWorldPosition, 1f);
        }
        
        private void HandleKeyboardPanning()
        {
            Vector2 movement = panInputKeyboard.ReadValue<Vector2>();

            if (movement.sqrMagnitude > 0.1f)
            {
                Vector3 panDirection = movement.x * GetCameraRight() + movement.y * GetCameraForward();
                rb.AddForce(panDirection.normalized * panForce, ForceMode.Acceleration);
            }

            rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, maxPanSpeed);
        }

        private void HandleMousePanning()
        {
            if (!Mouse.current.middleButton.isPressed)
                return;

            Vector2 mouseDelta = panInputMouse.ReadValue<Vector2>();
            if (mouseDelta.sqrMagnitude > 0.1f)
            {
                Vector3 panDirection = -mouseDelta.x * GetCameraRight() - mouseDelta.y * GetCameraForward();
                rb.AddForce(panDirection * mousePanSpeed, ForceMode.Acceleration);
            }
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
            float zoomDelta = context.ReadValue<Vector2>().y / 10f;
            Vector3 zoomDirection = cameraTransform.transform.forward * zoomDelta * zoomForce;
            rb.AddForce(zoomDirection, ForceMode.Acceleration);
        }

        private void ClampCameraPosition()
        {
            float currentHeight = cameraTransform.transform.localPosition.y;
            float clampedHeight = Mathf.Clamp(currentHeight, minZoom, maxZoom);

            Vector3 localPosition = cameraTransform.transform.localPosition;
            localPosition.y = clampedHeight;
            cameraTransform.transform.localPosition = localPosition;
        }

        private Vector3 GetCameraForward()
        {
            Vector3 forward = cameraTransform.transform.forward;
            forward.y = 0f;
            return forward.normalized;
        }

        private Vector3 GetCameraRight()
        {
            Vector3 right = cameraTransform.transform.right;
            right.y = 0f;
            return right.normalized;
        }
    }
}
