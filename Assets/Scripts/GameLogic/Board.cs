using System;
using System.Collections.Generic;
using System.Linq;

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

    public class Board
    {
        /*
         *       < < Player1 < <
         * 
         *      12 11 10  9  8  7
         *   13					  6
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
    }
}
