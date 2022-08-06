using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mancala.GameLogic
{
    public class Game
    {
        private readonly List<Player> _players = new(2);
        private readonly Board _board = new();
        private int _currentTurnPlayer;

        public Game(Player player0, Player player1)
        {
            _players.Add(player0);
            _players.Add(player1);

            _currentTurnPlayer = Random.Range(0, _players.Count);
            Progress();
        }

        private void Progress()
        {
            _players[_currentTurnPlayer].ProcessTurn(GetValidActions(_currentTurnPlayer));
        }

        public List<Action> GetValidActions(int player)
        {
            if (player != _currentTurnPlayer) return new();

            return Pot.PlayerPots[player]
                .Where(pot => _board[pot] > 0)
                .Select(pot => new Action(pot)).ToList();
        }

        public void PerformAction(int player, Action action)
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

            // Check for game end
            if (_board.IsGameEnded)
            {
                for (int p = 0; p < _players.Count; p++)
                {
                    _players[p].OnGameEnded(_board[Pot.ScoringPots[p]], _board[Pot.ScoringPots[1 - p]]);
                }
                
                return;
            }
            
            // Special rule #2: Bonus turn
            int nextTurnPlayer = opponent;
            if (lastPot == Pot.ScoringPots[player])
            {
                nextTurnPlayer = player;
            }

            _currentTurnPlayer = nextTurnPlayer;
            Progress();
        }
    }
}
