namespace Mancala.GameLogic
{
    public class BoardRenderData
    {
        public Board Board { get; }
        public int PickedCount { get; }
        public Action? Action { get; }
        public Pot? Cursor { get; }

        public BoardRenderData(Board board, int pickedCount = 0, Action? action = null, Pot? cursor = null)
        {
            Board = board;
            PickedCount = pickedCount;
            Action = action;
            Cursor = cursor;
        }
    }
}