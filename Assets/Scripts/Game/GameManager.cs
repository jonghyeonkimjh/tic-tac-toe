using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private BlockController blockController;
    [SerializeField] private GameObject startPanel; // TODO: 테스트 코드, 삭제 예정
    
    private enum PlayerType { None, PlayerA, PlayerB }
    private PlayerType[,] _board;
    private int  _name;
    private enum TurnType { PlayerA, PlayerB }
    private enum GameResult 
    { 
        None, // 게임 진행 중
        Win, // 플레이어 승
        Lose, // 플레이어 패배
        Draw // 비김
        
    }
    private void Start()
    {
        InitGame();
    }

    /// <summary>
    /// 게임 초기화 함수
    /// </summary>
    public void InitGame()
    {
        // board 초기화
        _board = new PlayerType[3, 3];
        
        // bloacks 초기화
        blockController.InitBlocks();
    }

    /// <summary>
    /// 게임 시작
    /// </summary>
    public void StartGame()
    {
        startPanel.SetActive(false); // TODO: 테스트 코드, 삭제 예정
        SetTurn(TurnType.PlayerA);
    }

    /// <summary>
    /// 게임 오버시 호출되는 메소드
    /// gameResult에 따라 결과 출력
    /// </summary>
    /// <param name="gameResult">Win,Lose, Draw</param>
    private void EndGame(GameResult gameResult)
    {
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
        if (!CanPlaceMarker(playerType, row, column)) return false;

        if (playerType == PlayerType.PlayerA)
        {
            _board[row, column] = playerType;
            blockController
                .PlaceMarker(Block.MarkerType.O, row, column);
            return true;
        }
        else if (playerType == PlayerType.PlayerB)
        {
            _board[row, column] = playerType;
            blockController
                .PlaceMarker(Block.MarkerType.X, row, column);
            return true;
        }
        
        return false;
    }

    private bool CanPlaceMarker(PlayerType playerType, int row, int column)
    {
        if (_board[row, column] == PlayerType.None) return true;
        return false;
    }

    private void SetTurn(TurnType turnType)
    {
        switch (turnType)
        {
            case TurnType.PlayerA:
                Debug.Log("Player A Turn");
                blockController.OnBlockClicked = null;
                blockController.OnBlockClicked = (row, column) =>
                {
                    var result = SetNewBoardValue(PlayerType.PlayerA, row, column);
                    if (result)
                    {
                        var gameResult = CheckGameResult();
                        if (gameResult == GameResult.None)
                        {
                            SetTurn(TurnType.PlayerB);
                        }
                        else
                        {
                            EndGame(gameResult);
                            blockController.OnBlockClicked = null;
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
                Debug.Log("Player B Turn");
                blockController.OnBlockClicked = null;
                blockController.OnBlockClicked = (row, column) =>
                {
                    var result = SetNewBoardValue(PlayerType.PlayerB, row, column);
                    if (result)
                    {
                        var gameResult = CheckGameResult();
                        if (gameResult == GameResult.None)
                        {
                            SetTurn(TurnType.PlayerA);
                        }
                        else
                        {
                            EndGame(gameResult);
                            blockController.OnBlockClicked = null;
                        }
                    }
                    else
                    {
                        // Todo: 이미 있는 곳에 마커를 두려 할때 처리
                        Debug.Log("Player B Turn Failed");
                    }
                };
                // Todo: AI에게 입력 받기
                break;
        }
    }

    /// <summary>
    /// 게임 결과 확인 함수
    /// </summary>
    /// <returns>플레이어 기준 게임 결과</returns>
    private GameResult CheckGameResult()
    {
        if (CheckGameWin(PlayerType.PlayerA)) return GameResult.Win;
        if (CheckGameWin(PlayerType.PlayerB)) return GameResult.Lose;
        if (IsAllBlockPlaced()) return GameResult.Draw;
        
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

    //게임의 승패를 판단하는 함수
    private bool CheckGameWin(PlayerType playerType)
    {
        
        // 가로 방향으로 마커가 일치하는지 확인
        for (var row = 0; row < _board.GetLength(0); row++)
        {
            if (_board[row, 0] == playerType && _board[row, 1] == playerType && _board[row, 2] == playerType)
            {
                return true;
            }
        }
        
        // 세로 방향으로 마커가 일치하는지 확인
        for (var column = 0; column < _board.GetLength(1); column++)
        {
            if (_board[0, column] == playerType && _board[1, column] == playerType && _board[2, column] == playerType)
            {
                return true;
            }
        }
        
        // 대각선 방향으로 마커가 일치하는지 확인
        if (_board[0, 0] == playerType && _board[1, 1] == playerType && _board[2, 2] == playerType)
        {
            return true;
        }
        
        if (_board[0, 2] == playerType && _board[1, 1] == playerType && _board[2, 0] == playerType)
        {
            return true;
        }
        
        return false;
    }
}
