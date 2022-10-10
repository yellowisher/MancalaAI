using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Mancala.Common;
using Mancala.GameLogic;
using Action = Mancala.GameLogic.Action;

namespace Mancala.AI
{
    [Serializable]
    public partial class MctsPlayer : Player
    {
        public int IterationCount = 20000;
        public float ExplorationFactor = 2f;

        public override UniTask<Action> ChooseAction(Board board)
        {
            return UniTask.RunOnThreadPool(() =>
            {
                var root = new Node(default, board, _playerIndex, null);

                int count = IterationCount;
                while (count-- > 0)
                {
                    var bestUcbNode = SelectLeafNodeWithBestUcb(root);

                    ExpandNode(bestUcbNode);

                    var nodeToExplore = bestUcbNode;
                    if (nodeToExplore.Children.Count != 0)
                    {
                        nodeToExplore = nodeToExplore.Children.PickRandom();
                    }

                    int winner = SimulateRandomPlayout(nodeToExplore);
                    BackPropagate(nodeToExplore, winner);
                }

                // Why robust?
                // https://ai.stackexchange.com/questions/16905/mcts-how-to-choose-the-final-action-from-the-root
                var bestChild = root.Children.OrderByDescending(child => child.VisitCount).First();

                string message = $"<color=yellow>[Mcts]</color> Player {_playerIndex} found best action {bestChild.Action}";
                message = root.Children.Aggregate(message,
                    (current, child) =>
                        current +
                        $"\nAction {child.Action}, Visit: {child.VisitCount}, Score: {child.Score}, Ucb: {child.CalculateUcb(root.VisitCount, ExplorationFactor)}");

                Log(message);

                return bestChild.Action;
            });
        }
    }

    public partial class MctsPlayer
    {
        private class Node
        {
            public Action Action { get; }
            public Board Board { get; }
            public int CurrentPlayer { get; }
            
            public int VisitCount { get; set; }
            public int Score { get; set; }

            public Node Parent { get; }
            public List<Node> Children { get; } = new();

            public Node(Action action, in Board board, int currentPlayer, Node parent)
            {
                Action = action;
                Board = board;
                CurrentPlayer = currentPlayer;
                Parent = parent;
            }

            public float CalculateUcb(int totalVisitCount, float explorationFactor)
            {
                if (VisitCount == 0) return float.MaxValue;
                float exploitation = (float)Score / VisitCount;
                float exploration = MathF.Sqrt(MathF.Log(totalVisitCount) / VisitCount);
                return exploitation + explorationFactor * exploration;
            }
        }

        private Node SelectLeafNodeWithBestUcb(Node node)
        {
            while (node.Children.Count != 0)
            {
                float bestUcb = float.MinValue;
                Node bestChild = null;

                foreach (var child in node.Children)
                {
                    float ucb = child.CalculateUcb(node.VisitCount, ExplorationFactor);

                    if (ucb > bestUcb)
                    {
                        bestUcb = ucb;
                        bestChild = child;
                    }
                }

                node = bestChild;
            }

            return node;
        }

        private void ExpandNode(Node node)
        {
            foreach (var action in node.Board.GetValidActions(node.CurrentPlayer))
            {
                var nextBoard = node.Board;
                int nextPlayer = nextBoard.PerformAction(action);
                
                var child = new Node(action, nextBoard, nextPlayer, node);
                node.Children.Add(child);
            }
        }

        private int SimulateRandomPlayout(Node node)
        {
            int player = node.CurrentPlayer;
            var board = node.Board;
            while (!board.IsGameEnded)
            {
                var actions = board.GetValidActions(player);
                var randomAction = actions.PickRandom();

                player = board.PerformAction(randomAction);
            }

            return board.GetWinner();
        }

        private void BackPropagate(Node node, int winner)
        {
            while (node != null)
            {
                node.VisitCount++;

                // Do not calculate score of root node
                if (node.Parent != null && winner != -1)
                {
                    node.Score += winner == node.Parent.CurrentPlayer ? 1 : -1;
                }

                node = node.Parent;
            }
        }
    }
}