using System;
using System.Collections.Generic;
using Mancala.GameLogic;
using Mancala.Unity;
using NaughtyAttributes;
using UnityEngine;

namespace Mancala
{
    public class TestScript : MonoBehaviour
    {
        private class MatchUp
        {
            public PlayerType Player0Type { get; }
            public PlayerType Player1Type { get; }

            public MatchUp(PlayerType player0Type, PlayerType player1Type)
            {
                Player0Type = player0Type;
                Player1Type = player1Type;
            }
        }

        private class Result
        {
            public int Win { get; set; }
            public int Lose { get; set; }
            public int Draw { get; set; }

            public void AddFromWinner(int winner)
            {
                switch (winner)
                {
                    case 0:
                    {
                        Win++;
                        break;
                    }
                    case 1:
                    {
                        Lose++;
                        break;
                    }
                    case -1:
                    {
                        Draw++;
                        break;
                    }
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }
        
        [SerializeField] private int _gameCount;

        private List<PlayerType> _playerTypes = new()
        {
            PlayerType.Random,
            PlayerType.Minimax,
            PlayerType.Mcts,
        };
        
        [Button]
        public async void Test()
        {
            var results = new Dictionary<MatchUp, Result>();
            
            foreach (var player0Type in _playerTypes)
            {
                foreach (var player1Type in _playerTypes)
                {
                    var matchUp = new MatchUp(player0Type, player1Type);
                    var result = new Result();
                    
                    results.Add(matchUp, result);
                    
                    for (int i = 0; i < _gameCount; i++)
                    {
                        var player0 = PlayerFactory.Create(player0Type);
                        var player1 = PlayerFactory.Create(player1Type);

                        var game = new Game();
                        game.IsSilent = true;

                        await game.Start(player0, player1, 0);
                        result.AddFromWinner(game.Winner);
                    }
                }
            }

            string report = string.Empty;
            foreach (var pair in results)
            {
                int winRate = pair.Value.Win * 100 / _gameCount;
                int loseRate = pair.Value.Lose * 100 / _gameCount;
                int drawRate = pair.Value.Draw * 100 / _gameCount;

                report += $"{pair.Key.Player0Type} vs {pair.Key.Player1Type}\n";
                report += $"Win: {winRate}% ({pair.Value.Win}), Lose: {loseRate}% ({pair.Value.Lose})";
                if (pair.Value.Draw > 0)
                {
                    report += $", Draw: {drawRate}% ({pair.Value.Draw})";
                }

                report += "\n\n";
            }

            Debug.Log(report);
        }
    }
}