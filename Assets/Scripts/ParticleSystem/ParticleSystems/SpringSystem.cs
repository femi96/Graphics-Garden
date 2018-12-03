using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringSystem : ParticleSystemCustom {

  public int numParticles = 4;
  private int[] fixedParticles = new int[] {0};
  private List<int> springPairs;

  private GameObject[] particlesObjs;
  public GameObject particleObj;

  [Header("Spring Settings")]
  public float Gravity = 10.0f;

  public float ParticleMass = 1.0f;
  public float ParticleDrag = 0.1f;

  public float SpringConstant = 10.0f;
  public float SpringDistance = 2.5f;

  public override void CreateState() {

    // Clear child objects
    foreach (Transform child in transform)
      GameObject.Destroy(child.gameObject);

    // State is (x, v)
    state = new Vector3[numParticles * 2];
    particlesObjs = new GameObject[numParticles];

    for (int i = 0; i < numParticles; ++i)
      ResetParticle(i);

    SetupSprings();
  }

  public void ResetParticle(int i) {

    // State is (x, v)
    state[i] = transform.position + Random.onUnitSphere * 0.1f; // x
    state[i + numParticles] = Vector3.zero; // v

    GameObject.Destroy(particlesObjs[i]);

    particlesObjs[i] = Instantiate(particleObj, state[i], Quaternion.identity, transform);
  }

  public void SetupSprings() {
    // Add springs
    springPairs = new List<int>();

    for (int i = 0; i < numParticles - 1; ++i) {
      springPairs.Add(i);
      springPairs.Add(i + 1);
    }
  }

  public override Vector3[] EvalF(Vector3[] evalState) {

    // Take state which is (x, v)
    // Output state which is (v, F/M)


    // F = -mg -dv -kx
    Vector3[] force = new Vector3[numParticles];

    // Apply particle forces
    for (int i = 0; i < numParticles; ++i) {
      force[i] = new Vector3();
      force[i] += new Vector3(0, -Gravity * ParticleMass, 0);

      Vector3 vel = evalState[numParticles + i];

      // Apply wind field to velocity
      if (windField != null) {
        Vector3 wind = windField.GetWind(state[i]);
        vel = vel - wind;
      }

      force[i] += -ParticleDrag * vel;
    }

    // Apply spring forces
    for (int i = 0; i < springPairs.Count; i += 2) {

      int particleIndex1 = springPairs[i];
      int particleIndex2 = springPairs[i + 1];

      Vector3 particle1 = evalState[particleIndex1];
      Vector3 particle2 = evalState[particleIndex2];

      Vector3 p1top2 = particle2 - particle1;
      Vector3 p2top1 = particle1 - particle2;

      float dist = p1top2.magnitude;
      float deltaForce = -SpringConstant * (dist - SpringDistance);

      force[particleIndex1] += deltaForce * p2top1 / dist;
      force[particleIndex2] += deltaForce * p1top2 / dist;
    }


    // Create newState
    Vector3[] newState = new Vector3[numParticles * 2];

    for (int i = 0; i < numParticles; ++i) {
      newState[i] = evalState[numParticles + i];
      newState[i + numParticles] = force[i] / ParticleMass;
    }

    foreach (int i in fixedParticles) {
      state[i] = transform.position;
      newState[i] = new Vector3();
      newState[i + numParticles] = new Vector3();
    }

    return newState;
  }

  public override void RenderState() {
    for (int i = 0; i < numParticles; ++i) {
      particlesObjs[i].transform.position = state[i];

      if (state[numParticles + i].magnitude > 0.0001f && i > 0)
        particlesObjs[i].transform.rotation = Quaternion.LookRotation(state[i - 1] - state[i], Vector3.up);
    }
  }

  public override void ResetParticles() {} // Never resets particles
}
