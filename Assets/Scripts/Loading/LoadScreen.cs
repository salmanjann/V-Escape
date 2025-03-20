using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;

public class LoadScreen : MonoBehaviour
{
    public String previous, next;
    public float delay;
    private AsyncOperation sceneloading;
    private AsyncOperation sceneunloading;
    private bool loaded;
    private bool loading;
    void Start()
    {
        loaded = false;
        loading = false;
        sceneloading = null;
        Invoke("Initiate_Load",0f); 
    }
    
    void Update()
    {
        LoadScene();
    }
    private void Initiate_Load()
    {
        if(next != null)
        {
            sceneloading = SceneManager.LoadSceneAsync(next);
            SceneManager.UnloadScene(previous);
            // sceneunloading = SceneManager.UnloadSceneAsync(previous);
            sceneloading.allowSceneActivation = true;
            // sceneunloading.allowSceneActivation = false;
            next = null;
            previous = null;
            loading = true;
        }
    }

    private void LoadScene()
    {
        if(loaded)
            return;
        if(loading)
        {
            // if(sceneloading.progress >= 0.9f && sceneunloading.progress >= 0.9f)
            // {
            //     Invoke("Loaded",delay);
            // }
            if(sceneloading.progress >= 0.9f)
            {
                Invoke("Loaded",delay);
                loading = false;
            }
        }
    }

    private void Loaded()
    {
        // sceneunloading.allowSceneActivation = true;
        sceneloading.allowSceneActivation = true;
    }
}
