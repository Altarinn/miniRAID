using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatSceneLoader : MonoBehaviour
{
    public string sceneName;

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.loadedSceneCount <= 1)
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }
    }
}
