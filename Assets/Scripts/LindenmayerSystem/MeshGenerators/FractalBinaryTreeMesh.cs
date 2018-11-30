using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FractalBinaryTreeMesh : MeshGenerator {

  public override void CreateSystem() {
    lSystem = new LindenmayerSystem("0");
    lSystem.AddRule('[', new LTerminal());
    lSystem.AddRule(']', new LTerminal());
    lSystem.AddRule('0', new LVariable("1[0]0"));
    lSystem.AddRule('1', new LVariable("11"));
  }

  // Game objects
  public GameObject segment;

  // LIFO save stack placement
  private Vector3 currentPos;
  private Quaternion currentRot;

  private List<Vector3> positionStack;
  private List<Quaternion> rotationStack;

  public override void GenerateFromState() {
    CreateMesh();

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
        AddSegment(currentPos, currentPos + currentRot * Vector3.up, 0.25f, 0.25f);
        currentPos += currentRot * Vector3.up;
        AddSegment(currentPos, currentPos + (currentRot * Vector3.up) / 3f, 0.25f, 0.0f);
        break;

      case '1':
        // Draw line segment
        AddSegment(currentPos, currentPos + currentRot * Vector3.up, 0.25f, 0.25f);
        currentPos += currentRot * Vector3.up;
        break;

      case '[':
        // Save placement, rotate 45deg
        positionStack.Add(currentPos);
        rotationStack.Add(currentRot);
        currentRot *= Quaternion.AngleAxis(45, Vector3.right);
        break;

      case ']':
        // Load placement, rotate -45deg
        currentPos = positionStack[stackEnd];
        currentRot = rotationStack[stackEnd];
        positionStack.RemoveAt(stackEnd);
        rotationStack.RemoveAt(stackEnd);
        currentRot *= Quaternion.AngleAxis(-45, Vector3.right);
        break;
      }
    }

    UpdateMesh();
  }
}
