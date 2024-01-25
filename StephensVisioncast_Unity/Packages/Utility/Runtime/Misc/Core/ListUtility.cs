using System.Collections.Generic;

namespace Stephens.Utility.Core
{
    public static class ListUtility
    {
        //
        public static List<T> ShiftLeft<T> (this List<T> list, int shiftBy)
        {
            if (list.Count <= shiftBy)
            {
                return list;
            }

            var result = list.GetRange (shiftBy, list.Count - shiftBy);
            result.AddRange (list.GetRange (0, shiftBy));
            return result;
        }


        //
        public static List<T> ShiftRight<T> (this List<T> list, int shiftBy)
        {
            if (list.Count <= shiftBy)
            {
                return list;
            }

            var result = list.GetRange (list.Count - shiftBy, shiftBy);
            result.AddRange (list.GetRange (0, list.Count - shiftBy));
            return result;
        }


        //
        public static void Shuffle <T> (this IList <T> ts)
        {
            var count = ts.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i)
            {
                var r = UnityEngine.Random.Range (i, count);
                var tmp = ts [i];
                ts [i] = ts [r];
                ts [r] = tmp;
            }
        }
    }
}
