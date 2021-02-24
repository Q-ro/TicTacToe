/*
    
Author : Andres Mrad
Date : Sunday 21/February/2021 @ 20:58:47 
Description : keeps track of the board positions and the current state of the position; handles touch input
    
*/

using GameplayEnum;
using UnityEngine;

public class BoardPosition : MonoBehaviour
{
    #region private properties

    bool _isOcupied = false;
    GameplayPieceTypes _pieceType;
    Vector2Int _gridPosition;

    #endregion

    #region Property Accessors

    public bool IsOcupied { get => _isOcupied; }
    public GameplayPieceTypes PieceType { get => _pieceType; }
    public Vector2Int GridPosition { get => _gridPosition; }

    #endregion


    public void SetGamePieceType(GameplayPieceTypes pieceType)
    {
        _pieceType = pieceType;
        _isOcupied = true;
        this.gameObject.SetActive(false);
    }

    public void InitBoardPosition(GameplayPieceTypes pieceType, Vector2Int gridPosition)
    {
        _pieceType = pieceType;
        _isOcupied = false;
        _gridPosition = gridPosition;
        this.gameObject.SetActive(true);
    }

    private void OnMouseDown()
    {
        // Ensure that plays are being made only if is player's turn belongs to a human
        if (!GameManager.Instance.AwaitingHumanInput() || GameManager.Instance.IsGameOver())
            return;

        // Make sure the player isn't tryign to make an invalid move (somehow)
        if (!_isOcupied)
        {
            _isOcupied = true;
            GameManager.Instance.MoveMade(_gridPosition);
            this.gameObject.SetActive(false);
        }

    }


}
