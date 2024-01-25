/// <summary>
/// Simple game object rotator.
/// Copyright 2015 www.crosstales.com
/// Created by Stefan Laubenberger
/// </summary>

using UnityEngine;

namespace Stephens.Utility
{
	public class Rotator : MonoBehaviour {
		//Inspector variables
		public Vector3 Speed;
		public bool RandomSpeed = false;
		public bool Active = true;

		private Transform tf;
		private Vector3 speed;

		void Start() {
			tf = transform;

			if (RandomSpeed) {
				speed.x = Random.Range(0, Speed.x);
				speed.y = Random.Range(0, Speed.y);
				speed.z = Random.Range(0, Speed.z);
			} else {
				speed = Speed;
			}
		}

		void OnEnable(){

			tf = transform;
			
			if (RandomSpeed) {
				speed.x = Random.Range(0, Speed.x);
				speed.y = Random.Range(0, Speed.y);
				speed.z = Random.Range(0, Speed.z);
			} else {
				speed = Speed;
			}

		}

		void Update() {
			if (Active) {
				speed = Speed;
				tf.Rotate (speed * Time.deltaTime);
			}
		}
	}
}
