using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LVariable : ProductionRule {
  /*
  An L-System variable
  */

  string productionOutput;

  public LVariable(string output) {
    productionOutput = output;
  }

  public string Output(string state, int i, int step) {
    return productionOutput;
  }
}
