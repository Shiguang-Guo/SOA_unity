using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameModeData", menuName = "GameModeData", order = 1)]
public class GameMode : ScriptableObject
{
    public string modeName;
    public bool isopen;
}