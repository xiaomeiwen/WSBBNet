using UnityEngine;
using System.Collections;

namespace WSBB {
	public class ItemDisplay : MonoBehaviour {
		public GUISkin guiSkin;
		public GUISkin boostSkin;
		public Team playerTeam;
		public Rect itemArea;
		public GUIText bonusText;
		public Color valueColor;
		public Color rechargeColor;
		public Color countdownColor;
		public static float ItemFlashTime = 1;
		
		void Awake() {
			if(Screen.height != ScreenHelpers.idealScreenHeight) {
				itemArea = ScreenHelpers.scaleRectToScreen(itemArea);
			}
		}

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
			foreach(Touch touch in Input.touches) {
				if(touch.phase == TouchPhase.Began) {
					Touch tempTouch = touch;
					if(ScreenHelpers.checkTouch(tempTouch.position, itemArea)) {
						playerTeam.bballers[playerTeam.playerBaller].SkinAndData.ballerData.item.Activate();
					}
				}
			}
		}
		
		public void OnGUI () {
			if(GameAdmin.GamePause)
				return;

			if(SharkTankRessurection.GameItems.instance) {
				int intNum;
				SharkTankRessurection.Item itemReference = playerTeam.bballers[playerTeam.playerBaller].SkinAndData.ballerData.item;
				if(itemReference.activate_inUse) {
					GUI.Label(itemArea, SharkTankRessurection.GameItems.instance.itemGlowTexture);
					intNum = (int) (itemReference.activate_currentTime);
					GUI.Label(itemArea, intNum + "");
				} else if(itemReference.activate_currentTime > 0) {
					bonusText.enabled = true;
					bonusText.text = "+" + (itemReference.effectValue + itemReference.building_boost);
					valueColor.a = itemReference.activate_currentTime / ItemDisplay.ItemFlashTime;
					bonusText.material.color = valueColor; 
				}
				GUI.Label(itemArea, itemReference.itemUseImage);
				if(!itemReference.activate_available) {
					GUI.Label(itemArea, SharkTankRessurection.GameItems.instance.useItemCover);
					intNum = (int) (itemReference.activate_currentTime);
					GUI.Label(itemArea, intNum + "");
				}
			}
		}
	}
}