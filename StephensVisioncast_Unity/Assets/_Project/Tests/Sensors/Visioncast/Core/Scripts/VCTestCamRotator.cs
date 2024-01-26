using Unity.Cinemachine;
using UnityEngine;

namespace VisioncastTests.Core
{
    public class VCTestCamRotator : MonoBehaviour
    {
        [SerializeField] private CinemachineOrbitalFollow _cmOrbital;
        [SerializeField] private float _speed;
    
        // Update is called once per frame
        void Update()
        {
            _cmOrbital.HorizontalAxis.Value += Time.deltaTime * _speed;
        }
    }
}