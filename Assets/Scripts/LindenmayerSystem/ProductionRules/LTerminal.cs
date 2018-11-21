using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LTerminal : ProductionRule {
  /*
  An L-System terminal
  */

  public LTerminal() {}

  public string Output(string state, int i) {
    return state[i].ToString();
  }
}