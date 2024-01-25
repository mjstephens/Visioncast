using UnityEngine;

namespace Stephens.Utility.Core
{
    public static class ColorUtility 
    {
        public static Color GetSimplifiedColor (Color c)
        {
            float simpR = (float) (System.Math.Truncate ((double) c.r * 100.0) / 100.0);
            float simpG = (float) (System.Math.Truncate ((double) c.g * 100.0) / 100.0);
            float simpB = (float) (System.Math.Truncate ((double) c.b * 100.0) / 100.0);
            float simpA = (float) (System.Math.Truncate ((double) c.a * 100.0) / 100.0);
            return new Color (simpR, simpG, simpB, simpA);
        }


        public static Color GetColorFromString (string s)
        {
            string ss = s.Split ('(') [1];
            ss = ss.Split (')') [0];
            string [] li = ss.Split (',');

            float sr = float.Parse (li [0].Trim ());
            float sg = float.Parse (li [1].Trim ());
            float sb = float.Parse (li [2].Trim ());
            float sa = float.Parse (li [3].Trim ());
            return new Color (sr, sg, sb, sa);
        }


        public static string ColorToHex (Color32 color)
        {
            string hex = color.r.ToString ("X2") + color.g.ToString ("X2") + color.b.ToString ("X2");
            return hex;
        }


        public static Color HexToColor (string hex)
        {
            byte r = byte.Parse (hex.Substring (0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse (hex.Substring (2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse (hex.Substring (4, 2), System.Globalization.NumberStyles.HexNumber);
            return new Color32 (r, g, b, 255);
        }


        public static Color CombineColors (params Color [] aColors)
        {
            Color result = new Color (0, 0, 0, 0);
            foreach (Color c in aColors)
            {
                result += c;
            }
            result /= aColors.Length;
            return result;
        }


        // Returns a random bright color
        public static Color GetRandomColorFromSeed (System.Random random)
        {
            return new Color ((float) random.NextDouble (), (float) random.NextDouble (), (float) random.NextDouble ());
        }


        // Returns a random bright color
        public static Color GetRandomBrightColor ()
        {
            Color col = Random.ColorHSV (0, 1, 1, 1, 1, 1);
            return col;
        }


        // Returns a random pastel color using light HSV saturation
        public static Color GetRandomPastelColor ()
        {
            Color col = Random.ColorHSV (0, 1, 0.15f, 0.15f, 1, 1);
            return col;
        }


        // Returns a random dark color using light HSV value
        public static Color GetRandomDarkColor ()
        {
            Color col = Random.ColorHSV (0, 1, 1, 1, 0.15f, 0.15f);
            return col;
        }
    }
}
