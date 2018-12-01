using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingParticleSystem : ParticleSystemCustom {

  public int numParticles = 4;
  private bool[] activeParticles;

  private GameObject[] particlesObjs;
  public GameObject[] prefabs;

  [Header("Boundary Settings")]
  public float boundaryFloor = 0f;
  public float boundaryDrag = 0.5f;

  [Header("Force Settings")]
  public float gravity = 10.0f;
  public float particleMass = 1.0f;
  public float particleDrag = 0.2f;
  public float particleNormalDrag = 0.8f;
  public float particleMoment = 0.1f;

  [Header("Particle Spawn Settings")]
  public float spawnRange = 1f;
  private float spawnTime = 0f;
  public float spawnRate = 1f;

  public override void CreateState() {

    // Clear child objects
    foreach (Transform child in transform)
      GameObject.Destroy(child.gameObject);


    // Active boids
    activeParticles = new bool[numParticles];

    for (int i = numParticles; i < numParticles; ++i)
      activeParticles[i] = false;


    // State is (x, v, normal, v_ang)
    state = new Vector3[numParticles * 4];
    particlesObjs = new GameObject[numParticles];

    for (int i = 0; i < numParticles; ++i)
      ResetParticle(i);
  }

  private void ResetParticle(int i) {

    // State is (x, v, normal, v_ang)
    state[i] = transform.position + Random.onUnitSphere * Random.Range(-spawnRange, spawnRange); // x
    state[i + numParticles] = new Vector3(); // v
    state[i + numParticles * 2] = (Random.onUnitSphere + Vector3.up * 0f).normalized; // normal
    state[i + numParticles * 3] = new Vector3(); // v_ang

    GameObject.Destroy(particlesObjs[i]);

    particlesObjs[i] = Instantiate(prefabs[Random.Range(0, prefabs.Length)], state[i], Quaternion.identity, transform);
    particlesObjs[i].SetActive(activeParticles[i]);
  }

  public override Vector3[] EvalF(Vector3[] evalState) {

    // Take state which is (x, v)
    // Output state which is (v, F/M)


    // F = -mg -dv -kx
    Vector3[] force = new Vector3[numParticles];
    Vector3[] moment = new Vector3[numParticles];

    // Apply particle forces
    for (int i = 0; i < numParticles; ++i) {
      Vector3 vel = evalState[numParticles + i];
      Vector3 velNormal = Vector3.Project(vel, state[i + numParticles * 2]);
      force[i] = Vector3.zero;
      force[i] += new Vector3(0, -gravity * particleMass, 0);
      force[i] += -particleDrag * (vel - velNormal);
      force[i] += -particleNormalDrag * velNormal;

      moment[i] = particleMoment * -(vel - velNormal).normalized;

      // Apply wind field
      if (windField != null) {
        force[i] += windField.GetWind();
      }
    }

    // Create newState
    Vector3[] newState = new Vector3[numParticles * 4];

    for (int i = 0; i < numParticles; ++i) {
      newState[i] = evalState[numParticles + i];
      newState[i + numParticles] = force[i] / particleMass;
      newState[i + numParticles * 2] = state[i + numParticles * 3];
      newState[i + numParticles * 3] = moment[i] / particleMass;
    }

    for (int i = 0; i < numParticles; ++i) {
      if (state[i].y <= boundaryFloor) {
        state[i] += new Vector3(0, boundaryFloor - state[i].y, 0);
        newState[i] += new Vector3(0, -newState[i].y, 0);
        newState[i + numParticles] = -newState[i] * boundaryDrag;
      }

      state[i + numParticles * 2] += state[i + numParticles * 2].normalized;
    }

    return newState;
  }

  public override void RenderState() {
    for (int i = 0; i < numParticles; ++i) {
      particlesObjs[i].transform.position = state[i];
      particlesObjs[i].transform.rotation = Quaternion.LookRotation(state[i + numParticles * 2], Vector3.up);
    }
  }

  public override void ResetParticles() {
    spawnTime += Time.deltaTime;

    if (spawnTime > spawnRate) {
      AddParticle();
      spawnTime -= spawnRate;
    }
  }

  private int nextAdd = 0;

  private void AddParticle() {
    activeParticles[nextAdd] = true;
    ResetParticle(nextAdd);
    nextAdd = (nextAdd + 1) % numParticles;
  }
}
