using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour {
    [Tooltip("the key that restarts")]
    [SerializeField] KeyCode m_RestartKey;

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(m_RestartKey)) {
            var activeScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(activeScene.buildIndex);
        }
    }
}