using System.Collections;
using UnityEngine;

public class pickScript : MonoBehaviour {
    public GameObject tensionWrench;

    private bool isBeingHit = false;
    private bool isBeingRotated = false;
    private bool cracked = false;
    private bool canPick = true;

    //ranges for the locks opening positons
    private int lowRange;
    private int highRange;
    [Range(20, 90)]
    public int stepCount = 20; //how large the sweet spot for the lock will be

    private Transform parentTrans; //this pick object is childed to this
    private GameObject pickObject;
    private GlowObject glowPick; //for debug
    private float angleThresh = 90.0f; //the parent game object can only rotate to these angles on X
    Vector3 v; //the final mouse pos

    //define the gradients to when you hit the true "sweet spot" of the lock
    private struct Ranges {
        public int FarOff;
        public int Within;
        public int Close;
        public int ReallyClose;
        public int JackPot;

        public Ranges(int steps) : this() {
            FarOff = steps;
            Within = steps / 2;
            Close = steps / 3;
            ReallyClose = steps / 4;
            JackPot = steps / 5;
        }
    }

    private Ranges ranges;

    void Start() {
        ranges = new Ranges(stepCount);
        parentTrans = transform.parent;
        glowPick = GetComponentInChildren<GlowObject>();
        //range = SweetSpotRange(0, 5);
        //this is just the pick (for changing colors, etc)
        pickObject = transform.GetChild(0).gameObject;
        lowRange = (int)Mathf.Floor(Random.Range(-angleThresh + stepCount, angleThresh - stepCount));
        highRange = lowRange + stepCount;
    }

    void Update() {
        SnapTension();
        if (isBeingHit && canPick) {
            RotatePick();
        }
        if (isBeingRotated) {
            RotateLock();
        } else {
            ResetLock();
        }
    }

    void RotateLock() {
        canPick = false;
        //Vector3 pEulerRot = parentTrans.rotation.eulerAngles;
        //parentTrans.eulerAngles = Vector3.Lerp(pEulerRot, new Vector3(1, pEulerRot.y, pEulerRot.z - 90), Time.deltaTime);
        Vector3 parentRot = parentTrans.localRotation.eulerAngles;
        if (parentRot.x > 50) {
            parentTrans.RotateAround(parentTrans.position, transform.right, 1f);
        } else {
            LockCracked();
        }
    }

    void ResetLock() {
        canPick = false;

        Vector3 parentRot = parentTrans.localRotation.eulerAngles;
        if (parentRot.x < 90) {
            parentTrans.RotateAround(parentTrans.position, -transform.right, 1f);
        } else {
            canPick = true;
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
        checkIfCracked(floored);

    }

    void checkIfCracked(int pickPosition) {
        //see if you are within the random range to crack the lock
        //int key = stepCount / 2;
        int nPos = pickPosition - lowRange;
        print(nPos);
        //print(nPos);
        if (nPos > 0 && nPos < ranges.FarOff) {
            pickObject.GetComponent<Renderer>().material.color = Color.cyan;
            //print("IN RANGE");
            if (nPos > ranges.Close && nPos < ranges.Within) {
                pickObject.GetComponent<Renderer>().material.color = Color.yellow;
                print("jack pot is : " + ranges.JackPot); 
                if (nPos == ranges.JackPot * 2) {
                    print("WINNER");
                    pickObject.GetComponent<Renderer>().material.color = Color.red;
                }
            }
        } else {
            pickObject.GetComponent<Renderer>().material.color = Color.blue;
        }
    }




    /*
    if (pickPosition > lowRange && pickPosition < highRange) {
        //pickpos - lowrange

        print(pickPosition - lowRange);

        glowPick.ColorSuccess();
        cracked = true;
        pickObject.GetComponent<Renderer>().material.color = Color.red;
    } else {
        cracked = false;
        //pickObject.GetComponent <
        pickObject.GetComponent<Renderer>().material.color = Color.blue;
        glowPick.ColorEnter();
    }*/

    void LockCracked() {
        if (cracked) {
            print("WIN!");
        } else {
            print("FAIL!");
        }
    }

    void OnMouseDown() {
        isBeingHit = !isBeingHit;
        if (isBeingHit) {
            glowPick.ColorEnter();
        } else {
            glowPick.ColorLeave();
        }
    }

    void SnapTension() {
        //when you press space, make the wrench pivot a little bit
        if (isBeingHit) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                isBeingRotated = true;
                tensionWrench.transform.Translate(Vector3.forward * 0.01f);
                //hitPick
            }
            if (Input.GetKeyUp(KeyCode.Space)) {
                isBeingRotated = false;
                tensionWrench.transform.Translate(-Vector3.forward * 0.01f);
            }
        }
    }
}