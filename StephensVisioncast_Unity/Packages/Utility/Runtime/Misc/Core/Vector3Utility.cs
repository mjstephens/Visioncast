using UnityEngine;

namespace Stephens.Utility
{
    public static class Vector3Utility
    {
        // TO USE:
        // transform.position = transform.position.Change( y: 1.3f );
        //public static Vector3 Change (this Vector3 org, object x = null, object y = null, object z = null)
        //{
        //    return new Vector3 ((x == null ? org.x, (float) x), (y == null ? org.y, (float) y), (z == null ? org.z, (float) z));
        //}

        public static Vector3 [] ToVector3Array (this Vector2 [] v2)
        {
            return System.Array.ConvertAll<Vector2, Vector3> (v2, GetV2fromV3);
        }


        public static Vector3 GetV2fromV3 (Vector2 v2)
        {
            return new Vector3 (v2.x, v2.y, 0);
        }


        public static Vector3 ClosestPointOnLine (Vector3 vA, Vector3 vB, Vector3 vPoint)
        {
            Vector3 vVector1 = vPoint - vA;
            Vector3 vVector2 = (vB - vA).normalized;

            float d = Vector3.Distance (vA, vB);
            float t = Vector3.Dot (vVector2, vVector1);

            if (t <= 0)
                return vA;

            if (t >= d)
                return vB;

            Vector3 vVector3 = vVector2 * t;

            Vector3 vClosestPoint = vA + vVector3;

            return vClosestPoint;
        }
        
        /// <summary>
        /// Gets the magnitude on an axis given a <see cref="Vector3"/>.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <param name="axis">The axis on which to calculate the magnitude.</param>
        /// <returns>The magnitude.</returns>
        public static float GetMagnitudeOnAxis(this Vector3 vector, Vector3 axis)
        {
            var vectorMagnitude = vector.magnitude;
            if (vectorMagnitude <= 0)
            {
                return 0;
            }
            var dot = Vector3.Dot(axis, vector / vectorMagnitude);
            var val = dot * vectorMagnitude;
            return val;
        }
		
        /// <summary>
        /// Get the square magnitude from vectorA to vectorB.
        /// </summary>
        /// <returns>The sqr magnitude.</returns>
        /// <param name="vectorA">First vector.</param>
        /// <param name="vectorB">Second vector.</param>
        public static float SqrMagnitudeFrom(this Vector3 vectorA, Vector3 vectorB)
        {
            var diff = vectorA - vectorB;
            return diff.sqrMagnitude;
        }
    }
}
