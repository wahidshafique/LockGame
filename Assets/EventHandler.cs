using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHandler : MonoBehaviour {
    public GameObject tensionWrench;
    private Vector3 tensionPos;
    public pickScript hitPick;
    public GlowObject glowPick;
    // Use this for initialization
    void Start() {
        tensionPos = tensionWrench.transform.position;
    }
    // Update is called once per frame
    void Update() {
        ClickPick();
        SnapTension();
    }

    void ClickPick() {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100)) {
                if (hit.collider.tag == "Pick") {
                    print("hitting pick");
                    hitPick.isBeingHit = !hitPick.isBeingHit;
                    if (hitPick.isBeingHit) {
                        glowPick.ColorEnter();
                    } else {
                        glowPick.ColorLeave();
                    }
                }
            }
        }
    }

    void SnapTension() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (hitPick.isBeingHit) {
                Vector3 newPos = new Vector3(tensionPos.x, tensionPos.y - 0.01f, tensionPos.z);
                Vector3 vel = Vector3.zero;
                tensionWrench.transform.position = Vector3.SmoothDamp(transform.position, newPos, ref vel, 0.1f);
            }

        }
    }
}
