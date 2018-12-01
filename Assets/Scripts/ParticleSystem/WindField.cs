using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WindPattern { None, Uniform };

public class WindField : MonoBehaviour {
  /*
  Class that keeps a wind field to apply a force to particles
  */

  public WindPattern pattern;
  public Vector3 windDirection;

  void Update() {

  }

  public Vector3 GetWind() {
    switch (pattern) {
    case WindPattern.Uniform:
      return windDirection;

    default:
      return Vector3.zero;
    }
  }
}
