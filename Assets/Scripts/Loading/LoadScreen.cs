using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;

public class LoadScreen : MonoBehaviour
{
    public List<GameObject> loadingPills;
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
        foreach(GameObject loadingPill in loadingPills)
        {
            loadingPill.SetActive(false);
        }
        Invoke("Initiate_Load",0f); 
    }
    
    void Update()
    {
        UpdatePills();
        LoadScene();
    }
    private void UpdatePills()
    {
        float loadedPercent = 0.9f / 8f;
        float current = loadedPercent;
        foreach(GameObject loadingPill in loadingPills)
        {
            if(sceneloading.progress >= current)
            {
                loadingPill.SetActive(true);
            }
            current += loadedPercent;
        }
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
