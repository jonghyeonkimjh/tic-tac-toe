using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject confirmPanel;
    [SerializeField] private GameObject signinPanel;
    [SerializeField] private GameObject signupPanel;
    [SerializeField] private GameObject leaderboardPanel;
    
    private GameUIController _gameUIController;
    private Canvas _canvas;
    
    private Constants.GameType _gameType;
    private GameLogic _gameLogic;

    private void Start()
    {
        // Application.runInBackground = true;
        NetworkManager.Instance.GetUser();
    }

    public void ChangeToGameScene(Constants.GameType gameType)
    {
        _gameType = gameType;
        SceneManager.LoadScene("Game");
    }

    public void ChangeToMainScene()
    {
        _gameLogic?.Dispose();
        _gameLogic = null;
        SceneManager.LoadScene("Main");
    }

    public void OpenSettingsPanel()
    {
        if (!_canvas) return;

        var settingsPanelObject = Instantiate(settingsPanel, _canvas.transform);
        settingsPanelObject.GetComponent<PanelController>().Show();
    }

    public void OpenConfirmPanel(string message, ConfirmPanelController.OnConfirmButtonClickHandler onConfirmButtonClickHandler)
    {
        if (!_canvas) return;

        var confirmPanelObject = Instantiate(confirmPanel, _canvas.transform);
        confirmPanelObject.GetComponent<ConfirmPanelController>().Show(message, onConfirmButtonClickHandler);
    }

    public void OpenSigninPanel()
    {
        if (!_canvas) return;

        var signinPanelObject = Instantiate(signinPanel, _canvas.transform);
        // signinPanelObject.GetComponent<SigninPanelController>().Show(message, onConfirmButtonClickHandler);
    }

    public void OpenSignupPanel()
    {
        if (!_canvas) return;

        var signupPanelObject = Instantiate(signupPanel, _canvas.transform);
        // sigininPanelObject.GetComponent<SignupPanelController>().Show(message, onConfirmButtonClickHandler);
    }
    
    public void OpenLeaderboardPanel()
    {
        if (!_canvas) return;

        var signupPanelObject = Instantiate(leaderboardPanel, _canvas.transform);
        // sigininPanelObject.GetComponent<SignupPanelController>().Show(message, onConfirmButtonClickHandler);
    }
    public void OpenGameOverPanel()
    {
        _gameUIController.SetGameUIMode(GameUIController.GameUIMode.GameOver);
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game")
        {
            // 씬에 배치된 오브젝트 찾기 (BlockController, GameUIController)
            var blockController = GameObject.FindObjectOfType<BlockController>();
            _gameUIController = GameObject.FindObjectOfType<GameUIController>();
        
            
            // Block Controller 초기화
            blockController.InitBlocks();
            
            // Game UI 초기화
            _gameUIController.SetGameUIMode(GameUIController.GameUIMode.Init);
            
            // Game Logic 객체 생성
            if (_gameLogic != null) _gameLogic.Dispose();
            _gameLogic = new GameLogic(blockController, _gameType);
        }
        
        _canvas = GameObject.FindObjectOfType<Canvas>();
    }

    private void OnApplicationQuit()
    {
        _gameLogic?.Dispose();
        _gameLogic = null;
    }
}
