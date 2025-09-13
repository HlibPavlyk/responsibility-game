using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
[CreateAssetMenu(fileName = "GameState", menuName = "ScriptableObjects/GameState", order =1 )]
public class GameState : ScriptableObject
{
    //Implement incapsulation
    public string playerSpawnLocation = "";
}
