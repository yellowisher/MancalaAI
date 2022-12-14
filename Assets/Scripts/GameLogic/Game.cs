using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Mancala.GameLogic
{
    public class Game
    {
        private Board _board;
        private readonly List<Player> _players = new(2);
        private int _currentTurnPlayer;

        private bool _autoProgress;
        
        public bool IsProgressing { get; private set; }
        public bool IsEnded => _currentTurnPlayer == -1;
        public int Winner => _board.GetWinner();
        public IReadOnlyList<Player> Players => _players;
        
        public Action<(Game game, BoardRenderData data)> RenderBoardFunction { get; set; }
        public bool IsSilent { get; set; }

        public UniTask Start(Player player0, Player player1, int startPlayer, bool autoProgress = true)
        {
            _board.Initialize();
            player0.ReadyToPlay(0);
            player1.ReadyToPlay(1);

            player0.IsSilent = IsSilent;
            player1.IsSilent = IsSilent;

            _players.Add(player0);
            _players.Add(player1);

            _currentTurnPlayer = startPlayer;
            if (_currentTurnPlayer != 0 && _currentTurnPlayer != 1)
            {
                _currentTurnPlayer = Random.Range(0, 2);
            }

            _autoProgress = autoProgress;

            RenderBoardFunction?.Invoke((this, new BoardRenderData(_board)));
            return Progress();
        }

        public async UniTask Progress()
        {
            if (IsProgressing || IsEnded)
            {
                return;
            }

            IsProgressing = true;
            
            double startComputationTime = Time.realtimeSinceStartupAsDouble;
            var action = await _players[_currentTurnPlayer].ChooseAction(_board);
            string log =
                $"{_currentTurnPlayer}'s Turn, Action: {action}, Computation time: {Time.realtimeSinceStartupAsDouble - startComputationTime}\n";

            log += Board.ToVisualizeString(_board);
            log += "\n";
            log += "\n";

            var prevBoard = _board;
            PerformAction(_currentTurnPlayer, action, data => RenderBoardFunction?.Invoke((this, data)));

            log += Board.ToVisualizeString(_board, prevBoard, action);
            Log(log);

            if (IsEnded)
            {
                int player0Score = _board[Pot.ScoringPots[0]];
                int player1Score = _board[Pot.ScoringPots[1]];
                
                int winner = _board.GetWinner();

                string resultString = "Game End! ";
                resultString += winner switch
                {
                    0 => $"{_players[0]} Win!\n",
                    1 => $"{_players[1]} Win!\n",
                    -1 => "Draw!\n",
                    _ => throw new ArgumentOutOfRangeException(),
                };

                resultString += $"Player 0({player0Score}) Player 1({player1Score})\n";
                Log(resultString);
            }

            IsProgressing = false;

            if (_autoProgress)
            {
                await Progress();
            }
        }

        private void Log(string message)
        {
            if (IsSilent) return;
            
            Debug.Log(message);
        }

        private void PerformAction(int player, Action action, Action<BoardRenderData> renderBoardFunction)
        {
            var validActions = _board.GetValidActions(player);
            if (!validActions.Contains(action))
            {
                throw new Exception($"Player {player} tried invalid action.\n" +
                                    $"Current turn player: {_currentTurnPlayer}\n" +
                                    $"Selected action: {action}\n" +
                                    $"While valid actions are: {string.Concat(validActions.Select(a => $"{a}, "))}");
            }

            _currentTurnPlayer = _board.PerformAction(action, renderBoardFunction);
        }
    }
}