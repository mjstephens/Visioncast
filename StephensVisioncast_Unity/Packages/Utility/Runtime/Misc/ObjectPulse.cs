/*============================================================
 	ObjectPulse.cs
	SMILE Inc. 2
 	
 	SUPER88 			http://www.super88.tv
 	
 	Created:      		May 23, 2017
 	Last Edited:		May 24, 2017
 	
 	Pulses the object.
============================================================*/


using UnityEngine;

namespace Stephens.Utility
{
	public class ObjectPulse : MonoBehaviour 
	{
		#region Variables

		#region Public 

		[Header ("Pulse")]
		public float pulseSizeX = 1.5f;
		public float pulseSpeed = 7;

		#endregion 

		#region Private 

		private Vector3 _defaultSize;
		private Vector3 _pulseSize;

		#endregion 

		#endregion 


		#region Pulse 

		// Begins a pulse
		public void Pulse ()
		{
			transform.localScale = _pulseSize;
		}


		// Main logic loop
		// Called automatically
		void Update ()
		{
			transform.localScale = Vector3.Lerp (transform.localScale, _defaultSize, Time.deltaTime * pulseSpeed);
		}

		#endregion 


		#region Initialization 

		// Initializaes
		// Called automatically
		void Start ()
		{
			_defaultSize = transform.localScale;
			_pulseSize = new Vector3 (pulseSizeX, pulseSizeX, pulseSizeX);
		}

		#endregion 
	}
}
