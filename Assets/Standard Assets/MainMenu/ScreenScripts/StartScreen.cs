using UnityEngine;
using System.Collections;

namespace WSBB {
	public class StartScreen : MonoBehaviour {
		public MenuStates nextState;
		public Transform [] cameraPath;

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}
		
		public void ScreenUpdate () {
			if(Input.touches.Length > 0 || Input.anyKeyDown) {
				MainMenuState.instance.nextState = nextState;
				MainMenuState.instance.currentState = MenuStates.Transition;
				MainMenuState.instance.cameraPath = cameraPath;
			} else {
				MainMenuState.instance.cameraTransform.position = cameraPath[0].position;
				MainMenuState.instance.cameraTransform.rotation = cameraPath[0].rotation;
			}
		}
		
		public void ScreenOnGUI() {
			
		}
	}
}