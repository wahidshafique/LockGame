using System.Collections;
using UnityEngine;

public class PickScript : MonoBehaviour {
    private Rigidbody prb;//parents rig body
    private Rigidbody rb;

    public GameObject tensionWrench;

    public bool gameover = false;
    private bool isBeingHit = false;
    private bool isBeingRotated = false;
    private bool cracked = false;
    private bool canPick = true;

    //ranges for the locks opening positons
    private int lowRange;
    private int highRange;
    [Range(20, 90)]
    public int stepCount = 20; //how large the sweet spot for the lock will be
    private int lockTwistAmount = 0;//this controls how much the lock rotates
    //(cont)..if you get it to twist fully, it is open

    private Transform parentTrans; //this pick object is childed to this
    private GameObject pickObject;
    Renderer por;
    private GlowObject glowPick; //for debug
    private float angleThresh = 90.0f; //the parent game object can only rotate to these angles on X
    Vector3 v; //the final mouse pos
    private MovieTexture movt;

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
    //debug
    bool toggle = true;

    void Start() {
        movt = ((MovieTexture)transform.root.GetComponentInChildren<Renderer>().material.mainTexture);

        ranges = new Ranges(stepCount);
        parentTrans = transform.parent;
        glowPick = GetComponentInChildren<GlowObject>();
        prb = transform.root.GetComponent<Rigidbody>();
        rb = GetComponent<Rigidbody>();

        //this is just the pick (for changing colors, etc)
        pickObject = transform.GetChild(0).gameObject;
        por = pickObject.GetComponent<Renderer>();
        lowRange = (int)Mathf.Floor(Random.Range(-angleThresh + stepCount, angleThresh - stepCount));
        highRange = lowRange + stepCount;
    }

    void Update() {
        int pickPos;
        if (!gameover) {
            SnapTension();
            if (isBeingHit && canPick) {
                pickPos = RotatePick();
                cracked = checkIfCracked(pickPos);
            }
            if (isBeingRotated) {
                RotateLock(lockTwistAmount);
            } else {
                ResetLock();
            }
        }
        //DEBUG
        if (Input.GetKeyDown(KeyCode.C)) {
            toggle = !toggle;
        }
        if (toggle) {
            por.material.color = Color.white;
        }
    }

    void RotateLock(int pos) {
        canPick = false;
        Vector3 parentRot = parentTrans.localRotation.eulerAngles;
        if (parentRot.x > pos) {
            parentTrans.RotateAround(parentTrans.position, transform.right, 1f);
        } else {
            LockCracked();
        }
    }

    void ResetLock() {
        canPick = false;
        Vector3 parentRot = parentTrans.localRotation.eulerAngles;
        //print("parent rot " + parentRot);
        if (parentRot.x < 90) {
            parentTrans.RotateAround(parentTrans.position, -transform.right, 1f);
        } else {
            canPick = true;
        }
    }

    int RotatePick() {
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
        return floored;
    }

    bool checkIfCracked(int pickPosition) {
        //see if you are within the random range to crack the lock
        int nPos = pickPosition - lowRange;
        if (nPos > 0 && nPos < ranges.FarOff) {
            lockTwistAmount = nPos * 4;
            por.material.color = Color.cyan;
            if (Scorer.currDiff == Difficulty.Diff.Easy) {
                return true;
            }
            if (nPos > ranges.Close && nPos < ranges.Within) {
                lockTwistAmount = nPos;
                por.material.color = Color.yellow;
                print("jack pot is : " + ranges.JackPot);
                if (Scorer.currDiff == Difficulty.Diff.Medium) {
                    return true;
                }
                if (nPos == ranges.JackPot * 2) {
                    lockTwistAmount = 0;
                    print("CLICK!");
                    por.material.color = Color.red;
                    if (Scorer.currDiff == Difficulty.Diff.Hard) {
                        return true;
                    }
                }
            }
        } else {
            por.material.color = Color.blue;
        }
        return false;
    }

    void LockCracked() {
        if (cracked) {
            print("WIN!");
            gameover = true;
            FindObjectOfType<Scorer>().WinGame();
            StartCoroutine(WinnerRotate());
        } else {
            print("FAIL!");
            //StartCoroutine(BreakWait());
            GameOver(true);
        }
    }

    IEnumerator WinnerRotate() {
        while (true) {
            print(transform.root.rotation.eulerAngles.z);
            if (transform.root.rotation.eulerAngles.z < 180) {
                transform.root.Rotate(0, 0, Mathf.Sin(50 * Time.deltaTime));
                if (movt.isReadyToPlay && !movt.isPlaying) {
                    movt.Play();
                    movt.loop = true;
                }
            }
            yield return null;
        }
    }

    IEnumerator BreakWait() {
        yield return new WaitForSeconds(1.0f);
        GameOver(true);
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
                lockTwistAmount = Random.Range(45, 90);
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

    public bool GameOver(bool g) {
        gameover = g;
        Scorer.pickNum--;
        FindObjectOfType<Scorer>().CheckPicks();
        StartCoroutine(WaitForNextRound());
        return gameover;
    }

    IEnumerator WaitForNextRound() {
        Vector3 resPos = transform.position;
        GameObject instatiatable = this.gameObject;
        print("waiting..");
        rb.isKinematic = false;
        rb.useGravity = true;
        yield return new WaitForSeconds(3);
        if (Scorer.pickNum > 0) {
            //createGhost(instatiatable, resPos, transform.rotation);
            isBeingRotated = false;
            transform.position = resPos;
            transform.rotation = Quaternion.identity;
            rb.isKinematic = true;
            rb.useGravity = false;
            gameover = false;
        } else {
            lostAllLives();

        }
    }

    void createGhost(GameObject insG, Vector3 insPos, Quaternion insRot) {
        Component[] comps = insG.GetComponents(typeof(Component));
        foreach (Component i in comps) {
            if (i.GetType() != typeof(Transform)) {
                Destroy(i);
            }
        }
        insG.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        Instantiate(insG, insPos, insRot);
    }

    void lostAllLives() {

        //this is a hard game over...
        Vector3 presPos = transform.root.position;
        prb.isKinematic = false;

    }
}
