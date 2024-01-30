using UnityEngine;

namespace GalaxyGourd.Visioncast
{
    public static class VisioncastUtility
    {
        /// <summary>
        /// Returns the center points for each face of a given collider bounds
        /// </summary>
        /// <param name="col"></param>
        /// <param name="offset"></param>
        /// <param name="facePoints"></param>
        /// <returns></returns>
        internal static Vector3[] GetColliderBoundsFaces(Collider col, float offset, Vector3[] facePoints)
        {
            Bounds bounds = col.bounds;
            Vector3 extents = bounds.extents;
            Vector3 center = bounds.center;
            
            facePoints[0] = center + (Vector3.up * (extents.y - offset));
            facePoints[1] = center - (Vector3.up * (extents.y - offset));
            facePoints[2] = center + (Vector3.right * (extents.x - offset));
            facePoints[3] = center - (Vector3.right * (extents.x - offset));
            facePoints[4] = center + (Vector3.forward * (extents.z - offset));
            facePoints[5] = center - (Vector3.forward * (extents.x - offset));

            return facePoints;
        }

        internal static Vector3[] GetBoxColliderExtentsFaces(BoxCollider col, float offset, Vector3[] facePoints)
        {
            Transform colTrans = col.transform;
            Vector3 center = col.center + colTrans.position;
            Vector3 size = col.size;
            Vector3 lossyScale = colTrans.lossyScale;
            Vector3 extents = new Vector3(size.x * lossyScale.x, size.y * lossyScale.y, size.z * lossyScale.z) / 2;
            
            Vector3 up = colTrans.up;
            facePoints[0] = center + (up * (extents.y - offset));
            facePoints[1] = center - (up * (extents.y - offset));
            
            Vector3 right = colTrans.right;
            facePoints[2] = center + (right * (extents.x - offset));
            facePoints[3] = center - (right * (extents.x - offset));
            
            Vector3 forward = colTrans.forward;
            facePoints[4] = center + (forward * (extents.z - offset));
            facePoints[5] = center - (forward * (extents.z - offset));

            return facePoints;
        }

        internal static Vector3[] GetBoxColliderExtentsCorners(BoxCollider col, float offset, Vector3[] cornerPoints)
        {
            Transform colTrans = col.transform;
            Vector3 center = col.center + colTrans.position;
            Vector3 size = col.size;
            Vector3 lossyScale = colTrans.lossyScale;
            Vector3 extents = new Vector3(size.x * lossyScale.x, size.y * lossyScale.y, size.z * lossyScale.z) / 2;
            
            Vector3 up = colTrans.up;
            Vector3 right = colTrans.right;
            Vector3 forward = colTrans.forward;

            cornerPoints[0] = center + (up * (extents.y - offset));
            cornerPoints[1] = center - (up * (extents.y - offset));
            
            cornerPoints[2] = center + (right * (extents.x - offset));
            cornerPoints[3] = center - (right * (extents.x - offset));
            
            cornerPoints[4] = center + (forward * (extents.z - offset));
            cornerPoints[5] = center - (forward * (extents.z - offset));

            return cornerPoints;
        }
    }
}