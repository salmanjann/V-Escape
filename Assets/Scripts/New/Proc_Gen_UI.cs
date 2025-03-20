using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Proc_Gen_UI : MonoBehaviour
{
    [SerializeField] private TMP_Text currentFloor;
    [SerializeField] private TMP_Text remainingKeys;
    [SerializeField] private TMP_Text remainingKeysText;

    public void UpdateCurrentFloor(int _currentFloor){
        currentFloor.text = _currentFloor.ToString();
    }

    public void UpdateRemainingKeys(int _remainingKeys){
        remainingKeys.text = _remainingKeys.ToString();
    }

    public void UpdateKeysText(string _text){
        remainingKeysText.text = _text;
        remainingKeys.text = ".";
    }
}
