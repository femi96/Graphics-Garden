using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {

  public GameObject loadTarget;
  public GameObject chunkPrefab;

  void Start() {
    LoadBlockOfChunks();
  }

  void Update() {
    // Load correct chunks
  }

  private void LoadBlockOfChunks() {
    for (int i = -2; i < 3; i++) {
      for (int j = -2; j < 3; j++) {
        Instantiate(chunkPrefab, new Vector3(i * 10f, 0, j * 10f), Quaternion.identity, transform);
      }
    }
  }

  public float GetHeight(Vector3 v) {
    return v.magnitude;
  }

  public float GetTemperature(Vector3 v) {
    return v.x;
  }

  public float GetHumidity(Vector3 v) {
    return v.x;
  }
}
