using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {

  public GameObject loadTarget;
  public GameObject chunkPrefab;

  public bool reset = true;
  public bool smoothChunk = false;

  void Start() {

  }

  void Update() {
    // Load correct chunks

    // Currently just a constant block
    if (reset) {
      reset = false;
      LoadBlockOfChunks();
    }
  }

  private void LoadBlockOfChunks() {
    foreach (Transform child in transform)
      Destroy(child.gameObject);

    for (int i = -2; i < 3; i++) {
      for (int j = -2; j < 3; j++) {
        Instantiate(chunkPrefab, new Vector3(i * Chunk.Size, 0, j * Chunk.Size), Quaternion.identity, transform);
      }
    }
  }

  public float GetHeight(Vector3 v) {
    v = v - Vector3.up * v.y;
    return 8f * (Perlin.Noise(v * 0.05f) + 0.5f);
    // return v.magnitude;
  }

  public float GetTemperature(Vector3 v) {
    return v.x;
  }

  public float GetHumidity(Vector3 v) {
    return v.x;
  }
}
