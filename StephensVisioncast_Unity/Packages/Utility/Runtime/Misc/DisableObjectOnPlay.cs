using UnityEngine;

namespace Stephens.Utility
{
	public class DisableObjectOnPlay : MonoBehaviour {

		// Use this for initialization
		void Start () {
			gameObject.SetActive (false);
		}
	}
}
