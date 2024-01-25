using UnityEngine;

namespace Stephens.Tick
{
    /// <summary>
    /// Acts as the single source of update callbacks for the tick router
    /// </summary>
    internal class TickSource : MonoBehaviour
    {
        #region SINGLETON

        private static TickSource _instance;
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;            
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            TickRouter.FlushQueued();
        }

        #endregion SINGLETON
        
        
        #region VARIABLES

        private float _delta;
        private float _fixedDelta;

        #endregion VARIABLES
        
        
        #region UPDATE

        private void Update()
        {
            _delta = Time.deltaTime;
            TickRouter.TickUpdate(_delta);
        }

        private void FixedUpdate()
        {
            _fixedDelta = Time.fixedDeltaTime;
            TickRouter.TickFixedUpdate(_fixedDelta);
        }

        private void LateUpdate()
        {
            TickRouter.TickLateUpdate(_delta);
        }

        #endregion UPDATE
    }
}