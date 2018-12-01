using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetalSystem : ParticleSystemCustom {

  public int numParticles = 9;
  private List<int> fixedParticles;
  private List<Vector3> fixedRelative;

  private List<int> springPairs;
  private List<float> springDistances;

  private GameObject[] particlesObjs;
  public GameObject particleObj;

  [Header("Spring Settings")]
  public float Gravity = 10.0f;

  public float ParticleMass = 0.1f;
  public float ParticleDrag = 0.1f;

  public float SpringConstant = 10.0f;
  public float SpringDistance = 1.0f;

  public override void CreateState() {

    // Clear child objects
    foreach (Transform child in transform)
      GameObject.Destroy(child.gameObject);

    // State is (x, v)
    state = new Vector3[numParticles * 2];
    particlesObjs = new GameObject[numParticles / 3];
    fixedParticles = new List<int>();
    fixedRelative = new List<Vector3>();

    for (int i = 0; i < numParticles / 3; ++i)
      ResetParticle(i);

    SetupSprings();
  }

  public void ResetParticle(int i) {

    // State is (x, v)
    int j = 3 * i;
    Vector3 dir = Random.onUnitSphere;
    state[j] = transform.position + dir * SpringDistance * 1; // x
    state[j + numParticles] = Vector3.zero; // v
    fixedParticles.Add(j);
    fixedRelative.Add(dir * SpringDistance * 1);

    state[j + 1] = transform.position + dir * SpringDistance * 2; // x
    state[j + numParticles + 1] = Vector3.zero; // v
    fixedParticles.Add(j + 1);
    fixedRelative.Add(dir * SpringDistance * 2);

    state[j + 2] = transform.position + dir * SpringDistance * 3; // x
    state[j + numParticles + 2] = Vector3.zero; // v

    GameObject.Destroy(particlesObjs[i]);

    particlesObjs[i] = Instantiate(particleObj, state[j + 2], Quaternion.identity, transform);
  }

  public void SetupSprings() {
    // Add springs
    springPairs = new List<int>();
    springDistances = new List<float>();

    for (int i = 0; i < numParticles / 3; ++i) {
      springPairs.Add(i * 3);
      springPairs.Add(i * 3 + 2);
      springDistances.Add(SpringDistance * 2);
      springPairs.Add(i * 3 + 1);
      springPairs.Add(i * 3 + 2);
      springDistances.Add(SpringDistance);
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
      force[i] += -ParticleDrag * evalState[numParticles + i];
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
      float deltaForce = -SpringConstant * (dist - springDistances[i / 2]);

      force[particleIndex1] += deltaForce * p2top1 / dist;
      force[particleIndex2] += deltaForce * p1top2 / dist;
    }


    // Create newState
    Vector3[] newState = new Vector3[numParticles * 2];

    for (int i = 0; i < numParticles; ++i) {
      newState[i] = evalState[numParticles + i];
      newState[i + numParticles] = force[i] / ParticleMass;
    }

    for (int j = 0; j < fixedParticles.Count; ++j) {
      int i = fixedParticles[j];
      state[i] = transform.position + fixedRelative[j];
      newState[i] = new Vector3();
      newState[i + numParticles] = new Vector3();
    }

    return newState;
  }

  public override void RenderState() {
    for (int i = 0; i < numParticles / 3; ++i) {
      particlesObjs[i].transform.position = state[i * 3 + 2];
      particlesObjs[i].transform.rotation = Quaternion.LookRotation(state[i * 3 + 2] - transform.position, Vector3.up);
    }
  }

  public override void ResetParticles() {} // Never resets particles
}
