using Harryanto.CookingGame.Customer;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public delegate void GameManagerDelegate(bool spawnerState);
    public static event GameManagerDelegate SetCustomerSpawnerState;
    
    public delegate void AddCustomerDelegate(int extraCustomerToSpawn);
    public static event AddCustomerDelegate AddExtraCustomerToSpawn;

    public delegate void RestartDelegate();
    public static event RestartDelegate RestartCustomerServer;
    public static event RestartDelegate RestartCustomerSpawner;

    [SerializeField] private float _gameDuration;
    private float _defaultGameDuration;
    private bool _isWin;

    [SerializeField] private GameObject _endGamePanel;
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private GameObject _restartPanel;
    [SerializeField] private Button _restartButtonOnGameOverPanel;
    [SerializeField] private Button _backButtonOnGameOverPanel;
    [SerializeField] private Button _restartButtonOnRestartPanel;
    [SerializeField] private Button _backButtonOnRestartPanel;
    [SerializeField] private TMP_InputField _extraCustomerToSpawn;
    [SerializeField] private TMP_Text _gameTimerTMP;

    private void Awake()
    {
        _gameTimerTMP.SetText($"Timer: {_gameDuration:F0}");
        _defaultGameDuration = _gameDuration;
    }

    private void Start()
    {
        _restartButtonOnGameOverPanel.onClick.RemoveAllListeners();
        _backButtonOnGameOverPanel.onClick.RemoveAllListeners();
        _restartButtonOnRestartPanel.onClick.RemoveAllListeners();
        _backButtonOnRestartPanel.onClick.RemoveAllListeners();

        _restartButtonOnGameOverPanel.onClick.AddListener(ShowRestartPanel);
        _backButtonOnGameOverPanel.onClick.AddListener(GoToLevelSelect);
        _restartButtonOnRestartPanel.onClick.AddListener(RestartGame);
        _backButtonOnRestartPanel.onClick.AddListener(CloseRestartPanel);

        CustomerServer.OnAllCustomerServed += CustomerServer_OnAllCustomerServed;
        CustomerServer.UpdateGameWinState += CustomerServer_UpdateGameWinState;
    }

    private void OnDestroy()
    {
        CustomerServer.OnAllCustomerServed -= CustomerServer_OnAllCustomerServed;
        CustomerServer.UpdateGameWinState -= CustomerServer_UpdateGameWinState;
    }

    private void FixedUpdate()
    {
        if (_gameDuration > 0)
        {
            _gameDuration -= Time.fixedDeltaTime;
            _gameTimerTMP.SetText($"Timer: {_gameDuration:F0}");
        }
        else
        {
            SetCustomerSpawnerState(false);
        }
    }

    private void RestartGame()
    {
        if (!string.IsNullOrEmpty(_extraCustomerToSpawn.text))
        {
            int count = int.Parse(_extraCustomerToSpawn.text);
            if (count >= 1)
            {
                Time.timeScale = 1;
                _restartPanel.SetActive(false);
                _endGamePanel.SetActive(false);
                AddExtraCustomerToSpawn(count);
                RestartCustomerServer();
                RestartCustomerSpawner();
                SetCustomerSpawnerState(true);
                _gameDuration = _defaultGameDuration;
            }
        }
        else
        {
            Debug.Log(_extraCustomerToSpawn + " must be filled");
        }
    }

    private void ShowRestartPanel()
    {
        _gameOverPanel.SetActive(false);
        _restartPanel.SetActive(true);
    }

    private void GoToLevelSelect()
    {
        SceneManager.LoadScene("LevelSelect");
    }

    private void CloseRestartPanel()
    {
        _restartPanel.SetActive(false);
        _gameOverPanel.SetActive(true);
    }

    private void CustomerServer_UpdateGameWinState(bool gameState)
    {
        _isWin = gameState;

        Time.timeScale = 0;

        if (_isWin)
        {
            GoToLevelSelect();
        }
        else
        {
            _endGamePanel.SetActive(true);
            _gameOverPanel.SetActive(true);
        }
    }

    private void CustomerServer_OnAllCustomerServed()
    {
        _endGamePanel.SetActive(true);
        _gameOverPanel.SetActive(true);
        SetCustomerSpawnerState(false);
    }
}
