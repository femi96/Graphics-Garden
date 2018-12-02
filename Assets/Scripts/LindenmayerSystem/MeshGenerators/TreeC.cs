using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeC : MeshGenerator {

  public override void CreateSystem() {
    lSystem = new LindenmayerSystem("1");
    lSystem.AddRule('+', new LTerminal());
    lSystem.AddRule('-', new LTerminal());
    lSystem.AddRule('*', new LTerminal());
    lSystem.AddRule('[', new LTerminal());
    lSystem.AddRule(']', new LTerminal());
    lSystem.AddRule('F', new LVariable("F*F"));
    lSystem.AddRule('L', new LTerminal());
    lSystem.AddRule('1', new LConditionalStep(new LVariable("F[+1]-1"), "FL", steps));
  }

  public float meshScaleL = 0.1f;
  public float meshScaleW = 0.1f;
  public int angleDeg = 80;
  public int angleDegStr = 5;
  public GameObject leaf;

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

    // LIFO save stack placement
    Vector3 currentPos = Vector3.zero;
    Quaternion currentRot = Quaternion.identity;
    int currentPoint = AddPoint(currentPos, upScld, meshScaleW);

    List<Vector3> positionStack = new List<Vector3>() { currentPos };
    List<Quaternion> rotationStack = new List<Quaternion>() { currentRot };
    List<int> pointStack = new List<int>() { currentPoint };

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

      case 'L':
        // Place Leaf
        GameObject.Instantiate(leaf, transform.position + currentPos, currentRot, transform);
        break;

      case '+':
        // Rotate +angleDeg
        currentRot *= Quaternion.AngleAxis(angleDeg, (Random.onUnitSphere + 3f * Vector3.right));
        break;

      case '-':
        // Rotate -angleDeg
        currentRot *= Quaternion.AngleAxis(-angleDeg, (Random.onUnitSphere + 3f * Vector3.right));
        break;

      case '*':

        // Rotate +-angleDeg
        if (Random.Range(0f, 1f) <= 0.5f)
          currentRot *= Quaternion.AngleAxis(angleDegStr, (Random.onUnitSphere + 5f * Vector3.right));
        else
          currentRot *= Quaternion.AngleAxis(-angleDegStr, (Random.onUnitSphere + 5f * Vector3.right));

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

      default:
        // Do nothing
        break;
      }
    }

    UpdateMesh();
  }
}
