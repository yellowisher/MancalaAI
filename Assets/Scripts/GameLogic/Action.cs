namespace Mancala.GameLogic
{
    public readonly struct Action
    {
        public Pot TargetPot { get; }

        public Action(Pot targetPot)
        {
            TargetPot = targetPot;
        }

        public override string ToString() => TargetPot.Index.ToString();
    }
}
