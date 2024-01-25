using UnityEngine;

// FROM:
// https://www.icode.com/c-function-for-a-bezier-curve/
namespace Stephens.Utility
{
    public static class BezierCurve
    {
        public static Vector2 GetPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            float cx = 3 * (p1.x - p0.x);
            float cy = 3 * (p1.y - p0.y);
            float bx = 3 * (p2.x - p1.x) - cx;
            float by = 3 * (p2.y - p1.y) - cy;
            float ax = p3.x - p0.x - cx - bx;
            float ay = p3.y - p0.y - cy - by;
            float Cube = t * t * t;
            float Square = t * t;

            float resX = (ax * Cube) + (bx * Square) + (cx * t) + p0.x;
            float resY = (ay * Cube) + (by * Square) + (cy * t) + p0.y;

            return new Vector2(resX, resY);
        }
    }
}