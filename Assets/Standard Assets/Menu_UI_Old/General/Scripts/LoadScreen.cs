using UnityEngine;
using System;
using System.Collections;

namespace WSBB {
	public class LoadScreen : MonoBehaviour {
		public float loadDelay;
		public String nextScreen;
		public Texture2D background;
		public Rect backgroundArea;
		public GUISkin guiSkin;
		public GUISkin guiSkinLarge;
		
		void Awake() {
			Time.timeScale = 1;
			loadDelay = 0.5f;
			if(Screen.height != ScreenHelpers.idealScreenHeight) {
				backgroundArea = ScreenHelpers.scaleRectToScreen(backgroundArea);
				guiSkin = guiSkinLarge;
			}
			PlayerPrefs.Save();
		}

		// Use this for initialization
		void Start () {
			//loadDelay -= Time.deltaTime;
			//if(loadDelay < 0.0f) {
				Debug.Log("Loading-:" + GameData.loadScreenTarget);
				Application.LoadLevel(GameData.loadScreenTarget);
				if(GameData.loadCourt) {
					GameData.loadCourt = false;
				Application.LoadLevelAdditive(GameData.loadCourtTarget);
				}
			//}
		}
		
		// Update is called once per frame
		void Update () {
		
		}
		
		void OnGUI () {
			GUI.skin = guiSkin;
			GUI.Label(backgroundArea, "---LOADING---");
		}
	}
}