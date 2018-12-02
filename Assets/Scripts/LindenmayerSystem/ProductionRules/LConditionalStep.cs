using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LConditionalStep : ProductionRule {
  /*
  An L-System conditional variable dependent on system step
  */

  string productionOutputT;
  string productionOutputF;
  ProductionRule ruleT = null;
  ProductionRule ruleF = null;
  int stepBound;

  public LConditionalStep(string outputT, string outputF, int step) {
    productionOutputT = outputT;
    productionOutputF = outputF;
    stepBound = step;
  }

  public LConditionalStep(string outputT, ProductionRule outputF, int step) {
    productionOutputT = outputT;
    ruleF = outputF;
    stepBound = step;
  }

  public LConditionalStep(ProductionRule outputT, string outputF, int step) {
    ruleT = outputT;
    productionOutputF = outputF;
    stepBound = step;
  }

  public string Output(string state, int i, int step) {
    if (step < stepBound) {
      if (ruleT != null)
        return ruleT.Output(state, i, step);

      return productionOutputT;
    } else {
      if (ruleF != null)
        return ruleF.Output(state, i, step);

      return productionOutputF;
    }
  }
}
