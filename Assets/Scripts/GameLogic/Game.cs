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
            player0.ReadyToPlay(_board);
            player1.ReadyToPlay(_board);

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
            
            Debug.Log("Game End!");
        }

        public List<Action> GetValidActions(int player)
        {
            if (player != _currentTurnPlayer) return new();

            return Pot.PlayerPots[player]
                .Where(pot => _board[pot] > 0)
                .Select(pot => new Action(pot)).ToList();
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

            int opponent = 1 - player;
            int remainStones = _board[action.TargetPot];
            _board[action.TargetPot] = 0;

            var cursor = action.TargetPot;
            while (remainStones > 0)
            {
                cursor = cursor.GetNextPot();
                if (cursor == Pot.ScoringPots[opponent])
                {
                    cursor = cursor.GetNextPot();
                }

                remainStones--;
                _board[cursor]++;
            }

            var lastPot = cursor;

            // Special rule #1: Capture
            if (_board[lastPot] == 1 && Pot.PlayerPots[player].Contains(lastPot))
            {
                var opponentPot = lastPot.GetOpponentPot();
                if (_board[opponentPot] != 0)
                {
                    int sum = _board[lastPot] + _board[opponentPot];
                    _board[lastPot] = 0;
                    _board[opponentPot] = 0;
                    _board[Pot.ScoringPots[player]] += sum;
                }
            }

            // Special rule #2: Bonus turn
            int nextTurnPlayer = opponent;
            if (lastPot == Pot.ScoringPots[player])
            {
                nextTurnPlayer = player;
            }

            _currentTurnPlayer = nextTurnPlayer;
        }
    }
}
