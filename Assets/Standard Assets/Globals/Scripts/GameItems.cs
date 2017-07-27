using UnityEngine;
using System.Collections;

namespace SharkTankRessurection {
	[System.Obsolete]public enum ItemType {passive, activate, building}
	[System.Obsolete]public enum ItemEffectTypes {power, skill, speed, shot, pass, steal, block, ballHandling, hoopDrive, dunk, guard}
	[System.Obsolete]public enum ItemEvent {shotMade, shotFail, passMade, passFail, blockMade, blockFail, stealMade, stealFail}

	[System.Obsolete]public class GameItems : MonoBehaviour {
		public static GameItems instance;
		public Vector2 [] playerAssignedItems;
		public Vector2 [] computerAssignedItems;

		public Texture2D itemGlowTexture;
		public Texture2D useItemCover;

		public SharkTankRessurection.ItemSet [] itemSets;

		void Awake() {
			GameItems.instance = this;
			//GameObject.DontDestroyOnLoad(this.gameObject);
			foreach(SharkTankRessurection.ItemSet itemSet in itemSets) {
				foreach(SharkTankRessurection.Item item in itemSet.items) {
					item.inGame = false;
				}
			}
		}
		
		// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		void Update () {
			
		}

		public void saveItemUnlock() {
			for(int i = 0; i < itemSets.Length; i++)
				saveItemUnlockGroup(i);
		}
		
		public void saveItemUnlockGroup(int num) {
			for(int j = 0; j < itemSets[num].items.Length; j++) {
				PlayerPrefs.SetInt("ItemLock-" + num + "-" + j, ItemBoolToInt(itemSets[num].items[j].unlocked));
			}
		}
		
		public void saveItemConfig() {
			for(int k = 0; k < playerAssignedItems.Length; k++) {
				PlayerPrefs.SetInt("PlayerItem" + k + "X", (int) playerAssignedItems[k].x);
				PlayerPrefs.SetInt("PlayerItem" + k + "Y", (int) playerAssignedItems[k].y);
			}
		}
		
		public void loadItemUnlock() {
			for(int i = 0; i < itemSets.Length; i++) {
				for(int j = 0; j < itemSets[i].items.Length; j++) {
					itemSets[i].items[j].unlocked = ItemIntToBool(PlayerPrefs.GetInt("ItemLock-" + i + "-" + j));
				}
			}
		}
		
		public void loadItemConfig() {
			for(int k = 0; k < playerAssignedItems.Length; k++) {
				playerAssignedItems[k].x = PlayerPrefs.GetInt("PlayerItem" + k + "X");
				playerAssignedItems[k].y = PlayerPrefs.GetInt("PlayerItem" + k + "Y");
			}
		}
		
		private int ItemBoolToInt(bool value) {
			if(value)
				return 1;
			return 0;
		}
		
		private bool ItemIntToBool(int value) {
			if(value == 1)
				return true;
			return false;
		}
	}
}