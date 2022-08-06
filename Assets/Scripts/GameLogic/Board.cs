using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mancala.GameLogic
{
    public readonly struct Pot : IEquatable<Pot>
    {
        public int Index { get; }

        public const int PotCount = 14;
        public static readonly IReadOnlyList<Pot> ScoringPots = new List<Pot> { new(6), new(13) };

        public static readonly IReadOnlyList<IReadOnlyList<Pot>> PlayerPots = new List<IReadOnlyList<Pot>>()
        {
            Enumerable.Range(0, 6).Select(i => new Pot(i)).ToList(),
            Enumerable.Range(7, 6).Select(i => new Pot(i)).ToList(),
        };

        //public static implicit operator int(Pot pot) => pot.Index;
        //public static implicit operator Pot(int index) => new(index);
        public static bool operator ==(Pot p0, Pot p1) => p0.Equals(p1);
        public static bool operator !=(Pot p0, Pot p1) => !p0.Equals(p1);

        public Pot(int index)
        {
            Index = index;
        }

        public Pot GetNextPot() => new((Index + 1) % PotCount);

        public Pot GetOpponentPot() => new((PotCount - 2) - Index);

        public bool Equals(Pot other) => Index == other.Index;

        public override bool Equals(object obj) => obj is Pot other && Equals(other);

        public override int GetHashCode() => Index.GetHashCode();
    }

    [Serializable]
    public class Board
    {
        /*
         *       < < Player1 < <
         * 
         *      12 11 10  9  8  7
         *   13					   6
         *       0  1  2  3  4  5
         * 
         *       > > Player0 > >
         */

        private readonly List<int> _stoneCounts = new(Pot.PotCount);

        public int this[Pot pot]
        {
            get => _stoneCounts[pot.Index];
            set => _stoneCounts[pot.Index] = value;
        }

        public Board()
        {
            for (int i = 0; i < Pot.PotCount; i++)
            {
                int stoneCount = Pot.ScoringPots.Contains(new Pot(i)) ? 0 : 4;
                _stoneCounts.Add(stoneCount);
            }
        }
        
        public List<Action> GetValidActions(int player)
        {
            return Pot.PlayerPots[player]
                .Where(pot => this[pot] > 0)
                .Select(pot => new Action(pot)).ToList();
        }

        public int PerformAction(Action action)
        {
            int player = Pot.PlayerPots[0].Contains(action.TargetPot) ? 0 : 1; 
            int opponent = 1 - player;
            int remainStones = this[action.TargetPot];
            this[action.TargetPot] = 0;

            var cursor = action.TargetPot;
            while (remainStones > 0)
            {
                cursor = cursor.GetNextPot();
                if (cursor == Pot.ScoringPots[opponent])
                {
                    cursor = cursor.GetNextPot();
                }

                remainStones--;
                this[cursor]++;
            }

            var lastPot = cursor;

            // Special rule #1: Capture
            if (this[lastPot] == 1 && Pot.PlayerPots[player].Contains(lastPot))
            {
                var opponentPot = lastPot.GetOpponentPot();
                if (this[opponentPot] != 0)
                {
                    int sum = this[lastPot] + this[opponentPot];
                    this[lastPot] = 0;
                    this[opponentPot] = 0;
                    this[Pot.ScoringPots[player]] += sum;
                }
            }

            // Special rule #2: Bonus turn
            int nextTurnPlayer = opponent;
            if (lastPot == Pot.ScoringPots[player])
            {
                nextTurnPlayer = player;
            }

            return nextTurnPlayer;
        }

        public bool IsGameEnded
        {
            get
            {
                for (int player = 0; player < 2; player++)
                {
                    int leftStoneCount = Pot.PlayerPots[player].Sum(pot => this[pot]);
                    if (leftStoneCount == 0)
                    {
                        return true;
                    }
                }

                return false;
            }
        }
        
        public string ToVisualizeString(Board prevBoard = null, Action? action = null)
        {
            string str = "\t\t<\t  Player 1\t<\n";

            str += Pot.PlayerPots[1].Reverse().Aggregate("", (current, pot) => $"{current}\t{GetColoredString(pot)}");
            str += "\n";
            str += $"{GetColoredString(Pot.ScoringPots[1])}\t\t\t\t\t\t\t{GetColoredString(Pot.ScoringPots[0])}";
            str += "\n";
            str += Pot.PlayerPots[0].Aggregate("", (current, pot) => $"{current}\t{GetColoredString(pot)}");
            str += "\n";
            str += "\t\t<\t  Player 0\t<";
            return str;


            string GetColoredString(Pot pot)
            {
                Color? color = null;
                if (action != null && action.Value.TargetPot == pot)
                {
                    color = Color.yellow;
                }
                else if (prevBoard != null)
                {
                    if (this[pot] > prevBoard[pot]) color = Color.green;
                    else if (this[pot] < prevBoard[pot]) color = Color.red;
                }

                if (color == null) return this[pot].ToString();
                return $"<color=#{ColorUtility.ToHtmlStringRGB(color.Value)}>{this[pot].ToString()}</color>";
            }
        }
    }
}
