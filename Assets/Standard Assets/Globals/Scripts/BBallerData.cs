using UnityEngine;
using System;
using System.Collections;

namespace WSBB {
	public enum SkinTone {Asian, Black, Hispanic, White}



	[System.Serializable] public class BBallerData {
		public String ballerName;
		public int arrayIndex;

		public SkinTone skinColor;
		public int headSkin;
		public Texture2D mugshot;

		public float baseSpeed;
		public float baseSkill;
		public float basePower;

		public BBaller_skin myBaller;
		public bool assigned;

		public SharkTankRessurection.Item item;
		public String dunk;

		public int unlockCost;
		public bool unlocked;


		public void ItemUpdate() {
			item.ItemUpdate();
		}
		
		public float getPower() {
			return basePower + item.GetEffect(SharkTankRessurection.ItemEffectTypes.power);
		}
		
		public float getSkill() {
			return baseSkill + item.GetEffect(SharkTankRessurection.ItemEffectTypes.skill);
		}
		
		public float getSpeed() {
			return baseSpeed + item.GetEffect(SharkTankRessurection.ItemEffectTypes.speed);
		}
		
		public float getBlock() {
			return getDerived(getPower(), getSkill()) + item.GetEffect(SharkTankRessurection.ItemEffectTypes.block);
		}
		
		public float getShoot() {
			return getDerived(getSkill(), getSpeed()) + item.GetEffect(SharkTankRessurection.ItemEffectTypes.shot);
		}
		
		public float getHoopDrive() {
			return getDerived(getPower(), getSkill()) + item.GetEffect(SharkTankRessurection.ItemEffectTypes.hoopDrive);
		}
		
		public float getDunk() {
			return getDerived(getPower(), getSkill()) + item.GetEffect(SharkTankRessurection.ItemEffectTypes.dunk);
		}
		
		public float getHandling() {
			return getDerived(getSkill(), getPower()) + item.GetEffect(SharkTankRessurection.ItemEffectTypes.ballHandling);
		}
		
		public float getSteal() {
			return getDerived(getSpeed(), getPower()) + item.GetEffect(SharkTankRessurection.ItemEffectTypes.steal);
		}
		
		public float getPass() {
			return getDerived(getSkill(), getSpeed()) + item.GetEffect(SharkTankRessurection.ItemEffectTypes.pass);
		}
		
		public float getGuard() {
			return getDerived(getPower(), getSkill()) + item.GetEffect(SharkTankRessurection.ItemEffectTypes.guard);
		}
		
		private float getDerived(float base1, float base2) {
			return ((base1 + (base2 / 2)) / 2);
		}
		
		public void fetchItem(Vector2 itemToFetch, bool inGame) {
			item = SharkTankRessurection.GameItems.instance.itemSets[(int) itemToFetch.x].items[(int) itemToFetch.y].CloneItemDataForGame(inGame);
		}
		
		public BBallerData cloneBallerData() {
			BBallerData newData = new BBallerData();
			newData.arrayIndex = arrayIndex;
			newData.skinColor = skinColor;
			newData.headSkin = headSkin;
			newData.baseSpeed = baseSpeed;
			newData.baseSkill = baseSkill;
			newData.basePower = basePower;
			newData.ballerName = ballerName;
			newData.mugshot = mugshot;
			newData.assigned = false;
			dunk = GameAnims.ClassicDunk;
			return newData;
		}
	}
}