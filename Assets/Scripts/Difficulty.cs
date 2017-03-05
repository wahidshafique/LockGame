using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Difficulty : MonoBehaviour {
    public enum Diff { Easy, Medium, Hard }

    public void Easy() {
        Scorer.setDiff(6, 45, Diff.Easy);
        SceneManager.LoadScene("Main");
    }

    public void Medium() {
        Scorer.setDiff(8, 65, Diff.Medium);
        SceneManager.LoadScene("Main");
    }

    public void Hard() {
        Scorer.setDiff(10, 75, Diff.Hard);
        SceneManager.LoadScene("Main");
    }
    public void Back() {
        SceneManager.LoadScene("Menu");
    }
}
