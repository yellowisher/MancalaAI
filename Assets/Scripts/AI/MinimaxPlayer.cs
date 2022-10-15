using System;
using Cysharp.Threading.Tasks;
using Mancala.GameLogic;
using Action = Mancala.GameLogic.Action;

namespace Mancala.AI
{
    [Serializable]
    public class MinimaxPlayer : Player
    {
        public int MaxDepth = 7;
        public int WeightScoreDifference = 1;
        public int WeightStonesOnSideDifference = 0;
        
        private int _leafNodeCount;

        public override async UniTask<Action> ChooseAction(Board board)
        {
            int bestScore = int.MinValue;
            Action bestAction = default;

            foreach (var action in board.GetValidActions(_playerIndex))
            {
                var newBoard = board;
                int nextPlayer = newBoard.PerformAction(action);

                int nextDepth = nextPlayer == _playerIndex ? 0 : 1;
                var (_, score) = MinimaxAlphaBetaPrune(newBoard, nextDepth, int.MinValue, int.MaxValue, nextPlayer);
                    
                if (score > bestScore)
                {
                    bestScore = score;
                    bestAction = action;
                }

                await UniTask.NextFrame();
            }

            Log($"<color=yellow>[Minimax]</color> Player {_playerIndex} found best action with score: {bestAction}\n" +
                $"Leaf node count: {_leafNodeCount}");

            _leafNodeCount = 0;

            return bestAction;
        }

        private (Action action, int score) MinimaxAlphaBetaPrune(in Board board, int depth, int alpha, int beta, int playerIndex)
        {
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
                    var (_, score) = MinimaxAlphaBetaPrune(newBoard, nextDepth, alpha, beta, nextPlayer);
                    
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
                    var (_, score) = MinimaxAlphaBetaPrune(newBoard, nextDepth, alpha, beta, nextPlayer);
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