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

  public void CreateMesh() {
    mesh = new Mesh();
    vertices = new List<Vector3>();
    triangles = new List<int>();
    GetComponent<MeshFilter>().mesh = mesh;
  }

  public void AddSegment(Vector3 p1, Vector3 p2, float w1, float w2) {
    Vector3 p1t2 = p2 - p1;
    Quaternion zRotation = Quaternion.LookRotation(p1t2, Vector3.up);

    int z1 = vertices.Count;

    for (int i = 0; i < 4; i++) {
      float rad = 2f * Mathf.PI * i / 4;
      Vector3 v = new Vector3(Mathf.Sin(rad), Mathf.Cos(rad), 0f);
      vertices.Add(p1 + (zRotation * v).normalized * w1);
    }

    int z2 = vertices.Count;

    for (int i = 0; i < 4; i++) {
      float rad = 2f * Mathf.PI * i / 4;
      Vector3 v = new Vector3(Mathf.Sin(rad), Mathf.Cos(rad), 0f);
      vertices.Add(p2 + (zRotation * v).normalized * w2);
    }

    triangles.Add(z1 + 0);
    triangles.Add(z2 + 0);
    triangles.Add(z1 + 1);
    triangles.Add(z2 + 0);
    triangles.Add(z2 + 1);
    triangles.Add(z1 + 1);

    triangles.Add(z1 + 1);
    triangles.Add(z2 + 1);
    triangles.Add(z1 + 2);
    triangles.Add(z2 + 1);
    triangles.Add(z2 + 2);
    triangles.Add(z1 + 2);

    triangles.Add(z1 + 2);
    triangles.Add(z2 + 2);
    triangles.Add(z1 + 3);
    triangles.Add(z2 + 2);
    triangles.Add(z2 + 3);
    triangles.Add(z1 + 3);

    triangles.Add(z1 + 3);
    triangles.Add(z2 + 3);
    triangles.Add(z1 + 0);
    triangles.Add(z2 + 3);
    triangles.Add(z2 + 0);
    triangles.Add(z1 + 0);
  }

  public void UpdateMesh() {
    mesh.vertices = vertices.ToArray();
    mesh.triangles = triangles.ToArray();
    mesh.RecalculateNormals();
  }
}