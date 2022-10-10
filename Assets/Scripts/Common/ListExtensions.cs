using System.Collections.Generic;

namespace Mancala.Common
{
    public static class ListExtensions
    {
        public static T PickRandom<T>(this List<T> list)
        {
            var random = new System.Random();
            if (list == null || list.Count == 0) return default;
            return list[random.Next(list.Count)];
        }
    }
}
