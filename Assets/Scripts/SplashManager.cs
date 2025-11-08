using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SplashManager : MonoBehaviour
{
    [Header("Settings")]
    public float autoLoadDelay = 1.5f;
    public string nextSceneName = "Main";

    private bool canSkip = false;

    void Start()
    {
        StartCoroutine(EnableSkip());
        StartCoroutine(AutoLoad());
    }

    void Update()
    {
        if (canSkip)
        {
            // PC: Mouse click
            if (Input.GetMouseButtonDown(0))
            {
                LoadNextScene();
            }

            // Mobile: Touch
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                LoadNextScene();
            }
        }
    }

    IEnumerator EnableSkip()
    {
        yield return new WaitForSeconds(0.5f);
        canSkip = true;
    }

    IEnumerator AutoLoad()
    {
        yield return new WaitForSeconds(autoLoadDelay);
        LoadNextScene();
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}