using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LindenmayerSystem {
  /*
  Implementation of LindenmayerSystem or L-System for procedural fractal generation

  L-System state is represented by state string.
  */

  public int steps;
  private string state;
  private Dictionary<char, ProductionRule> productionRules;

  public LindenmayerSystem(string init) {
    steps = 0;
    state = init;
    productionRules = new Dictionary<char, ProductionRule>();
  }

  public void AddRule(char c, ProductionRule rule) {
    /*
    Add production rule to L-System
    */
    productionRules.Add(c, rule);
  }

  public string GetState() {
    /*
    Returns current L-System state
    */
    return state;
  }

  public int Step() {
    /*
    Increment L-System state by one step and return new step number
    */

    steps += 1;

    string newState = string.Empty;
    char c;

    for (int i = 0; i < state.Length; i++) {
      c = state[i];
      newState += productionRules[c].Output(state, i, steps);
    }

    state = newState;
    return steps;
  }
}