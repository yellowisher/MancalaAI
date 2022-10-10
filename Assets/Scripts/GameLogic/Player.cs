using System;
using Cysharp.Threading.Tasks;

namespace Mancala.GameLogic
{
    [Serializable]
    public abstract class Player
    {
        protected int _playerIndex;

        public void ReadyToPlay(int playerIndex)
        {
            _playerIndex = playerIndex;
        }
        
        public abstract UniTask<Action> ChooseAction(Board board);
    }
}