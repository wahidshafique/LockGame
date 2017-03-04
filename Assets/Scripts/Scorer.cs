using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Scorer : MonoBehaviour {
    public static int pickNum;
    public Text text;
    // Use this for initialization
    void Awake() {
      //  text = GetComponent<Text>();
        pickNum = 10;
    }

    // Update is called once per frame
    void Update() {
        text.text = "Picks remaining: " + pickNum;
    }
}
