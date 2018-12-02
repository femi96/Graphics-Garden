using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SierpinskiTriangleMesh : MeshGenerator {

  public override void CreateSystem() {
    lSystem = new LindenmayerSystem("F-G-G");
    lSystem.AddRule('-', new LTerminal());
    lSystem.AddRule('+', new LTerminal());
    lSystem.AddRule('F', new LVariable("F-G+F+G-F"));
    lSystem.AddRule('G', new LVariable("GG"));
  }

  public int angleDeg = 120;
  public float meshScaleL = 0.1f;
  public float meshScaleW = 0.1f;

  // LIFO save stack placement
  private Vector3 currentPos;
  private Quaternion currentRot;
  private int currentPoint;

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

    foreach (char c in state) {
      Vector3 upRel = currentRot * upScld;
      int p0, p1;

      switch (c) {

      case 'F':
        // Draw line segment with leaf
        p0 = currentPoint;

        p1 = AddPoint(currentPos + upRel, upRel, meshScaleW);
        AddSegment(p0, p1);

        currentPos += upRel;
        currentPoint = p1;
        break;

      case 'G':
        // Draw line segment
        p0 = currentPoint;

        p1 = AddPoint(currentPos + upRel, upRel, meshScaleW);
        AddSegment(p0, p1);

        currentPos += currentRot * upScld;
        currentPoint = p1;
        break;

      case '+':
        // Rotate angleDeg
        currentRot *= Quaternion.AngleAxis(angleDeg, Vector3.right);
        break;

      case '-':
        // Rotate -angleDeg
        currentRot *= Quaternion.AngleAxis(-angleDeg, Vector3.right);
        break;
      }
    }

    UpdateMesh();
  }
}
