using System.Collections.Generic;
using UnityEngine;

public class TrackMover : MonoBehaviour
{
    #region VARIABLES

    [Header("References")]
    [SerializeField] private List<Transform> _track;
    
    [Header("Values")]
    [SerializeField] private int _startIndex = 1;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _timePerTarget;

    private Transform _currentTarget;
    private Transform _previousTarget;
    private float _currentTime;
    
    #endregion VARIABLES


    #region INITIALIZATION

    private void Awake()
    {
        _previousTarget = _track[_startIndex - 1];
        if (_startIndex - 1 < 0)
            _previousTarget = _track[^1];
        _currentTarget = _track[_startIndex];

        transform.position = _previousTarget.position;
    }

    #endregion INITIALIZATION


    #region UPDATE

    private void Update()
    {
        _currentTime += Time.deltaTime;
        if (_currentTime > _timePerTarget)
        {
            SetNextTarget();
            return;
        }

        float norm = _currentTime / _timePerTarget;
        transform.position = Vector3.Lerp(_previousTarget.position, _currentTarget.position, norm);
        
        if (_rotationSpeed == 0)
            return;
        
        Vector3 relativePos = _currentTarget.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * _rotationSpeed);
    }

    private void SetNextTarget()
    {
        int i = _track.IndexOf(_currentTarget) + 1;
        if (i >= _track.Count)
            i = 0;

        _previousTarget = _currentTarget;
        _currentTarget = _track[i];
        _currentTime = 0;
    }
    
    #endregion UPDATE
}
