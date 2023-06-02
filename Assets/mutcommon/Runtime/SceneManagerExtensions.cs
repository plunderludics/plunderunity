using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MutCommon
{
    public class SceneManagerExtensions : MonoBehaviour
    {
        public void LoadSceneByName(string name)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(name, LoadSceneMode.Single);
        }
    }
}
