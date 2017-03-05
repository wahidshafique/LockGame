using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {
    public Transform target;
    Animator anim;
	// Use this for initialization
	void Start () {
        Scorer.startTimer = false;
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void LateUpdate () {
        transform.LookAt(target);
        if (Input.GetKeyDown(KeyCode.Space)) {
            anim.speed = 4.0f;
        }
	}

    void StartTime() {
        Scorer.startTimer = true;
    }
}
