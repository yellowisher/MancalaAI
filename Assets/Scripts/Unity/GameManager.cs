using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Mancala.GameLogic;
using TMPro;
using UnityEngine;

namespace Mancala.Unity
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private float _renderDelay;

        [SerializeField] private TMP_Dropdown _player0Dropdown;
        [SerializeField] private TMP_Dropdown _player1Dropdown;
        [SerializeField] private TMP_Dropdown _startPlayerDropdown;
        [SerializeField] private TextMeshProUGUI _winnerText;
        [SerializeField] private AudioClip _audioClip;

        private Game _playingGame;
        private Pot _pickingPot;
        private float _nextRenderTime;
        private readonly Queue<BoardRenderData> _renderQueue = new();

        private AudioSource _audioSource;

        public List<Pot> Pots { get; private set; }

        public static GameManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;

            _audioSource = GetComponentInChildren<AudioSource>();
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

            Pots = FindObjectsOfType<Pot>(true)
                .OrderBy(pot => pot.Index)
                .ToList();

            // Picking pot for animation
            _pickingPot = Pots[0];
            Pots.RemoveAt(0);

            Pot.OnClickEvent.AddListener(OnClick_Pot);

            StartNewGame();
        }

        public async void StartNewGame()
        {
            _playingGame = new Game();
            _playingGame.RenderBoardFunction = AppendRenderData;

            var player0 = PlayerFactory.Create(GetPlayerTypeFromDropdownIndex(_player0Dropdown.value));
            var player1 = PlayerFactory.Create(GetPlayerTypeFromDropdownIndex(_player1Dropdown.value));
            int startPlayer = _startPlayerDropdown.value;

            _renderQueue.Clear();

            foreach (var pot in Pots)
            {
                pot.SetInteractable(false);
            }

            _winnerText.gameObject.SetActive(false);

            var game = _playingGame;
            await _playingGame.Start(player0, player1, startPlayer);
            await UniTask.WaitUntil(() => _renderQueue.Count == 0);

            if (_playingGame == game)
            {
                string gameEndString = "Game End;\n";
                gameEndString += _playingGame.Winner switch
                {
                    0 => "Player 0 win!",
                    1 => "Player 1 win!",
                    _ => "Draw!",
                };

                _winnerText.gameObject.SetActive(true);
                _winnerText.text = gameEndString;
            }
        }

        private void AppendRenderData((Game game, BoardRenderData data) pair)
        {
            if (_playingGame != pair.game) return;
            _renderQueue.Enqueue(pair.data);
        }

        private void Update()
        {
            if (Time.time < _nextRenderTime) return;
            if (_renderQueue.Count == 0) return;

            _nextRenderTime = Time.time + _renderDelay;
            RenderBoard(_renderQueue.Dequeue());
        }

        private void RenderBoard(BoardRenderData data)
        {
            for (int i = 0; i < GameLogic.Pot.PotCount; i++)
            {
                Pots[i].SetStoneCount(data.Board[new GameLogic.Pot(i)]);
                bool isActionStartedPot = data.Action != null && data.Action.Value.TargetPot.Index == i;
                Pots[i].SetOutlineColor(isActionStartedPot ? Color.red : Color.black);
            }

            if (data.Action != null)
            {
                _audioSource.PlayOneShot(_audioClip);
            }

            _pickingPot.gameObject.SetActive(data.PickedCount > 0);
            _pickingPot.SetStoneCount(data.PickedCount);
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
