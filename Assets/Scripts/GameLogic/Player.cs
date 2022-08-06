using System.Collections.Generic;

namespace Mancala.GameLogic
{
    public abstract class Player
    {
        protected Board _board;
        protected int _playerIndex;

        public void ReadyToPlay(Board board, int playerIndex)
        {
            _board = board;
            _playerIndex = playerIndex;
        }
        
        public abstract Action ChooseAction(List<Action> actions);
    }
}