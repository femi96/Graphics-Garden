using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour {
  /*
  Controller that handles camera inputs and behaviors
  */

  public bool lookMode;  // Camera in look mode vs menu mode
  public bool sceneManagerMode = false;
  public GameObject dirLight;

  private float x = 0.0f; // Current camera angles
  private float y = 30.0f;

  private float distance = 7f;  // Current distance from wand

  private float xSpeed = 9.0f;    // Angular change rate
  private float ySpeed = 12.0f;
  private float yMinLimit = 5f;   // Angle bounds
  private float yMaxLimit = 80f;

  private float distanceSpeed = 0.5f; // Distance change rate
  private float distanceMin = 5f;     // Distance bounds
  private float distanceMax = 20f;

  public int currentScene = -1;
  private string[] scenes = new string[] {
    "Showcase1_LSys",
    "Showcase2_LSys",
    "Showcase3_LSys",
    "Showcase4_Particles",
    "Showcase5_Boids",
    "Showcase6_Flowers",
    "Showcase7_Leaves",
    "Showcase8_BigTree",
    "Showcase9_Trees",
    "Showcase10_TreesLeaves",
    "Showcase11_TreesBees",
    "Showcase12_TreesBirds",
    "Showcase13_Aquarium",
    "Showcase14_AquariumDark",
    "Showcase15_BonusStar",
  };
  private bool prepSetActive = false;

  void Start() {

    // Lock cursor to screen
    SetCameraMode(true);
    ChangeScene(0);

    // Camera positioning
    Vector3 angles = transform.eulerAngles;
    x = angles.y;
    y = angles.x;
  }

  void Update() {

    if (prepSetActive) {
      SceneManager.SetActiveScene(SceneManager.GetSceneByName(scenes[currentScene]));
      prepSetActive = false;
    }

    if (Input.GetMouseButtonDown(0))
      lookMode = true;

    if (Input.GetKeyDown(KeyCode.Space))
      lookMode = !lookMode;

    if (Input.GetKeyDown(KeyCode.W))
      ChangeScene(currentScene); // Reload scene

    if (Input.GetKeyDown(KeyCode.Q)) {
      if (currentScene == 0)
        ChangeScene((scenes.Length - 1) % scenes.Length); // Prev scene
      else
        ChangeScene((currentScene - 1) % scenes.Length); // Prev scene
    }

    if (Input.GetKeyDown(KeyCode.E))
      ChangeScene((currentScene + 1) % scenes.Length); // Next scene


    // Lock cursor to screen on input
    if (lookMode) {
      Cursor.lockState = CursorLockMode.Locked;
      Cursor.visible = false;
    } else {
      Cursor.lockState = CursorLockMode.None;
      Cursor.visible = true;
    }
  }

  private void ChangeScene(int nextScene) {
    if (!sceneManagerMode)
      return;

    if (currentScene >= 0)
      SceneManager.UnloadSceneAsync(scenes[currentScene]);

    SceneManager.LoadScene(scenes[nextScene], LoadSceneMode.Additive);

    if (nextScene == 13) {
      prepSetActive = true;
      dirLight.SetActive(false);
    } else {
      SceneManager.SetActiveScene(SceneManager.GetSceneByName("BaseScene"));
      dirLight.SetActive(true);
    }

    currentScene = nextScene;
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