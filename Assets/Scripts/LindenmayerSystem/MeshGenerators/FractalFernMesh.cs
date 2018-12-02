using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FractalFernMesh : MeshGenerator {

  public override void CreateSystem() {
    lSystem = new LindenmayerSystem("X");
    lSystem.AddRule('+', new LTerminal());
    lSystem.AddRule('-', new LTerminal());
    lSystem.AddRule('[', new LTerminal());
    lSystem.AddRule(']', new LTerminal());
    lSystem.AddRule('F', new LVariable("FF"));
    lSystem.AddRule('X', new LVariable("F+[[X]-X]-F[-FX]+X"));
  }

  public float meshScaleL = 0.1f;
  public float meshScaleW = 0.1f;
  public int angleDeg = 25;

  // LIFO save stack placement
  private Vector3 currentPos;
  private Quaternion currentRot;
  private int currentPoint;

  private List<Vector3> positionStack;
  private List<Quaternion> rotationStack;
  private List<int> pointStack;

  private List<float> CalculateWidths(string state) {

    List<Node> nodes = new List<Node>();
    Node currentNode = new Node();
    List<Node> nodeStack = new List<Node>() { currentNode };

    foreach (char c in state) {
      switch (c) {

      case 'F':
        Node nNode = new Node();
        nodes.Add(nNode);
        currentNode.AddChild(nNode);
        currentNode = nNode;
        break;

      case '[':
        // Save placement
        nodeStack.Add(currentNode);
        break;

      case ']':
        // Load placement
        currentNode = nodeStack[nodeStack.Count - 1];
        nodeStack.RemoveAt(nodeStack.Count - 1);
        break;

      default:
        break;
      }
    }

    float maxWidth = (float)nodes[0].GetHeight();
    List<float> frac = new List<float>();

    foreach (Node j in nodes) {
      frac.Add((float)j.GetHeight() / maxWidth);
    }

    return frac;
  }

  public override void GenerateFromState() {
    CreateMesh();

    foreach (Transform child in transform) {
      GameObject.Destroy(child.gameObject);
    }

    string state = lSystem.GetState();
    Vector3 upScld = Vector3.up * meshScaleL;

    // Calculate part widths by preprocessing state
    List<float> widths = CalculateWidths(state);
    int currentWidth = 0;

    currentPos = new Vector3();
    currentRot = Quaternion.identity;
    currentPoint = AddPoint(currentPos, upScld, meshScaleW);

    positionStack = new List<Vector3>() { currentPos };
    rotationStack = new List<Quaternion>() { currentRot };
    pointStack = new List<int>() { currentPoint };

    foreach (char c in state) {
      int stackEnd = positionStack.Count - 1;
      Vector3 upRel = currentRot * upScld;
      int p0, p1;

      switch (c) {

      case 'F':
        // Draw line segment
        p0 = currentPoint;

        p1 = AddPoint(currentPos + upRel, upRel, widths[currentWidth] * meshScaleW);
        AddSegment(p0, p1);

        currentWidth += 1;
        currentPos += currentRot * upScld;
        currentPoint = p1;
        break;

      case 'X':
        // Do nothing
        break;

      case '+':
        // Rotate +angleDeg
        currentRot *= Quaternion.AngleAxis(angleDeg, Vector3.right);
        break;

      case '-':
        // Rotate -angleDeg
        currentRot *= Quaternion.AngleAxis(-angleDeg, Vector3.right);
        break;

      case '[':
        // Save placement
        positionStack.Add(currentPos);
        rotationStack.Add(currentRot);
        pointStack.Add(currentPoint);
        break;

      case ']':
        // Load placement
        currentPos = positionStack[stackEnd];
        currentRot = rotationStack[stackEnd];
        currentPoint = pointStack[stackEnd];
        positionStack.RemoveAt(stackEnd);
        rotationStack.RemoveAt(stackEnd);
        pointStack.RemoveAt(stackEnd);
        break;
      }
    }

    UpdateMesh();
  }
}
