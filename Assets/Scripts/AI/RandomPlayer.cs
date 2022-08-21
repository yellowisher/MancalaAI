using System.Collections.Generic;
using Mancala.GameLogic;
using UnityEngine;

namespace Mancala.AI
{
    public class RandomPlayer : Player
    {
        public override Action ChooseAction(in Board board)
        {
            var actions = board.GetValidActions(_playerIndex);
            return actions[Random.Range(0, actions.Count)];
        }
    }
}
