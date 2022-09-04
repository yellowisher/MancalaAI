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
        public enum PlayerType
        {
            Human,
            Random,
            Minimax,
            Mcts,
        }

        [SerializeField] private PlayerType _player0Type;
        [SerializeField] private PlayerType _player1Type;
        [SerializeField] private bool _randomStartPlayer;

        [Header("Minimax")] 
        [SerializeField] private int _maxDepthForMinimax;

        [Header("Mcts")] 
        [SerializeField] private float _mctsTimeout;

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
                var player0 = CreatePlayerOf(_player0Type);
                var player1 = CreatePlayerOf(_player1Type);

                var game = new Game();
                _playingGame = game.Start(player0, player1, _randomStartPlayer);
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

        public Player CreatePlayerOf(PlayerType type)
        {
            return type switch
            {
                PlayerType.Human => throw new ArgumentOutOfRangeException(nameof(type), type, null),
                PlayerType.Random => new RandomPlayer(),
                PlayerType.Minimax => new MinimaxPlayer(_maxDepthForMinimax),
                PlayerType.Mcts => new MctsPlayer(_mctsTimeout),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
}