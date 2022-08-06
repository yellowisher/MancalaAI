using System.Collections.Generic;

namespace Mancala.GameLogic
{
    public abstract class Player
    {
        protected Board _board;

        public void ReadyToPlay(Board board)
        {
            _board = board;
        }
        
        public abstract Action ChooseAction(List<Action> actions);
    }
}