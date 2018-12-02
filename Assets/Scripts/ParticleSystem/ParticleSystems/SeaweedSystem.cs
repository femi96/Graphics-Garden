using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaweedSystem : RopeSystem {

  public override void RenderState() {
    for (int i = 0; i < numParticles; ++i) {
      particlesObjs[i].transform.position = state[i];

      if (state[numParticles + i].magnitude > 0.0001f) {
        Vector3 v = state[numParticles + i];
        Vector3 d = new Vector3(Mathf.Max(0.1f, Mathf.Abs(v.x)), Mathf.Max(0.1f, Mathf.Abs(v.y)), Mathf.Max(0.1f, Mathf.Abs(v.z)));
        particlesObjs[i].transform.rotation = Quaternion.LookRotation(d, Vector3.up);
      }
    }
  }
}
