using System;
using Mancala.AI;
using Mancala.GameLogic;

namespace Mancala.Unity
{
    public enum PlayerType
    {
        Human,
        Random,
        Minimax,
        Mcts,
    }
    
    public static class PlayerFactory
    {
        public static Player Create(PlayerType playerType)
        {
            return playerType switch
            {
                PlayerType.Human => new HumanPlayer(),
                PlayerType.Random => new RandomPlayer(),
                PlayerType.Minimax => new MinimaxPlayer(),
                PlayerType.Mcts => new MctsPlayer(),
                _ => throw new NotImplementedException()
            };
        }
    }
}