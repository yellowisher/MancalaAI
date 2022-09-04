using System.Collections.Generic;
using UnityEngine;

namespace Mancala.Common
{
    public static class ListExtensions
    {
        public static T PickRandom<T>(this List<T> list)
        {
            if (list == null || list.Count == 0) return default;
            return list[Random.Range(0, list.Count)];
        }
    }
}
