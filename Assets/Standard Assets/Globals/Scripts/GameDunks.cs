using UnityEngine;
using System.Collections;

namespace WSBB {
	public class GameDunks : MonoBehaviour {
		public static GameDunks instance;
		public DunkItem [] dunks;
		public Texture2D lockedDunk;
		
		void Awake () {
			GameDunks.instance = this;
		}
		
		// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		void Update () {
			
		}

		public void saveDunkUnlock() {
			for(int i = 0; i < dunks.Length; i++) {
				PlayerPrefs.SetInt("TeamDunkUnlock" + i, BoolToInt(dunks[i].unlocked));
			}
		}
		
		public void loadDunkUnlock() {
			for(int i = 0; i < dunks.Length; i++) {
				dunks[i].unlocked = IntToBool(PlayerPrefs.GetInt("TeamDunkUnlock" + 1));
			}
		}
		
		private bool IntToBool(int value) {
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