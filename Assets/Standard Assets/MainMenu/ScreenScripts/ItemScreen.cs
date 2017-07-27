using UnityEngine;
using System;
using System.Collections;
using WSBB;

namespace SharkTankRessurection {
	[System.Serializable] public class ArrowButton {
		public Rect buttonRect;
		public GUIStyle buttonStyle;
		public GUIStyle buttonStyleLarge;
		public String buttonWord;
	}

	public class ItemScreen : MonoBehaviour {		
		public MenuScreen menuBase;
		public PurchasePopUp purchaseScreen;
		public bool unlockDisplay;
		public ArrowButton rightArrow;
		public ArrowButton leftArrow;
		public Vector2 ballerBarPos;

		public Rect sectionRect;
		public Rect playerRightArrow;
		public Rect playerLeftArrow;
		public Rect playerLabelRect;
		public Rect currentItemRect;
		public Rect currentItemName;
		public Rect currentItemDesc;

		public GUIStyle labelStyle;
		public GUIStyle labelStyleLarge;
		public GUIStyle itemButtonStyle;
		public GUIStyle selectedButtonStyle;
		
		
		public WSBB.BBaller_skin ballerSkin;
		
		public Rect itemBackgrounds;
		public Rect itemRect;
		public Vector2 itemSpacing;
		public int itemsInRow;
		public int maxRows;
		public int rowOffset;
		public int storedLength;
				
		public int currentSetIndex;
		public int currentBallerIndex;
		private int storedIndex;
		
		void Awake() {
			if(Screen.height != ScreenHelpers.idealScreenHeight) {
				rightArrow.buttonRect = ScreenHelpers.scaleRectToScreen(rightArrow.buttonRect);
				leftArrow.buttonRect = ScreenHelpers.scaleRectToScreen(leftArrow.buttonRect);
				rightArrow.buttonStyle = rightArrow.buttonStyleLarge;
				leftArrow.buttonStyle = leftArrow.buttonStyleLarge;
				labelStyle = labelStyleLarge;
				sectionRect = ScreenHelpers.scaleRectToScreen(sectionRect);
				playerRightArrow = ScreenHelpers.scaleRectToScreen(playerRightArrow);
				playerLeftArrow = ScreenHelpers.scaleRectToScreen(playerLeftArrow);
				playerLabelRect = ScreenHelpers.scaleRectToScreen(playerLabelRect);
				ballerBarPos = ScreenHelpers.scaleVector2ToScreen(ballerBarPos);
				itemBackgrounds = ScreenHelpers.scaleRectToScreen(itemBackgrounds);
				itemRect = ScreenHelpers.scaleRectToScreen(itemRect);
				itemSpacing = ScreenHelpers.scaleVector2ToScreen(itemSpacing);
				currentItemRect = ScreenHelpers.scaleRectToScreen(currentItemRect);
				currentItemName = ScreenHelpers.scaleRectToScreen(currentItemName);
				currentItemDesc = ScreenHelpers.scaleRectToScreen(currentItemDesc);
				
			}
			unlockDisplay = false;
			currentBallerIndex = 0;
			ballerSkin.dataIndex = currentBallerIndex;
			ballerSkin.refreshUVPos();
		}

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}
		
		public void ScreenUpdate() {
			menuBase.ScreenUpdate();	
		}
		
		public void ScreenOnGUI() {
			if(unlockDisplay) {
				switch(purchaseScreen.DisplayPurchasePopUp()) {
					case PopUpReturn.Success:
						unlockDisplay = false;
						UnlockAndEquipItem();
						break;
					case PopUpReturn.Failure:
						unlockDisplay = false;
						break;
					default:
						break;
				}
				return;
			}
			menuBase.ScreenOnGUI();
			DrawSetandBallerArrows();
			GUI.Box(itemBackgrounds, "");
			GUI.Box(itemBackgrounds, "");
			for(int i = 0; i < GameItems.instance.itemSets[currentSetIndex].items.Length; i++) {
				int storedValue = DisplayItemIcon(i, GameItems.instance.itemSets[currentSetIndex].items[i].itemStoreImage,
				                                  GameItems.instance.itemSets[currentSetIndex].items[i].unlocked,
				                                  GameItems.instance.itemSets[currentSetIndex].items[i].unlockMessage,
				                                  GameItems.instance.itemSets[currentSetIndex].items[i].unlockCost,
				                                  GameItems.instance.itemSets[currentSetIndex].items[i].levelNeeded);
				if(storedValue != -1) {
					GameItems.instance.playerAssignedItems[currentBallerIndex].x = currentSetIndex;
					GameItems.instance.playerAssignedItems[currentBallerIndex].y = i;
					GameData.save(SaveTarget.ItemConfig);
				}						
			}
			
			PlayerBars.DrawStatBar(ballerBarPos, GameBallers.instance.ballers[GameData.instance.teamBallers[currentBallerIndex]]);
			
			Vector2 currentPlayerItem = GameItems.instance.playerAssignedItems[currentBallerIndex];
			GUI.Label(currentItemRect, GameItems.instance.itemSets[(int) currentPlayerItem.x].items[(int) currentPlayerItem.y].itemStoreImage, selectedButtonStyle);
			GUI.Box(currentItemDesc, GameItems.instance.itemSets[(int) currentPlayerItem.x].items[(int) currentPlayerItem.y].itemDesc);
			GUI.Box(currentItemName, GameItems.instance.itemSets[(int) currentPlayerItem.x].items[(int) currentPlayerItem.y].itemName);
		}
		
		private void DrawSetandBallerArrows() 	{
			if(GUI.Button(rightArrow.buttonRect, rightArrow.buttonWord, rightArrow.buttonStyle)) {
				currentSetIndex++;
				if(currentSetIndex >= GameItems.instance.itemSets.Length)
					currentSetIndex = 0;
			}
			if(GUI.Button(leftArrow.buttonRect, leftArrow.buttonWord, leftArrow.buttonStyle)) {
				currentSetIndex--;
				if(currentSetIndex < 0)
					currentSetIndex = GameItems.instance.itemSets.Length - 1;
			}
			if(GUI.Button(playerRightArrow, "", rightArrow.buttonStyle)) {
				currentBallerIndex++;
				if(currentBallerIndex > 5)
					currentBallerIndex = 0;
				ballerSkin.dataIndex = currentBallerIndex;
				ballerSkin.fetchAndRefresh();
			}
			if(GUI.Button(playerLeftArrow, "", leftArrow.buttonStyle)) {
				currentBallerIndex--;
				if(currentBallerIndex < 0)
					currentBallerIndex = 5;
				ballerSkin.dataIndex = currentBallerIndex;
				ballerSkin.fetchAndRefresh();
			}
			GUI.Label(sectionRect, GameItems.instance.itemSets[currentSetIndex].name, labelStyle);
			GUI.Label(playerLabelRect, GameBallers.instance.ballers[GameData.instance.teamBallers[currentBallerIndex]].ballerName, labelStyle);
		}
		
		private void UnlockAndEquipItem() {
			GameItems.instance.itemSets[currentSetIndex].items[storedIndex].unlocked = true;
			GameItems.instance.playerAssignedItems[currentBallerIndex].x = currentSetIndex;
			GameItems.instance.playerAssignedItems[currentBallerIndex].y = storedIndex;
			GameData.save(SaveTarget.ItemConfig);
			GameData.save(SaveTarget.ItemUnlock);
			PlayerPrefs.Save();
		}
		
		private int DisplayItemIcon(int i, Texture2D itemImage, bool unlocked, String unlockMess, int unlockCost, int unlockLevel) {
			int valueToReturn = -1;
			int columnPos = i % itemsInRow;
			int rowPos = ((i - columnPos) / itemsInRow) - rowOffset;
			if(rowPos >= maxRows || rowPos < 0)
				return valueToReturn;
			Rect tempRect = itemRect;
			tempRect = itemRect;
			tempRect.x += itemSpacing.x * columnPos;
			tempRect.y += itemSpacing.y * rowPos;
			if(GameItems.instance.playerAssignedItems[currentBallerIndex].x == currentSetIndex 
			   && GameItems.instance.playerAssignedItems[currentBallerIndex].y == i) {
				GUI.Label(tempRect, itemImage, selectedButtonStyle);
			} else {
				if(unlocked) {
					if(GUI.Button(tempRect, itemImage, itemButtonStyle)) {
						valueToReturn = i;
					}
				} else {
					if(GUI.Button(tempRect, GameUniforms.instance.lockedIcon,itemButtonStyle)) {
						purchaseScreen.PopUpImage = itemImage;
						purchaseScreen.PopUpMessage = unlockMess;
						purchaseScreen.PopUpCost = unlockCost;
						purchaseScreen.PopUpLevel = unlockLevel;
						storedIndex = i;
						unlockDisplay = true;
					}
				}
			}
			return valueToReturn;
		}
	}
}