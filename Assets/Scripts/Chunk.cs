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

  public Transform plantContainer;
  private bool hasPlants = false;

  void Start() {
    nodeDistance = Size / (NumNodes - 1);

    mf = GetComponent<MeshFilter>();
    // mr = GetComponent<MeshRenderer>();
    world = GameObject.Find("World").GetComponent<World>();

    UpdateMesh();
  }

  private void UpdateMesh() {
    Vector3 baseV = transform.position;

    float[,] heights = new float[NumNodes, NumNodes];
    float[,] temps = new float[NumNodes, NumNodes];
    float[,] humids = new float[NumNodes, NumNodes];
    Biome[,] biomes = new Biome[NumNodes, NumNodes];

    // Fill values
    for (int i = 0; i < NumNodes; i++) {
      for (int j = 0; j < NumNodes; j++) {
        Vector3 v = baseV + new Vector3(i * nodeDistance, 0, j * nodeDistance);
        heights[i, j] = world.GetHeight(v);
        temps[i, j] = world.GetTemperature(v);
        humids[i, j] = world.GetHumidity(v);
        biomes[i, j] = world.GetBiome(v);
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

          Biome[] biomesQuad = new Biome[5] {
            biomes[i + 0, j + 0],
            biomes[i + 0, j + 1],
            biomes[i + 1, j + 0],
            biomes[i + 1, j + 1],
            world.GetBiome(vc_),
          };
          AddUV(newUV, biomesQuad);

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

  public void AddUV(List<Vector2> newUV, Biome[] biomes) {
    int i = 0;
    int j = 0;

    Dictionary<Biome, int> biomeCount = new Dictionary<Biome, int>();

    foreach (Biome biome in biomes) {
      if (!biomeCount.ContainsKey(biome))
        biomeCount[biome] = 1;
      else
        biomeCount[biome] += 1;
    }

    int maxCount = 0;
    Biome maxBiome = biomes[0];

    foreach (KeyValuePair<Biome, int> entry in biomeCount) {
      if (entry.Value > maxCount) {
        maxCount = entry.Value;
        maxBiome = entry.Key;
      }
    }

    int[] uv = GetUV(maxBiome);
    i = uv[0];
    j = uv[1];

    int f = 128;
    int b = 32;
    float s = 512f;

    int x0 = i * f + b;
    int x1 = (i + 1) * f - b;
    int y0 = j * f + b;
    int y1 = (j + 1) * f - b;

    newUV.Add(new Vector2(x0 / s, y0 / s));
    newUV.Add(new Vector2(x0 / s, y1 / s));
    newUV.Add(new Vector2(x1 / s, y0 / s));
    newUV.Add(new Vector2(x1 / s, y1 / s));
    newUV.Add(new Vector2((x0 + x1) / (2 * s), (y0 + y1) / (2 * s)));
  }

  private int[] GetUV(Biome biome) {
    int i = 0;
    int j = 0;

    switch (biome) {
    case Biome.Steppes:
      i = 0; j = 0;
      break;

    case Biome.Plains:
      i = 0; j = 1;
      break;

    case Biome.Swamp:
      i = 0; j = 2;
      break;

    case Biome.Water:
      i = 0; j = 3;
      break;

    case Biome.Desert:
      i = 1; j = 0;
      break;

    case Biome.Savanna:
      i = 1; j = 1;
      break;

    case Biome.Jungle:
      i = 1; j = 2;
      break;

    case Biome.Tropics:
      i = 1; j = 3;
      break;

    case Biome.Tundra:
      i = 2; j = 0;
      break;

    case Biome.Taiga:
      i = 2; j = 1;
      break;

    case Biome.Icefield:
      i = 2; j = 2;
      break;

    case Biome.Icefloat:
      i = 2; j = 3;
      break;

    case Biome.Icecap:
      i = 3; j = 2;
      break;

    case Biome.Mountain:
      i = 3; j = 3;
      break;

    case Biome.Beach:
      i = 1; j = 0;
      break;

    case Biome.Shore:
      i = 0; j = 3;
      break;
    }

    return new int[2] { i, j };
  }

  public GameObject plantPrefab;

  public void SetPlants(bool plantStatus) {
    if (!hasPlants && plantStatus) {
      PlacePlants();
    }

    if (hasPlants && !plantStatus) {

      foreach (Transform child in plantContainer)
        Destroy(child.gameObject);
    }

    hasPlants = plantStatus;
  }

  private void PlacePlants() {
    for (int i = 0; i < 3; i++) {
      Vector3 v = new Vector3(0, 0, 0);
      v += transform.position;
      float x = Size * (0.5f + Perlin.Noise(0.07f * v + 5f * i * Vector3.up));
      float z = Size * (0.5f + Perlin.Noise(0.05f * v + 7f * i * Vector3.up));
      v += new Vector3(x, 0, z);
      v += Vector3.up * world.GetHeight(v);
      GameObject plantPrefab = GetPlant(v);
      Instantiate(plantPrefab, v, Quaternion.identity, transform);
    }
  }

  private GameObject GetPlant(Vector3 v) {
    return plantPrefab;
  }
}
