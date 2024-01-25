using System.Collections;
using UnityEngine;

namespace Stephens.Utility
{
    public class SineScale : MonoBehaviour
    {
        #region Variables 

        #region Public 

        [Header ("Values")]
        public float scaleStrength;
        public float scaleSpeed;

        #endregion

        #region Private 

        private Vector3 _originalScale;

        #endregion

        #endregion


        #region Scale

        //
        IEnumerator ScaleTick ()
        {
            while (true)
            {
                float newScale = _originalScale.x + (Mathf.Sin (Time.time * scaleSpeed) * scaleStrength);
                transform.localScale = new Vector3 (newScale, newScale, newScale);
                yield return null;
            }
        }

        #endregion


        #region Enable 

        void OnEnable ()
        {
            _originalScale = transform.localScale;
            StartCoroutine ("ScaleTick");
        }


        void OnDisable ()
        {
            transform.localScale = _originalScale;
            StopCoroutine ("ScaleTick");
        }

        #endregion 
    }
}
