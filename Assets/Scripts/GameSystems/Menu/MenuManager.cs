using System.Collections;
using System.Collections.Generic;
using Core.Events;
using Core.Interfaces;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using VContainer;

[CreateAssetMenu(fileName = "MenuManager", menuName = "ScriptableObjects/Manager/MenuManager", order = 1)]
public class MenuManager : ScriptableObject
{
    [SerializeField]
    private string startSceneName;

    private GameObject settingsPanel;
    private GameObject background;
    
    [Inject] private GameState gameState;
    
    // Start is called before the first frame update
    public void InitiateDialogueMenu(GameObject canvas)
    {
        background = canvas.transform.Find("Background").gameObject;
        settingsPanel = background.transform.Find("SettingsWindow").gameObject;
        settingsPanel.SetActive(false);
    }
    public void NewGame()
    {
        SaveLoadManager.DeleteSaves();
        GameEvents.Level.levelExit.Invoke(startSceneName, "");
        //LevelEvents.levelExit.Invoke("Hall", "");
        gameState.playerStats.currentSceneName = startSceneName;

    }


    public void ContinueGame()
    {
        SaveLoadManager.LoadGame();
        GameEvents.Level.levelExit.Invoke(gameState.playerStats.currentSceneName, "");
        /*SceneManager.LoadScene(GameManager.Instance.PlayerManager.PlayerStats.currentSceneName, LoadSceneMode.Single);*/
        /*SceneManager.LoadScene(GameManager.Instance.PlayerManager.PlayerStats.currentSceneIndex*//**//*, LoadSceneMode.Single);*/
    }

    public void LoadGame()
    {

    }

    [System.Obsolete]
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
