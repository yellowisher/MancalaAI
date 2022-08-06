using System.Collections.Generic;

namespace Mancala.GameLogic
{
    public abstract class Player
    {
        public abstract void ProcessTurn(List<Action> actions);

        public virtual void OnGameEnded(int myScore, int opponentScore)
        {
        }
    }
}