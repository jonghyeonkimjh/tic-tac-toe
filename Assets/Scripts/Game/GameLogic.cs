
using System;
using Common;

// public interface IPlayerState
// {
//     void OnEnter(GameLogic gameLogic); 
//     void OnExit(GameLogic gameLogic);
//     void HandleMove(GameLogic  gameLogic, int row, int col);
// }

public abstract class BasePlayerState
{
    public abstract void OnEnter(GameLogic gameLogic);
    public abstract void OnExit(GameLogic gameLogic);
    public abstract void HandleMove(GameLogic gameLogic, int row, int col);
    protected abstract void HandleNextTurn(GameLogic gameLogic);

    protected void ProcessMove(GameLogic gameLogic, Constants.PlayerType playerType, int row, int col)
    {
        if (gameLogic.SetNewBoardValue(playerType, row, col))
        {
            var gameResult = gameLogic.CheckGameResult();

            if (gameResult == GameLogic.GameResult.None)
            {
                HandleNextTurn(gameLogic);
            }
            else
            {
                gameLogic.EndGame(gameResult);
            }
        }
    }
}

// 직접 플레이 (싱글, 네트워크)
public class PlayerState: BasePlayerState
{
    private Constants.PlayerType _playerType;
    private bool _isFirstPlayer;
    
    private MultiplayManager _multiplayManager;
    private string _roomId;
    private bool _isMultiplay;

    public PlayerState(bool isFirstPlayer)
    {
        _isFirstPlayer = isFirstPlayer;
        _playerType = _isFirstPlayer? Constants.PlayerType.PlayerA : Constants.PlayerType.PlayerB;
        _isMultiplay = false;
    }

    public PlayerState(bool isFirstPlayer, MultiplayManager multiplayManager, string roomId):this(isFirstPlayer)
    {
        _multiplayManager = multiplayManager;
        _roomId = roomId;
        _isMultiplay = true;
    }

    public override void OnEnter(GameLogic gameLogic)
    {
        gameLogic.BlockController.OnBlockClicked = (row, column) =>
        {
            HandleMove(gameLogic, row, column);
        };
    }

    public override void OnExit(GameLogic gameLogic)
    {
        gameLogic.BlockController.OnBlockClicked = null;
    }

    public override void HandleMove(GameLogic gameLogic, int row, int col)
    {
        ProcessMove(gameLogic, _playerType, row, col);
        if (_isMultiplay)
        {
            _multiplayManager.SendPlayerMove(_roomId, row * 3 + col);
        }
    }

    protected override void HandleNextTurn(GameLogic gameLogic)
    {
        if (_isFirstPlayer)
        {
            gameLogic.SetState(gameLogic.SecondPlayerState);
        }
        else
        {
            gameLogic.SetState(gameLogic.FirstPlayerState);
        }
    }
}
// AI 플레이
public class AIState: BasePlayerState
{
    public override void OnEnter(GameLogic gameLogic)
    {
        var result = MinMaxAIController.GetBestMove(gameLogic.GetBoard());
        if (result.HasValue)
        {
            // 언박싱
            HandleMove(gameLogic, result.Value.row, result.Value.column);
        }
        else
        {
            gameLogic.EndGame(GameLogic.GameResult.Draw);
        }
    }

    public override void OnExit(GameLogic gameLogic)
    {
        
    }

    public override void HandleMove(GameLogic gameLogic, int row, int col)
    {
        ProcessMove(gameLogic, Constants.PlayerType.PlayerB, row, col);
    }

    protected override void HandleNextTurn(GameLogic gameLogic)
    {
        gameLogic.SetState(gameLogic.FirstPlayerState);
    }
}
// 네트워크 플레이
public class MultiplayState: BasePlayerState
{
    private Constants.PlayerType _playerType;
    private bool _isFirstPlayer;

    private MultiplayManager _multiplayManager;

    public MultiplayState(bool isFirstPlayer, MultiplayManager multiplayManager)
    {
        _isFirstPlayer = isFirstPlayer;
        _playerType = _isFirstPlayer? Constants.PlayerType.PlayerA : Constants.PlayerType.PlayerB;
        _multiplayManager = multiplayManager;
    }
    public override void OnEnter(GameLogic gameLogic)
    {
        _multiplayManager.OnOpponentMove = moveData =>
        {
            var row = moveData.position / 3;
            var col = moveData.position % 3;
            UnityThread.executeInUpdate(
                    () =>
                    {
                        HandleMove(gameLogic, row, col);
                    });
        };
    }

    public override void OnExit(GameLogic gameLogic)
    {
        _multiplayManager.OnOpponentMove = null;
    }

    public override void HandleMove(GameLogic gameLogic, int row, int col)
    {
        ProcessMove(gameLogic, _playerType, row, col);
    }

    protected override void HandleNextTurn(GameLogic gameLogic)
    {
        if (_isFirstPlayer)
        {
            gameLogic.SetState(gameLogic.SecondPlayerState);
        }
        else
        {
            gameLogic.SetState(gameLogic.FirstPlayerState);
        }
    }
}
public class GameLogic: IDisposable
{
    public BlockController BlockController;
    private Constants.PlayerType[,] _board;
    
    public BasePlayerState FirstPlayerState;
    public BasePlayerState SecondPlayerState;
    public BasePlayerState CurrentPlayerState;
    
    private MultiplayManager _multiplayManager;
    private string _roomId;

    public enum GameResult 
    { 
        None, // 게임 진행 중
        Win, // 플레이어 승
        Lose, // 플레이어 패배
        Draw // 비김
    }

    public GameLogic(BlockController blockController,
            Constants.GameType gameType,
            bool isFirstPlayer = false
            )
    {
        BlockController = blockController;

        // _board 초기화
        _board = new Constants.PlayerType[3, 3];
        switch (gameType)
        {
            case Constants.GameType.SinglePlayer:
            {
                FirstPlayerState = new PlayerState(true);
                SecondPlayerState = new AIState();
                SetState(FirstPlayerState);
                break;
            }
            case Constants.GameType.DualPlayer:
            {
                FirstPlayerState = new PlayerState(true);
                SecondPlayerState = new PlayerState(false);
                SetState(FirstPlayerState);
                break;
            }
            case Constants.GameType.MultiPlayer:
            {
                _multiplayManager = new MultiplayManager(
                        (state, roomId) =>
                        {
                            _roomId = roomId;
                            switch (state)
                            {
                                case Constants.MultplayManagerState.CreateRoom:
                                    //TODO: 대기화면 표시
                                    //GameManager에게 대기화면 표시 요청
                                    break;
                                case Constants.MultplayManagerState.JoinRoom:
                                    FirstPlayerState = new MultiplayState(true, _multiplayManager);
                                    SecondPlayerState = new PlayerState(false, _multiplayManager, _roomId);
                                    
                                    SetState(FirstPlayerState);
                                    break;
                                case Constants.MultplayManagerState.ExitRoom:
                                    break;
                                case Constants.MultplayManagerState.StartGame:
                                    FirstPlayerState = new PlayerState(true, _multiplayManager, _roomId);
                                    SecondPlayerState = new MultiplayState(false, _multiplayManager);
                                    
                                    SetState(FirstPlayerState);
                                    break;
                                case Constants.MultplayManagerState.EndGame:
                                    break;
                            }
                        });
                break;
            }
        }
    }

    public void SetState(BasePlayerState state)
    {
        CurrentPlayerState?.OnExit(this);
        CurrentPlayerState = state;
        CurrentPlayerState?.OnEnter(this);
    }
        
    /// <summary>
    /// _board에 새로운 값을 할당하는 함수
    /// </summary>
    /// <param name="playerType">할당하고자 하는 플레이어 타입</param>
    /// <param name="row">Row</param>
    /// <param name="column">Col</param>
    /// <returns>False가 반환되면 할당 할 수 없음 True는 할당이 완료됨</returns>
    public bool SetNewBoardValue(Constants.PlayerType playerType, int row, int column)
    {
        if (_board[row, column] != Constants.PlayerType.None) return false;

        if (playerType == Constants.PlayerType.PlayerA)
        {
            _board[row, column] = playerType;
            BlockController.PlaceMarker(Block.MarkerType.O, row, column);
            return true;
        }
        else if (playerType == Constants.PlayerType.PlayerB)
        {
            _board[row, column] = playerType;
            BlockController.PlaceMarker(Block.MarkerType.X, row, column);
            return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// 게임 결과 확인 함수
    /// </summary>
    /// <returns>플레이어 기준 게임 결과</returns>
    public GameResult CheckGameResult()
    {
        if (CheckGameWin(Constants.PlayerType.PlayerA))
        {
            // NetworkManager.Instance.UpdateScore(new NewScoreData() { score = 10 });
            return GameResult.Win;
        };
        if (CheckGameWin(Constants.PlayerType.PlayerB))
        {
            return GameResult.Lose;
        }
        if (MinMaxAIController.IsAllBlockPlaced(_board))
        {
            return GameResult.Draw;
        }
        
        return GameResult.None;
    }
    
    public bool CheckGameWin(Constants.PlayerType playerType)
    {
        
        // 가로 방향으로 마커가 일치하는지 확인
        for (var row = 0; row < _board.GetLength(0); row++)
        {
            if (_board[row, 0] == playerType && _board[row, 1] == playerType && _board[row, 2] == playerType)
            {
                (int, int)[] blocks = {(row,0), (row,1), (row,2) };
                BlockController.SetBlockColor(playerType, blocks);
                return true;
            }
        }
        
        // 세로 방향으로 마커가 일치하는지 확인
        for (var column = 0; column < _board.GetLength(1); column++)
        {
            if (_board[0, column] == playerType && _board[1, column] == playerType && _board[2, column] == playerType)
            {
                (int, int)[] blocks = {(0,column), (1,column), (2,column) };
                BlockController.SetBlockColor(playerType, blocks);
                return true;
            }
        }
        
        // 대각선 방향으로 마커가 일치하는지 확인
        if (_board[0, 0] == playerType && _board[1, 1] == playerType && _board[2, 2] == playerType)
        {
            (int, int)[] blocks = {(0,0), (1,1), (2,2) };
            BlockController.SetBlockColor(playerType, blocks);
            return true;
        }
        
        if (_board[0, 2] == playerType && _board[1, 1] == playerType && _board[2, 0] == playerType)
        {
            (int, int)[] blocks = {(0,2), (1,1), (2,0)};
            BlockController.SetBlockColor(playerType, blocks);
            return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// 게임 오버시 호출되는 메소드
    /// gameResult에 따라 결과 출력
    /// </summary>
    /// <param name="gameResult">Win,Lose, Draw</param>
    public void EndGame(GameResult gameResult)
    {
        SetState(null);
        FirstPlayerState = null;
        SecondPlayerState = null;
        
        GameManager.Instance.OpenGameOverPanel();
    }

    public Constants.PlayerType[,] GetBoard()
    {
        return _board;
    }

    public void Dispose()
    {
        _multiplayManager?.LeaveRoom(_roomId);
        _multiplayManager?.Dispose();
    }
}
