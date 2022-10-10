using Cysharp.Threading.Tasks;
using Mancala.GameLogic;
using NaughtyAttributes;
using UnityEngine;

namespace Mancala
{
    public class BattleScene : MonoBehaviour
    {
        [SerializeField, SerializeReference, SubclassSelector] private Player _player0;
        [SerializeField, SerializeReference, SubclassSelector] private Player _player1;
        
        [Header("0 or 1 (otherwise -> random)")]
        [SerializeField] private int _startPlayer;

        private Game _playingGame;
        private bool IsPlaying => _playingGame != null;
        private bool IsNotPlaying => !IsPlaying;
        private bool IsProgressing => _playingGame.IsProgressing;

        [Button]
        [ShowIf(nameof(IsNotPlaying))]
        private async UniTask StartNewGame()
        {
            _playingGame = new Game();
            await _playingGame.Start(_player0, _player1, _startPlayer);
        }
        
        [Button]
        [ShowIf(nameof(IsNotPlaying))]
        private async void StartNewWholeGame()
        {
            await StartNewGame();
            PlayWholeGame();
        }
        
        [Button]
        [ShowIf(nameof(IsPlaying))]
        [DisableIf(nameof(IsProgressing))]
        private async UniTask PlayOneStep()
        {
            await _playingGame.Progress();
            if (_playingGame.IsEnded)
            {
                _playingGame = null;
            }
        }
        
        [Button]
        [ShowIf(nameof(IsPlaying))]
        [DisableIf(nameof(IsProgressing))]
        private async void PlayWholeGame()
        {
            while (IsPlaying)
            {
                await PlayOneStep();
            }
        }

        [Button]
        [ShowIf(nameof(IsPlaying))]
        [DisableIf(nameof(IsProgressing))]
        private void StopPlaying()
        {
            _playingGame = null;
        }
    }
}