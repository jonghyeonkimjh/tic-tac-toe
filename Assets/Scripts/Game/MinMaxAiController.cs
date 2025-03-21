﻿using System;
using UnityEngine;

public static class MinMaxAIController
{
    public static (int row, int column)? GetBestMove(Constants.PlayerType[,] board)
    {
        float bestScore = float.MinValue; // 나에게 이득이 되는 경우의 수 가중치 중 가장 큰 값.
        (int row, int column)? bestMove = null;

        for (var row = 0; row < board.GetLength(0); row++)
        {
            for (var column = 0; column < board.GetLength(1); column++)
            {
                if (board[row, column]  == Constants.PlayerType.None)
                {
                    Debug.Log(board[row, column]);
                    board[row,column] = Constants.PlayerType.PlayerB;
                    var score = DoMinMax(board, 0, false);
                    board[row,column] = Constants.PlayerType.None;

                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestMove = (row, column);
                    }
                }
            }
        }
        
        return bestMove;
    }
    private static float DoMinMax(Constants.PlayerType[,] board, int depth, bool isMaximizing)
    {
        if (CheckGameWin(Constants.PlayerType.PlayerA, board)) return -10f + depth; // A Win
        if (CheckGameWin(Constants.PlayerType.PlayerB, board)) return 10f  - depth; // B Win
        if (IsAllBlockPlaced(board)) return 0.0f; // draw

        if (isMaximizing)
        {
            var bestScore = float.MinValue;
            for (var row = 0; row < board.GetLength(0); row++)
            {
                for (var column = 0; column < board.GetLength(1); column++)
                {
                    if (board[row, column] == Constants.PlayerType.None)
                    {
                        board[row, column] = Constants.PlayerType.PlayerB;
                        var score = DoMinMax(board, depth + 1, false);
                        board[row, column] = Constants.PlayerType.None;
                        bestScore = Math.Max(bestScore, score);
                    };
                }
            }
            return bestScore;
        }
        else
        {
            var bestScore = float.MaxValue;
            for (var row = 0; row < board.GetLength(0); row++)
            {
                for (var column = 0; column < board.GetLength(1); column++)
                {
                    if (board[row, column] == Constants.PlayerType.None)
                    {
                        board[row, column] = Constants.PlayerType.PlayerA;
                        var score = DoMinMax(board, depth + 1, true);
                        board[row, column] = Constants.PlayerType.None;
                        bestScore = Math.Min(bestScore, score);
                    };
                }
            }
            return bestScore;
        }
    }
    public static bool IsAllBlockPlaced(Constants.PlayerType[,] board)
    {
        for (var row = 0; row < board.GetLength(0); row++)
        {
            for (var column = 0; column < board.GetLength(1); column++)
            {
                if (board[row, column] == Constants.PlayerType.None) return false;
            }
        }
        return true;
    }
    
    /// <summary>
    /// 게임의 승패를 판단하는 함수
    /// </summary>
    /// <param name="playerType"></param>
    /// <param name="board"></param>
    /// <returns></returns>
    public static bool CheckGameWin(Constants.PlayerType playerType, Constants.PlayerType[,] board)
    {
        
        // 가로 방향으로 마커가 일치하는지 확인
        for (var row = 0; row < board.GetLength(0); row++)
        {
            if (board[row, 0] == playerType && board[row, 1] == playerType && board[row, 2] == playerType)
                return true;
        }
        
        // 세로 방향으로 마커가 일치하는지 확인
        for (var column = 0; column < board.GetLength(1); column++)
        {
            if (board[0, column] == playerType && board[1, column] == playerType && board[2, column] == playerType) 
                return true;
        }
        
        // 대각선 방향으로 마커가 일치하는지 확인
        if (board[0, 0] == playerType && board[1, 1] == playerType && board[2, 2] == playerType) 
            return true;
        
        if (board[0, 2] == playerType && board[1, 1] == playerType && board[2, 0] == playerType) 
            return true;
        
        return false;
    }
}
