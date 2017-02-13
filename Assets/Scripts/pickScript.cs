using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickScript : MonoBehaviour {

    void Start() {

    }

    void Update() {
        rotatePick();
    }

    void rotatePick() {
        Vector3 mousePos = Input.mousePosition;
        mousePos.x -= Screen.width / 2;
        mousePos += transform.position * 2;
        float angle = Vector3.Angle(mousePos, Vector3.up);
        if (mousePos.x > 0) {
            angle = 360 - angle;
        }
        Debug.Log(this.transform.rotation.eulerAngles);
        this.transform.rotation = Quaternion.Euler(angle, 0, 0);
    }
}
