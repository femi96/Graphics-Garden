using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WindPattern { None, Uniform, Rotation, UniformRotation, SinusoidTime };

[System.Serializable]
public struct WindLayer {
  public WindPattern pattern;
  public Vector3 windDirection;
  public float windMagnitude;
  public Vector3 windCenter;
  public float windMax;
}

public class WindField : MonoBehaviour {
  /*
  Class that keeps a wind field to apply a force to particles
  */

  public WindLayer[] windLayers;

  private float time = 0f;

  void Update() {
    time += Time.deltaTime;
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
        Vector3 windTemp = Vector3.Cross(pos - w.windCenter, w.windDirection) * w.windMagnitude;
        wind += windTemp * Mathf.Min(1,  w.windMax / windTemp.magnitude);
        break;

      case WindPattern.SinusoidTime:
        wind += w.windDirection.normalized * Mathf.Sin(time * w.windDirection.magnitude) * w.windMagnitude;
        break;

      case WindPattern.UniformRotation:
        wind += Vector3.Cross(pos - w.windCenter, w.windDirection).normalized * w.windMagnitude;
        break;

      default:
        wind += Vector3.zero;
        break;
      }
    }

    return wind;
  }
}
