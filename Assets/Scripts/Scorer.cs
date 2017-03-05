using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;


public class Scorer : MonoBehaviour {
    public static int pickNum = 3;
    public Text text;

    public static float timeAmt = 15;
    private float time;

    public Text timerText;
    public Image fillImage;

    public static bool startTimer = false;

    public static Difficulty.Diff currDiff = Difficulty.Diff.Easy;

    void Awake() {
        time = timeAmt;
    }

    void Update() {
        ReturnToMain();
        if (startTimer) {
            Timer();
        }
    }

    void Timer() {
        if (time > 0) {
            time -= Time.deltaTime;
            fillImage.fillAmount = time / timeAmt;
            timerText.text = time.ToString("F");
            if (time >= timeAmt / 2 - 0.1f && time <= timeAmt / 2 + 0.1f) {
                fillImage.color = new Color32(255, 255, 0, 255);
            }
            if (time >= timeAmt / 3 - 0.1f && time <= timeAmt / 3 + 0.1f) {
                fillImage.color = new Color32(255, 0, 0, 255);
            }
        } else {
            timerText.text = "Times up!";
            pickNum = 0;
            if (!FindObjectOfType<PickScript>().gameover) {
                FindObjectOfType<PickScript>().GameOver(true);
            }
        }
    }

    public static void setDiff(int pNum, int tAmt, Difficulty.Diff d) {
        timeAmt = tAmt;
        pickNum = pNum;
        currDiff = d;
    }

    public void CheckPicks() {
        if (pickNum > 0) {
            text.text = "Picks remaining: " + pickNum;
        } else {
            text.text = "You lost all of your lives, press esc to return";
        }
    }

    public void WinGame() {
        text.text = "Congratulations!!!! (Press esc to return to main)";
    }

    void ReturnToMain() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            SceneManager.LoadScene("Menu");
        }
    }
}
