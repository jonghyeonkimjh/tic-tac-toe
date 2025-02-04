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


// SetTurn 부분의 델리게이트 초기화를 추가하여 턴 종료 및 게임 종료
// SetNewBoardValue에 CanPlaceMarker를 추가하여 이미 마커를 둔곳은 못두게 변경
