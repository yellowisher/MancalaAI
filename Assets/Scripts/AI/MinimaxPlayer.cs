﻿using System;
using Cysharp.Threading.Tasks;
using Mancala.GameLogic;
using UnityEngine;
using Action = Mancala.GameLogic.Action;

namespace Mancala.AI
{
    [Serializable]
    public class MinimaxPlayer : Player
    {
        public int MaxDepth = 10;
        public int MoreThanHalfWeight = 1000;
        public int DifferenceWeight = 1;
        
        private int _leafNodeCount;

        public override UniTask<Action> ChooseAction(Board board)
        {
            return UniTask.RunOnThreadPool(() =>
            {
                var (action, score) = MinimaxAlphaBetaPrune(board, 0, int.MinValue, int.MaxValue, _playerIndex);

                Debug.Log($"<color=yellow>[Minimax]</color> Player {_playerIndex} found best action with score: {score}\n" +
                          $"Leaf node count: {_leafNodeCount}");

                _leafNodeCount = 0;

                return action;
            });
        }

        private (Action action, int score) MinimaxAlphaBetaPrune(in Board board, int depth, int alpha, int beta, int playerIndex)
        {
            int myScore = board[Pot.ScoringPots[_playerIndex]];
            int opponentScore = board[Pot.ScoringPots[1 - _playerIndex]];
            int difference = myScore - opponentScore;

            int moreThanHalf = 0;
            if (myScore > Board.HalfOfTotalStoneCount) moreThanHalf = 1;
            else if (opponentScore > Board.HalfOfTotalStoneCount) moreThanHalf = -1;

            int evaluatedScore = moreThanHalf * MoreThanHalfWeight + difference * DifferenceWeight;
            if (depth > MaxDepth)
            {
                _leafNodeCount++;
                return (default, evaluatedScore);
            }

            if (board.IsGameEnded)
            {
                _leafNodeCount++;
                return (default, evaluatedScore);
            }

            int bestScore;
            Action bestAction = default;

            if (playerIndex == _playerIndex)
            {
                // My turn; maximize
                bestScore = int.MinValue;
                foreach (var action in board.GetValidActions(playerIndex))
                {
                    var newBoard = board;
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
                    var newBoard = board;
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