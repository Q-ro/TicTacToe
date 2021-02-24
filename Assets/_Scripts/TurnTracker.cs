/*
    
Author : Andres Mrad
Date : Sunday 21/February/2021 @ 21:52:17 
Description : Handles the turn tracking of the game
    
*/
using System;
using GameplayEnum;
using UnityEngine;

[Serializable]
public class TurnTracker
{

    #region Inspector Properties

    GameplayPieceTypes player1GamePiece;
    GameplayPlayerTypes player1Type;
    GameplayPlayerTypes player2Type;
    [SerializeField] GameplayPlayerTurnTypes startingPlayerTurn;

    #endregion

    #region Private Properties

    GameplayPlayerTurnTypes _currentPlayerTurn = GameplayPlayerTurnTypes.P1Turn;
    GameplayPieceTypes _player2GamePiece;

    #endregion

    public void InitTurnTracker(GameplayMatchPresets presets)
    {
        player1GamePiece = presets.P1Pieces;
        _player2GamePiece = player1GamePiece == GameplayPieceTypes.O_Pieces ? GameplayPieceTypes.X_Pieces : GameplayPieceTypes.O_Pieces;

        player1Type = presets.P1Type;
        player2Type = presets.P2Type;

        _currentPlayerTurn = GameplayPlayerTurnTypes.P1Turn;

        // _currentPlayerTurn = startingPlayerTurn;
        // _player2GamePiece = player1GamePiece == GameplayPieceTypes.O_Pieces ? GameplayPieceTypes.X_Pieces : GameplayPieceTypes.O_Pieces;
    }

    public GameplayPieceTypes GetCurrentTurnPieceType()
    {
        return _currentPlayerTurn == GameplayPlayerTurnTypes.P1Turn ? player1GamePiece : _player2GamePiece;
    }

    public GameplayPieceTypes GetRivalTurnPieceType()
    {
        return _currentPlayerTurn == GameplayPlayerTurnTypes.P1Turn ? _player2GamePiece : player1GamePiece;
    }

    public void MoveToNextTurn()
    {
        _currentPlayerTurn = _currentPlayerTurn == GameplayPlayerTurnTypes.P1Turn ? GameplayPlayerTurnTypes.P2Turn : GameplayPlayerTurnTypes.P1Turn;
    }

    public GameplayPlayerTurnTypes GetCurrentPlayerTurn()
    {
        return _currentPlayerTurn;
    }
    public GameplayPlayerTypes GetCurrentPlayerTurnType()
    {
        return _currentPlayerTurn == GameplayPlayerTurnTypes.P1Turn ? this.player1Type : this.player2Type;
    }


}
