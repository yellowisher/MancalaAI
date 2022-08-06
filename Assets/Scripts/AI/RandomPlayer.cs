using System.Collections.Generic;
using Mancala.GameLogic;
using UnityEngine;

namespace Mancala.AI
{
    public class RandomPlayer : Player
    {
        public override void ProcessTurn(List<Action> actions)
        {
            var action = actions[Random.Range(0, actions.Count)];
            Game.PerformAction(this, action);
        }
    }
}
