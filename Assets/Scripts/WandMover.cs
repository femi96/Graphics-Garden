using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandMover : MonoBehaviour {
  /*
  Controller that handles wand movement
  */

  public Transform shape;
  private World world;

  void Start() {
    world = GameObject.Find("World").GetComponent<World>();
  }

  void Update() {
    MoveWand();
  }

  public float moveSpeed = 4f;

  // Move wand each frame
  private void MoveWand() {
    MoveWandFromInput();
    LimitWandPosition();
  }

  // Move wand with inputs
  private void MoveWandFromInput() {

    // Transform input direction based on camera forward
    float spd = moveSpeed * Time.deltaTime;

    Vector3 moveDirection = Camera.main.transform.forward;
    moveDirection.y = 0;

    float x = Input.GetAxis("Horizontal");
    float z = Input.GetAxis("Vertical");

    moveDirection = Vector3.Normalize(moveDirection);   // Don't normalize your inputs
    Vector3 moveDirectionF = moveDirection * z;         // Project z onto forward direction vector
    Vector3 moveDirectionR = new Vector3(moveDirection.z, 0, -moveDirection.x); // Create right vector
    moveDirectionR *= x;                                // Project x onto right direction vector

    moveDirection = moveDirectionF + moveDirectionR;
    moveDirection *= spd;

    // Apply move direction to transform
    transform.Translate(moveDirection.x, 0, moveDirection.z);
  }

  private void LimitWandPosition() {
    transform.position -= Vector3.up * (transform.position.y - Mathf.Max(0, world.GetHeight(transform.position)));
    shape.position = transform.position + Vector3.up * 0.2f;
  }
}