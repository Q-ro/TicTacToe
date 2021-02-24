/*
    
Author : Andres Mrad
Date : Sunday 21/February/2021 @ 20:58:05 
Description : The main control script for keeping tack of the game states
    
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameplayEnum;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    #region Inspector Properties

    [SerializeField] GameObject OGamePiecePrefab;
    [SerializeField] GameObject XGamePiecePrefab;
    [SerializeField] GameObject boardPrefab;
    [SerializeField] TurnTracker turnTracker = new TurnTracker();
    [SerializeField] float boardPositionSpacing = 2;
    [SerializeField] bool isIndestructible = false;
    [SerializeField] GameObject gamePresetsCanvas;
    [SerializeField] GameObject gameOverCanvas;
    [SerializeField] Text gameOverDisplay;



    #endregion

    #region Private Properties

    Board _gameBoard;

    bool _isGameOver = false;

    GameAI _gameAI = new GameAI();

    GameplayMatchPresets _currentPresets;

    #endregion


    private void Awake()
    {
        _isIndestructible = isIndestructible;

        gamePresetsCanvas.SetActive(true);

    }


    // Start is called before the first frame update
    void Start()
    {
    }

    internal void InitGamePlay(GameplayMatchPresets presets)
    {
        this._currentPresets = presets;
        gamePresetsCanvas.SetActive(false);

        Debug.Log(presets.P1Difficulty);
        Debug.Log(presets.P2Difficulty);

        this.turnTracker.InitTurnTracker(presets);

        if (!_gameBoard)
            _gameBoard = Instantiate(boardPrefab, new Vector3(0, 0, 0), Quaternion.identity).GetComponent<Board>();
        _gameBoard.InitializeBoard(boardPositionSpacing);

        //Check if the next turn if from a none human player, start tbe bot movement if so
        if (!AwaitingHumanInput())
        {
            MakeAIMove();

        }
    }

    internal void MoveMade(Vector2Int gridPosition)
    {
        if (_isGameOver)
            return;

        // determine the game piece to be placed having into account who's turn it is, and what piece they're using
        var gamePieceToUse = this.turnTracker.GetCurrentTurnPieceType() == GameplayPieceTypes.O_Pieces ? OGamePiecePrefab : XGamePiecePrefab;

        //Update the board 
        this._gameBoard.UpdateBoardPosition(gridPosition, this.turnTracker.GetCurrentTurnPieceType(), gamePieceToUse, boardPositionSpacing);

        // check if the move ends the game
        if (CheckGameOver(gridPosition.x, gridPosition.y, this.turnTracker.GetCurrentTurnPieceType()))
        {
            // if so, end it
            _isGameOver = true;
            EndGame(false);
            return;

        }
        //Check if a tie has been reached
        else if (!(_gameBoard.GetAvailableLocations().Count > 0))
        {
            _isGameOver = true;
            EndGame(true);
            return;
        }
        else
        {
            // else, move to the next turn
            this.turnTracker.MoveToNextTurn();
        }

        //Check if the next turn if from a none human player, start tbe bot movement if so
        if (!AwaitingHumanInput())
        {
            MakeAIMove();
            // MoveMade(_gameAI.MakeMove(_gameBoard, this.turnTracker, GameplayBotDifficulty.HARD));
        }
    }

    private void MakeAIMove()
    {
        var moveDifficulty = this.turnTracker.GetCurrentPlayerTurn() == GameplayPlayerTurnTypes.P1Turn ? this._currentPresets.P1Difficulty : this._currentPresets.P2Difficulty;
        MoveMade(_gameAI.MakeMove(_gameBoard, this.turnTracker, moveDifficulty));
    }

    public bool AwaitingHumanInput()
    {
        return this.turnTracker.GetCurrentPlayerTurnType() == GameplayPlayerTypes.Human;
    }

    public bool IsGameOver()
    {
        return _isGameOver;
    }

    private void EndGame(bool isTie)
    {
        Debug.Log("ended");
        this.gameOverCanvas.SetActive(true);
        if (!isTie)
            gameOverDisplay.text = String.Format("WINNER : {0}\n---------------\n(PLAYER {1})", turnTracker.GetCurrentTurnPieceType() == GameplayPieceTypes.O_Pieces ? "O" : "X", (int)turnTracker.GetCurrentPlayerTurn() + 1);
        else
            gameOverDisplay.text = "It's a tie";
    }


    private bool CheckGameOver(int x, int y, GameplayPieceTypes gamePiece)
    {

        int score = 1;
        int pieceGoal = 3;
        // check for 3 in a row
        for (int i = 1; i < 3; i++)
        {
            if (x + i < this._gameBoard.BoardWidth)
            {
                if (this._gameBoard.GetSimplifiedGameBoard()[x + i, y] == gamePiece)
                {
                    score++;
                }
                else
                {
                    break;
                }
            }
        }
        if (score >= pieceGoal)
        {
            return true;
        }
        for (int i = 1; i < 3; i++)
        {
            if (x - i >= 0)
            {
                if (this._gameBoard.GetSimplifiedGameBoard()[x - i, y] == gamePiece)
                {
                    score++;
                }
                else
                {
                    break;
                }
            }
        }
        if (score >= pieceGoal)
        {
            return true;
        }

        // 3 in a colum
        score = 1;
        for (int i = 1; i < 3; i++)
        {
            if (y + i < this._gameBoard.BoardHeight)
            {
                if (this._gameBoard.GetSimplifiedGameBoard()[x, y + i] == gamePiece)
                {
                    score++;
                }
                else
                {
                    break;
                }
            }
        }
        if (score >= pieceGoal)
        {
            return true;
        }
        for (int i = 1; i < 3; i++)
        {
            if (y - i >= 0)
            {
                if (this._gameBoard.GetSimplifiedGameBoard()[x, y - i] == gamePiece)
                {
                    score++;
                }
                else
                {
                    break;
                }
            }
        }
        if (score >= pieceGoal)
        {
            return true;
        }

        // 3 in diagonal
        score = 1;
        for (int i = 1; i < 3; i++)
        {
            if (x + i < this._gameBoard.BoardWidth && y + i < this._gameBoard.BoardHeight)
            {
                if (this._gameBoard.GetSimplifiedGameBoard()[x + i, y + i] == gamePiece)
                {
                    score++;
                }
                else
                {
                    break;
                }
            }
        }
        if (score >= pieceGoal)
        {
            return true;
        }
        for (int i = 1; i < 3; i++)
        {
            if (x - i >= 0 && y - i >= 0)
            {
                if (this._gameBoard.GetSimplifiedGameBoard()[x - i, y - i] == gamePiece)
                {
                    score++;
                }
                else
                {
                    break;
                }
            }
        }
        if (score >= pieceGoal)
        {
            return true;
        }

        //3 in inverse diagonal
        score = 1;
        for (int i = 1; i < 3; i++)
        {
            if (x + i < this._gameBoard.BoardWidth && y - i >= 0)
            {
                if (this._gameBoard.GetSimplifiedGameBoard()[x + i, y - i] == gamePiece)
                {
                    score++;
                }
                else
                {
                    break;
                }
            }
        }
        if (score >= pieceGoal)
        {
            return true;
        }
        for (int i = 1; i < 3; i++)
        {
            if (x - i >= 0 && y + i < this._gameBoard.BoardHeight)
            {
                if (this._gameBoard.GetSimplifiedGameBoard()[x - i, y + i] == gamePiece)
                {
                    score++;
                }
                else
                {
                    break;
                }
            }
        }
        if (score >= pieceGoal)
        {
            return true;
        }
        return false;

    }

}
