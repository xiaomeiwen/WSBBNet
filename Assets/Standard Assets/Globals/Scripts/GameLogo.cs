using UnityEngine;
using System.Collections;

namespace WSBB {
	public class GameLogo : MonoBehaviour {
		public static GameLogo instance;
		public LogoItem [] logos;
		public Texture2D lockedLogo;
		
		void Awake () {
			GameLogo.instance = this;
		}

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		public void saveLogoUnlocks() {
			for(int i = 0; i < logos.Length; i++) {
				PlayerPrefs.SetInt("TeamLogoUnlock" + i, BoolToInt(logos[i].unlocked));
			}
		}
		
		public void loadLogoUnlocks() {
			for(int i = 0; i < logos.Length; i++) {
				logos[i].unlocked = IntToBool(PlayerPrefs.GetInt("TeamLogoUnlock" + 1));
			}
		}
		
		private bool IntToBool(int value)  {
			if(value == 0)
				return false;			
			return true;
		}
		
		private int BoolToInt(bool value) {
			if(value)
				return 1;			
			return 0;
		}
	}
}