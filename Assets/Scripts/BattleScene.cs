using System.Collections;
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

        private IEnumerator _playingGame;
        private bool IsPlaying => _playingGame != null;

        [Button]
        private void PlayWholeGame()
        {
            if (_playingGame == null)
            {
                PlayOneStep();
            }

            while (IsPlaying)
            {
                PlayOneStep();
            }
        }

        [Button]
        private void PlayOneStep()
        {
            if (_playingGame == null)
            {
                var game = new Game();
                _playingGame = game.Start(_player0, _player1, _startPlayer);
            }

            if (!_playingGame.MoveNext())
            {
                _playingGame = null;
            }
        }

        [ShowIf(nameof(IsPlaying))]
        [Button]
        private void StopPlaying()
        {
            _playingGame = null;
        }
    }
}