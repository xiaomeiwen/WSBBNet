using UnityEngine;
using System.Collections;

namespace WSBB {
	public class TeamCheck : MonoBehaviour {
		public bool checkPlayers;
		public bool checkStarters;
		public bool forceLock;
		
		public int targetOption;

		public MenuScreen targetMenu;

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
			if(GameData.instance == null)
				return;
			bool buttonLock = false;
			int i;
			if(checkPlayers) {
				for(i = 0; i < GameData.instance.teamBallers.Length; i++) {
					if(GameData.instance.teamBallers[i] == -1)
						buttonLock = true;
				}
			}
			if(checkStarters) {
				for(i = 0; i < GameData.instance.teamStarters.Length; i++) {
					if(GameData.instance.teamStarters[i] == -1)
						buttonLock = true;
				}
			}
			targetMenu.options[targetOption].lockBtn = (buttonLock || forceLock);		
		}
	}
}