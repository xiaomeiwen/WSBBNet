using UnityEngine;
using System.Collections;

namespace SharkTankRessurection {
	public class Disappear : MonoBehaviour {
		void Awake() {
			gameObject.GetComponent<Renderer>().enabled = false;
		}

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}
	}
}