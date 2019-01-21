﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {

  public GameObject loadTarget;
  public GameObject chunkPrefab;
  public Transform chunkContainer;

  public bool reset = false;
  public bool smoothChunk = false;

  public float chunkRangeLoad = 50f;
  public float chunkRangeUnload = 100f;

  private const int ChunksLoadPerFrame = 50;

  private Dictionary<Vector2Int, GameObject> chunks;

  void Start() {
    chunks = new Dictionary<Vector2Int, GameObject>();
  }

  void Update() {
    // Load chunks in range
    LoadChunksInRange();

    // Unload chunks out of range
    UnloadChunksInRange();

    // Reset
    if (reset) {
      reset = false;
      UnloadAllChunks();
    }
  }

  private Vector2Int PosToKey(Vector3 v) {
    return new Vector2Int(Mathf.RoundToInt(v.x / Chunk.Size), Mathf.RoundToInt(v.z / Chunk.Size));
  }

  private float RoundToChunk(float f) {
    return Chunk.Size * Mathf.RoundToInt(f / Chunk.Size);
  }

  private void LoadChunksInRange() {
    int range = Mathf.CeilToInt(chunkRangeLoad / Chunk.Size);
    Vector3 target = loadTarget.transform.position;
    int loaded = 0;

    for (int i = -range; i <= range; i++) {
      for (int j = -range; j <= range; j++) {
        Vector3 v = target + new Vector3(i * Chunk.Size, 0, j * Chunk.Size);
        Vector2Int k = PosToKey(v);

        if (chunks.ContainsKey(k))
          continue;

        v = new Vector3(RoundToChunk(v.x), 0, RoundToChunk(v.z));
        float dist = (v - target).magnitude;

        if (dist >= chunkRangeLoad)
          continue;

        GameObject go = Instantiate(chunkPrefab, v, Quaternion.identity, chunkContainer);
        chunks.Add(PosToKey(go.transform.position), go);
        loaded += 1;

        if (loaded >= ChunksLoadPerFrame)
          return;
      }
    }
  }

  private void UnloadChunksInRange() {
    Vector3 target = loadTarget.transform.position;
    List<Vector2Int> toUnloadKeys = new List<Vector2Int>();

    foreach (KeyValuePair<Vector2Int, GameObject> entry in chunks) {
      float dist = (entry.Value.transform.position - target).magnitude;

      if (dist >= chunkRangeUnload)
        toUnloadKeys.Add(entry.Key);
    }

    foreach (Vector2Int k in toUnloadKeys) {
      Destroy(chunks[k]);
      chunks.Remove(k);
    }
  }

  private void UnloadAllChunks() {
    List<Vector2Int> toUnloadKeys = new List<Vector2Int>();

    foreach (KeyValuePair<Vector2Int, GameObject> entry in chunks)
      toUnloadKeys.Add(entry.Key);

    foreach (Vector2Int k in toUnloadKeys) {
      Destroy(chunks[k]);
      chunks.Remove(k);
    }
  }

  public float blocks = 1f, offset = 10f;
  public Vector3[] noiseLayers;

  public float GetHeight(Vector3 v) {
    float output = 0f;
    Vector3 u = Vector3.up;
    v = v - u * v.y;

    output += offset;

    foreach (Vector3 a in noiseLayers)
      output += a.x * Perlin.Noise(v * a.y + u * a.z);

    if (blocks != 0f)
      output = blocks * Mathf.RoundToInt(output / blocks);

    return output;
  }

  public float GetTemperature(Vector3 v) {
    return v.x;
  }

  public float GetHumidity(Vector3 v) {
    return v.x;
  }
}
