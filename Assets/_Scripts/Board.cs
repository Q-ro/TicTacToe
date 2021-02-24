/*
    
Author : Andres Mrad
Date : Sunday 21/February/2021 @ 21:06:04 
Description : Main board controller, keeps track of the occupied positions and the board position states
    
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameplayEnum;

public class Board : MonoBehaviour
{
    #region Inspector Properties

    [SerializeField] GameObject boardPositionPrefab;

    [SerializeField] int boardWidth = 3;
    [SerializeField] int boardHeight = 3;

    #endregion


    #region Private Properties

    BoardPosition[,] _boardPositions;

    public int BoardHeight { get => boardHeight; }
    public int BoardWidth { get => boardWidth; }

    #endregion

    internal void InitializeBoard(float spacing)
    {
        _boardPositions = new BoardPosition[boardWidth, boardHeight];


        for (int i = 0; i < boardHeight; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                _boardPositions[i, j] = Instantiate(boardPositionPrefab, new Vector3(i * spacing, 0, j * spacing), Quaternion.identity).GetComponent<BoardPosition>();
                _boardPositions[i, j].InitBoardPosition(GameplayPieceTypes.NONE, new Vector2Int(i, j));
            }
        }
    }

    internal List<Vector2Int> GetAvailableLocations()
    {
        List<Vector2Int> availablePositions = new List<Vector2Int>();

        foreach (var item in _boardPositions)
        {
            if (!item.IsOcupied)
                availablePositions.Add(item.GridPosition);
        }

        return availablePositions;
    }

    internal List<Vector2Int> GetOwnedLocationsByPieceType(GameplayPieceTypes pieceType)
    {
        List<Vector2Int> availablePositions = new List<Vector2Int>();

        foreach (var item in _boardPositions)
        {
            if (item.PieceType == pieceType)
                availablePositions.Add(item.GridPosition);
        }

        return availablePositions;
    }

    internal void UpdateBoardPosition(Vector2Int gridPosition, GameplayPieceTypes pieceType, GameObject gamePieceToUse, float boardPositionSpacing)
    {
        // Update the information on the board
        _boardPositions[gridPosition.x, gridPosition.y].SetGamePieceType(pieceType);

        // Instantiate game pieces to reflect the state of the board
        Instantiate(gamePieceToUse, new Vector3(gridPosition.x * boardPositionSpacing, 0, gridPosition.y * boardPositionSpacing), Quaternion.identity);
    }

    internal GameplayPieceTypes[,] GetSimplifiedGameBoard()
    {
        GameplayPieceTypes[,] simplifiedBoard = new GameplayPieceTypes[boardWidth, boardHeight];
        for (int i = 0; i < boardHeight; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                simplifiedBoard[i, j] = _boardPositions[i, j].PieceType;
            }
        }

        return simplifiedBoard;
    }
}
