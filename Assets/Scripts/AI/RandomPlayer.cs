using System.Collections.Generic;
using Mancala.GameLogic;
using UnityEngine;

namespace Mancala.AI
{
    public class RandomPlayer : Player
    {
        public override Action ChooseAction(List<Action> actions)
        {
            return actions[Random.Range(0, actions.Count)];
        }
    }
}
