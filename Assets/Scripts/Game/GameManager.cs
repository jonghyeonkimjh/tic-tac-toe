using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private BlockController blockController;
    public enum PlayerType { None, PlayerA, PlayerB }
    private PlayerType[,] _board;
    private int  _name;
    public enum TurnType { PlayerA, PlayerB }
    private void Start()
    {
        InitGame();
        
        //테스트
        blockController.OnBlockClicked = (row, column) =>
        {
            Debug.Log("Block Clicked " + row + ", " + column);
        };
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

    private void SetTurn(TurnType turnType)
    {
        switch (turnType)
        {
            case TurnType.PlayerA:
                break;
            case TurnType.PlayerB:
                break;
        }
    }
}
