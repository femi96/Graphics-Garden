using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WindPattern { None, Uniform, Rotation, UniformRotation };

[System.Serializable]
public struct WindLayer {
  public WindPattern pattern;
  public Vector3 windDirection;
  public float windMagnitude;
}

public class WindField : MonoBehaviour {
  /*
  Class that keeps a wind field to apply a force to particles
  */

  public WindLayer[] windLayers;

  void Update() {

  }

  public Vector3 GetWind(Vector3 pos) {
    Vector3 wind = Vector3.zero;

    foreach (WindLayer w in windLayers) {
      switch (w.pattern) {
      case WindPattern.None:
        wind += Vector3.zero;
        break;

      case WindPattern.Uniform:
        wind += w.windDirection.normalized * w.windMagnitude;
        break;

      case WindPattern.Rotation:
        wind += Vector3.Cross(pos, w.windDirection) * w.windMagnitude;
        break;

      case WindPattern.UniformRotation:
        wind += Vector3.Cross(pos, w.windDirection).normalized * w.windMagnitude;
        break;

      default:
        wind += Vector3.zero;
        break;
      }
    }

    return wind;
  }
}
