using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }

    [SerializeField]
    private GameState startingState;
    public GameState GameState { get; private set; }
    public LevelManager LevelManager;
    public PlayerManager PlayerManager;
    public DialogueManager DialogueManager;
    public InputManager InputManager;
    public MenuManager MenuManager;
    public SaveLoadManager SaveLoadManager;
    private PlayerController playerController;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }

        GameState = Instantiate(startingState);
        LevelManager.GameState = GameState;
        PlayerManager.GameState = GameState;
    }



}