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

  public int AddPoint(Vector3 point, Vector3 normal, float width) {

    int z = vertices.Count;
    Quaternion zRotation = Quaternion.LookRotation(normal, Vector3.up);

    for (int i = 0; i < 4; i++) {
      float rad = 2f * Mathf.PI * i / 4;
      Vector3 v = new Vector3(Mathf.Sin(rad), Mathf.Cos(rad), 0f);
      vertices.Add(point + (zRotation * v).normalized * width);
    }

    return z;
  }

  public void AddSegment(int p1, int p2) {

    int offset = 0;
    float dist = (vertices[p1] - vertices[p2 + 0]).magnitude;

    for (int i = 0; i < 4; i++) {
      float newDist = (vertices[p1] - vertices[p2 + i]).magnitude;

      if (newDist < dist) {
        offset = i;
      }
    }

    triangles.Add(p1 + 0);
    triangles.Add(p2 + (0 + offset) % 4);
    triangles.Add(p1 + 1);
    triangles.Add(p2 + (0 + offset) % 4);
    triangles.Add(p2 + (1 + offset) % 4);
    triangles.Add(p1 + 1);

    triangles.Add(p1 + 1);
    triangles.Add(p2 + (1 + offset) % 4);
    triangles.Add(p1 + 2);
    triangles.Add(p2 + (1 + offset) % 4);
    triangles.Add(p2 + (2 + offset) % 4);
    triangles.Add(p1 + 2);

    triangles.Add(p1 + 2);
    triangles.Add(p2 + (2 + offset) % 4);
    triangles.Add(p1 + 3);
    triangles.Add(p2 + (2 + offset) % 4);
    triangles.Add(p2 + (3 + offset) % 4);
    triangles.Add(p1 + 3);

    triangles.Add(p1 + 3);
    triangles.Add(p2 + (3 + offset) % 4);
    triangles.Add(p1 + 0);
    triangles.Add(p2 + (3 + offset) % 4);
    triangles.Add(p2 + (0 + offset) % 4);
    triangles.Add(p1 + 0);
  }

  public void UpdateMesh() {
    mesh.vertices = vertices.ToArray();
    mesh.triangles = triangles.ToArray();
    mesh.RecalculateNormals();
  }
}