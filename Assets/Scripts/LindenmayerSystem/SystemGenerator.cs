using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SystemGenerator : MonoBehaviour {

  public LindenmayerSystem lSystem;
  public int steps = 4;
  public bool reset = true;

  void Update() {

    if (reset) {
      CreateSystem();
      RunSystem();
      GenerateFromState();

      reset = false;
    }
  }

  /*
  Create L-System with production rules and initiator
  */
  public abstract void CreateSystem();

  /*
  Step L-System steps times
  */
  public void RunSystem() {
    int i = 0;

    while (i < steps) {
      i = lSystem.Step();
    }
  }

  /*
  Generate graphical representation from L-System
  */
  public abstract void GenerateFromState();
}