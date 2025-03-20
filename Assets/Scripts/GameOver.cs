using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public Text mainMenu;
    public Text retry;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MainMenu()
    {
        mainMenu.color = Color.red;
        retry.color = Color.white;
    }

    public void Retry()
    {
        retry.color = Color.red;
        mainMenu.color = Color.white;

    }
}
