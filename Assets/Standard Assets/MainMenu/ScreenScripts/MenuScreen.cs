using UnityEngine;
using System;
using System.Collections;

namespace WSBB {
	[System.Serializable] public class MenuButton{
		public Rect buttonArea;
		public String disText;
		public MenuStates targetState;
		public Transform [] cameraPath;
		public bool lockBtn;
		public bool worldTourMode;
	}

	//!!! This class initially extended MonoBehaviour. But removed it as it needs to be Serializable
	[System.Serializable] public class BackButton{
		public Rect backButtonArea;
		public Transform [] backCameraPath;
		public MenuStates backState ;
		public String backButtonWord;
		public GUISkin backButtonSkin;
		public GUISkin backButtonSkinLarge;
		
		public void Awake() {
			if(Screen.height != ScreenHelpers.idealScreenHeight) {
				backButtonArea = ScreenHelpers.scaleRectToScreen(backButtonArea);
				backButtonSkin = backButtonSkinLarge;
			}
		}
		
		public void ScreenUpdate () {
			
		}
		
		public void ScreenOnGUI() {
			GUISkin tempSkin = GUI.skin;
			GUI.skin = backButtonSkin;
			if(GUI.Button(backButtonArea, backButtonWord)) {
				PlayerPrefs.Save();
				MainMenuState.instance.nextState = backState;
				MainMenuState.instance.currentState = MenuStates.Transition;
				MainMenuState.instance.cameraPath = backCameraPath;
			}
			GUI.skin = tempSkin;
		}
	}

	public class MenuScreen : MonoBehaviour {
		
		public MenuButton [] options;
		public BackButton backButton;
		
		public GUISkin mySkin;
		public GUISkin mySkin_Large;
		
		public void Awake() {
			backButton.Awake();
			if(Screen.height != ScreenHelpers.idealScreenHeight) {
				foreach(MenuButton option in options) {
					option.buttonArea = ScreenHelpers.scaleRectToScreen(option.buttonArea);
				}
				mySkin = mySkin_Large;
			}
		}

		// Use this for initialization
		public void Start () {
		
		}
		
		// Update is called once per frame
		public void Update () {
		
		}

		public void ScreenUpdate() {
			
		}
		
		public void ScreenOnGUI() {
			GUI.skin = mySkin;
			foreach(MenuButton menuOption in options) {
				if(menuOption.lockBtn) {
					GUI.Box(menuOption.buttonArea, menuOption.disText);
				} else if(GUI.Button(menuOption.buttonArea, menuOption.disText)) {
					MainMenuState.instance.nextState = menuOption.targetState;
					MainMenuState.instance.currentState = MenuStates.Transition;
					MainMenuState.instance.cameraPath = menuOption.cameraPath;
					MainMenuState.instance.worldTourMode = menuOption.worldTourMode;
				}
			}
			backButton.ScreenOnGUI();
		}
	}
}