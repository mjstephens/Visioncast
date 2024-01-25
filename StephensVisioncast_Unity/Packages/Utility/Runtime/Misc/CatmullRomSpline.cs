using System.Collections.Generic;
using UnityEngine;


// FROM:
// https://github.com/SneakyBrian/Catmull-Rom-Sample/blob/master/Catmull–Rom-Sample/CatmullRomSpline.cs
namespace Stephens.Utility
{
	public static class CatmullRomSpline
	{
		/// <summary>
		/// Generate spline series of points from array of keyframe points
		/// </summary>
		/// <param name="points">array of key frame points</param>
		/// <param name="numPoints">number of points to generate in spline between each point</param>
		/// <returns>array of points describing spline</returns>
		public static Vector2 [] Generate (List <Vector2> points, int numPoints)
		{      
			var splinePoints = new List <Vector2> ();
        
			for (int i = 0; i < points.Count - 3; i++)
			{
				for (int j = 0; j < numPoints; j++)
				{
					splinePoints.Add (PointOnCurve (points [i], points [i + 1], points [i + 2], points [i + 3], (1f / numPoints) * j));
				}
			}

			splinePoints.Add (points [points.Count - 2]);

			return splinePoints.ToArray ();
		}

		/// <summary>
		/// Calculates interpolated point between two points using Catmull-Rom Spline
		/// </summary>
		/// <remarks>
		/// Points calculated exist on the spline between points two and three.
		/// </remarks>
		/// <param name="p0">First Point</param>
		/// <param name="p1">Second Point</param>
		/// <param name="p2">Third Point</param>
		/// <param name="p3">Fourth Point</param>
		/// <param name="t">
		/// Normalised distance between second and third point 
		/// where the spline point will be calculated
		/// </param>
		/// <returns>
		/// Calculated Spline Point
		/// </returns>
		public static Vector2 PointOnCurve (Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
		{
			Vector2 ret = new Vector2 ();

			float t2 = t * t;
			float t3 = t2 * t;

			ret.x = 0.5f * ((2.0f * p1.x) +
			                (-p0.x + p2.x) * t +
			                (2.0f * p0.x - 5.0f * p1.x + 4 * p2.x - p3.x) * t2 +
			                (-p0.x + 3.0f * p1.x - 3.0f * p2.x + p3.x) * t3);
        
			ret.y = 0.5f * ((2.0f * p1.y) +
			                (-p0.y + p2.y) * t +
			                (2.0f * p0.y - 5.0f * p1.y + 4 * p2.y - p3.y) * t2 +
			                (-p0.y + 3.0f * p1.y - 3.0f * p2.y + p3.y) * t3);

			return ret;
		}

    

		public static Vector2 [] CatmulRom (List <Vector2> points, int denisty, float alpha)
		{
			//
			List <Vector2> splinePoints = new List <Vector2> ();

			for (int i = 0; i < points.Count - 3; i++)
			{
				Vector2 p0 = new Vector2 (points [i].x, points [i].y);
				Vector2 p1 = new Vector2 (points [i + 1].x, points [i + 1].y);
				Vector2 p2 = new Vector2 (points [i + 2].x, points [i + 2].y);
				Vector2 p3 = new Vector2 (points [i + 3].x, points [i + 3].y);

				float t0 = 0.0f;
				float t1 = GetT (t0, p0, p1, alpha);
				float t2 = GetT (t1, p1, p2, alpha);
				float t3 = GetT (t2, p2, p3, alpha);

				for (float t = t1; t < t2; t += ((t2 - t1) / denisty))
				{
					Vector2 A1 = (t1 - t) / (t1 - t0) * p0 + (t - t0) / (t1 - t0) * p1;
					Vector2 A2 = (t2 - t) / (t2 - t1) * p1 + (t - t1) / (t2 - t1) * p2;
					Vector2 A3 = (t3 - t) / (t3 - t2) * p2 + (t - t2) / (t3 - t2) * p3;

					Vector2 B1 = (t2 - t) / (t2 - t0) * A1 + (t - t0) / (t2 - t0) * A2;
					Vector2 B2 = (t3 - t) / (t3 - t1) * A2 + (t - t1) / (t3 - t1) * A3;

					Vector2 C = (t2 - t) / (t2 - t1) * B1 + (t - t1) / (t2 - t1) * B2;

					splinePoints.Add (C);
				}
			}

			//
			return splinePoints.ToArray ();
		}


		public static float GetT (float t, Vector2 p0, Vector2 p1, float alpha)
		{
			float a = Mathf.Pow ((p1.x - p0.x), 2.0f) + Mathf.Pow ((p1.y - p0.y), 2.0f);
			float b = Mathf.Pow (a, 0.5f);
			float c = Mathf.Pow (b, alpha);

			return (c + t);
		}
	}
}
