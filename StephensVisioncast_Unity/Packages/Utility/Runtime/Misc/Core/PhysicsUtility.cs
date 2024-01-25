using UnityEngine;

namespace Stephens.Utility.Core
{
    public static class PhysicsUtility
    {
        public static Vector3 GetRandomPointInsideCollider (this Collider collider)
        {
            Vector3 extents = collider.bounds.extents;
            Vector3 point = new Vector3(
                Random.Range( -extents.x, extents.x ),
                Random.Range( -extents.y, extents.y ),
                Random.Range( -extents.z, extents.z )
            );
 
            return collider.transform.TransformPoint(point);
        }
    }
}