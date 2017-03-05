using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Menu : MonoBehaviour {
    public bool manualLoad = false;

    void Update() {
        if (manualLoad)
            SceneManager.LoadScene("Main");
    }

    public void Play() {
        SceneManager.LoadScene("Difficult");
        print("play");
    }
    public void Quit() {
        Application.Quit();
    }
}
