using System;
using Mancala.GameLogic;
using Action = Mancala.GameLogic.Action;
using Random = UnityEngine.Random;

namespace Mancala.AI
{
    [Serializable]
    public class RandomPlayer : Player
    {
        public override Action ChooseAction(in Board board)
        {
            var actions = board.GetValidActions(_playerIndex);
            return actions[Random.Range(0, actions.Count)];
        }
    }
}
