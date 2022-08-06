using System.Collections.Generic;
using System.Linq;
using Mancala.Common;
using UnityEngine;

namespace Mancala.GameLogic
{
    public class Game
    {
        private readonly Board _board = new();
        private readonly List<Player> _players = new(2);
        private int _currentTurnPlayer;

        public void Start(Player player0, Player player1, bool selectRandomStartPlayer = true)
        {
            player0.ReadyToPlay(_board, 0);
            player1.ReadyToPlay(_board, 1);

            _players.Add(player0);
            _players.Add(player1);

            if (selectRandomStartPlayer)
            {
                _currentTurnPlayer = Random.Range(0, _players.Count);
            }

            while (!_board.IsGameEnded)
            {
                var action = _players[_currentTurnPlayer].ChooseAction(GetValidActions(_currentTurnPlayer));
                string log = $"{_currentTurnPlayer}'s Turn, Action: {action}\n";

                log += _board.ToVisualizeString();
                log += "\n";
                log += "\n";
                
                var prevBoard = _board.DeepClone();
                PerformAction(_currentTurnPlayer, action);
                
                log += _board.ToVisualizeString(prevBoard, action);
                Debug.Log(log);
            }

            int player0Score = _board[Pot.ScoringPots[0]];
            int player1Score = _board[Pot.ScoringPots[1]];
            
            string resultString = $"Game End! ";
            if (player0Score < player1Score) resultString += $"{_players[0]} Win!\n";
            else if (player0Score > player1Score) resultString += $"{_players[1]} Win!\n";
            else resultString += "Draw!\n";

            resultString += $"Player 0({player0Score}) Player 1({player1Score})";
            Debug.Log(resultString);
        }

        public List<Action> GetValidActions(int player)
        {
            if (player != _currentTurnPlayer) return new();

            return _board.GetValidActions(player);
        }
        
        private void PerformAction(int player, Action action)
        {
            var validActions = GetValidActions(player);
            if (!validActions.Contains(action))
            {
                Debug.LogError($"Player {player} tried invalid action.\n" +
                               $"Current turn player: {_currentTurnPlayer}\n" +
                               $"Selected action: {action}\n" +
                               $"While valid actions are: {string.Concat(validActions.Select(a => $"{a}, "))}");

                return;
            }

            _currentTurnPlayer = _board.PerformAction(action);
        }
    }
}
