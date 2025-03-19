using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
// using UnityEditor.SearchService;
using UnityEngine;

public class ManagerCutscene : MonoBehaviour
{
    protected int scene;
    // private int previous_scene;
    public int scenesCount;
    protected List<GameObject> Scenes;
    protected List<String> Text;
    protected Coroutine nextpage_routine;
    public TMP_Text textmeshpro;
    void Start()
    {
        Scenes = new List<GameObject>();
        Text = new List<string>();
        scene = 0;
        // previous_scene = scene;
        for(int i = 1; i <= scenesCount; i++)
        {
            GameObject scene_found = GameObject.Find(i.ToString());
            Scenes.Add(scene_found);
            scene_found.SetActive(false);
        }
        Scenes[scene].SetActive(true);
        nextpage_routine = StartCoroutine("goNext");
        initialize_Text();
        textmeshpro.text = Text[scene];
    }

    protected virtual void initialize_Text()
    {
        for(int i = 0; i < scenesCount; i++)
        {
            Text.Add("");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space))
        {
            nextpage();
        }
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            previouspage();
        }
    }
    protected void nextpage()
    {
        if(scene + 1 < scenesCount)
        {
            StopCoroutine(nextpage_routine);
            Scenes[scene].SetActive(false);
            scene++;
            Scenes[scene].SetActive(true);
            nextpage_routine = StartCoroutine("goNext");
            textmeshpro.text = Text[scene];
        }
    }
    protected void previouspage()
    {
        if(scene - 1 >= 0)
        {
            StopCoroutine(nextpage_routine);
            Scenes[scene].SetActive(false);
            scene--;
            Scenes[scene].SetActive(true);
            nextpage_routine = StartCoroutine("goNext");
            textmeshpro.text = Text[scene];
        }
    }

    protected IEnumerator goNext()
    {
        yield return new WaitForSecondsRealtime(10f);
        nextpage();
    }
}
