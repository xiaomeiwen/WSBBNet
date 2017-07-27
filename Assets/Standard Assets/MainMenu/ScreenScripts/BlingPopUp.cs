using UnityEngine;
using System;
using System.Collections;

namespace WSBB {
	[System.Serializable] public class BlingPurchase {
		public int blingAmount;
		public float cost;
	}

	public class BlingPopUp : MonoBehaviour {		
		public bool credMode;
		public BlingPurchase[] blingPurchases;
		public int itemsPerRow;
		public Rect backgroundRect;
		public Rect popUpLabel;
		public String popUpWord;
		public Rect blingDisplayLabel;
		public String blingDisplayWord;
		public Rect blingDisplayValue;
		public Rect progressBarRect;
		public Texture2D progressBarImage;
		public Texture2D progressBarFrame;
		public Rect buyRect;
		public String buyWord;
		public String forWord;
		public Vector2 offset;
		public GUISkin mainSkin;
		public GUISkin mainSkinLarge;
		private GUISkin lastSkin;
		private bool makingPurchase;
		private int storedValue;
		
		void Awake() {
			if(Screen.height != ScreenHelpers.idealScreenHeight) {
				mainSkin = mainSkinLarge;
				backgroundRect = ScreenHelpers.scaleRectToScreen(backgroundRect);
				popUpLabel = ScreenHelpers.scaleRectToScreen(popUpLabel);
				blingDisplayLabel = ScreenHelpers.scaleRectToScreen(blingDisplayLabel);
				blingDisplayValue = ScreenHelpers.scaleRectToScreen(blingDisplayValue);
				progressBarRect = ScreenHelpers.scaleRectToScreen(progressBarRect);
				buyRect = ScreenHelpers.scaleRectToScreen(buyRect);
				offset = ScreenHelpers.scaleVector2ToScreen(offset);
			}
			makingPurchase = false;
		}

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		public bool DisplayPopUp() {
			Rect tempRect;
			if(makingPurchase) {
				makingPurchase = false;
				return true;
			}
			lastSkin = GUI.skin;
			GUI.skin = mainSkin;
			GUI.Box(backgroundRect, "");
			GUI.Box(backgroundRect, "");
			GUI.Box(popUpLabel, popUpWord);
			GUI.Box(blingDisplayLabel, blingDisplayWord);
			if(credMode) {
				GUI.Box(blingDisplayValue, GameData.instance.teamExp + "/" + 100);
				GUI.BeginGroup(progressBarRect);
				tempRect = progressBarRect;
				tempRect.y = 0;
				tempRect.x = -(tempRect.width * (100.0f - GameData.instance.teamExp) / 100.0f);
				GUI.Label(tempRect, progressBarImage);
				tempRect.x = 0;
				GUI.Label(tempRect, progressBarFrame);
				GUI.EndGroup();
			} else
				GUI.Box(blingDisplayValue, "(&)" + GameData.instance.teamMoney);
			
			for(int i = 0; i < blingPurchases.Length; i++) {
				tempRect = buyRect;
				tempRect.x += offset.x * (i % itemsPerRow);
				tempRect.y += offset.y * ((i - (i % itemsPerRow)) / itemsPerRow);
				if(GUI.Button(tempRect, buyWord + blingPurchases[i].blingAmount + forWord + " $" + blingPurchases[i].cost)) {
					if(credMode) {
						storedValue = blingPurchases[i].blingAmount;
						BuyCred();
						makingPurchase = true;
					} else {
						storedValue =  blingPurchases[i].blingAmount;
						BuyBling();
						makingPurchase = true;
					}
				}
			}
			GUI.skin = lastSkin;
			return false;
			
		}
		
		public bool BuyBling() {
			GameData.instance.teamMoney += storedValue;
			GameData.save(SaveTarget.BlingAmount);
			PlayerPrefs.Save();
			return true;
		}
		
		public bool BuyCred() {
			GameData.instance.teamExp += storedValue;
			GameData.instance.runLevelCheck();
			return true;
		}
	}
}