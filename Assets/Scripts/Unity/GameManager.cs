using System.Collections.Generic;
using Mancala.GameLogic;
using UnityEngine;

namespace Mancala.Unity
{
    public class GameManager : MonoBehaviour
    {
        private Player _player0;
        private Player _player1;
        private int startPlayer;
        
        private Game _game;
        private IEnumerator<Board> _playingGame;
        
        public void StartNewGame(PlayerType player0Type, PlayerType player1Type)
        {
            _game = new Game();

            _player0 = PlayerFactory.Create(player0Type);
            _player1 = PlayerFactory.Create(player1Type);

            //_playingGame = _game.Start(_player0, _player1, startPlayer);
        }
    }
}