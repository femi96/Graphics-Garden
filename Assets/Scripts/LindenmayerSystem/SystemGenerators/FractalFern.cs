using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FractalFern : SystemGenerator {

  public override void CreateSystem() {
    lSystem = new LindenmayerSystem("X");
    lSystem.AddRule('+', new LTerminal());
    lSystem.AddRule('-', new LTerminal());
    lSystem.AddRule('[', new LTerminal());
    lSystem.AddRule(']', new LTerminal());
    lSystem.AddRule('F', new LVariable("FF"));
    lSystem.AddRule('X', new LVariable("F+[[X]-X]-F[-FX]+X"));
  }

  // Game objects
  public GameObject segment;
  public int angleDeg = 25;

  // LIFO save stack placement
  private Vector3 currentPos;
  private Quaternion currentRot;

  private List<Vector3> positionStack;
  private List<Quaternion> rotationStack;

  public override void GenerateFromState() {

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

      case 'F':
        // Draw line segment
        GameObject.Instantiate(segment, currentPos, currentRot, transform);
        currentPos += currentRot * new Vector3(1, 0, 0);
        break;

      case 'X':
        // Do nothing
        break;

      case '+':
        // Rotate +angleDeg
        currentRot *= Quaternion.AngleAxis(angleDeg, Vector3.up);
        break;

      case '-':
        // Rotate -angleDeg
        currentRot *= Quaternion.AngleAxis(-angleDeg, Vector3.up);
        break;

      case '[':
        // Save placement
        positionStack.Add(currentPos);
        rotationStack.Add(currentRot);
        break;

      case ']':
        // Load placement
        currentPos = positionStack[stackEnd];
        currentRot = rotationStack[stackEnd];
        positionStack.RemoveAt(stackEnd);
        rotationStack.RemoveAt(stackEnd);
        break;
      }
    }
  }
}
