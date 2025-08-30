using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;

public class OpeningSequence : MonoBehaviour
{
    public SpriteRenderer blackFade;
    public TMPro.TextMeshPro pressAnyKey;

    public string sceneToGo;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        blackFade.DOColor(Color.clear, 3.0f);

        InputSystem.onAnyButtonPress.CallOnce(currentAction =>
        {
            StartCoroutine(LoadScene(sceneToGo));
        });
    }

    public IEnumerator LoadScene(string scene)
    {
        yield return blackFade.DOColor(Color.black, 0.7f)
            .WaitForCompletion();
        
        var load = SceneManager.LoadSceneAsync(scene);

        while (!load.isDone)
        {
            yield return null;
        }
    }

    private void Awake()
    {
        blackFade.color = Color.black;
    }

    private void Update()
    {
        Color c = pressAnyKey.color;
        c.a = Mathf.Pow(0.5f + 0.5f * Mathf.Sin(Time.time * 3.0f), 0.5f);
        pressAnyKey.color = c;
    }
}
