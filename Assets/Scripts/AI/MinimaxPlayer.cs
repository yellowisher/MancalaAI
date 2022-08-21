using System;
using System.Collections.Generic;
using Mancala.GameLogic;
using UnityEngine;
using Action = Mancala.GameLogic.Action;

namespace Mancala.AI
{
    public class MinimaxPlayer : Player
    {
        private readonly int _maxDepth;
        private int _leafNodeCount;

        public MinimaxPlayer(int maxDepth)
        {
            _maxDepth = maxDepth;
        }

        public override Action ChooseAction(List<Action> actions)
        {
            var (action, score) = MinimaxAlphaBetaPrune(_board, 0, int.MinValue, int.MaxValue, _playerIndex);

            Debug.Log($"<color=yellow>[Minimax]</color> Player {_playerIndex} found best action with score: {score}\n" +
                      $"Leaf node count: {_leafNodeCount}");

            _leafNodeCount = 0;

            return action;
        }

        private (Action action, int score) MinimaxAlphaBetaPrune(Board board, int depth, int alpha, int beta, int playerIndex)
        {
            int myScore = board[Pot.ScoringPots[_playerIndex]];
            int opponentScore = board[Pot.ScoringPots[1 - _playerIndex]];
            int difference = myScore - opponentScore;

            if (depth > _maxDepth)
            {
                _leafNodeCount++;
                return (default, difference);
            }

            if (board.IsGameEnded)
            {
                _leafNodeCount++;
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
                    var newBoard = new Board(board);
                    int nextPlayer = newBoard.PerformAction(action);

                    var (_, score) = MinimaxAlphaBetaPrune(newBoard, depth + 1, alpha, beta, nextPlayer);
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestAction = action;
                    }

                    if (score >= beta) break; // Opponent is not gonna choose this action anyway
                    alpha = Math.Max(alpha, score);
                }
            }
            else
            {
                // Opponent's turn; minimize
                bestScore = int.MaxValue;
                foreach (var action in board.GetValidActions(playerIndex))
                {
                    var newBoard = new Board(board);
                    int nextPlayer = newBoard.PerformAction(action);

                    var (_, score) = MinimaxAlphaBetaPrune(newBoard, depth + 1, alpha, beta, nextPlayer);
                    if (score < bestScore)
                    {
                        bestScore = score;
                        bestAction = action;
                    }

                    if (score <= alpha) break; // I'm not gonna choose this action anyway
                    beta = Math.Min(beta, score);
                }
            }

            return (bestAction, bestScore);
        }
    }
}