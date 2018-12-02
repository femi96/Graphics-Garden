using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LConditionalStep : ProductionRule {
  /*
  An L-System conditional variable dependent on system step
  */

  string productionOutputT;
  string productionOutputF;
  int stepBound;

  public LConditionalStep(string outputT, string outputF, int step) {
    productionOutputT = outputT;
    productionOutputF = outputF;
    stepBound = step;
  }

  public string Output(string state, int i, int step) {
    if (step < stepBound) {
      return productionOutputT;
    } else {
      return productionOutputF;
    }
  }
}
