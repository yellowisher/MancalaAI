using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mancala.GameLogic
{
    public class Game
    {
        public Board Board { get; } = new();

        private readonly List<Player> _players = new(2);
        private int _currentTurnPlayer;

        public void Start(Player player0, Player player1, bool selectRandomStartPlayer = true)
        {
            player0.ReadyToPlay(this, 0);
            player1.ReadyToPlay(this, 1);

            _players.Add(player0);
            _players.Add(player1);

            if (selectRandomStartPlayer)
            {
                _currentTurnPlayer = Random.Range(0, _players.Count);
            }

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
                .Where(pot => Board[pot] > 0)
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
            int remainStones = Board[action.TargetPot];
            Board[action.TargetPot] = 0;

            var cursor = action.TargetPot;
            while (remainStones > 0)
            {
                cursor = cursor.GetNextPot();
                if (cursor == Pot.ScoringPots[opponent])
                {
                    cursor = cursor.GetNextPot();
                }

                remainStones--;
                Board[cursor]++;
            }

            var lastPot = cursor;

            // Special rule #1: Capture
            if (Board[lastPot] == 1 && Pot.PlayerPots[player].Contains(lastPot))
            {
                var opponentPot = lastPot.GetOpponentPot();
                if (Board[opponentPot] != 0)
                {
                    int sum = Board[lastPot] + Board[opponentPot];
                    Board[lastPot] = 0;
                    Board[opponentPot] = 0;
                    Board[Pot.ScoringPots[player]] += sum;
                }
            }

            // Check for game end
            if (Board.IsGameEnded)
            {
                for (int p = 0; p < _players.Count; p++)
                {
                    _players[p].OnGameEnded(Board[Pot.ScoringPots[p]], Board[Pot.ScoringPots[1 - p]]);
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
