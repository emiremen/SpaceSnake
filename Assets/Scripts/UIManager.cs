using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] bool resetPlayerPrefsData;
    int score;
    [SerializeField] TextMeshProUGUI scoreTxt;
    [SerializeField] TextMeshProUGUI playerLevelTxt;
    [SerializeField] TextMeshProUGUI pausePanelBestScoreTxt;
    [SerializeField] TextMeshProUGUI gameOverPanelScoreTxt;
    [SerializeField] TextMeshProUGUI gameOverPanelBestScoreTxt;

    [SerializeField] GameObject startPanel;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject pausePanel;


    private void OnEnable()
    {
        EventManager.GainScore += GainScore;
        EventManager.GetScore += GetScore;
        EventManager.ShowGameOverPanel += ShowGameOverPanel;
        EventManager.ShowPausePanel += TogglePausePanel;
    }

    private void OnDisable()
    {
        EventManager.GainScore -= GainScore;
        EventManager.GetScore -= GetScore;
        EventManager.ShowGameOverPanel -= ShowGameOverPanel;
        EventManager.ShowPausePanel -= TogglePausePanel;
    }

    void Start()
    {
        if (resetPlayerPrefsData) PlayerPrefs.DeleteAll();
        UpdateUIData();
        gameOverPanel.SetActive(false);
        pausePanel.SetActive(false);
        startPanel.SetActive(true);
    }

    private void GainScore()
    {
        score++;
        if (score % 20 == 0)
        {
            int _playerLevel = EventManager.GetPlayerLevel.Invoke() + 1;
            EventManager.SetPlayerLevel?.Invoke(_playerLevel);
        }
        UpdateUIData();
    }

    private int GetScore()
    {
        return score;
    }

    public void TogglePausePanel()
    {
        if (EventManager.GetGameState?.Invoke() == GameState.Playing)
        {
            EventManager.SetGameState?.Invoke(GameState.Paused);
            Time.timeScale = 0f;
            pausePanel.SetActive(true);
            pausePanelBestScoreTxt.text = "Best Score: " + PlayerPrefs.GetInt("BestScore");
        }
        else if (EventManager.GetGameState?.Invoke() == GameState.Paused)
        {
            EventManager.SetGameState?.Invoke(GameState.Playing);
            Time.timeScale = 1f;
            pausePanel.SetActive(false);
        }
    }

    private void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true);
        UpdateUIData();
        SaveData();
    }

    public void PlayGameButton()
    {
        EventManager.SetGameState.Invoke(GameState.Playing);
        startPanel.SetActive(false);
    }

    public void RetryGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    void SaveData()
    {
        if (score > PlayerPrefs.GetInt("BestScore")) PlayerPrefs.SetInt("BestScore", score);
    }

    void UpdateUIData()
    {
        scoreTxt.text = "Score: " + score;
        playerLevelTxt.text = "Level: " + EventManager.GetPlayerLevel?.Invoke();
        gameOverPanelScoreTxt.text = "Score: " + score;
        gameOverPanelBestScoreTxt.text = (score > PlayerPrefs.GetInt("BestScore")) ? "*New Best Score*: " + score : "Best Score: " + PlayerPrefs.GetInt("BestScore");
    }

    public void MoveLeftButtonDown()
    {
        EventManager.isMovingLeftWithButton?.Invoke(true);
    }

    public void MoveRightButtonDown()
    {
        EventManager.isMovingRihtWithButton?.Invoke(true);
    }

    public void MoveLeftButtonUp()
    {
        EventManager.isMovingLeftWithButton?.Invoke(false);
    }

    public void MoveRightButtonUp()
    {
        EventManager.isMovingRihtWithButton?.Invoke(false);
    }
}
