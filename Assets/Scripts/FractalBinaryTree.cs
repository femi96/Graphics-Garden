using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FractalBinaryTree : MonoBehaviour {

  public GameObject segment;
  public GameObject leaf;

  public int steps = 3;
  public bool reset = true;

  void Update() {

    if (reset) {
      // Create L-System with production rules
      LindenmayerSystem lSystem = new LindenmayerSystem("0");
      lSystem.AddRule('[', new LTerminal());
      lSystem.AddRule(']', new LTerminal());
      lSystem.AddRule('0', new LVariable("1[0]0"));
      lSystem.AddRule('1', new LVariable("11"));

      // Step L-System steps times
      int i = 0;

      while (i < steps) {
        i = lSystem.Step();
      }

      // Generate mesh from L-System
      GenerateFromState(lSystem);
      reset = false;
    }
  }

  // LIFO save stack placement
  private Vector3 currentPos;
  private Quaternion currentRot;

  private List<Vector3> positionStack;
  private List<Quaternion> rotationStack;

  private void GenerateFromState(LindenmayerSystem lSystem) {

    foreach (Transform child in transform) {
      GameObject.Destroy(child.gameObject);
    }

    string state = lSystem.GetState();
    currentPos = new Vector3();
    currentRot = Quaternion.identity;

    positionStack = new List<Vector3>() { currentPos };
    rotationStack = new List<Quaternion>() { currentRot };

    foreach (char c in state) {
      int stackEnd = positionStack.Count - 1;

      switch (c) {

      case '0':
        // Draw line segment with leaf
        GameObject.Instantiate(segment, currentPos, currentRot, transform);
        currentPos += currentRot * new Vector3(1, 0, 0);
        GameObject.Instantiate(leaf, currentPos, currentRot, transform);
        break;

      case '1':
        // Draw line segment
        GameObject.Instantiate(segment, currentPos, currentRot, transform);
        currentPos += currentRot * new Vector3(1, 0, 0);
        break;

      case '[':
        // Save placement, rotate 45deg
        positionStack.Add(currentPos);
        rotationStack.Add(currentRot);
        currentRot *= Quaternion.AngleAxis(45, Vector3.up);
        break;

      case ']':
        // Load placement, rotate -45deg
        currentPos = positionStack[stackEnd];
        currentRot = rotationStack[stackEnd];
        positionStack.RemoveAt(stackEnd);
        rotationStack.RemoveAt(stackEnd);
        currentRot *= Quaternion.AngleAxis(-45, Vector3.up);
        break;
      }
    }
  }
}
