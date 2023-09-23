using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager
{
    public static Func<GameState> GetGameState; 
    public static Action<GameState> SetGameState;

    public static Action GainScore;
    public static Func<int> GetScore;
    public static Action<int> SetPlayerLevel;
    public static Func<int> GetPlayerLevel;
    public static Action ShowGameOverPanel;
    public static Action ShowPausePanel;

    public static Action<bool> isMovingLeftWithButton;
    public static Action<bool> isMovingRihtWithButton;
}
