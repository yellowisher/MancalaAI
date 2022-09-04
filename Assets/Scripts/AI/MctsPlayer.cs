using System;
using System.Collections.Generic;
using System.Linq;
using Mancala.GameLogic;
using UnityEngine;
using Action = Mancala.GameLogic.Action;
using Random = System.Random;

namespace Mancala.AI
{
    public partial class MctsPlayer : Player
    {
        private readonly double _searchTimeout;
        private readonly Random _random = new();

        private const float ExplorationFactor = 2f;

        public MctsPlayer(double searchTimeout)
        {
            _searchTimeout = searchTimeout;
        }

        public override Action ChooseAction(in Board board)
        {
            var root = new Node(default, board, _playerIndex, null);

            double searchUntil = Time.realtimeSinceStartupAsDouble + _searchTimeout;
            while (Time.realtimeSinceStartupAsDouble < searchUntil)
            {
                var bestUctNode = SelectLeafNodeWithBestUcb(root);
                if (!bestUctNode.Board.IsGameEnded)
                {
                    ExpandNode(bestUctNode);
                }

                var nodeToExplore = bestUctNode;
                if (nodeToExplore.Children.Count != 0)
                {
                    nodeToExplore = nodeToExplore.Children[_random.Next(nodeToExplore.Children.Count)];
                }

                int simulateResult = SimulateRandomPlayout(nodeToExplore);
                BackPropagate(nodeToExplore, simulateResult);
            }

            var bestChild = root.Children.OrderByDescending(child => child.VisitCount).First();
            return bestChild.Action;
        }
    }

    public partial class MctsPlayer : Player
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
        }

        private Node SelectLeafNodeWithBestUcb(Node node)
        {
            while (node.Children.Count != 0)
            {
                float bestUcb = float.MinValue;
                Node bestChild = null;

                foreach (var child in node.Children)
                {
                    float ucb = float.MaxValue;
                    if (child.VisitCount > 0)
                    {
                        float exploitation = (float)child.Score / child.VisitCount;
                        float exploration =  MathF.Sqrt(MathF.Log(node.VisitCount) / child.VisitCount);
                        ucb = exploitation + ExplorationFactor * exploration;
                    }

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
            int player = _playerIndex;
            var board = node.Board;
            while (!board.IsGameEnded)
            {
                var actions = board.GetValidActions(player);
                var randomAction = actions[_random.Next(actions.Count)];

                player = board.PerformAction(randomAction);
            }

            int playerScore = board[Pot.ScoringPots[_playerIndex]];
            int opponentScore = board[Pot.ScoringPots[1 - _playerIndex]];
            return Math.Sign(playerScore - opponentScore);
        }

        private void BackPropagate(Node node, int result)
        {
            while (node != null)
            {
                node.VisitCount++;
                node.Score += result;
                node = node.Parent;
            }
        }
    }
}