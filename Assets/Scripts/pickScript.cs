using System.Collections;
using UnityEngine;

public class pickScript : MonoBehaviour {
    public bool isBeingHit = false;
    //this pick object is childed to this
    private GameObject pickObject;
    //this is the rotund end of the pick, it glows (for debug?)
    private GlowObject glowPick;
    //the parent game object can only rotate to these angles on X
    private float angleThresh = 90.0f;
    //the final mouse pos
    Vector3 v;
    int[] range;

    void Start() {
        glowPick = GetComponentInChildren<GlowObject>();
        range = SweetSpotRange(0, 5);
        //this is just the pick (for changing colors, etc)
        //needs to be seperate so it has different anchor
        pickObject = transform.GetChild(0).gameObject;
    }

    void Update() {
        if (isBeingHit) {
            RotatePick();
        }
    }

    void RotatePick() {
        Vector3 mousePos = Input.mousePosition;
        //reduce the mouses positional range
        mousePos.x -= Screen.width / 2;
        mousePos.y -= Screen.height / 2;
        v = mousePos + v / 2;

        float angleRads = Mathf.Atan2(v.x, v.y);
        float angleDegs = angleRads * Mathf.Rad2Deg;
        if (angleDegs > angleThresh) {
            angleDegs = angleThresh;
        } else if (angleDegs < -angleThresh) {
            angleDegs = -angleThresh;
        }
        this.transform.rotation = Quaternion.Euler(angleDegs, 0, 0);
        int floored = (int)Mathf.Floor(angleDegs);



        foreach (int x in range) {
            print(x);
            if (x.Equals(floored)) {
                glowPick.ColorSuccess();
                Debug.Log("Find the int ..." + x);
                pickObject.GetComponent<Renderer>().material.color = Color.red;
                return;
            } else {
                pickObject.GetComponent<Renderer>().material.color = Color.blue;
                glowPick.ColorEnter();
            }
        }
    }

    int[] SweetSpotRange(int sweetSpot, int steps) {
        int[] sweetSpotRanger = new int[steps];
        sweetSpotRanger[0] = sweetSpot;
        for (int i = 1; i < sweetSpotRanger.Length; i++) {
            sweetSpotRanger[i] = sweetSpotRanger[i - 1] + 1;
            print(sweetSpotRanger[i]);
        }
        return sweetSpotRanger;
    }
}
