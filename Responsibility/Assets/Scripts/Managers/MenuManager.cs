using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "MenuManager", menuName = "ScriptableObjects/Manager/MenuManager", order = 1)]
public class MenuManager : ScriptableObject
{
    [SerializeField]
    private SceneAsset startScene;

    private GameObject settingsPanel;
    private GameObject background;
    // Start is called before the first frame update
    public void InitiateDialogueMenu(GameObject canvas)
    {
        background = canvas.transform.Find("Background").gameObject;
        settingsPanel = background.transform.Find("SettingsWindow").gameObject;
        settingsPanel.SetActive(false);
    }
    public void NewGame()
    {
        LevelEvents.levelExit.Invoke(startScene, "");
    }

    public void ContinueGame()
    {

    }

    public void LoadGame()
    {

    }

    //[System.Obsolete]
    public void OpenSettings()
    {
        if (settingsPanel.active == false)
        {
            settingsPanel.SetActive(true);
        }
        else 
        {
            settingsPanel.SetActive(false);
        }
    }

    public void ShowAboutInfo()
    {

    }

    public void ExitGame() 
    { 
        Application.Quit();
    }
    /*// Update is called once per frame
    void Update()
    {
        
    }*/
}
