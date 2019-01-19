using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour {

  private int ChunkSize = 11;
  private float NodeDistance = 1f;

  private MeshFilter mf;
  private MeshRenderer mr;
  private MeshCollider mc;
  private World world;

  private float[,] heights;
  private float[,] temps;
  private float[,] humids;

  void Start() {
    mf = GetComponent<MeshFilter>();
    // mr = GetComponent<MeshRenderer>();
    world = GameObject.Find("World").GetComponent<World>();

    heights = new float[ChunkSize, ChunkSize];
    temps = new float[ChunkSize, ChunkSize];
    humids = new float[ChunkSize, ChunkSize];

    UpdateMesh();
  }

  private void UpdateMesh() {
    Vector3 baseV = transform.position;

    // Fill values
    for (int i = 0; i < ChunkSize; i++) {
      for (int j = 0; j < ChunkSize; j++) {
        Vector3 v = baseV + new Vector3(i * NodeDistance, 0, j * NodeDistance);
        heights[i, j] = world.GetHeight(v);
        temps[i, j] = world.GetTemperature(v);
        humids[i, j] = world.GetHumidity(v);
      }
    }

    // Fill mesh
    Mesh mesh = new Mesh();

    Vector3[] newVertices = new Vector3[ChunkSize * ChunkSize];
    Vector2[] newUV = new Vector2[ChunkSize * ChunkSize];
    List<int> newTriangles = new List<int>();

    for (int i = 0; i < ChunkSize; i++) {
      for (int j = 0; j < ChunkSize; j++) {
        int k = i + j * ChunkSize;
        Vector3 v = new Vector3(i * NodeDistance, heights[i, j], j * NodeDistance);
        newVertices[k] = v;
        newUV[k] = new Vector2(0, 0);
      }
    }

    for (int i = 0; i < ChunkSize - 1; i++) {
      for (int j = 0; j < ChunkSize - 1; j++) {
        int k00 = (i + 0) + (j + 0) * ChunkSize;
        int k01 = (i + 0) + (j + 1) * ChunkSize;
        int k10 = (i + 1) + (j + 0) * ChunkSize;
        int k11 = (i + 1) + (j + 1) * ChunkSize;
        newTriangles.Add(k00);
        newTriangles.Add(k01);
        newTriangles.Add(k10);
        newTriangles.Add(k10);
        newTriangles.Add(k01);
        newTriangles.Add(k11);
      }
    }

    mesh.vertices = newVertices;
    mesh.uv = newUV;
    mesh.triangles = newTriangles.ToArray();
    mesh.RecalculateNormals();

    mf.mesh = mesh;
  }
}
