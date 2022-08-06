using System;
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
        }

        [SerializeField] private PlayerType _player0Type;
        [SerializeField] private PlayerType _player1Type;
        [SerializeField] private bool _randomStartPlayer;
        [SerializeField] private int _maxDepthForMinimax;

        [Button]
        public void Battle()
        {
            var player0 = CreatePlayerOf(_player0Type);
            var player1 = CreatePlayerOf(_player1Type);

            var game = new Game();
            game.Start(player0, player1, _randomStartPlayer);
        }

        public Player CreatePlayerOf(PlayerType type)
        {
            return type switch
            {
                PlayerType.Human => throw new ArgumentOutOfRangeException(nameof(type), type, null),
                PlayerType.Random => new RandomPlayer(),
                PlayerType.Minimax => new MinimaxPlayer(_maxDepthForMinimax),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
}
