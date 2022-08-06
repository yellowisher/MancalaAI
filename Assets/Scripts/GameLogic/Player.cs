using System.Collections.Generic;

namespace Mancala.GameLogic
{
    public abstract class Player
    {
        protected Game _game;
        protected int _playerIndex;

        public void ReadyToPlay(Game game, int playerIndex)
        {
            _game = game;
            _playerIndex = playerIndex;
        }
        
        public abstract void ProcessTurn(List<Action> actions);

        public virtual void OnGameEnded(int myScore, int opponentScore)
        {
        }
    }
}