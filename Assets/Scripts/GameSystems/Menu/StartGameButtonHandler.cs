using ResponsibilityGame.Core.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;
using VContainer;

public class StartGameButtonHandler : MonoBehaviour
{
    /*[Inject] private IMenuManager _menuManager;


    private void OnClickStart()
    {
        _menuManager.NewGame(); // тут викликаєш метод сервісу
    }*/
    
    [Inject] private IMenuManager _menuManager;

    [SerializeField] private Button button;

    void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();

        button.onClick.AddListener(OnClickStart);
    }

    private void OnClickStart()
    {
        _menuManager.NewGame(); // тут викликаєш метод сервісу
    }
}
