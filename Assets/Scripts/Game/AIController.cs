using System.Collections.Generic;
using UnityEngine;

public static class AIController
{
    public static (int row, int column) FindNextMove(Constants.PlayerType[,] board)
    {
        var canPlaces = new List<(int row, int column)>();
        for (int rowIndex = 0; rowIndex < board.GetLength(0); rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < board.GetLength(1); columnIndex++)
            {
                if (board[rowIndex, columnIndex] == Constants.PlayerType.None)
                {
                    // 내가 이기거나 상대가 두면 이기는 위치에 수를 두기
                    board[rowIndex, columnIndex] = Constants.PlayerType.PlayerB;
                    if (IsRowWin(board, rowIndex, Constants.PlayerType.PlayerB))
                    {
                        board[rowIndex, columnIndex] = Constants.PlayerType.None;
                        return (rowIndex, columnIndex);
                    }
                    
                    if (IsColumnWin(board, columnIndex, Constants.PlayerType.PlayerB))
                    {
                        board[rowIndex, columnIndex] = Constants.PlayerType.None;
                        return (rowIndex, columnIndex);
                    }

                    if (IsDiagonalWin(board, Constants.PlayerType.PlayerB))
                    {
                        board[rowIndex, columnIndex] = Constants.PlayerType.None;
                        return (rowIndex, columnIndex);
                    }
                    
                    board[rowIndex, columnIndex] = Constants.PlayerType.PlayerA;
                    if (IsRowWin(board, rowIndex, Constants.PlayerType.PlayerA))
                    {
                        board[rowIndex, columnIndex] = Constants.PlayerType.None;
                        return (rowIndex, columnIndex);
                    }
                    
                    if (IsColumnWin(board, columnIndex, Constants.PlayerType.PlayerA))
                    {
                        board[rowIndex, columnIndex] = Constants.PlayerType.None;
                        return (rowIndex, columnIndex);
                    }

                    if (IsDiagonalWin(board, Constants.PlayerType.PlayerA))
                    {
                        board[rowIndex, columnIndex] = Constants.PlayerType.None;
                        return (rowIndex, columnIndex);
                    }
                    board[rowIndex, columnIndex] = Constants.PlayerType.None;
                    
                    
                    canPlaces.Add((rowIndex, columnIndex));
                }
            }
        }
        

        var randomRange = Random.Range(0, canPlaces.Count);
        // 내가 이기는 경우의 수가 없다면 마지막으로 확인한 아무 빈곳에 두기
        return (canPlaces[randomRange].row, canPlaces[randomRange].column);
    }

    /// <summary>
    /// 가로 승리 확인
    /// </summary>
    /// <param name="board"></param>
    /// <param name="row"></param>
    /// <param name="playerType"></param>
    /// <returns></returns>
    private static bool IsRowWin(Constants.PlayerType[,] board, int row, Constants.PlayerType playerType)
    {
        if (board[row, 0] == playerType && board[row, 1] == playerType && board[row, 2] == playerType)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 세로 승리 확인
    /// </summary>
    /// <param name="board"></param>
    /// <param name="column"></param>
    /// <param name="playerType"></param>
    /// <returns></returns>
    private static bool IsColumnWin(Constants.PlayerType[,] board, int column, Constants.PlayerType playerType)
    {
        if (board[0, column] == playerType && board[1, column] == playerType && board[2, column] == playerType)
        {
            return true;
        }

        return false;
    }

    
    /// <summary>
    /// 대각선 승리 확인
    /// </summary>
    /// <param name="board"></param>
    /// <param name="playerType"></param>
    /// <returns></returns>
    private static bool IsDiagonalWin(Constants.PlayerType[,] board, Constants.PlayerType playerType)
    {
        if (board[0, 0] == playerType && board[1, 1] == playerType && board[2, 2] == playerType)
        {
            return true;
        }
        
        if (board[0, 2] == playerType && board[1, 1] == playerType && board[2, 0] == playerType)
        {
            return true;
        }

        return false;
    }
}