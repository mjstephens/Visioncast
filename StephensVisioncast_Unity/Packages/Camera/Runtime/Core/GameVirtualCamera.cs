using Unity.Cinemachine;
using UnityEngine;

namespace Stephens.Camera
{
    [RequireComponent(typeof(CinemachineCamera))]
    public abstract class GameVirtualCamera : MonoBehaviour
    {
        #region VARIABLES

        [Header("References")]
        [SerializeField] protected CinemachineCamera _vCam;
        
        public CinemachineCamera VCam => _vCam;

        #endregion VARIABLES
    }
}