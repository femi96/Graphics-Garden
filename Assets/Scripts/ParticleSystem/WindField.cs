using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WindPattern { None, Uniform, Rotation, UniformRotation };

public class WindField : MonoBehaviour {
  /*
  Class that keeps a wind field to apply a force to particles
  */

  public WindPattern pattern;
  public Vector3 windDirection;
  public float windMagnitude;

  void Update() {

  }

  public Vector3 GetWind(Vector3 pos) {
    switch (pattern) {
    case WindPattern.Uniform:
      return windDirection.normalized * windMagnitude;

    case WindPattern.Rotation:
      return Vector3.Cross(pos, windDirection) * windMagnitude;

    case WindPattern.UniformRotation:
      return Vector3.Cross(pos, windDirection).normalized * windMagnitude;

    default:
      return Vector3.zero;
    }
  }
}
