using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidSystem : ParticleSystemCustom {

  public int numBoids = 20;

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

  [Header("Factor Weight Settings")]
  public float WeightSeparation = 1.0f;
  public float WeightAlignment = 1.0f;
  public float WeightCohesion = 1.0f;
  public float WeightNoise = 0.2f;
  public float WeightAvoidance = 1.0f;

  public override void CreateState() {

    // Clear child objects
    foreach (Transform child in transform)
      GameObject.Destroy(child.gameObject);


    // State is (x, v)
    state = new Vector3[numBoids * 2];

    for (int i = 0; i < numBoids; ++i)
      state[i] = Random.onUnitSphere * Random.Range(-5, 5); // x

    for (int i = numBoids; i < numBoids * 2; ++i)
      state[i] = Random.onUnitSphere; // v


    // Create render objects
    boidObjects = new GameObject[numBoids];

    for (int i = 0; i < numBoids; ++i)
      boidObjects[i] = Instantiate(boidObj, state[i], Quaternion.identity, transform);
  }

  public override Vector3[] EvalF(Vector3[] evalState) {

    // Take state which is (x, v)
    // Output state which is (v, A)


    // A = steer + speed control
    Vector3[] accel = new Vector3[numBoids];

    for (int i = 0; i < numBoids; ++i) {
      accel[i] = new Vector3();
      Vector3 boidPos = evalState[i];
      Vector3 boidVel = evalState[numBoids + i];

      // Steer = separation + alignment + cohesion
      List<int> flockIndices = new List<int>();

      for (int j = 0; j < numBoids; ++j) {
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
      accel[i] += WeightAvoidance * -boidPos.normalized * Mathf.Max(0, boidPos.magnitude - BoundaryDistance);
      accel[i] += WeightAvoidance * Vector3.up * Mathf.Max(0, -(boidPos.y - 1));

      // Speed control
      accel[i] += BoidSpeedDragConstant * (BoidSpeed - boidVel.magnitude) * boidVel.normalized;
    }


    // Create newState
    Vector3[] newState = new Vector3[numBoids * 2];

    for (int i = 0; i < numBoids; ++i) {
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

  public override void ResetParticles() {} // Never resets particles
}
