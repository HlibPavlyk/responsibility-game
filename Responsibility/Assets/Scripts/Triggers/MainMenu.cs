using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{

    public GameObject SettingsPanel;
    // Start is called before the first frame update
    public void NewGame()
    {
        //SceneManager.LoadScene
    }

    public void ContinueGame()
    {

    }

    public void LoadGame()
    {

    }

    public void OpenSettings()
    {
        if (SettingsPanel.active == false)
        {
            SettingsPanel.SetActive(true);
        }
        else 
        {
            SettingsPanel.SetActive(false);
        }
    }

    public void ShowAboutInfo()
    {

    }

    public void ExitGame() 
    { 
        Application.Quit();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
