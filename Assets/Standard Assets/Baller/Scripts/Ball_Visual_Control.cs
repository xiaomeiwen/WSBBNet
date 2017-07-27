using UnityEngine;
using System.Collections;

namespace WSBB {
	public class Ball_Visual_Control : MonoBehaviour {
		[HideInInspector]public Renderer ballRenderer;
		private BBaller myBaller;
		[HideInInspector]public bool EnableDebugMsgs = true;

		void Awake() {
			myBaller = GetComponent<BBaller>();	

			//ballRenderer = this.transform.Find("Basketballer/Ball_Mesh").GetComponent<Renderer>();
		}

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
//			if (ballRenderer != null)
//				ballRenderer.enabled = myBaller.HasBall;
//			else if(EnableDebugMsgs)
//				Debug.LogError("Ball_Visual_Control.cs: ballRenderer has not been attached to the respective Ball_Mesh");

		}
	}
}