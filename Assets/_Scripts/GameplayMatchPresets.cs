/*
    
Author : Andres Mrad
Date : Wednesday 24/February/2021 @ 02:28:35 
Description : represents the options for a match
    
*/


using System;
using GameplayEnum;
using UnityEngine;

[Serializable]
public class GameplayMatchPresets
{
    #region Inspector Properties

    [SerializeField] GameplayEnum.GameplayPieceTypes p1Pieces;
    [SerializeField] GameplayEnum.GameplayPieceTypes p2Pieces;
    [SerializeField] GameplayEnum.GameplayBotDifficulty p1Difficulty;
    [SerializeField] GameplayEnum.GameplayBotDifficulty p2Difficulty;
    [SerializeField] GameplayEnum.GameplayPlayerTypes p1Type;
    [SerializeField] GameplayEnum.GameplayPlayerTypes p2Type;

    public GameplayPieceTypes P1Pieces { get => p1Pieces; set => p1Pieces = value; }
    public GameplayPieceTypes P2Pieces { get => p2Pieces; set => p2Pieces = value; }
    public GameplayPlayerTypes P1Type { get => p1Type; set => p1Type = value; }
    public GameplayPlayerTypes P2Type { get => p2Type; set => p2Type = value; }
    public GameplayBotDifficulty P1Difficulty { get => p1Difficulty; set => p1Difficulty = value; }
    public GameplayBotDifficulty P2Difficulty { get => p2Difficulty; set => p2Difficulty = value; }

    public GameplayMatchPresets(
        GameplayPieceTypes p1Pieces,
        GameplayPieceTypes p2Pieces,
        GameplayPlayerTypes p1Type,
        GameplayPlayerTypes p2Type,
        GameplayBotDifficulty p1Difficulty,
        GameplayBotDifficulty p2Difficulty
        )
    {
        this.p1Pieces = p1Pieces;
        this.p2Pieces = p2Pieces;
        this.p1Type = p1Type;
        this.p2Type = p2Type;
        this.p1Difficulty = p1Difficulty;
        this.p2Difficulty = p2Difficulty;
    }

    #endregion


}