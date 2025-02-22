using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject confirmPanel;
    
    private BlockController _blockController;
    private GameUIController _gameUIController;
    private Canvas _canvas;
    
    public enum PlayerType { None, PlayerA, PlayerB }
    private GameType _gameType;
    
    private PlayerType[,] _board;
    private enum TurnType { PlayerA, PlayerB }
    private enum GameResult 
    { 
        None, // 게임 진행 중
        Win, // 플레이어 승
        Lose, // 플레이어 패배
        Draw // 비김
    }
    
    public enum GameType
    {
        SinglePlayer,
        DualPlayer,
    }

    public void ChangeToGameScene(GameType gameType)
    {
        _gameType = gameType;
        SceneManager.LoadScene("Game");
    }

    public void ChangeToMainScene()
    {
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
    /// <summary>
    /// 게임 시작
    /// </summary>
    private void StartGame()
    {
        // board 초기화
        _board = new PlayerType[3, 3];
        
        // bloacks 초기화
        _blockController.InitBlocks();
        
        // Game UI 초기화
        _gameUIController.SetGameUIMode(GameUIController.GameUIMode.Init);
        
        // 턴 시작
        SetTurn(TurnType.PlayerA);
    }

    /// <summary>
    /// 게임 오버시 호출되는 메소드
    /// gameResult에 따라 결과 출력
    /// </summary>
    /// <param name="gameResult">Win,Lose, Draw</param>
    private void EndGame(GameResult gameResult)
    {
        _gameUIController.SetGameUIMode(GameUIController.GameUIMode.GameOver);
        _blockController.OnBlockClicked = null;

        // Todo: 나중에 구현!
        switch (gameResult)
        {
            case GameResult.Win:
                Debug.Log("Player win!");
                break;
            case GameResult.Lose:
                Debug.Log("Player lost!");
                break;
            case GameResult.Draw:
                Debug.Log("Draw!");
                break;
        }
    }

    /// <summary>
    /// _board에 새로운 값을 할당하는 함수
    /// </summary>
    /// <param name="playerType">할당하고자 하는 플레이어 타입</param>
    /// <param name="row">Row</param>
    /// <param name="column">Col</param>
    /// <returns>False가 반환되면 할당 할 수 없음 True는 할당이 완료됨</returns>
    private bool SetNewBoardValue(PlayerType playerType, int row, int column)
    {
        if (_board[row, column] != PlayerType.None) return false;

        if (playerType == PlayerType.PlayerA)
        {
            _board[row, column] = playerType;
            _blockController
                .PlaceMarker(Block.MarkerType.O, row, column);
            return true;
        }
        else if (playerType == PlayerType.PlayerB)
        {
            _board[row, column] = playerType;
            _blockController
                .PlaceMarker(Block.MarkerType.X, row, column);
            return true;
        }
        
        return false;
    }

    private void SetTurn(TurnType turnType)
    {
        switch (turnType)
        {
            case TurnType.PlayerA:
                _gameUIController.SetGameUIMode(GameUIController.GameUIMode.TurnA);
                _blockController.OnBlockClicked = (row, column) =>
                {
                    var isPlaced = SetNewBoardValue(PlayerType.PlayerA, row, column);
                    if (isPlaced)
                    {
                        var gameResult = CheckGameResult();
                        if (gameResult == GameResult.None)
                        {
                            SetTurn(TurnType.PlayerB);
                        }
                        else
                        {
                            EndGame(gameResult);
                            _blockController.OnBlockClicked = null;
                        }
                    }
                    else
                    {
                        // Todo: 이미 있는 곳에 마커를 두려 할때 처리
                        Debug.Log("Player A Turn Failed");
                    }
                };
                break;
            case TurnType.PlayerB:
                _gameUIController.SetGameUIMode(GameUIController.GameUIMode.TurnB);
                if (_gameType == GameType.SinglePlayer)
                {
                    var result = MinMaxAIController.GetBestMove(_board);
                    if (result.HasValue)
                    {
                        var isPlaced = SetNewBoardValue(PlayerType.PlayerB, result.Value.row, result.Value.column);
                        if (isPlaced)
                        {
                            var gameResult = CheckGameResult();
                            if (gameResult == GameResult.None)
                            {
                                SetTurn(TurnType.PlayerA);
                            }
                            else
                            {
                                EndGame(gameResult);
                                _blockController.OnBlockClicked = null;
                            }
                        }
                        else
                        {
                            // Todo: 이미 있는 곳에 마커를 두려 할때 처리
                            Debug.Log("Player B Turn Failed");
                        }
                    }
                    else
                    {
                        EndGame(GameResult.Win);
                    }
                }
                else if (_gameType == GameType.DualPlayer)
                {
                    _blockController.OnBlockClicked = (row, column) =>
                    {
                        var isPlaced = SetNewBoardValue(PlayerType.PlayerB, row, column);
                        if (isPlaced)
                        {
                            var gameResult = CheckGameResult();
                            if (gameResult == GameResult.None)
                            {
                                SetTurn(TurnType.PlayerA);
                            }
                            else
                            {
                                EndGame(gameResult);
                                _blockController.OnBlockClicked = null;
                            }
                        }
                        else
                        {
                            // Todo: 이미 있는 곳에 마커를 두려 할때 처리
                            Debug.Log("Player A Turn Failed");
                        }
                    }; 
                }
                break;
        }
    }

    /// <summary>
    /// 게임 결과 확인 함수
    /// </summary>
    /// <returns>플레이어 기준 게임 결과</returns>
    private GameResult CheckGameResult()
    {
        if (MinMaxAIController.CheckGameWin(PlayerType.PlayerA,_board)) return GameResult.Win;
        if (MinMaxAIController.CheckGameWin(PlayerType.PlayerB,_board)) return GameResult.Lose;
        if (MinMaxAIController.IsAllBlockPlaced(_board)) return GameResult.Draw;
        
        return GameResult.None;
    }
    
    /// <summary>
    /// 모든 마커가 보드에 배치 되었는지 확인하는 함수
    /// </summary>
    /// <returns>True = 모두 배치</returns>
    private bool IsAllBlockPlaced()
    {
        for (var row = 0; row < _board.GetLength(0); row++)
        {
            for (var column = 0; column < _board.GetLength(1); column++)
            {
                if (_board[row, column] == PlayerType.None)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private bool CheckGameWin(PlayerType playerType)
    {
        
        // 가로 방향으로 마커가 일치하는지 확인
        for (var row = 0; row < _board.GetLength(0); row++)
        {
            if (_board[row, 0] == playerType && _board[row, 1] == playerType && _board[row, 2] == playerType)
            {
                (int, int)[] blocks = {(row,0), (row,1), (row,2) };
                _blockController.SetBlockColor(playerType, blocks);
                return true;
            }
        }
        
        // 세로 방향으로 마커가 일치하는지 확인
        for (var column = 0; column < _board.GetLength(1); column++)
        {
            if (_board[0, column] == playerType && _board[1, column] == playerType && _board[2, column] == playerType)
            {
                (int, int)[] blocks = {(0,column), (1,column), (2,column) };
                _blockController.SetBlockColor(playerType, blocks);
                return true;
            }
        }
        
        // 대각선 방향으로 마커가 일치하는지 확인
        if (_board[0, 0] == playerType && _board[1, 1] == playerType && _board[2, 2] == playerType)
        {
            (int, int)[] blocks = {(0,0), (1,1), (2,2) };
            _blockController.SetBlockColor(playerType, blocks);
            return true;
        }
        
        if (_board[0, 2] == playerType && _board[1, 1] == playerType && _board[2, 0] == playerType)
        {
            (int, int)[] blocks = {(0,2), (1,1), (2,0)};
            _blockController.SetBlockColor(playerType, blocks);
            return true;
        }
        
        return false;
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game")
        {
            _blockController = GameObject.FindObjectOfType<BlockController>();
            _gameUIController = GameObject.FindObjectOfType<GameUIController>();
            
            // 게임 시작
            StartGame();
        }
        
        _canvas = GameObject.FindObjectOfType<Canvas>();
    }
}
