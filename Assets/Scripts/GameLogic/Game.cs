using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Mancala.GameLogic
{
    public class Game
    {
        private Board _board;
        private readonly List<Player> _players = new(2);
        private int _currentTurnPlayer;

        public bool IsEnded => _board.IsGameEnded;

        public IEnumerator Start(Player player0, Player player1, bool selectRandomStartPlayer = true)
        {
            _board.Initialize();
            player0.ReadyToPlay(0);
            player1.ReadyToPlay(1);

            _players.Add(player0);
            _players.Add(player1);

            if (selectRandomStartPlayer)
            {
                _currentTurnPlayer = Random.Range(0, _players.Count);
            }

            double startTime = Time.realtimeSinceStartupAsDouble;
            while (!_board.IsGameEnded)
            {
                double startComputationTime = Time.realtimeSinceStartupAsDouble;
                var action = _players[_currentTurnPlayer].ChooseAction(_board);
                string log = $"{_currentTurnPlayer}'s Turn, Action: {action}, Computation time: {Time.realtimeSinceStartupAsDouble - startComputationTime}\n";

                log += Board.ToVisualizeString(_board);
                log += "\n";
                log += "\n";
                
                var prevBoard = _board;
                PerformAction(_currentTurnPlayer, action);
                
                log += Board.ToVisualizeString(_board, prevBoard, action);
                Debug.Log(log);
                yield return null;
            }

            int player0Score = _board[Pot.ScoringPots[0]];
            int player1Score = _board[Pot.ScoringPots[1]];
            
            string resultString = $"Game End! ";
            if (player0Score > player1Score) resultString += $"{_players[0]} Win!\n";
            else if (player0Score < player1Score) resultString += $"{_players[1]} Win!\n";
            else resultString += "Draw!\n";

            resultString += $"Player 0({player0Score}) Player 1({player1Score})\n";
            resultString += $"Playing time: {Time.realtimeSinceStartupAsDouble - startTime}";
            Debug.Log(resultString);
        }
        
        private void PerformAction(int player, Action action)
        {
            var validActions = _board.GetValidActions(player);
            if (!validActions.Contains(action))
            {
                Debug.LogError($"Player {player} tried invalid action.\n" +
                               $"Current turn player: {_currentTurnPlayer}\n" +
                               $"Selected action: {action}\n" +
                               $"While valid actions are: {string.Concat(validActions.Select(a => $"{a}, "))}");

                throw new Exception();
            }

            _currentTurnPlayer = _board.PerformAction(action);
        }
    }
}
