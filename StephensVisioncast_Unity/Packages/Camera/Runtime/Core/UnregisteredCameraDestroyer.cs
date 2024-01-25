using UnityEngine;

namespace Stephens.Camera
{
    public class UnregisteredCameraDestroyer : MonoBehaviour
    {
        #region INITIALIZATION

        private void Awake()
        {
            UnityEngine.Camera[] cams = FindObjectsOfType<UnityEngine.Camera>(true);
            for (int i = cams.Length - 1; i >= 0; i--)
            {
                if (cams[i].GetComponent<GameCamera>() == null && cams[i].GetComponentInParent<GameCamera>() == null)
                {
                    Destroy(cams[i].gameObject);
                }
            }
            
            Destroy(gameObject);
        }

        #endregion INITIALIZATION
    }
}