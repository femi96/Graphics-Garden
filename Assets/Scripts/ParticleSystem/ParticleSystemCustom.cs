using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ParticleSystemCustom : MonoBehaviour {
  /*
  Particle system abstract class using first order derivatives
  */

  public WindField windField;
  public Vector3[] state;
  public bool reset = true;

  void Update() {

    if (reset) {
      windField = Object.FindObjectOfType<WindField>();
      CreateState();
      reset = false;
    }

    ResetParticles();
    StepRungeKutta4(Time.deltaTime);
    RenderState();
  }

  /*
  Creates the state vector and initial render variables
  */
  public abstract void CreateState();

  /*
  Calculate first derivative of given state
  */
  public abstract Vector3[] EvalF(Vector3[] evalState);

  /*
  Render or change rendered state
  */
  public abstract void RenderState();

  /*
  Reset particles if applicable
  */
  public abstract void ResetParticles();

  /*
  Step the system with Runge-Kutta 4 method
  */
  public void StepRungeKutta4(float stepSize) {
    /*
    RK4 from 6.837 Lecture 8 slides

      k1 = f(xn, tn)
      k2 = f(xn + h/2 k1, tn + h/2)
      k3 = f(xn + h/2 k2, tn + h/2)
      k4 = f(xn + h k3, tn + h)

      xn+1 = xn + h/6 (k1 + 2 k2 + 2 k3 + k4)
      tn+1 = tn + h
    */

    int s = state.Length;
    float h = stepSize;

    // Set k1
    Vector3[] xn = state;
    Vector3[] k1 = EvalF(xn);

    Vector3[] xk1 = new Vector3[s];
    Vector3[] xk2 = new Vector3[s];
    Vector3[] xk3 = new Vector3[s];
    Vector3[] xn1 = new Vector3[s];

    // Set xk1 & k2
    for (int i = 0; i < s; ++i)
      xk1[i] = xn[i] + (k1[i] * (h / 2f));

    Vector3[] k2 = EvalF(xk1);

    // Set xk2 & k3
    for (int i = 0; i < s; ++i)
      xk2[i] = xn[i] + (k2[i] * (h / 2f));

    Vector3[] k3 = EvalF(xk2);

    // Set xk3 & k4
    for (int i = 0; i < s; ++i)
      xk3[i] = xn[i] + (k3[i] * h);

    Vector3[] k4 = EvalF(xk3);

    // Set xn+1
    for (int i = 0; i < s; ++i)
      xn1[i] = xn[i] + ((k1[i] + (k2[i] * 2f) + (k3[i] * 2f) + k4[i]) * (h / 6f));

    state = xn1;
  }
}
