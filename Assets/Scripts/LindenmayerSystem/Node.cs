using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {
  private List<Node> children = new List<Node>();
  private int height = -1;

  public Node() {}

  public int GetHeight() {
    if (height >= 0)
      return height;

    height = 0;

    foreach (Node child in children) {
      height = Mathf.Max(height, child.GetHeight() + 1);
    }

    return height;
  }

  public void AddChild(Node node) {
    children.Add(node);
  }
}
