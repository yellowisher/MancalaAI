using System;
using System.Collections;
using Mancala.AI;
using Mancala.GameLogic;
using NaughtyAttributes;
using UnityEngine;

namespace Mancala
{
    public class BattleScene : MonoBehaviour
    {
        [SerializeField, SerializeReference, SubclassSelector] private Player _player0;
        [SerializeField, SerializeReference, SubclassSelector] private Player _player1;
        [SerializeField] private bool _randomStartPlayer;

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
                _playingGame = game.Start(_player0, _player1, _randomStartPlayer);
            }

            if (!_playingGame.MoveNext())
            {
                _playingGame = null;
            }
        }

        [ShowIf(nameof(IsPlaying))]
        [Button]
        private void StopGame()
        {
            _playingGame = null;
        }
    }
}