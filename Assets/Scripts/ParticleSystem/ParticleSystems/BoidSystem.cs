using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidSystem : ParticleSystemCustom {

  public int numBoids = 20;
  private bool[] activeBoids;

  public GameObject boidObj;
  private GameObject[] boidObjects;

  [Header("Boid Speed Settings")]
  public float BoidSpeed = 1.0f;
  public float BoidSpeedDragConstant = 1.0f;

  [Header("Flock Settings")]
  public float FlockMinDistance = 1f;
  public float FlockMaxDistance = 2.5f;

  [Header("Boundary Settings")]
  public float BoundaryDistance = 4f;
  public bool BoundaryFloor = true;

  [Header("Factor Weight Settings")]
  public float WeightSeparation = 1.0f;
  public float WeightAlignment = 1.0f;
  public float WeightCohesion = 1.0f;
  public float WeightNoise = 0.2f;
  public float WeightAvoidance = 1.0f;

  [Header("Spawn Settings")]
  public float SpawnRange = 5;

  public override void CreateState() {

    spawnTime = Random.Range(0f, 1f) * spawnRate;

    // Clear child objects
    foreach (Transform child in transform)
      GameObject.Destroy(child.gameObject);


    // Active boids
    activeBoids = new bool[numBoids];

    for (int i = numBoids; i < numBoids; ++i)
      activeBoids[i] = false;


    // State is (x, v)
    state = new Vector3[numBoids * 2];
    boidObjects = new GameObject[numBoids];

    for (int i = 0; i < numBoids; ++i)
      ResetParticle(i);
  }

  public override Vector3[] EvalF(Vector3[] evalState) {

    // Take state which is (x, v)
    // Output state which is (v, A)


    // A = steer + speed control
    Vector3[] accel = new Vector3[numBoids];

    for (int i = 0; i < numBoids; ++i) {
      accel[i] = new Vector3();

      if (!activeBoids[i]) // Don't calculate if not active
        continue;

      Vector3 boidPos = evalState[i];
      Vector3 boidVel = evalState[numBoids + i];

      // Steer = separation + alignment + cohesion
      List<int> flockIndices = new List<int>();

      for (int j = 0; j < numBoids; ++j) {
        if (!activeBoids[i]) // Don't calculate if not active
          continue;

        Vector3 frenPos = evalState[j];
        Vector3 deltaPos = boidPos - frenPos;

        if (deltaPos.magnitude <= FlockMaxDistance) flockIndices.Add(j);
      }

      foreach (int j in flockIndices) {
        if (i == j) continue;

        Vector3 frenPos = evalState[j];
        Vector3 frenVel = evalState[numBoids + j];
        Vector3 deltaPos = boidPos - frenPos;

        // Separation
        accel[i] += deltaPos.normalized * Mathf.Max(0, FlockMinDistance - deltaPos.magnitude) * WeightSeparation / FlockMinDistance;
        // Alignment
        accel[i] += frenVel.normalized * WeightAlignment / (flockIndices.Count - 1);
        // Cohesion
        accel[i] += -deltaPos * WeightCohesion / (flockIndices.Count - 1);
      }

      // Noise
      accel[i] += Random.onUnitSphere * Random.Range(0, WeightNoise);
      // Limit
      accel[i] += WeightAvoidance * -(boidPos - transform.position).normalized * Mathf.Max(0, (boidPos - transform.position).magnitude - BoundaryDistance);

      if (BoundaryFloor)
        accel[i] += WeightAvoidance * Vector3.up * Mathf.Max(0, -(boidPos.y - 0.2f));

      // Speed control
      // Apply wind field to velocity
      if (windField != null) {
        Vector3 wind = windField.GetWind(state[i]);
        boidVel = boidVel - wind;
      }

      accel[i] += BoidSpeedDragConstant * (BoidSpeed - boidVel.magnitude) * boidVel.normalized;
    }


    // Create newState
    Vector3[] newState = new Vector3[numBoids * 2];

    for (int i = 0; i < numBoids; ++i) {
      if (!activeBoids[i]) {
        newState[i] = new Vector3();
        newState[i + numBoids] = new Vector3();
        continue;
      }

      newState[i] = evalState[numBoids + i];
      newState[i + numBoids] = accel[i];
    }

    return newState;
  }

  public override void RenderState() {
    for (int i = 0; i < numBoids; ++i) {
      boidObjects[i].transform.position = state[i];

      if (state[numBoids + i].magnitude > 0.0001f)
        boidObjects[i].transform.rotation =  Quaternion.LookRotation(state[numBoids + i], Vector3.up);
    }
  }

  [Header("Particle Spawn Settings")]
  private float spawnTime = 0f;
  public float spawnRate = 1f;

  public override void ResetParticles() {
    spawnTime += Time.deltaTime;

    if (spawnTime > spawnRate) {
      AddParticle();
      spawnTime -= spawnRate;
    }
  }

  private void ResetParticle(int i) {

    // State is (x, v)
    state[i] = transform.position + Random.onUnitSphere * Random.Range(-SpawnRange, SpawnRange); // x
    state[i + numBoids] = (Random.onUnitSphere + Vector3.up).normalized; // v

    GameObject.Destroy(boidObjects[i]);

    boidObjects[i] = Instantiate(boidObj, state[i], Quaternion.identity, transform);
    boidObjects[i].SetActive(activeBoids[i]);
  }

  private void AddParticle() {
    int j = -1;

    for (int i = 0; i < numBoids; ++i) {
      if (!activeBoids[i]) {
        j = i;
        break;
      }
    }

    if (j == -1)
      return;

    activeBoids[j] = true;
    ResetParticle(j);
  }

  private void RemoveParticle(int i) {
    activeBoids[i] = false;
    boidObjects[i].SetActive(activeBoids[i]);
  }
}
