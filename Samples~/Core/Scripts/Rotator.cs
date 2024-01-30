using UnityEngine;

namespace VisioncastSamples.Core
{
    public class Rotator : MonoBehaviour
    {
        [SerializeField] private float _speed;
        

        // Update is called once per frame
        void Update()
        {
            transform.Rotate(Vector3.up, Time.deltaTime * _speed);
        }
    }
}

