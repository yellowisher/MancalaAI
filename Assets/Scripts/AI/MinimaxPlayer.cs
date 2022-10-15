using System;
using Cysharp.Threading.Tasks;
using Mancala.GameLogic;
using UnityEngine;
using Action = Mancala.GameLogic.Action;

namespace Mancala.AI
{
    [Serializable]
    public class MinimaxPlayer : Player
    {
        public int MaxDepth = 7;
        public int WeightScoreDifference = 1;
        public int WeightStonesOnSideDifference = 0;
        public double ComputationBudgetForOneFrame = 0.16;
        
        private int _leafNodeCount;
        private double _computeUntil; 

        public override async UniTask<Action> ChooseAction(Board board)
        {
            _computeUntil = Time.realtimeSinceStartupAsDouble + ComputationBudgetForOneFrame;

            var (action, score) = await MinimaxAlphaBetaPrune(board, 0, int.MinValue, int.MaxValue, _playerIndex);
            
            Log($"<color=yellow>[Minimax]</color> Player {_playerIndex} found best action with score: {score}\n" +
                $"Leaf node count: {_leafNodeCount}");

            _leafNodeCount = 0;

            return action;
        }

        private async UniTask<(Action action, int score)> MinimaxAlphaBetaPrune(Board board, int depth, int alpha, int beta, int playerIndex)
        {
            if (Time.realtimeSinceStartupAsDouble > _computeUntil)
            {
                await UniTask.NextFrame();
                _computeUntil = Time.realtimeSinceStartupAsDouble + ComputationBudgetForOneFrame;
            }
            
            if (depth > MaxDepth || board.IsGameEnded)
            {
                _leafNodeCount++;

                int opponent = 1 - _playerIndex;
                
                int myScore = board[Pot.ScoringPots[_playerIndex]];
                int opponentScore = board[Pot.ScoringPots[opponent]];
                int scoreDifference = myScore - opponentScore;

                int stonesOnMySide = 0;
                foreach (var pot in Pot.PlayerPots[_playerIndex])
                {
                    stonesOnMySide += board[pot];
                }

                int stonesOnOpponentSide = 0;
                foreach (var pot in Pot.PlayerPots[opponent])
                {
                    stonesOnOpponentSide += board[pot];
                }

                int stonesOnSideDifference = stonesOnMySide - stonesOnOpponentSide; 

                int evaluatedScore =
                    WeightScoreDifference * scoreDifference +
                    WeightStonesOnSideDifference * stonesOnSideDifference;

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

                    int nextDepth = depth + (nextPlayer == playerIndex ? 0 : 1);
                    var (_, score) = await MinimaxAlphaBetaPrune(newBoard, nextDepth, alpha, beta, nextPlayer);
                    
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

                    int nextDepth = depth + (nextPlayer == playerIndex ? 0 : 1);
                    var (_, score) = await MinimaxAlphaBetaPrune(newBoard, nextDepth, alpha, beta, nextPlayer);
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