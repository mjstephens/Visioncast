using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Stephens.Utility
{
    public static class UIUtility 
    {
        // Checks if an input is on a UI object
        public static bool IsPointerOverUIObject ()
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData (EventSystem.current);
            eventDataCurrentPosition.position = new Vector2 (UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y);
            List <RaycastResult> results = new List <RaycastResult> ();
            EventSystem.current.RaycastAll (eventDataCurrentPosition, results);
            return results.Count > 0;
        }


        public static bool IsPositionOverUIObject (Vector2 position)
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData (EventSystem.current);
            eventDataCurrentPosition.position = position;
            List <RaycastResult> results = new List<RaycastResult> ();
            EventSystem.current.RaycastAll (eventDataCurrentPosition, results);
            return results.Count > 0;
        }


        /// <summary>
        /// Calulates Position for RectTransform.position from a transform.position. Does not Work with WorldSpace Canvas!
        /// </summary>
        /// <param name="_Canvas"> The Canvas parent of the RectTransform.</param>
        /// <param name="_Position">Position of in world space of the "Transform" you want the "RectTransform" to be.</param>
        /// <param name="_Cam">The Camera which is used. Note this is useful for split screen and both RenderModes of the Canvas.</param>
        /// <returns></returns>
        public static Vector3 CalculatePositionFromTransformToRectTransform (Canvas _Canvas, Vector3 _Position, UnityEngine.Camera _Cam)
        {
            Vector3 Return = Vector3.zero;
            if (_Canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                Return = _Cam.WorldToScreenPoint (_Position);
            }
            else if (_Canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle (
                    _Canvas.transform as RectTransform, 
                    _Cam.WorldToScreenPoint (_Position), 
                    _Cam, 
                    out var tempVector);
                Return = _Canvas.transform.TransformPoint (tempVector);
            }

            return Return;
        }


        /// <summary>
        /// Calulates Position for RectTransform.position Mouse Position. Does not Work with WorldSpace Canvas!
        /// </summary>
        /// <param name="_Canvas">The Canvas parent of the RectTransform.</param>
        /// <param name="_Cam">The Camera which is used. Note this is useful for split screen and both RenderModes of the Canvas.</param>
        /// <returns></returns>
        public static Vector3 CalculatePositionFromMouseToRectTransform (Canvas _Canvas, UnityEngine.Camera _Cam)
        {
            Vector3 Return = Vector3.zero;

            if (_Canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                Return = UnityEngine.Input.mousePosition;
            }
            else if (_Canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                Vector2 tempVector = Vector2.zero;
                RectTransformUtility.ScreenPointToLocalPointInRectangle (
                    _Canvas.transform as RectTransform,
                    UnityEngine.Input.mousePosition,
                    _Cam, out tempVector);
                Return = _Canvas.transform.TransformPoint (tempVector);
            }

            return Return;
        }


        /// <summary>
        /// Calculates Position for "Transform".position from a "RectTransform".position. Does not Work with WorldSpace Canvas!
        /// </summary>
        /// <param name="_Canvas">The Canvas parent of the RectTransform.</param>
        /// <param name="_Position">Position of the "RectTransform" UI element you want the "Transform" object to be placed to.</param>
        /// <param name="_Cam">The Camera which is used. Note this is useful for split screen and both RenderModes of the Canvas.</param>
        /// <returns></returns>
        public static Vector3 CalculatePositionFromRectTransformToTransform (Canvas _Canvas, Vector3 _Position, UnityEngine.Camera _Cam)
        {
            Vector3 Return = Vector3.zero;
            if (_Canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                Return = _Cam.ScreenToWorldPoint (_Position);
            }
            else if (_Canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                RectTransformUtility.ScreenPointToWorldPointInRectangle (_Canvas.transform as RectTransform, _Cam.WorldToScreenPoint (_Position), _Cam, out Return);
            }
            return Return;
        }
        
        public static Vector3 GetRectOverlapOutside(
            Vector3[] rectCache1,
            Vector3[] rectCache2,
            RectTransform container, 
            RectTransform target, 
            UnityEngine.Camera cam, 
            float graceArea = 0)
        {
            rectCache1 = GetScreenCorners(rectCache1, container, cam);
            rectCache2 = GetScreenCorners(rectCache2, target, cam);
            // container.GetWorldCorners(rectCache1);
            // target.GetWorldCorners(rectCache2);
            
            //
            // for (int i = 0; i < rectCache1.Length; i++)
            // {
            //     Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(cam, rectCache1[i]);
            //     rectCache1[i] = screenPos;
            // }
            // for (int i = 0; i < rectCache2.Length; i++)
            // {
            //     Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(cam, rectCache2[i]);
            //     rectCache2[i] = screenPos;
            // }

            // Calculate X overlap
            float xOverlap = 0;
            if (rectCache2[0].x + graceArea < rectCache1[0].x) xOverlap = (rectCache1[0].x - rectCache2[0].x);
            else if (rectCache2[3].x - graceArea > rectCache1[3].x) xOverlap = (rectCache1[3].x - rectCache2[3].x);
            
            // Calculate Y overlap
            float yOverlap = 0;
            if (rectCache2[0].y + graceArea < rectCache1[0].y) yOverlap = (rectCache1[0].y - rectCache2[0].y);
            else if (rectCache2[1].y - graceArea > rectCache1[1].y) yOverlap = (rectCache1[1].y - rectCache2[1].y);

            return new Vector3(xOverlap, yOverlap);
        }

        public static Vector3[] GetScreenCorners(Vector3[] corners, RectTransform target, UnityEngine.Camera cam)
        {
            target.GetWorldCorners(corners);
            for (int i = 0; i < corners.Length; i++)
            {
                Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(cam, corners[i]);
                corners[i] = screenPos;
            }

            return corners;
        }
        
        public static void FitRectInsideParentRect(
            this RectTransform rect,
            Vector3[] rectCache1,
            Vector3[] rectCache2,
            RectTransform container, 
            UnityEngine.Camera cam, 
            float graceArea = 0)
        {
            // Gets overlap in SCREEN SPACE
            Vector3 overlapOutside = GetRectOverlapOutside(rectCache1, rectCache2, container, rect, cam);
            Vector3 ssPosition = cam.WorldToScreenPoint(rect.position) + overlapOutside;
            rect.position = cam.ScreenToWorldPoint(ssPosition);
        }
        
        // From: https://stackoverflow.com/questions/65417634/draw-bounding-rectangle-screen-space-around-a-game-object-with-a-renderer-wor
        public static Rect RendererBoundsInScreenSpace(Renderer r, UnityEngine.Camera cam, out Vector3[] screenSpaceCorners) 
        {
            // This is the space occupied by the object's visuals
            // in WORLD space.
            Bounds bigBounds = r.bounds;
            screenSpaceCorners = new Vector3[8];

            // For each of the 8 corners of our renderer's world space bounding box,
            // convert those corners into screen space.
            screenSpaceCorners[0] = cam.WorldToScreenPoint( new Vector3( bigBounds.center.x + bigBounds.extents.x, bigBounds.center.y + bigBounds.extents.y, bigBounds.center.z + bigBounds.extents.z ) );
            screenSpaceCorners[1] = cam.WorldToScreenPoint( new Vector3( bigBounds.center.x + bigBounds.extents.x, bigBounds.center.y + bigBounds.extents.y, bigBounds.center.z - bigBounds.extents.z ) );
            screenSpaceCorners[2] = cam.WorldToScreenPoint( new Vector3( bigBounds.center.x + bigBounds.extents.x, bigBounds.center.y - bigBounds.extents.y, bigBounds.center.z + bigBounds.extents.z ) );
            screenSpaceCorners[3] = cam.WorldToScreenPoint( new Vector3( bigBounds.center.x + bigBounds.extents.x, bigBounds.center.y - bigBounds.extents.y, bigBounds.center.z - bigBounds.extents.z ) );
            screenSpaceCorners[4] = cam.WorldToScreenPoint( new Vector3( bigBounds.center.x - bigBounds.extents.x, bigBounds.center.y + bigBounds.extents.y, bigBounds.center.z + bigBounds.extents.z ) );
            screenSpaceCorners[5] = cam.WorldToScreenPoint( new Vector3( bigBounds.center.x - bigBounds.extents.x, bigBounds.center.y + bigBounds.extents.y, bigBounds.center.z - bigBounds.extents.z ) );
            screenSpaceCorners[6] = cam.WorldToScreenPoint( new Vector3( bigBounds.center.x - bigBounds.extents.x, bigBounds.center.y - bigBounds.extents.y, bigBounds.center.z + bigBounds.extents.z ) );
            screenSpaceCorners[7] = cam.WorldToScreenPoint( new Vector3( bigBounds.center.x - bigBounds.extents.x, bigBounds.center.y - bigBounds.extents.y, bigBounds.center.z - bigBounds.extents.z ) );

            // Now find the min/max X & Y of these screen space corners.
            float min_x = screenSpaceCorners[0].x;
            float min_y = screenSpaceCorners[0].y;
            float max_x = screenSpaceCorners[0].x;
            float max_y = screenSpaceCorners[0].y;

            for (int i = 1; i < 8; i++) {
                if(screenSpaceCorners[i].x < min_x) {
                    min_x = screenSpaceCorners[i].x;
                }
                if(screenSpaceCorners[i].y < min_y) {
                    min_y = screenSpaceCorners[i].y;
                }
                if(screenSpaceCorners[i].x > max_x) {
                    max_x = screenSpaceCorners[i].x;
                }
                if(screenSpaceCorners[i].y > max_y) {
                    max_y = screenSpaceCorners[i].y;
                }
            }

            return Rect.MinMaxRect( min_x, min_y, max_x, max_y );

        }

        public static Rect PointsBoundsInScreenSpace(Vector3[] points, UnityEngine.Camera cam)
        {
            Vector3[] ssPoints = new Vector3[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                ssPoints[i] = cam.WorldToScreenPoint(points[i]);
            }
            
            // Find the min/max X & Y of these screen space points
            float min_x = ssPoints[0].x;
            float min_y = ssPoints[0].y;
            float max_x = ssPoints[0].x;
            float max_y = ssPoints[0].y;
            foreach (Vector3 point in ssPoints)
            {
                if (point.x < min_x)
                {
                    min_x = point.x;
                }
                if (point.x > max_x)
                {
                    max_x = point.x;
                }
                if (point.y < min_y)
                {
                    min_y = point.y;
                }
                if (point.y > max_y)
                {
                    max_y = point.y;
                }
            }
            
            return Rect.MinMaxRect( min_x, min_y, max_x, max_y );
        }
    }
}
