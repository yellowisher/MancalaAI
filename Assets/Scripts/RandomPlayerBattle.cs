using System;
using Mancala.AI;
using Mancala.GameLogic;
using UnityEngine;

namespace Mancala
{
    public class RandomPlayerBattle : MonoBehaviour
    {
        private void Start()
        {
            var game = new Game();
            var player0 = new RandomPlayer();
            var player1 = new RandomPlayer();

            game.Start(player0, player1);
        }
    }
}
