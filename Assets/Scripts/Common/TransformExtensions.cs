using UnityEngine;

namespace Mancala.Common
{
    public static class TransformExtensions
    {
        public static void DestroyAllActiveChildren(this Transform transform)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                var child = transform.GetChild(i);
                if (!child.gameObject.activeSelf) continue;
                
                Object.Destroy(child.gameObject);
            }
        }
    }
}