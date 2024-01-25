using UnityEngine;

namespace Stephens.Utility.Core
{
    public static class Vector2Utility
    {
        // TO USE:
        // myVec.position = myVec.position.Change( y: 1.3f );
        //public static Vector2 Change (this Vector2 org, object x = null, object y = null)
        //{
        //    return new Vector2 ((x == null ? org.x, (float) x), (y == null ? org.y, (float) y));
        //}


        public static Vector2 [] ToVector2Array (this Vector3 [] v3)
        {
            return System.Array.ConvertAll<Vector3, Vector2> (v3, GetV3fromV2);
        }


        public static Vector2 GetV3fromV2 (Vector3 v3)
        {
            return new Vector2 (v3.x, v3.y);
        }

        public static bool Contains(this Vector3 v2, float target, bool minInclusive = true, bool maxInclusive = true)
        {
            bool min = maxInclusive ? target >= v2.x : target > v2.x;
            bool max = maxInclusive ? target <= v2.y : target < v2.y;
            return min && max;
        }
    }
}
