using UnityEngine;
using System;
using System.Collections;

namespace SharkTankRessurection {
	[System.Obsolete][System.Serializable] public class ItemSet {
		public String name;
		public Item [] items;
	}

	[System.Obsolete][System.Serializable] public class Item {
		public String itemName;
		public String itemDesc;
		public String unlockMessage;

		public bool activate_inUse;
		public bool activate_available;
		public bool unlocked;
		public bool inGame;

		public int building_increment;
		public int building_maxBoost;
		public int building_boost;
		public int unlockCost;
		public int levelNeeded;
		public int effectValue;
		
		public float activate_effectTime;
		public float activate_rechargeTime;
		public float activate_currentTime;

		public Texture2D itemStoreImage;
		public Texture2D itemUseImage;

		public ItemType itemType;
		public ItemEffectTypes itemEffect;
		public ItemEvent building_success;
		public ItemEvent building_failure;

		
		public Item CloneItemDataForGame(bool inGameValue) {
			Item newItem = new Item();
			newItem.itemName = itemName;
			newItem.itemUseImage = itemUseImage;
			newItem.itemStoreImage = itemStoreImage;
			newItem.inGame = inGameValue;
			newItem.itemType = itemType;
			newItem.itemEffect = itemEffect;
			newItem.effectValue = effectValue;
			newItem.activate_effectTime = activate_effectTime;
			newItem.activate_rechargeTime = activate_rechargeTime;
			newItem.activate_inUse = activate_inUse;
			newItem.activate_available = true;
			newItem.building_success = building_success;
			newItem.building_failure = building_failure;
			newItem.building_increment = building_increment;
			newItem.building_maxBoost = building_maxBoost;
			newItem.building_boost = building_boost;
			return newItem;
		}
		
		public int GetEffect(ItemEffectTypes desiredItemEffect) {
			if(desiredItemEffect != itemEffect)
				return 0;

			int finalEffectValue = 0;

			switch(itemType) {
				case ItemType.activate:
					if(activate_inUse || !inGame)
						finalEffectValue = effectValue;
					else
						finalEffectValue = 0;
					break;
				case ItemType.building:
					finalEffectValue = effectValue + building_boost;
					break;
				default:
					finalEffectValue = effectValue;
					break;
			}

			return finalEffectValue;
		}
		
		public void ItemUpdate() {
			switch(itemType) {
				case ItemType.activate:
					if(activate_inUse) {
						activate_currentTime -= Time.deltaTime;
						if(activate_currentTime <= 0) {
							activate_currentTime = activate_rechargeTime;
							activate_inUse = false;
						}
					}
					break;
				default:
					break;
			}
			if(activate_currentTime > 0) {
				activate_currentTime -= Time.deltaTime;
			}
			if(itemType == ItemType.activate && !activate_inUse && !activate_available) {
				if(activate_currentTime <= 0)
					activate_available = true;
			}
		}
		
		public void TakeEvent(ItemEvent itemEvent) {
			if(itemEvent == building_failure) {
				building_boost = 0;
			}
			if(itemEvent == building_success) {
				building_boost += building_increment;
				if(building_boost > building_maxBoost) {
					building_boost = building_maxBoost;
				}
			}
		}
		
		public void Activate() {
			Debug.Log("Item Activated");
			switch(itemType) {
				case ItemType.activate:
					if(activate_available) {
						activate_inUse = true;
						activate_available = false;
						activate_currentTime = activate_effectTime;
					}
					break;
				default:
					break;
			}
		}
	}
}