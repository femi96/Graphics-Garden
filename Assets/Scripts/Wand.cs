using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Wand : MonoBehaviour {

  public Text text;
  private World world;

  void Start() {
    world = GameObject.Find("World").GetComponent<World>();
  }

  void Update() {
    Vector3 v = transform.position;
    string str = "";
    str += "Ht. " + Mathf.RoundToInt(world.GetHeight(v)).ToString() + "\n";
    str += "Hm. " + Mathf.RoundToInt(world.GetHumidity(v)).ToString() + "\n";
    str += "Temp. " + Mathf.RoundToInt(world.GetTemperature(v)).ToString() + "\n";
    str += "Biome: " + world.GetBiome(v).ToString();
    text.text = str;
  }
}
