using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Proc_Gen_UI : MonoBehaviour
{
    [SerializeField] private TMP_Text currentFloor;
    [SerializeField] private TMP_Text remainingKeys;
    [SerializeField] private TMP_Text remainingKeysText;
    [SerializeField] private Image trapDoorImg;
    
    [SerializeField] private GameObject objectivePanel;

    public void DisplayObjective()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        objectivePanel.SetActive(true);
        Time.timeScale = 0f; // Pause the game
    }

    public void HideObjective()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        objectivePanel.SetActive(false);
        Time.timeScale = 1f; // Resume the game
    }
    public void UpdateCurrentFloor(int _currentFloor)
    {
        currentFloor.text = _currentFloor.ToString();
    }

    public void UpdateRemainingKeys(int _remainingKeys, bool firstFloor = false)
    {
        if (firstFloor)
            remainingKeys.text = "";
        else
            remainingKeys.text = _remainingKeys.ToString();
    }

    public void UpdateKeysText(string _text)
    {
        remainingKeysText.text = _text;
        remainingKeys.text = ".";
    }

    public void UpdateTrapDoorImage(string color, bool isFirstFloor = false)
    {
        if (color == "red")
            trapDoorImg.color = Color.red;
        else
            trapDoorImg.color = Color.green;

        // if (isFirstFloor)
        // {
        //     RectTransform rt = trapDoorImg.GetComponent<RectTransform>();
        //     Vector2 currentPos = rt.anchoredPosition;
        //     rt.sizeDelta = new Vector2(350f, 10f);
        //     rt.anchoredPosition = new Vector2(0f, currentPos.y -15f);
        // }
    }
}
