using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SierpinskiTriangle : SystemGenerator {

  public override void CreateSystem() {
    lSystem = new LindenmayerSystem("F-G-G");
    lSystem.AddRule('-', new LTerminal());
    lSystem.AddRule('+', new LTerminal());
    lSystem.AddRule('F', new LVariable("F-G+F+G-F"));
    lSystem.AddRule('G', new LVariable("GG"));
  }

  // Game objects
  public GameObject segment;
  public int angleDeg = 120;

  // Draw placement
  private Vector3 currentPos;
  private Quaternion currentRot;

  public override void GenerateFromState() {

    foreach (Transform child in transform) {
      GameObject.Destroy(child.gameObject);
    }

    string state = lSystem.GetState();
    currentPos = new Vector3();
    currentRot = Quaternion.identity;

    foreach (char c in state) {

      switch (c) {

      case 'F':
        // Draw line segment with leaf
        GameObject.Instantiate(segment, currentPos, currentRot, transform);
        currentPos += currentRot * new Vector3(1, 0, 0);
        break;

      case 'G':
        // Draw line segment
        GameObject.Instantiate(segment, currentPos, currentRot, transform);
        currentPos += currentRot * new Vector3(1, 0, 0);
        break;

      case '+':
        // Rotate angleDeg
        currentRot *= Quaternion.AngleAxis(angleDeg, Vector3.up);
        break;

      case '-':
        // Rotate -angleDeg
        currentRot *= Quaternion.AngleAxis(-angleDeg, Vector3.up);
        break;
      }
    }
  }
}
