using UnityEngine;

namespace Stephens.Utility.Core
{
    public static class TransformUtility
    {
        // Sets the layer of an object and can changes the layer of the children.
        public static void SetLayer (Transform parentObject, int layerIndex, bool includeChildren)
        {
            parentObject.gameObject.layer = layerIndex;
            if (includeChildren)
            {
                Transform [] _transformChildren = parentObject.GetComponentsInChildren <Transform> ();         
                for (int i = 0; i < _transformChildren.Length; i++)            
                    _transformChildren [i].gameObject.layer = layerIndex;
            }      
        }


        // Deletes all children under parentObject.
        public static void DeleteChildren (Transform parentObject)
        {      
            foreach (Transform child in parentObject)
            {
                Object.Destroy (child.gameObject);
            }
        }


        // Traverses all parents of childObject until it detects one with a specific script.
        // Example: _trapData = (TrapData)TransformUtility.FindComponentInParent (gameObject, typeof(TrapData));
        public static Component FindComponentInParent (GameObject childObject, System.Type _componentType)
        {
            Transform t = childObject.transform;
            while (t.parent != null)
            {
                if (t.parent.GetComponent (_componentType))
                    return t.parent.GetComponent (_componentType);
                t = t.parent.transform;
            }      
            return null;      
        }


        // Finds child with name in all hierarchy children
        public static Transform FindDeepChild (this Transform aParent, string aName)
        {
            var result = aParent.Find (aName);
            if (result != null)
                return result;
            foreach (Transform child in aParent)
            {
                result = child.FindDeepChild (aName);
                if (result != null)
                    return result;
            }
            return null;
        }
        
        public static Rect GetWorldRect(this RectTransform rectTransform)
        {
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            // Get the bottom left corner.
            Vector3 position = corners[0];
         
            Vector2 size = new Vector2(
                rectTransform.lossyScale.x * rectTransform.rect.size.x,
                rectTransform.lossyScale.y * rectTransform.rect.size.y);
 
            return new Rect(position, size);
        }
        
        public static Rect RectTransformToScreenSpace(this RectTransform transform)
        {
            Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
            return new Rect((Vector2)transform.position - (size * 0.5f), size);
        }
        
        public static Rect GetScreenPositionFromRect(this RectTransform rt, UnityEngine.Camera camera)
        {
            // getting the world corners
            var corners = new Vector3[4];
            rt.GetWorldCorners(corners);
             
            // getting the screen corners
            for (var i = 0; i < corners.Length; i++)
                corners[i] = camera.WorldToScreenPoint(corners[i]);
             
            // getting the top left position of the transform
            var position = (Vector2) corners[1];
            // inverting the y axis values, making the top left corner = 0.
            position.y = Screen.height - position.y;
            // calculate the siz, width and height, in pixle format
            var size = corners[2] - corners[0];
             
            return new Rect(position, size);
        }
    }
}
