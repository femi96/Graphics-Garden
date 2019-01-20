using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour {

  public static int NumNodes = 6;
  public static float Size = 10f;
  private float nodeDistance;

  private MeshFilter mf;
  private MeshRenderer mr;
  private MeshCollider mc;
  private World world;

  private float[,] heights;
  private float[,] temps;
  private float[,] humids;

  void Start() {
    nodeDistance = Size / (NumNodes - 1);

    mf = GetComponent<MeshFilter>();
    // mr = GetComponent<MeshRenderer>();
    world = GameObject.Find("World").GetComponent<World>();

    heights = new float[NumNodes, NumNodes];
    temps = new float[NumNodes, NumNodes];
    humids = new float[NumNodes, NumNodes];

    UpdateMesh();
  }

  private void UpdateMesh() {
    Vector3 baseV = transform.position;

    // Fill values
    for (int i = 0; i < NumNodes; i++) {
      for (int j = 0; j < NumNodes; j++) {
        Vector3 v = baseV + new Vector3(i * nodeDistance, 0, j * nodeDistance);
        heights[i, j] = world.GetHeight(v);
        temps[i, j] = world.GetTemperature(v);
        humids[i, j] = world.GetHumidity(v);
      }
    }

    // Fill mesh
    Mesh mesh = new Mesh();

    List<Vector3> newVertices = new List<Vector3>();
    List<Vector2> newUV = new List<Vector2>();
    List<int> newTriangles = new List<int>();

    if (!world.smoothChunk) {
      for (int i = 0; i < NumNodes - 1; i++) {
        for (int j = 0; j < NumNodes - 1; j++) {
          Vector3 v00 = new Vector3((i + 0) * nodeDistance, heights[i + 0, j + 0], (j + 0) * nodeDistance);
          Vector3 v01 = new Vector3((i + 0) * nodeDistance, heights[i + 0, j + 1], (j + 1) * nodeDistance);
          Vector3 v10 = new Vector3((i + 1) * nodeDistance, heights[i + 1, j + 0], (j + 0) * nodeDistance);
          Vector3 v11 = new Vector3((i + 1) * nodeDistance, heights[i + 1, j + 1], (j + 1) * nodeDistance);
          Vector3 vc_ = (v00 + v01 + v10 + v11) * 0.25f;

          int baseK = newVertices.Count;
          int k00 = baseK + 0;
          int k01 = baseK + 1;
          int k10 = baseK + 2;
          int k11 = baseK + 3;
          int kc_ = baseK + 4;

          newVertices.Add(v00);
          newVertices.Add(v01);
          newVertices.Add(v10);
          newVertices.Add(v11);
          newVertices.Add(vc_);

          newUV.Add(new Vector2(0, 0));
          newUV.Add(new Vector2(0, 0));
          newUV.Add(new Vector2(0, 0));
          newUV.Add(new Vector2(0, 0));
          newUV.Add(new Vector2(0, 0));

          newTriangles.Add(k00);
          newTriangles.Add(k01);
          newTriangles.Add(kc_);

          newTriangles.Add(k01);
          newTriangles.Add(k11);
          newTriangles.Add(kc_);

          newTriangles.Add(k11);
          newTriangles.Add(k10);
          newTriangles.Add(kc_);

          newTriangles.Add(k10);
          newTriangles.Add(k00);
          newTriangles.Add(kc_);
        }
      }
    } else {
      for (int j = 0; j < NumNodes; j++) {
        for (int i = 0; i < NumNodes; i++) {
          Vector3 v = new Vector3(i * nodeDistance, heights[i, j], j * nodeDistance);
          newVertices.Add(v);
          newUV.Add(new Vector2(0, 0));
        }
      }

      for (int i = 0; i < NumNodes - 1; i++) {
        for (int j = 0; j < NumNodes - 1; j++) {
          int k00 = (i + 0) + (j + 0) * NumNodes;
          int k01 = (i + 0) + (j + 1) * NumNodes;
          int k10 = (i + 1) + (j + 0) * NumNodes;
          int k11 = (i + 1) + (j + 1) * NumNodes;
          newTriangles.Add(k00);
          newTriangles.Add(k01);
          newTriangles.Add(k10);
          newTriangles.Add(k10);
          newTriangles.Add(k01);
          newTriangles.Add(k11);
        }
      }
    }

    mesh.vertices = newVertices.ToArray();
    mesh.triangles = newTriangles.ToArray();
    mesh.uv = newUV.ToArray();
    mesh.RecalculateNormals();

    mf.mesh = mesh;
  }
}
