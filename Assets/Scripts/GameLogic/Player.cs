using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Mancala.GameLogic
{
    [Serializable]
    public abstract class Player
    {
        public bool IsSilent { get; set; }
        
        protected int _playerIndex;

        public void ReadyToPlay(int playerIndex)
        {
            _playerIndex = playerIndex;
        }

        protected void Log(string message)
        {
            if (IsSilent) return;
            Debug.Log(message);
        }
        
        public abstract UniTask<Action> ChooseAction(Board board);
    }
}