using Core.DI;
using ResponsibilityGame.Core.Interfaces;
using UnityEngine;
using VContainer;

namespace ResponsibilityGame.GameSystems.Menu
{
    [InjectableMonoBehaviour]
    public class MenuManagerBehaviour : MonoBehaviour
    {
        private GameObject settingsPanel;
        private GameObject background;
        
        [Inject] private IMenuManager levelManager;

        private void Awake()
        {
            background = gameObject.transform.Find("Background").gameObject;
            settingsPanel = background.transform.Find("SettingsWindow").gameObject;
            settingsPanel.SetActive(false);
        }
        
        public void NewGame() => levelManager.NewGame();
        public void ContinueGame() => levelManager.ContinueGame();
        public void LoadGame() => Debug.Log("LoadGame");
        
        public void OpenSettings()
        {
            settingsPanel.SetActive(settingsPanel.activeSelf == false);
        }

        public void ShowAboutInfo()
        {
            Debug.Log("ShowAboutInfo");
        }

        public void ExitGame() 
        { 
            Application.Quit();
        }
    }
}