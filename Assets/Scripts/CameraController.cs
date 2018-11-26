using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
  /*
  Controller that handles camera inputs and behaviors
  */

  public bool lookMode;  // Camera in look mode vs menu mode

  private float x = 0.0f; // Current camera angles
  private float y = 30.0f;

  private float distance = 7f;  // Current distance from wand

  private float xSpeed = 9.0f;    // Angular change rate
  private float ySpeed = 12.0f;
  private float yMinLimit = 5f;   // Angle bounds
  private float yMaxLimit = 80f;

  private float distanceSpeed = 0.5f; // Distance change rate
  private float distanceMin = 5f;     // Distance bounds
  private float distanceMax = 10f;

  void Start() {

    // Lock cursor to screen
    SetCameraMode(true);

    // Camera positioning
    Vector3 angles = transform.eulerAngles;
    x = angles.y;
    y = angles.x;
  }

  void Update() {

    // Lock cursor to screen on input
    if (lookMode) {
      Cursor.lockState = CursorLockMode.Locked;
    }
  }

  void LateUpdate() {

    // Don't move camera if in menu
    if (!lookMode)
      return;

    // Update camera position based on mouse movement
    x += Input.GetAxis("MouseX") * xSpeed * 0.02f;
    y -= Input.GetAxis("MouseY") * ySpeed * 0.02f;
    x = (x + 360f) % 360f;
    y = Mathf.Clamp(y, yMinLimit, yMaxLimit);

    Quaternion rotation = Quaternion.Euler(y, x, 0);

    distance = Mathf.Clamp(distance - Input.GetAxis("MouseScrollWheel") * distanceSpeed, distanceMin, distanceMax);

    Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
    Vector3 position = rotation * negDistance;

    transform.rotation = rotation;
    transform.position = position;
  }

  public void SetCameraMode(bool mode) {
    lookMode = mode;
  }

  // Returns 0 to 1 for zoom level where 0 in low, 1 is high
  public float GetZoom() {
    return distance - distanceMin / distanceMax - distanceMin;
  }
}