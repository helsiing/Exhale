#region
using System;
using TouchScript.Gestures.TransformGestures;
using UnityEngine;
#endregion

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform pivot;
    [SerializeField] private Camera camera;
    [SerializeField] private ScreenTransformGesture twoFingerMoveGesture;
    [SerializeField] private ScreenTransformGesture manipulationGesture;
    [SerializeField] private float panSpeed = 200f;
    [SerializeField] private float rotationSpeed = 200f;
    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private float maxZoom = 75f;
    [SerializeField] private float minZoom = 15f;
    [SerializeField] private Transform board;

    private void OnEnable()
    {
        twoFingerMoveGesture.Transformed += OnTwoFingerMoveGesture;
        manipulationGesture.Transformed += OnManipulationGesture;
    }

    private void OnDisable()
    {
        twoFingerMoveGesture.Transformed -= OnTwoFingerMoveGesture;
        manipulationGesture.Transformed -= OnManipulationGesture;
    }

    private void OnManipulationGesture(object sender, EventArgs e)
    {
        pivot.RotateAround(board.position, Vector3.up, manipulationGesture.DeltaPosition.y + 2 * rotationSpeed);
        camera.fieldOfView = Math.Clamp(camera.fieldOfView + (manipulationGesture.DeltaScale - 1f) * zoomSpeed, maxZoom, minZoom);
    }

    private void OnTwoFingerMoveGesture(object sender, EventArgs e)
    {
        pivot.localPosition += pivot.rotation * twoFingerMoveGesture.DeltaPosition * panSpeed;
    }
}