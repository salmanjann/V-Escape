using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;

public class LoadScreen : MonoBehaviour
{
    private float load_delay;
    private bool freeze;
    private int pillsLoaded;
    public List<GameObject> loadingPills;
    public String previous, next;
    public float delay;
    private AsyncOperation sceneloading;
    private AsyncOperation sceneunloading;
    private bool loaded;
    private bool loading;
    void Start()
    {
        load_delay = 0f;
        freeze = true;
        pillsLoaded = 0;
        loaded = false;
        loading = false;
        sceneloading = null;
        foreach(GameObject loadingPill in loadingPills)
        {
            loadingPill.SetActive(false);
        }
        Invoke("unfreeze",0.4f + load_delay);
        Invoke("Initiate_Load",0f); 
    }
    
    void Update()
    {
        UpdatePills();
        LoadScene();
    }
    private void unfreeze()
    {
        pillsLoaded++;
        freeze = false;
    }
    private void UpdatePills()
    {
        if(freeze)
            return;
        float loadedPercent = 0.8f / 8f;
        float current = loadedPercent;
        foreach(GameObject loadingPill in loadingPills)
        {
            if(!loadingPill.activeInHierarchy && sceneloading.progress >= current)
            {
                loadingPill.SetActive(true);
                freeze = true;
                load_delay += 0.05f;
                Invoke("unfreeze",0.05f + load_delay);
                break;
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
            sceneloading.allowSceneActivation = false;
            // sceneunloading.allowSceneActivation = false;
            next = null;
            previous = null;
            loading = true;
        }
    }

    private void LoadScene()
    {
        if(pillsLoaded<loadingPills.Count)
            return;
        if(freeze)
            return;
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
