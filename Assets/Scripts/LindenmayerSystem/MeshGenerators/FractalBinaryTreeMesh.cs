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

  public float meshScaleL = 0.1f;
  public float meshScaleW = 0.1f;

  // LIFO save stack placement
  private Vector3 currentPos;
  private Quaternion currentRot;
  private int currentPoint;

  private List<Vector3> positionStack;
  private List<Quaternion> rotationStack;
  private List<int> pointStack;

  public override void GenerateFromState() {
    CreateMesh();

    foreach (Transform child in transform) {
      GameObject.Destroy(child.gameObject);
    }

    string state = lSystem.GetState();
    Vector3 upScld = Vector3.up * meshScaleL;

    currentPos = new Vector3();
    currentRot = Quaternion.identity;
    currentPoint = AddPoint(currentPos, upScld, meshScaleW);

    positionStack = new List<Vector3>() { currentPos };
    rotationStack = new List<Quaternion>() { currentRot };
    pointStack = new List<int>() { currentPoint };

    foreach (char c in state) {
      int stackEnd = positionStack.Count - 1;
      Vector3 upRel = currentRot * upScld;
      int p0, p1, p2;

      switch (c) {

      case '0':
        // Draw line segment with leaf
        p0 = currentPoint;

        p1 = AddPoint(currentPos + upRel, upRel, meshScaleW);
        AddSegment(p0, p1);

        currentPos += upRel;
        currentPoint = p1;

        p2 = AddPoint(currentPos + upRel * 1f, upRel, 0.001f);
        AddSegment(p1, p2);
        break;

      case '1':
        // Draw line segment
        p0 = currentPoint;

        p1 = AddPoint(currentPos + upRel, upRel, meshScaleW);
        AddSegment(p0, p1);

        currentPos += currentRot * upScld;
        currentPoint = p1;
        break;

      case '[':
        // Save placement, rotate 45deg
        positionStack.Add(currentPos);
        rotationStack.Add(currentRot);
        pointStack.Add(currentPoint);
        currentRot *= Quaternion.AngleAxis(45, Vector3.right);
        break;

      case ']':
        // Load placement, rotate -45deg
        currentPos = positionStack[stackEnd];
        currentRot = rotationStack[stackEnd];
        currentPoint = pointStack[stackEnd];
        positionStack.RemoveAt(stackEnd);
        rotationStack.RemoveAt(stackEnd);
        pointStack.RemoveAt(stackEnd);
        currentRot *= Quaternion.AngleAxis(-45, Vector3.right);
        break;
      }
    }

    UpdateMesh();
  }
}
