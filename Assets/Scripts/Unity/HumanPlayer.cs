using System.Linq;
using Cysharp.Threading.Tasks;
using Mancala.GameLogic;

namespace Mancala.Unity
{
    public class HumanPlayer : Player
    {
        private UniTaskCompletionSource<Action> _tcs;
        private Board _board;
        private bool _isChoosing => _tcs != null;
        
        public override UniTask<Action> ChooseAction(Board board)
        {
            _tcs = new UniTaskCompletionSource<Action>();
            _board = board;
            return _tcs.Task;
        }

        public void OnClick_Pot(int index)
        {
            if (!_isChoosing) return;
            if (!GameLogic.Pot.PlayerPots[_playerIndex].Contains(new GameLogic.Pot(index))) return;
            if (_board[new GameLogic.Pot(index)] == 0) return;

            var prevTcs = _tcs;
            _tcs = null;
            
            prevTcs.TrySetResult(new Action(index));
        }
    }
}