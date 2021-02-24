/*
    
Author : Andres Mrad
Date : Monday 22/February/2021 @ 09:39:52 
Description : AI controller for the game
    
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameplayEnum;
using System.Text.RegularExpressions;

public class GameAI
{

    static System.Random random = new System.Random();


    // Somewhat naive approach to implement a Tic Tac Toe A.I.
    // I honestly don't know much about the strategy involved in the game, but i'm assuming
    // the steps described below make sense and work as a base for a somewhat decent A.I. that can play this sort of games
    public Vector2Int MakeMove(Board gameplayBoard, TurnTracker turnTracker, GameplayBotDifficulty difficulty = GameplayBotDifficulty.EASY)
    {

        // Get available spaces
        var boardFreePositions = gameplayBoard.GetAvailableLocations();

        // if the game is set to easy difficulty
        if (difficulty == GameplayBotDifficulty.EASY)
        {
            // Just pick something at random
            return boardFreePositions[Mathf.FloorToInt(Random.Range(0, boardFreePositions.Count))];

        }

        // Else make a move using a better strategy

        // Get the play pieces for each player
        GameplayPieceTypes ownedPieces = turnTracker.GetCurrentTurnPieceType();
        GameplayPieceTypes rivalPieces = turnTracker.GetRivalTurnPieceType();

        //if first move, or the center has not been taken, take center (assuming this is the best possible position to take, as this is the case in most board games)
        var bestMove = new Vector2Int(1, 1);

        if (difficulty == GameplayBotDifficulty.MEDIUM)
        {
            if (boardFreePositions.Contains((bestMove)))
            {
                // Just pick something at random
                return boardFreePositions[Mathf.FloorToInt(Random.Range(0, boardFreePositions.Count))];
            }



            var scoresMedium = this.AlphaBetaMax(gameplayBoard, 0, Mathf.Infinity, -Mathf.Infinity, ownedPieces);

            // return bestMove;
            return new Vector2Int(scoresMedium[0], scoresMedium[1]);

        }

        //if first move, or the center has not been taken, take center (assuming this is the best possible position to take, as this is the case in most board games)
        if (boardFreePositions.Contains((bestMove)))
        {
            return bestMove;
        }

        // higher depth means, even further analisis of deeper brancher of the possible plays
        var scoresHard = this.AlphaBetaMax(gameplayBoard, 3, Mathf.Infinity, -Mathf.Infinity, ownedPieces);

        // return bestMove;
        return new Vector2Int(scoresHard[0], scoresHard[1]);
    }

    // a sort of alpha-beta pruning greedy algorithm, hacky, but it works (Originally used for GOMOKU, but the principle should still work)
    private int[] AlphaBetaMax(Board board, int depth, float alpha, float beta, GameplayPieceTypes playingPieces)
    {

        int[] v = MaxModelPoints((int)playingPieces, board);

        if (depth <= 0)
        {
            return v;
        }
        int[] best = new int[] { -1, -1, -10000 };
        ArrayList points = Gen(board);

        for (int i = 0; i < points.Count; i++)
        {
            int[] p = (int[])points[i];
            board.GetSimplifiedGameBoard()[p[0], p[1]] = (int)playingPieces == 1 ? GameplayPieceTypes.X_Pieces : GameplayPieceTypes.O_Pieces;
            v = AlphaBetaMax(board, depth - 1, best[2] > alpha ? best[2] : alpha, beta, playingPieces);
            board.GetSimplifiedGameBoard()[p[0], p[1]] = 0;
            if (v[2] > best[2])
            {
                best = v;
            }
            if (v[2] > beta)
            {
                break;
            }
        }
        return best;
    }
    public int[] MaxModelPoints(int playingPieces, Board board)
    {
        ArrayList localBestPoints = FindMaxModel(playingPieces, board);
        ArrayList counterBestPoints = FindMaxModel(playingPieces == 1 ? 2 : 1, board);
        if (Mathf.Abs(((int[])localBestPoints[0])[2]) > Mathf.Abs(((int[])counterBestPoints[0])[2] / 5 * 4))
        {
            return (int[])localBestPoints[random.Next(localBestPoints.Count)];
        }
        return (int[])counterBestPoints[random.Next(counterBestPoints.Count)];
    }

    private ArrayList Gen(Board board)
    {
        ArrayList points = new ArrayList();
        var width = board.BoardWidth;
        var height = board.BoardHeight;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (HasNeighbor(x, y, board) && board.GetSimplifiedGameBoard()[x, y] == (int)GameplayPieceTypes.NONE)
                {
                    points.Add(new int[] { x, y });
                }
            }
        }
        return points;
    }

    private ArrayList FindMaxModel(int playingPieces, Board board)
    {
        ArrayList bestPoints = new ArrayList();
        int max_score = -1;
        int max_x = -1;
        int max_y = -1;

        for (int x = 0; x < board.BoardWidth; x++)
        {
            for (int y = 0; y < board.BoardHeight; y++)
            {
                if (HasNeighbor(x, y, board) && board.GetSimplifiedGameBoard()[x, y] == (int)GameplayPieceTypes.NONE)
                {
                    int score = GetTotalScoreModel(x, y, playingPieces, board);
                    if (max_x < 3 || max_y < 3 || max_x > 11 || max_y > 11)
                    {
                        score = score / 5 * 4;
                    }
                    if (score > max_score)
                    {
                        bestPoints.Clear();
                        max_score = score;
                        max_x = x;
                        max_y = y;
                    }
                    if (score == max_score)
                    {
                        bestPoints.Add(new int[] { max_x, max_y, playingPieces == (int)GameplayPieceTypes.O_Pieces ? max_score : -max_score });
                    }
                }
            }
        }
        return bestPoints;
    }

    private int GetTotalScoreModel(int x, int y, int gamePiece, Board board)
    {

        ArrayList blackNo = new ArrayList();

        int totalScore = 0;
        string model1 = "3";
        string model2 = "3";
        string model3 = "3";
        string model4 = "3";
        blackNo.Clear();

        // TODO : Fix this supreme hack 
        #region Supreme Hack

        // predict the outcome of the play by performing it and then evaluating the board
        //Add the rows
        for (int i = 1; i < 5; i++)
        {
            if (x + i < board.BoardWidth)
            {
                if (board.GetSimplifiedGameBoard()[x + i, y] == (GameplayPieceTypes)gamePiece && model1.IndexOf("00") == -1)
                {
                    model1 += '1';
                }
                else if (board.GetSimplifiedGameBoard()[x + i, y] == 0 && model1.IndexOf("00") == -1)
                {
                    model1 += '0';
                }
                else
                {
                    model1 += '2';
                    break;
                }
                if (model1.IndexOf("00") != -1)
                {
                    model1 = model1.Substring(0, model1.Length - 1);
                    break;
                }
            }
            else
            {
                model1 += '2';
                break;
            }
        }
        for (int i = 1; i < 5; i++)
        {
            if (x - i >= 0)
            {
                if (board.GetSimplifiedGameBoard()[x - i, y] == (GameplayPieceTypes)gamePiece && model1.IndexOf("00") == -1)
                {
                    model1 = '1' + model1;
                }
                else if (board.GetSimplifiedGameBoard()[x - i, y] == 0 && model1.IndexOf("00") == -1)
                {
                    model1 = '0' + model1;
                }
                else
                {
                    model1 = '2' + model1;
                    break;
                }
                if (model1.IndexOf("00") != -1)
                {
                    model1 = model1.Substring(1, model1.Length - 1);
                    break;
                }
            }
            else
            {
                model1 = '2' + model1;
                break;
            }
        }

        //Add the columns
        for (int i = 1; i < 5; i++)
        {
            if (y + i < board.BoardWidth)
            {
                if (board.GetSimplifiedGameBoard()[x, y + i] == (GameplayPieceTypes)gamePiece && model2.IndexOf("00") == -1)
                {
                    model2 += '1';
                }
                else if (board.GetSimplifiedGameBoard()[x, y + i] == 0 && model2.IndexOf("00") == -1)
                {
                    model2 += '0';
                }
                else
                {
                    model2 += '2';
                    break;
                }
                if (model2.IndexOf("00") != -1)
                {
                    model2 = model2.Substring(0, model2.Length - 1);
                    break;
                }
            }
            else
            {
                model2 += '2';
                break;
            }
        }
        for (int i = 1; i < 5; i++)
        {
            if (y - i >= 0)
            {
                if (board.GetSimplifiedGameBoard()[x, y - i] == (GameplayPieceTypes)gamePiece && model2.IndexOf("00") == -1)
                {
                    model2 = '1' + model2;
                }
                else if (board.GetSimplifiedGameBoard()[x, y - i] == 0 && model2.IndexOf("00") == -1)
                {
                    model2 = '0' + model2;
                }
                else
                {
                    model2 = '2' + model2;
                    break;
                }
                if (model2.IndexOf("00") != -1)
                {
                    model2 = model2.Substring(1, model2.Length - 1);
                    break;
                }
            }
            else
            {
                model2 = '2' + model2;
                break;
            }
        }

        //Add the diagonal
        for (int i = 1; i < 5; i++)
        {
            if (x + i < board.BoardWidth && y + i < board.BoardWidth)
            {
                if (board.GetSimplifiedGameBoard()[x + i, y + i] == (GameplayPieceTypes)gamePiece && model3.IndexOf("00") == -1)
                {
                    model3 += '1';
                }
                else if (board.GetSimplifiedGameBoard()[x + i, y + i] == 0 && model3.IndexOf("00") == -1)
                {
                    model3 += '0';
                }
                else
                {
                    model3 += '2';
                    break;
                }
                if (model3.IndexOf("00") != -1)
                {
                    model3 = model3.Substring(0, model3.Length - 1);
                    break;
                }
            }
            else
            {
                model3 += '2';
                break;
            }
        }
        for (int i = 1; i < 5; i++)
        {
            if (x - i >= 0 && y - i >= 0)
            {
                if (board.GetSimplifiedGameBoard()[x - i, y - i] == (GameplayPieceTypes)gamePiece && model3.IndexOf("00") == -1)
                {
                    model3 = '1' + model3;
                }
                else if (board.GetSimplifiedGameBoard()[x - i, y - i] == 0 && model3.IndexOf("00") == -1)
                {
                    model3 = '0' + model3;
                }
                else
                {
                    model3 = '2' + model3;
                    break;
                }
                if (model3.IndexOf("00") != -1)
                {
                    model3 = model3.Substring(1, model3.Length - 1);
                    break;
                }
            }
            else
            {
                model3 = '2' + model3;
                break;
            }
        }

        //add the inverted diagonal
        for (int i = 1; i < 5; i++)
        {
            if (x + i < board.BoardWidth && y - i >= 0)
            {
                if (board.GetSimplifiedGameBoard()[x + i, y - i] == (GameplayPieceTypes)gamePiece && model4.IndexOf("00") == -1)
                {
                    model4 += '1';
                }
                else if (board.GetSimplifiedGameBoard()[x + i, y - i] == 0 && model4.IndexOf("00") == -1)
                {
                    model4 += '0';
                }
                else
                {
                    model4 += '2';
                    break;
                }
                if (model4.IndexOf("00") != -1)
                {
                    model4 = model4.Substring(0, model4.Length - 1);
                    break;
                }
            }
            else
            {
                model4 += '2';
                break;
            }
        }
        for (int i = 1; i < 5; i++)
        {
            if (x - i >= 0 && y + i < board.BoardWidth)
            {
                if (board.GetSimplifiedGameBoard()[x - i, y + i] == (GameplayPieceTypes)gamePiece && model4.IndexOf("00") == -1)
                {
                    model4 = '1' + model4;
                }
                else if (board.GetSimplifiedGameBoard()[x - i, y + i] == 0 && model4.IndexOf("00") == -1)
                {
                    model4 = '0' + model4;
                }
                else
                {
                    model4 = '2' + model4;
                    break;
                }
                if (model4.IndexOf("00") != -1)
                {
                    model4 = model4.Substring(1, model4.Length - 1);
                    break;
                }
            }
            else
            {
                model4 = '2' + model4;
                break;
            }
        }

        // Check the possible configurations of the board and score them accordingly
        string[] models = new string[] { model1, model2, model3, model4 };
        string cPattern = "(^0*(1*31+|1+31*)0*2*$)|(^2*0*(1*31+|1+31*)0*$)";
        string bPattern = "(^2+0*(1*31+|1+31*)0*2*$)|(^2*0*(1*31+|1+31*)0*2+$)";
        string ncPattern = "[2]*[0]*(((1110|1101|1011|110|101)?3(0111|1011|1101|011|101))|((1110|1101|1011|110|101)3(0111|1011|1101|011|101)?)|((1110|1101|1011|110|101)3(0111|1011|1101|011|101)))[0]*[2]*";
        string n2Pattern = "[2]*[0]*10301[0]*[2]*";
        string n3Pattern = "[2]*[0]*(1301|1031)[0]*[2]*";
        string n4Pattern = "[2]*[0]*(11301|10311|10131|13101)[0]*[2]*";
        string nd4Pattern = "[2]*[0]*(1013101|111030111|11013011|11031011)[0]*[2]*";

        foreach (string model in models)
        {
            int twoFour = 0;
            int twoThree = 0;
            int blockThree = 0;
            if (Regex.IsMatch(model, nd4Pattern))
            {
                //Debug.Log(String.Format("ND4:{0},{1}", model, totalScore));
                if (gamePiece == 1)
                {
                    blackNo.Add(new int[x, y]);
                    totalScore = 0;
                    break;
                }
                totalScore += (int)GameplayMovementScore.FOUR;
            }
            else if (Regex.IsMatch(model, n4Pattern))
            {
                //Debug.Log(String.Format("N4:{0},{1}", model, totalScore));
                twoFour++;
                if (twoFour >= 2 && gamePiece == 1)
                {
                    blackNo.Add(new int[x, y]);
                    totalScore = 0;
                    break;
                }
                totalScore += (int)GameplayMovementScore.FOUR;
            }
            else if (Regex.IsMatch(model, n3Pattern))
            {
                //Debug.Log(String.Format("N3:{0},{1}", model, totalScore));
                if (model.Trim('2').Length >= 5)
                {
                    twoThree++;
                    if (twoThree >= 2 && gamePiece == 1)
                    {
                        blackNo.Add(new int[x, y]);
                        totalScore = 0;
                        break;
                    }
                    else if (twoThree >= 2 && gamePiece == 2)
                    {
                        totalScore += (int)GameplayMovementScore.TWO_THREE;
                        twoThree--;
                    }
                    else if (twoThree == 1 && blockThree >= 1)
                    {
                        totalScore += (int)GameplayMovementScore.THREE_THREE;
                        twoThree--;
                        blockThree--;
                    }
                    else
                    {
                        totalScore += (int)GameplayMovementScore.THREE;
                    }
                }
                else
                {
                    blockThree++;
                    if (twoThree == 1 && blockThree >= 1 && gamePiece == 1)
                    {
                        totalScore += (int)GameplayMovementScore.THREE_THREE;
                        blockThree--;
                    }
                    else if (twoThree == 1 && blockThree >= 1 && gamePiece == 2)
                    {
                        totalScore += (int)GameplayMovementScore.THREE_THREE;
                        twoThree--;
                        blockThree--;
                    }
                    else
                    {
                        totalScore += (int)GameplayMovementScore.BLOCKED_THREE;
                    }
                }
            }
            else if (Regex.IsMatch(model, ncPattern))
            {
                //Debug.Log(String.Format("NC:{0},{1}", model, totalScore));
                if (model.Trim('2').Trim('0').Length == 9)
                {
                    twoFour++;
                    if (twoFour >= 2 && gamePiece == 1)
                    {
                        blackNo.Add(new int[x, y]);
                        totalScore = 0;
                        break;
                    }
                    totalScore += (int)GameplayMovementScore.FOUR;
                }
                else if (model.Trim('2').Trim('0').Length == 8)
                {
                    twoFour++;
                    twoThree++;
                    if ((twoThree >= 2 || twoFour >= 2) && gamePiece == 1)
                    {
                        blackNo.Add(new int[x, y]);
                        totalScore = 0;
                        break;
                    }
                    totalScore += (int)GameplayMovementScore.FOUR;
                }
                else if (model.Trim('2').Trim('0').Length == 7)
                {
                    if (model.Trim('2').Trim('0').Length == model.Trim('0').Length)
                    {
                        totalScore += (int)GameplayMovementScore.TWO_THREE;
                    }
                    else
                    {
                        totalScore += (int)GameplayMovementScore.THREE_THREE;
                    }
                }
            }
            else if (Regex.IsMatch(model, n2Pattern))
            {
                //Debug.Log(String.Format("N2:{0},{1}", model, totalScore));
                if (model.Trim('2').Trim('0').Length == 5)
                {
                    if (model.Trim('2').Trim('0').Length == model.Trim('0').Length)
                    {
                        totalScore += (int)GameplayMovementScore.TWO_TWO;
                    }
                    else
                    {
                        totalScore += (int)GameplayMovementScore.BLOCKED_THREE;
                    }
                }
            }
            else if (Regex.IsMatch(model, bPattern))
            {
                if (model.Trim('2').Trim('0').Length > 5 && gamePiece == 1)
                {
                    blackNo.Add(new int[x, y]);
                    totalScore = 0;
                    break;
                }
                if (model.Trim('2').Trim('0').Length >= 5)
                {
                    totalScore += (int)GameplayMovementScore.FIVE;
                    //Debug.Log(String.Format("B:{0},{1}", model, totalScore));
                }
                else if (model.Trim('2').Trim('0').Length == 4)
                {
                    twoFour++;
                    if (twoFour >= 2 && gamePiece == 1)
                    {
                        blackNo.Add(new int[x, y]);
                        totalScore = 0;
                        break;
                    }
                    totalScore += (int)GameplayMovementScore.BLOCKED_FOUR;
                }
                else if (model.Trim('2').Trim('0').Length == 3)
                {
                    totalScore += (int)GameplayMovementScore.BLOCKED_THREE;
                }
                else if (model.Trim('2').Trim('0').Length == 2)
                {
                    totalScore += (int)GameplayMovementScore.BLOCKED_TWO;
                }
                else
                {
                    totalScore += (int)GameplayMovementScore.BLOCKED_ONE;
                    //Debug.Log(String.Format("B:{0},{1}", model, totalScore));
                }
            }
            else if (Regex.IsMatch(model, cPattern))
            {
                if (model.Trim('0').Length > 5 && gamePiece == 1)
                {
                    blackNo.Add(new int[x, y]);
                    totalScore = 0;
                    break;
                }
                if (model.Trim('0').Length >= 5)
                {
                    totalScore += (int)GameplayMovementScore.FIVE;
                    //Debug.Log(String.Format("C:{0},{1}", model, totalScore));
                }
                else if (model.Trim('0').Length == 4)
                {
                    twoFour++;
                    if (twoFour >= 2 && gamePiece == 1)
                    {
                        blackNo.Add(new int[x, y]);
                        totalScore = 0;
                        break;
                    }
                    totalScore += (int)GameplayMovementScore.FOUR;
                }
                else if (model.Trim('0').Length == 3)
                {
                    twoThree++;
                    if (twoThree >= 2 && gamePiece == 1)
                    {
                        blackNo.Add(new int[x, y]);
                        totalScore = 0;
                        break;
                    }
                    totalScore += (int)GameplayMovementScore.THREE;
                }
                else if (model.Trim('0').Length == 2)
                {
                    totalScore += (int)GameplayMovementScore.TWO;
                }
                else
                {
                    totalScore += (int)GameplayMovementScore.ONE;
                }
            }
        }

        #endregion

        return totalScore;

    }


    private bool HasNeighbor(int x, int y, Board board, int distance = 2)
    {
        for (int i = x - distance; i < x + distance; i++)
        {
            if (i < 0 || i >= board.BoardWidth)
            {
                continue;
            }
            for (int j = y - distance; j < y + distance; j++)
            {
                if (j < 0 || j >= board.BoardHeight)
                {
                    continue;
                }
                if (i == x && j == y)
                {
                    continue;
                }
                if (board.GetSimplifiedGameBoard()[i, j] != 0)
                {
                    return true;
                }
            }
        }
        return false;
    }

}
