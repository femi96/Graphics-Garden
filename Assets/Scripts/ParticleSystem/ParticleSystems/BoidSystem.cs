using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidSystem : ParticleSystemCustom {

  public int numParticles = 7;

  public GameObject particleObj;
  private GameObject[] particlesObjs;

  public const float BoidSpeed = 1.0f;
  public const float BoidSpeedConstant = 1.0f;

  public const float FlockMinDistance = 1f;
  public const float FlockMaxDistance = 2.5f;

  public const float EdgeLimit = 5f;

  public const float WeightSeparation = 1.0f;
  public const float WeightAlignment = 1.0f;
  public const float WeightCohesion = 1.0f;
  public const float WeightNoise = 0.2f;
  public const float WeightAvoidance = 1.0f;

  public override void CreateState() {

    // Clear child objects
    foreach (Transform child in transform)
      GameObject.Destroy(child.gameObject);


    // State is (x, v)
    state = new Vector3[numParticles * 2];

    for (int i = 0; i < numParticles; ++i)
      state[i] = Random.onUnitSphere * Random.Range(-5, 5); // x

    for (int i = numParticles; i < numParticles * 2; ++i)
      state[i] = Random.onUnitSphere; // v


    // Create render objects
    particlesObjs = new GameObject[numParticles];

    for (int i = 0; i < numParticles; ++i)
      particlesObjs[i] = Instantiate(particleObj, state[i], Quaternion.identity, transform);
  }

  public override Vector3[] EvalF(Vector3[] evalState) {

    // Take state which is (x, v)
    // Output state which is (v, A)


    // A = steer + speed control
    Vector3[] accel = new Vector3[numParticles];

    for (int i = 0; i < numParticles; ++i) {
      accel[i] = new Vector3();
      Vector3 boidPos = evalState[i];
      Vector3 boidVel = evalState[numParticles + i];

      // Steer = separation + alignment + cohesion
      List<int> flockIndices = new List<int>();

      for (int j = 0; j < numParticles; ++j) {
        Vector3 frenPos = evalState[j];
        Vector3 deltaPos = boidPos - frenPos;

        if (deltaPos.magnitude <= FlockMaxDistance) flockIndices.Add(j);
      }

      foreach (int j in flockIndices) {
        if (i == j) continue;

        Vector3 frenPos = evalState[j];
        Vector3 frenVel = evalState[numParticles + j];
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
      accel[i] += WeightAvoidance * -boidPos.normalized * Mathf.Max(0, boidPos.magnitude - EdgeLimit);

      // Speed control
      accel[i] += BoidSpeedConstant * (BoidSpeed - boidVel.magnitude) * boidVel.normalized;
    }


    // Create newState
    Vector3[] newState = new Vector3[numParticles * 2];

    for (int i = 0; i < numParticles; ++i) {
      newState[i] = evalState[numParticles + i];
      newState[i + numParticles] = accel[i];
    }

    return newState;
  }

  public override void RenderState() {
    for (int i = 0; i < numParticles; ++i) {
      particlesObjs[i].transform.position = state[i];

      if (state[numParticles + i].magnitude > 0.0001f)
        particlesObjs[i].transform.rotation =  Quaternion.LookRotation(state[numParticles + i], Vector3.up);
    }
  }

  public override void ResetParticles() {} // Never resets particles
}
