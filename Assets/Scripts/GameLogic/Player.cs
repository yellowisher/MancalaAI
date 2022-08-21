using System.Collections.Generic;

namespace Mancala.GameLogic
{
    public abstract class Player
    {
        protected int _playerIndex;

        public void ReadyToPlay(int playerIndex)
        {
            _playerIndex = playerIndex;
        }
        
        public abstract Action ChooseAction(in Board board);
    }
}