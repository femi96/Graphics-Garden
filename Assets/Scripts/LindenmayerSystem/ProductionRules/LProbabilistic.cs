using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LProbabilistic : ProductionRule {
  /*
  An L-System probabilistic
  */

  string productionOutputT;
  string productionOutputF;
  float c;

  public LProbabilistic(string outputT, string outputF, float chance) {
    productionOutputT = outputT;
    productionOutputF = outputF;
    c = chance;
  }

  public string Output(string state, int i, int step) {
    if (Random.Range(0f, 1f) < c) {
      return productionOutputT;
    } else {
      return productionOutputF;
    }
  }
}
