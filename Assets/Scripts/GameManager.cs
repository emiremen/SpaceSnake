using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameState gameState;

    private void OnEnable()
    {
        EventManager.GetGameState += GetGameState;
        EventManager.SetGameState += SetGameState;
    }

    private void OnDisable()
    {
        EventManager.GetGameState -= GetGameState;
        EventManager.SetGameState -= SetGameState;
    }

    private void Start()
    {
        gameState = GameState.Paused;
    }

    private GameState GetGameState()
    {
        return gameState;
    }

    private void SetGameState(GameState _gameState)
    {
        gameState = _gameState;
    }
}
