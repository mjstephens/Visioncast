using System;
using System.Collections.Generic;
using System.Linq;
using Habrador_Computational_Geometry;
using UnityEngine;

namespace GalaxyGourd.Visioncast
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class VisionCone : MonoBehaviour
    {
        #region VARIABLES

        [Range(1, 60)]
        [SerializeField] private int _meshDetail = 10;

        private MeshRenderer _renderer;
        private MeshFilter _filter;
        private float _cacheRange;
        private float _cacheFoV;
        private int _cacheDetail;
        private Mesh _mesh;

        #endregion VARIABLES


        #region INITIALIZATION

        private void Awake()
        {
            _filter = GetComponent<MeshFilter>();
            _renderer = GetComponent<MeshRenderer>();
        }
        
        public void Toggle(bool on)
        {
            _renderer.enabled = on;
        }

        #endregion INITIALIZATION
        

        #region CONE

        public void CalculateCone(VisioncastSource source)
        {
            // Only continue if our data has changed
            if (Math.Abs(_cacheRange - source.Range) < Mathf.Epsilon && 
                Math.Abs(_cacheFoV - source.FieldOfView) < Mathf.Epsilon &&
                _cacheDetail == _meshDetail)
                return;
            
            // Compile points for our cone
            List<Vector3> points = CompileConePoints(source);
            
            // Generate mesh
            if (points.Count > 3)
            {
                DrawMesh(points);
            }
            
            // Set caches
            _cacheRange = source.Range;
            _cacheFoV = source.FieldOfView;
            _cacheDetail = _meshDetail;
        }

        private List<Vector3> CompileConePoints(VisioncastSource source)
        {
            // Add edge points we know we want
            Vector3 leftDirection = Quaternion.AngleAxis(-source.FieldOfView, source.transform.up) * source.Heading;
            Vector3 rightDirection = Quaternion.AngleAxis(source.FieldOfView, source.transform.up) * source.Heading;
            Vector3 upDirection = Quaternion.AngleAxis(-source.FieldOfView, source.transform.right) * source.Heading;
            Vector3 downDirection = Quaternion.AngleAxis(source.FieldOfView, source.transform.right) * source.Heading;
            List<Vector3> points = new()
            {
                source.transform.InverseTransformPoint(source.Position),
                source.transform.InverseTransformPoint(source.Position + (source.Heading * source.Range)),
                source.transform.InverseTransformPoint(source.Position + (leftDirection * source.Range)),
                source.transform.InverseTransformPoint(source.Position + (rightDirection * source.Range)),
                source.transform.InverseTransformPoint(source.Position + (upDirection * source.Range)),
                source.transform.InverseTransformPoint(source.Position + (downDirection * source.Range))
            };

            float angle = 360.0f / _meshDetail * Mathf.PI / 180.0f;
            for (int i = 0; i < _meshDetail; i++)
            {
                points.Add(GetPointOnUnitSphereCap(
                    Quaternion.LookRotation(source.transform.InverseTransformDirection(source.Heading)), 
                    source.FieldOfView,
                    angle * i) * source.Range);

                points.Add(GetPointOnUnitSphereCap(
                    Quaternion.LookRotation(source.transform.InverseTransformDirection(source.Heading)),
                    source.FieldOfView / 3,
                    angle * i) * source.Range);
                
                points.Add(GetPointOnUnitSphereCap(
                    Quaternion.LookRotation(source.transform.InverseTransformDirection(source.Heading)),
                    (source.FieldOfView / 3) *2,
                    angle * i) * source.Range);
            }

            return points;
        }
        
        /// <summary>
        /// https://github.com/Habrador/Computational-geometry
        /// </summary>
        private void DrawMesh(IEnumerable<Vector3> raw)
        {
            HashSet<MyVector3> points = new HashSet<MyVector3>(raw.Select(x => x.ToMyVector3()));
            Normalizer3 normalizer = new Normalizer3(new List<MyVector3>(points));
            HashSet<MyVector3> points_normalized = normalizer.Normalize(points);
            HalfEdgeData3 convexHull_normalized = _ConvexHull.Iterative_3D(points_normalized, false, normalizer);

            if (convexHull_normalized != null)
            {
                HalfEdgeData3 convexHull = normalizer.UnNormalize(convexHull_normalized);
                MyMesh myMesh = convexHull.ConvertToMyMesh("visioncone_mesh", MyMesh.MeshStyle.HardEdges);
                _mesh = myMesh.ConvertToUnityMesh(generateNormals: false, myMesh.meshName);
                _filter.mesh = _mesh;
            }
        }
        
        /// <summary>
        /// from: https://discussions.unity.com/t/random-onunitsphere-but-within-a-defined-range/19066
        /// </summary>
        private static Vector3 GetPointOnUnitSphereCap(Quaternion targetDirection, float angle, float increment)
        {
            float angleInRad = angle * Mathf.Deg2Rad;
            float x = Mathf.Cos(increment);
            float y = Mathf.Sin(increment);
            Vector2 PointOnCircle = new Vector2(x, y) * Mathf.Sin(angleInRad);
            Vector3 V = new Vector3(PointOnCircle.x,PointOnCircle.y,Mathf.Cos(angleInRad));
            return targetDirection * V;
        }
        
        #endregion CONE
    }
}