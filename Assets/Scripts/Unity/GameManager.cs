using System;
using System.Collections.Generic;
using System.Linq;
using Mancala.GameLogic;
using TMPro;
using UnityEngine;

namespace Mancala.Unity
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown _player0Dropdown;
        [SerializeField] private TMP_Dropdown _player1Dropdown;
        [SerializeField] private TMP_Dropdown _startPlayerDropdown;

        private Game _playingGame;
        
        public List<Pot> Pots { get; private set; }
        
        public static GameManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            var options = new List<TMP_Dropdown.OptionData>();
            foreach (PlayerType value in Enum.GetValues(typeof(PlayerType)))
            {
                options.Add(new TMP_Dropdown.OptionData(value.ToString()));
            }
            
            _player0Dropdown.options = options;
            _player1Dropdown.options = options;

            Pots = FindObjectsOfType<Pot>()
                .OrderBy(pot => pot.Index)
                .ToList();
            
            Pot.OnClickEvent.AddListener(OnClick_Pot);

            StartNewGame();
        }

        public void StartNewGame()
        {
            _playingGame = new Game();
            _playingGame.RenderBoardAction = RenderBoard;

            var player0 = PlayerFactory.Create(GetPlayerTypeFromDropdownIndex(_player0Dropdown.value));
            var player1 = PlayerFactory.Create(GetPlayerTypeFromDropdownIndex(_player1Dropdown.value));
            int startPlayer = _startPlayerDropdown.value;

            _playingGame.Start(player0, player1, startPlayer);
        }

        private void RenderBoard(Board board)
        {
            for (int i = 0; i < GameLogic.Pot.PotCount; i++)
            {
                Pots[i].SetStoneCount(board[new GameLogic.Pot(i)]);
            }
        }
        
        private void OnClick_Pot(int index)
        {
            foreach (var player in _playingGame.Players)
            {
                (player as HumanPlayer)?.OnClick_Pot(index);
            }
        }

        private static PlayerType GetPlayerTypeFromDropdownIndex(int index)
        {
            return (PlayerType)index;
        }
    }
}