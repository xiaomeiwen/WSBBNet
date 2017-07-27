using UnityEngine;
using System.Collections;

namespace WSBB {
	public class TeamLogoSelect : MonoBehaviour {
		public Rect tempRect;

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}
		
		void Awake() {
			if(Screen.height != ScreenHelpers.idealScreenHeight) {
				tempRect = ScreenHelpers.scaleRectToScreen(tempRect);
			}
		}
		
		public void ScreenUpdate () {
			
		}
		
		public void ScreenOnGUI() {
			GUI.skin = null;
			GUI.Box(tempRect, "Team Logo Selection Goes Here");
		}
	}
}