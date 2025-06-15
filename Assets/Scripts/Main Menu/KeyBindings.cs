using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class KeyBindings : MonoBehaviour
{
    public Text up;
    public Text down;
    public Text left;
    public Text right;
    public Text jump;
    public Text action;

    void Start()
    {
        up.text = PlayerPrefs.GetString("Up", "W");
        down.text = PlayerPrefs.GetString("Down", "S");
        left.text = PlayerPrefs.GetString("Left", "A");
        right.text = PlayerPrefs.GetString("Right", "D");
        jump.text = PlayerPrefs.GetString("Jump", "Space");
        action.text = PlayerPrefs.GetString("Action", "E");
    }

    void Update()
    {
        if (up.text == "Input") WaitForKey("Up", up);
        else if (down.text == "Input") WaitForKey("Down", down);
        else if (left.text == "Input") WaitForKey("Left", left);
        else if (right.text == "Input") WaitForKey("Right", right);
        else if (jump.text == "Input") WaitForKey("Jump", jump);
        else if (action.text == "Input") WaitForKey("Action", action);
    }

    void WaitForKey(string keyName, Text targetText)
    {
        foreach (KeyCode keycode in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(keycode))
            {
                // Prevent duplicate keys
                if (IsKeyAlreadyUsed(keycode.ToString()))
                {
                    Debug.Log("Key already assigned!");
                    return;
                }

                targetText.text = keycode.ToString();
                PlayerPrefs.SetString(keyName, keycode.ToString());
                PlayerPrefs.Save();
                break;
            }
        }
    }

    bool IsKeyAlreadyUsed(string key)
    {
        return
            key == up.text ||
            key == down.text ||
            key == left.text ||
            key == right.text ||
            key == jump.text ||
            key == action.text;
    }

    public void Up_KeyChange() => up.text = "Input";
    public void Down_KeyChange() => down.text = "Input";
    public void Left_KeyChange() => left.text = "Input";
    public void Right_KeyChange() => right.text = "Input";
    public void Jump_KeyChange() => jump.text = "Input";
    public void Action_KeyChange() => action.text = "Input";
}
