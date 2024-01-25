using UnityEngine;

namespace Project.GameTests
{
    public class TEST_Visioncast : MonoBehaviour
    {
        #region VARIABLES

        [Header("References")] 
        [SerializeField] private GameObject _prefabTestVisionSource;
        [SerializeField] private Transform _visionSourceSpawnerParent;
        
        #endregion VARIABLES
        
        
        #region INITIALIZATION

        private void Awake()
        {
            for (int i = 0; i < _visionSourceSpawnerParent.childCount; i++)
            {
                if (!_visionSourceSpawnerParent.GetChild(i).gameObject.activeInHierarchy)
                    continue;
                
                Transform t = Instantiate(_prefabTestVisionSource, _visionSourceSpawnerParent.GetChild(i)).transform;
                t.localPosition = Vector3.zero;                
                t.localEulerAngles = Vector3.zero;                
            }
        }
        
        #endregion INITIALIZATION
    }
}

