using Cysharp.Threading.Tasks;
using Mancala.GameLogic;

namespace Mancala.Unity
{
    public class HumanPlayer : Player
    {
        public override UniTask<Action> ChooseAction(Board board)
        {
            throw new System.NotImplementedException();
        }
    }
}