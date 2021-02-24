/*
    
Author : Andres Mrad
Date : Wednesday 24/February/2021 @ 02:27:00 
Description : Handles the option selection menu for the upcoming match match
    
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameplayEnum;

public class GameplayPresetMenu : MonoBehaviour
{
    #region Inspector Properties

    [SerializeField] Slider p1Pieces;
    [SerializeField] Slider p1Type;
    [SerializeField] Slider p2Type;
    [SerializeField] GameObject p1AIDifficultySelection;
    [SerializeField] GameObject p2AIDifficultySelection;
    [SerializeField] Dropdown p1Difficulty;
    [SerializeField] Dropdown p2Difficulty;

    #endregion

    #region Private Properties

    GameplayMatchPresets _currentMatchPresets = new GameplayMatchPresets(0, 0, 0, 0, 0, 0);

    #endregion

    void Start()
    {
        p1AIDifficultySelection.SetActive(false);
        p2AIDifficultySelection.SetActive(false);
        this._currentMatchPresets.P1Pieces = GameplayPieceTypes.O_Pieces;
        this._currentMatchPresets.P2Pieces = GameplayPieceTypes.X_Pieces;
    }

    public void SelectP1Pieces()
    {
        this._currentMatchPresets.P1Pieces = (GameplayPieceTypes)this.p1Pieces.value + 1;
        this.SelectP2Pieces();
    }

    private void SelectP2Pieces()
    {
        this._currentMatchPresets.P2Pieces = (this._currentMatchPresets.P1Pieces == GameplayPieceTypes.O_Pieces) ? GameplayPieceTypes.X_Pieces : GameplayPieceTypes.O_Pieces;
    }

    public void SelectP1Type()
    {
        this._currentMatchPresets.P1Type = (GameplayPlayerTypes)this.p1Type.value;
        p1AIDifficultySelection.SetActive((int)this.p1Type.value != 0);
    }

    public void SelectP1Difficulty()
    {
        this._currentMatchPresets.P1Difficulty = (GameplayBotDifficulty)this.p1Difficulty.value;
    }

    public void SelectP2Difficulty()
    {
        this._currentMatchPresets.P2Difficulty = (GameplayBotDifficulty)this.p2Difficulty.value;
    }

    public void SelectP2Type()
    {
        this._currentMatchPresets.P2Type = (GameplayPlayerTypes)this.p2Type.value;
        p2AIDifficultySelection.SetActive((int)this.p2Type.value != 0);
    }


    public void PlayButtonSelectionClick()
    {
        GameManager.Instance.InitGamePlay(this._currentMatchPresets);
    }
}
