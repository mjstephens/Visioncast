// Called with this event:
// Events.CAM_SHAKE.FireEvent (duration, intensity, dempeningFactor);


using System.Collections;
using UnityEngine;


// Shake the camera after an explosion
namespace Stephens.Utility
{
	public class CameraShake : MonoBehaviour 
	{
		#region Variables

		//public GameController gameController;
	
		// Transform of the camera to shake
		private Transform _camTransform;
		// The original position of the camera
		private Vector3 originalPos;
		// How long the object should shake for.
		private float _shake = 0f;
		// Amplitude of the shake. A larger value shakes the camera harder.
		private float _shakeAmount = 0.2f;
		// How quickly the shake decreases
		private float _decreaseFactor = 1.0f;
		// If the camera should be shaking
		private bool _isShaking = false;
	
		#endregion
	
	
		#region Update
	
		// Main logic loop
		// Called automatically by Unity every frame
		WaitForFixedUpdate tt = new WaitForFixedUpdate ();
		IEnumerator ShakeTick ()
		{
			_isShaking = true;
			Vector3 savedLocal = _camTransform.localPosition;
			while (_shake > 0)
			{
				//
				Vector3 targPos = _camTransform.localPosition + ((Vector3) Random.insideUnitCircle * _shakeAmount);
				_camTransform.localPosition = targPos;
				_shake -= Time.fixedDeltaTime * _decreaseFactor;

				// Next frame
				yield return tt;
			}

			// Done shaking
			_isShaking = false;

			// if (gameController.currentState != GameController.GameState.Gameplay)
			// 	_camTransform.localPosition = savedLocal;
			_shake = 0f;
		}
	
		#endregion
	
	
		#region Start Shaking
	
		//
		// Called from CAMERA_SHAKE event
		public void OnCameraShake (float shakeTime, float intensity, float dampeningFactor)
		{
			if (_isShaking)
				return;
			
			_shake = shakeTime;
			_shakeAmount = intensity;
			_decreaseFactor = dampeningFactor;
			StartCoroutine ("ShakeTick");
		}


		//
		//
		public void SustainCamShake (float intensity)
		{
			_shake = Mathf.Max (_shake, 0.1f);
			_shakeAmount = Mathf.Lerp (_shakeAmount, intensity, 2f);

			if (!_isShaking)
				StartCoroutine ("ShakeTick");
		}


		//
		//
		public void CancelCamShake ()
		{
			_shake = 0;
			//_shakeAmount = 0;
			_isShaking = false;
			StopCoroutine ("ShakeTick");
		}
	
		#endregion
	
	
		#region Event Subscription
	
		// Subscribe to events
		void OnEnable ()
		{
			//Events.CAMERA_SHAKE.Event += OnCameraShake;
		}
	
	
		// Unsubscribe from events
		void OnDisable ()
		{
			//Events.CAMERA_SHAKE.Event -= OnCameraShake;
		}
	
		#endregion
	
	
		#region Initialization
	
		// Used for initialization
		// Called automatically by Unity
		void Awake ()
		{
			// Assign the camera transform
			_camTransform = transform;
		}
	
		#endregion
	}
}
