using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ProductionRule {
  /*
  Procution Rule of an L-System, mapped to from chars in the L-System

  Rule Types:

  Terminals - Terminals or constants in L-System
  Variables - Variables in L-System
  Probabilistic Variables - Probabilistic varibles in L-System
  */

  string Output(string state, int i, int step);
}
