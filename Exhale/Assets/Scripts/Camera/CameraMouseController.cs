using UnityEngine;
using UnityEngine.InputSystem;

namespace Exhale.Board
{
    public class CameraMouseController : MonoBehaviour
    {
        [SerializeField] private float maxSpeed = 5f;

        [SerializeField] private float acceleration = 10f;

        [SerializeField] private float damping = 15f;

        [SerializeField] private float stepSize = 2f;

        [SerializeField] private float zoomDampening = 7.5f;

        [SerializeField] private float minHeight = 5f;

        [SerializeField] private float maxHeight = 50f;

        [SerializeField] private float zoomSpeed = 2f;

        [SerializeField] private float maxRotationSpeed = 1f;

        [SerializeField] [Range(0f, 0.1f)] private float edgeTolerance = 0.05f;

        private CameraControlActions cameraActions;
        private Transform cameraTransform;

        //used to track and maintain velocity w/o a rigidbody
        private Vector3 horizontalVelocity;
        private Vector3 lastPosition;
        private InputAction movement;
        private float speed;

        //tracks where the dragging action started
        private Vector3 startDrag;

        //value set in various functions 
        //used to update the position of the camera base object.
        private Vector3 targetPosition;

        private float zoomHeight;

        private void Awake()
        {
            cameraActions = new CameraControlActions();
            cameraTransform = GetComponentInChildren<Camera>().transform;
        }

        private void Update()
        {
            //inputs
            GetKeyboardMovement();
#if !UNITY_EDITOR
            CheckMouseAtScreenEdge();
#endif
            DragCamera();

            //move base and camera objects
            UpdateVelocity();
            UpdateBasePosition();
            UpdateCameraPosition();
        }

        private void OnEnable()
        {
            zoomHeight = cameraTransform.localPosition.y;
            cameraTransform.LookAt(transform);

            lastPosition = transform.position;

            movement = cameraActions.Camera.Movement;
            cameraActions.Camera.RotateCamera.performed += RotateCamera;
            cameraActions.Camera.ZoomCamera.performed += ZoomCamera;
            cameraActions.Camera.Enable();
        }

        private void OnDisable()
        {
            cameraActions.Camera.RotateCamera.performed -= RotateCamera;
            cameraActions.Camera.ZoomCamera.performed -= ZoomCamera;
            cameraActions.Camera.Disable();
        }

        private void UpdateVelocity()
        {
            horizontalVelocity = (transform.position - lastPosition) / Time.deltaTime;
            horizontalVelocity.y = 0f;
            lastPosition = transform.position;
        }

        private void GetKeyboardMovement()
        {
            var inputValue = movement.ReadValue<Vector2>().x * GetCameraRight()
                             + movement.ReadValue<Vector2>().y * GetCameraForward();

            inputValue = inputValue.normalized;

            if (inputValue.sqrMagnitude > 0.1f)
                targetPosition += inputValue;
        }

        private void DragCamera()
        {
            if (!Mouse.current.rightButton.isPressed)
                return;

            //create plane to raycast to
            var plane = new Plane(Vector3.up, Vector3.zero);
            var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (plane.Raycast(ray, out var distance))
            {
                if (Mouse.current.rightButton.wasPressedThisFrame)
                    startDrag = ray.GetPoint(distance);
                else
                    targetPosition += startDrag - ray.GetPoint(distance);
            }
        }

        private void CheckMouseAtScreenEdge()
        {
            //mouse position is in pixels
            var mousePosition = Mouse.current.position.ReadValue();
            var moveDirection = Vector3.zero;

            //horizontal scrolling
            if (mousePosition.x < edgeTolerance * Screen.width)
                moveDirection += -GetCameraRight();
            else if (mousePosition.x > (1f - edgeTolerance) * Screen.width)
                moveDirection += GetCameraRight();

            //vertical scrolling
            if (mousePosition.y < edgeTolerance * Screen.height)
                moveDirection += -GetCameraForward();
            else if (mousePosition.y > (1f - edgeTolerance) * Screen.height)
                moveDirection += GetCameraForward();

            targetPosition += moveDirection;
        }

        private void UpdateBasePosition()
        {
            if (targetPosition.sqrMagnitude > 0.1f)
            {
                //create a ramp up or acceleration
                speed = Mathf.Lerp(speed, maxSpeed, Time.deltaTime * acceleration);
                transform.position += targetPosition * speed * Time.deltaTime;
            }
            else
            {
                //create smooth slow down
                horizontalVelocity = Vector3.Lerp(horizontalVelocity, Vector3.zero, Time.deltaTime * damping);
                transform.position += horizontalVelocity * Time.deltaTime;
            }

            //reset for next frame
            targetPosition = Vector3.zero;
        }

        private void ZoomCamera(InputAction.CallbackContext obj)
        {
            var inputValue = -obj.ReadValue<Vector2>().y / 100f;

            if (Mathf.Abs(inputValue) > 0.1f)
            {
                zoomHeight = cameraTransform.localPosition.y + inputValue * stepSize;

                if (zoomHeight < minHeight)
                    zoomHeight = minHeight;
                else if (zoomHeight > maxHeight)
                    zoomHeight = maxHeight;
            }
        }

        private void UpdateCameraPosition()
        {
            //set zoom target
            var zoomTarget = new Vector3(cameraTransform.localPosition.x, zoomHeight, cameraTransform.localPosition.z);
            //add vector for forward/backward zoom
            zoomTarget -= zoomSpeed * (zoomHeight - cameraTransform.localPosition.y) * Vector3.forward;

            cameraTransform.localPosition =
                Vector3.Lerp(cameraTransform.localPosition, zoomTarget, Time.deltaTime * zoomDampening);
            cameraTransform.LookAt(transform);
        }

        private void RotateCamera(InputAction.CallbackContext obj)
        {
            if (!Mouse.current.middleButton.isPressed)
                return;

            var inputValue = obj.ReadValue<Vector2>().x;
            transform.rotation =
                Quaternion.Euler(0f, inputValue * maxRotationSpeed + transform.rotation.eulerAngles.y, 0f);
        }

        //gets the horizontal forward vector of the camera
        private Vector3 GetCameraForward()
        {
            var forward = cameraTransform.forward;
            forward.y = 0f;
            return forward;
        }

        //gets the horizontal right vector of the camera
        private Vector3 GetCameraRight()
        {
            var right = cameraTransform.right;
            right.y = 0f;
            return right;
        }
    }
}