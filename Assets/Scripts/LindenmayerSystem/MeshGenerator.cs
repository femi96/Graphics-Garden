using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MeshGenerator : SystemGenerator {
  /*
  Subclass of SystemGenerators that generate meshes
  */

  private Mesh mesh;
  private List<Vector3> vertices;
  private List<int> triangles;

  [Header("Mesh Generator Settings")]
  public int segmentSides = 3; // Must be greater than 3

  public void CreateMesh() {
    mesh = new Mesh();
    vertices = new List<Vector3>();
    triangles = new List<int>();
    GetComponent<MeshFilter>().mesh = mesh;
  }

  public int AddPoint(Vector3 point, Vector3 normal, float width) {

    int z = vertices.Count;
    Quaternion zRotation = Quaternion.LookRotation(normal, Vector3.up);

    for (int i = 0; i < segmentSides; i++) {
      float rad = 2f * Mathf.PI * i / segmentSides;
      Vector3 v = new Vector3(Mathf.Sin(rad), Mathf.Cos(rad), 0f);
      vertices.Add(point + (zRotation * v).normalized * width);
    }

    return z;
  }

  public void AddSegment(int p1, int p2) {

    int offset = -1;
    float lowestSum = 0f;

    for (int i = 0; i < segmentSides; i++) {
      float sum = 0f;

      for (int j = 0; j < segmentSides; j++) {
        sum += (vertices[p1 + j] - vertices[p2 + (j + i) % segmentSides]).magnitude;
      }

      if (offset == -1 || sum < lowestSum) {
        offset = i;
        lowestSum = sum;
      }
    }

    for (int i = 0; i < segmentSides; i++) {

      triangles.Add(p1 + i % segmentSides);
      triangles.Add(p2 + (i + offset) % segmentSides);
      triangles.Add(p1 + (i + 1) % segmentSides);
      triangles.Add(p2 + (i + offset) % segmentSides);
      triangles.Add(p2 + (i + 1 + offset) % segmentSides);
      triangles.Add(p1 + (i + 1) % segmentSides);
    }
  }

  public void UpdateMesh() {
    mesh.vertices = vertices.ToArray();
    mesh.triangles = triangles.ToArray();
    mesh.RecalculateNormals();
  }
}