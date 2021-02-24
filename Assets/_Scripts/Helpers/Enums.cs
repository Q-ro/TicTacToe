/*

Author: Andres Mrad (Q-ro)
Date: Thursday 21/February/2021 @ 17:17:54
Description:  Game pieces enums 

*/

namespace GameplayEnum
{
    //GamePlayPieceTypes
    public enum GameplayPieceTypes
    {
        NONE,
        X_Pieces,
        O_Pieces
    }

    public enum GameplayPlayerTypes
    {
        Human,
        Bot,
    }

    public enum GameplayPlayerTurnTypes
    {
        P1Turn,
        P2Turn
    }

    public enum GameplayMovementScore
    {

        ONE = 0,
        TWO = 20,
        THREE = 80,
        FOUR = 320,
        FIVE = 1280,
        BLOCKED_ONE = 0,
        BLOCKED_TWO = 5,
        BLOCKED_THREE = 20,
        BLOCKED_FOUR = 80,
        TWO_THREE = 310,
        THREE_THREE = 270,
        TWO_TWO = 40


        // ONE = 80,
        // TWO = 320,
        // THREE = 1280,

        // ONE_RIVAL = -80,
        // TWO_RIVAL = -320,
        // THREE_RIVAL = -1280,


    }

    public enum GameplayBotDifficulty
    {
        EASY,
        MEDIUM,
        HARD
    }
}