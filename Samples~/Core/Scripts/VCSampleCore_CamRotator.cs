using Unity.Cinemachine;
using UnityEngine;

namespace VisioncastSamples.Core
{
    public class VCSampleCore_CamRotator : MonoBehaviour
    {
        [SerializeField] private CinemachineOrbitalFollow _cmOrbital;
        [SerializeField] private float _speed;
    
        // Update is called once per frame
        private void Update()
        {
            _cmOrbital.HorizontalAxis.Value += Time.deltaTime * _speed;
        }
    }
}