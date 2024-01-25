// FROM 
// https://docs.unity3d.com/Manual/BestPracticeUnderstandingPerformanceInUnity5.html

namespace Stephens.Utility.Core
{
    public static class StringUtility
    {
        public static bool CustomEndsWith (string a, string b)
        {
            int ap = a.Length - 1;
            int bp = b.Length - 1;

            while (ap >= 0 && bp >= 0 && a [ap] == b [bp])
            {
                ap--;
                bp--;
            }

            return (bp < 0 && a.Length >= b.Length) || (ap < 0 && b.Length >= a.Length);
        }


        // 
        public static bool CustomStartsWith (string a, string b)
        {
            int aLen = a.Length;
            int bLen = b.Length;
            int ap = 0; int bp = 0;

            while (ap < aLen && bp < bLen && a [ap] == b [bp])
            {
                ap++;
                bp++;
            }

            return (bp == bLen && aLen >= bLen) || (ap == aLen && bLen >= aLen);
        }


        // From: https://stackoverflow.com/questions/5796383/insert-spaces-between-words-on-a-camel-cased-token
        // Turns "IBMMakeStuffAndSellIt" into "IBM Make Stuff And Sell It"
        public static string SplitCamelCase (string str)
        {
            return System.Text.RegularExpressions.Regex.Replace (
                System.Text.RegularExpressions.Regex.Replace (
                    str,
                    @"(\P{Ll})(\P{Ll}\p{Ll})",
                    "$1 $2"
                ),
                @"(\p{Ll})(\P{Ll})",
                "$1 $2"
            );
        }


        // Converts arbitrary string to hashed int (repeatable cross-platform/process)
        // From: https://stackoverflow.com/questions/36845430/persistent-hashcode-for-strings
        public static int GetStableHashCode (this string str)
        {
            unchecked
            {
                int hash1 = 5381;
                int hash2 = hash1;

                for (int i = 0; i < str.Length && str [i] != '\0'; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ str [i];
                    if (i == str.Length - 1 || str [i + 1] == '\0')
                        break;
                    hash2 = ((hash2 << 5) + hash2) ^ str [i + 1];
                }

                return hash1 + (hash2 * 1566083941);
            }
        }


        //
        //
        public static string GetRandomString (System.Random random)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char [8];

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars [i] = chars [random.Next (chars.Length)];
            }

            var finalString = new string (stringChars);
            return finalString;
        }
    }
}
