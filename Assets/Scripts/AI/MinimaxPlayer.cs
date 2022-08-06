using System;
using System.Collections.Generic;
using Mancala.Common;
using Mancala.GameLogic;
using UnityEngine;
using Action = Mancala.GameLogic.Action;

namespace Mancala.AI
{
    public class MinimaxPlayer : Player
    {
        public override Action ChooseAction(List<Action> actions)
        {
            var (action, score) = Minimax(_board, 0, _playerIndex);
            Debug.Log($"<color=yellow>[Minimax]</color> Player {_playerIndex} found best action with score: {score}");
            return action;
        }

        private (Action action, int score) Minimax(Board board, int depth, int playerIndex)
        {
            if (board.IsGameEnded)
            {
                int myScore = board[Pot.ScoringPots[_playerIndex]];
                int opponentScore = board[Pot.ScoringPots[1 - _playerIndex]];

                int difference = myScore - opponentScore;
                int score = 1000 * Math.Sign(difference) + difference;
                return (default, score);
            }

            int bestScore;
            Action bestAction = default;
            
            if (playerIndex == _playerIndex)
            {
                // My turn; maximize
                bestScore = int.MinValue;
                foreach (var action in board.GetValidActions(playerIndex))
                {
                    var newBoard = board.DeepClone();
                    int nextPlayer = newBoard.PerformAction(action);

                    var (_, score) = Minimax(newBoard, depth + 1, nextPlayer);
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestAction = action;
                    }
                }
            }
            else
            {
                // Opponent's turn; minimize
                bestScore = int.MaxValue;
                foreach (var action in board.GetValidActions(playerIndex))
                {
                    var newBoard = board.DeepClone();
                    int nextPlayer = newBoard.PerformAction(action);

                    var (_, score) = Minimax(newBoard, depth + 1, nextPlayer);
                    if (score < bestScore)
                    {
                        bestScore = score;
                        bestAction = action;
                    }
                }
            }

            return (bestAction, bestScore);
        }
    }
}
