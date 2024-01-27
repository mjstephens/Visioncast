using UnityEngine;
using UnityEngine.UI;

namespace Stephens.Player
{
    public class UIVisibleObjectMarkerInstance : MonoBehaviour
    {
        #region VARIABLES

        [Header("References")]
        [SerializeField] private Image _image;
        [SerializeField] private Image _grabFill;
        
        internal RectTransform RectTransform { get; private set; }

        #endregion VARIABLES


        #region INITIALIZATION

        private void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
        }

        #endregion INITIALIZATION


        #region UI

        public void SetAsKey(bool b)
        {
            _image.color = b ? Color.yellow : Color.white;
            if (b)
            {
                transform.SetAsLastSibling();
            }
            
            if (!b && _grabFill)
            {
                _grabFill.fillAmount = 0;
            }
        }

        public void SetInteractionDurationProgress(float progress)
        {
            _grabFill.fillAmount = progress;
        }

        #endregion UI
    }
}