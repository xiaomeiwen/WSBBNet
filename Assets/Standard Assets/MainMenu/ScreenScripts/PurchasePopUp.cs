using UnityEngine;
using System;
using System.Collections;

namespace WSBB {
	public enum PopUpReturn {StillOpen, Success, Failure}

	public class PurchasePopUp : MonoBehaviour {
		public bool divertToBlingPurchase;
		public bool divertToCredPurchase;

		public int PopUpCost;
		public int PopUpLevel;
		
		public String PopUpMessage;
		public String buyWord;
		public String getMoreBlingWord;
		public String getMoreCredWord;
		public String cancelWord;
		public String levelWord;
		public String notEnoughBlingMessage;
		public String notEnoughCredMessage;
		
		public Rect backgroundRect;
		public Rect imageRect;
		public Rect descRect;
		public Rect costRect;
		public Rect requiredLevel;
		public Rect buyRect;
		public Rect cancelRect;
		public Rect levelRect;
		public Rect levelLabel;
		public Rect progressBarRect;
		public Rect blingAmountRect;
		
		public Texture2D PopUpImage;
		public Texture2D progressBarFrame;
		public Texture2D progressBarImage;

		public BlingPopUp blingPopUp;
		public BlingPopUp credPopUp;

		public GUISkin popUpSkin;
		public GUISkin popUpSkinLarge;
		private GUISkin oldSkin;
		
		void Awake() {
			if(Screen.height != ScreenHelpers.idealScreenHeight) {
				popUpSkin = popUpSkinLarge;
				backgroundRect = ScreenHelpers.scaleRectToScreen(backgroundRect);
				imageRect = ScreenHelpers.scaleRectToScreen(imageRect);
				descRect = ScreenHelpers.scaleRectToScreen(descRect);
				costRect = ScreenHelpers.scaleRectToScreen(costRect);
				requiredLevel = ScreenHelpers.scaleRectToScreen(requiredLevel);
				buyRect = ScreenHelpers.scaleRectToScreen(buyRect);
				cancelRect = ScreenHelpers.scaleRectToScreen(cancelRect);
				levelRect = ScreenHelpers.scaleRectToScreen(levelRect);
				levelLabel = ScreenHelpers.scaleRectToScreen(levelLabel);
				progressBarRect = ScreenHelpers.scaleRectToScreen(progressBarRect);
				blingAmountRect = ScreenHelpers.scaleRectToScreen(blingAmountRect);
			}
			divertToBlingPurchase = false;
		}

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}
		
		public PopUpReturn DisplayPurchasePopUp() {
			if(divertToBlingPurchase) {
				if(blingPopUp.DisplayPopUp())
					divertToBlingPurchase = false;
				return PopUpReturn.StillOpen;
			} else if(divertToCredPurchase) {
				if(credPopUp.DisplayPopUp())
					divertToCredPurchase = false;
				return PopUpReturn.StillOpen;
			}
			
			oldSkin = GUI.skin;
			GUI.skin = popUpSkin;
			GUI.Box(backgroundRect, "");
			GUI.Box(imageRect, PopUpImage);
			if(PopUpLevel > GameData.instance.teamLevel)
				GUI.Box(descRect, PopUpMessage + notEnoughCredMessage);
			else if(PopUpCost > GameData.instance.teamMoney)
				GUI.Box(descRect, PopUpMessage + notEnoughBlingMessage);
			else
				GUI.Box(descRect, PopUpMessage);
			
			GUI.Box(costRect, "(&)" + PopUpCost);
			GUI.Box(blingAmountRect,"(&)" + GameData.instance.teamMoney);
			GUI.Box(levelRect, levelWord);
			GUI.Box(levelLabel, GameData.instance.teamLevel + "");
			if(PopUpLevel != 0)
				GUI.Box(requiredLevel, PopUpLevel + "");
			GUI.BeginGroup(progressBarRect);
			Rect tempRect = progressBarRect;
			tempRect.y = 0;
			tempRect.x = (float)-(tempRect.width * (100 - GameData.instance.teamExp) / 100.0f);
			GUI.Label(tempRect, progressBarImage);
			tempRect.x = 0;
			GUI.Label(tempRect, progressBarFrame);
			GUI.EndGroup();
			if(GameData.instance.teamLevel < PopUpLevel) {
				if(GUI.Button(buyRect, getMoreCredWord)) {
					divertToCredPurchase = true;
				}
				
			} else if(GameData.instance.teamMoney < PopUpCost) {
				if(GUI.Button(buyRect, getMoreBlingWord)) {
					divertToBlingPurchase = true;
				}
			} else {
				if(GUI.Button(buyRect, buyWord)) {
					GameData.instance.teamMoney -= PopUpCost;
					return PopUpReturn.Success;
				}
			}
			
			if(GUI.Button(cancelRect, cancelWord))
				return PopUpReturn.Failure;

			GUI.skin = oldSkin;
			return PopUpReturn.StillOpen;
		}
	}
}