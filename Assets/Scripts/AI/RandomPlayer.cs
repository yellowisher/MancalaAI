using System;
using Cysharp.Threading.Tasks;
using Mancala.GameLogic;
using Action = Mancala.GameLogic.Action;
using Random = UnityEngine.Random;

namespace Mancala.AI
{
    [Serializable]
    public class RandomPlayer : Player
    {
        public override UniTask<Action> ChooseAction(Board board)
        {
            var actions = board.GetValidActions(_playerIndex);
            return new UniTask<Action>(actions[Random.Range(0, actions.Count)]);
        }
    }
}
