using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class GameManager : MonoBehaviour
{

    /*public static GameManager Instance { get; private set; }

    [SerializeField]
    //private GameState startingState;
    //public GameState GameState { get; private set; }
   //  public LevelManager LevelManager;
   // public PlayerManager PlayerManager;
   // public DialogueManager DialogueManager;
   // public InputManager InputManager;
    //public MenuManager MenuManager;
    //private PlayerController playerController;
    
    [Inject] private GameState gameState;

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
       // gameState = startingState;
        /*gameState = Instantiate(startingState);
        LevelManager.GameState = GameState;#1#
       // PlayerManager.GameState = GameState;
    }*/



}