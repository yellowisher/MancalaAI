using System.Collections.Generic;
using Mancala.GameLogic;
using UnityEngine;

namespace Mancala.AI
{
    public class RandomPlayer : Player
    {
        public override void ProcessTurn(List<Action> actions)
        {
            string log = $"{_playerIndex}'s Turn:\n{_game.Board}\n";
                
            var action = actions[Random.Range(0, actions.Count)];
            log += $"Select action: {action}\n\n";
            
            _game.PerformAction(_playerIndex, action);
            log += _game.Board;
            
            Debug.Log(log);
        }
    }
}
