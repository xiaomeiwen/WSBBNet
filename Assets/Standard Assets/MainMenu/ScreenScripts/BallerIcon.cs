using UnityEngine;
using System;
using System.Collections;

namespace WSBB {
	public class BallerIcon : MonoBehaviour {
		public Rect iconRect;
		public Rect rightQuad;
		public Rect leftQuad;
		public Rect nameSpace;
		public Texture2D texture2DBase;
		public Texture2D emptyBase;
		public String emptyWord;
		public GUISkin mainSkin;
		public GUISkin mainSkinLarge;
		
		void Awake() {
			if(Screen.height != ScreenHelpers.idealScreenHeight) {
				mainSkin = mainSkinLarge;
				iconRect = ScreenHelpers.scaleRectToScreen(iconRect);
				rightQuad = ScreenHelpers.scaleRectToScreen(rightQuad);
				leftQuad = ScreenHelpers.scaleRectToScreen(leftQuad);
				nameSpace = ScreenHelpers.scaleRectToScreen(nameSpace);
			}
		}

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}
		
		public void DrawTeamPlayerIcon(int playerIndex, Vector2 offset, bool stats) {
			GUISkin oldSkin = GUI.skin;
			GUI.skin = mainSkin;
			Rect tempRect;
			tempRect = iconRect;
			tempRect.x += offset.x;
			tempRect.y += offset.y;
			if(GameData.instance.teamBallers[playerIndex] == -1) {
				GUI.Label(tempRect, emptyWord);
			} else {
				GUI.Label(tempRect, texture2DBase);
				tempRect = rightQuad;
				tempRect.x += offset.x;
				tempRect.y += offset.y;
				GUI.Label(tempRect, GameBallers.instance.ballers[GameData.instance.teamBallers[playerIndex]].mugshot);
				tempRect = nameSpace;
				tempRect.x += offset.x;
				tempRect.y += offset.y;
				GUI.Label(tempRect, GameBallers.instance.ballers[GameData.instance.teamBallers[playerIndex]].ballerName);
				tempRect = leftQuad;
				tempRect.x += offset.x;
				tempRect.y += offset.y;
				if(stats)
					GUI.Label(tempRect, "STATS");
				else
					GUI.Label(tempRect, "ITEM");
			}
			GUI.skin = oldSkin;
		}
		
		public void DrawBallerIcon(int playerIndex, Vector2 offset, bool stats, bool cover) {
			GUISkin oldSkin = GUI.skin;
			GUI.skin = mainSkin;
			Rect tempRect;
			tempRect = iconRect;
			tempRect.x += offset.x;
			tempRect.y += offset.y;
			if(GameBallers.instance.ballers[playerIndex] == null) {//JS == -1) {
				GUI.Label(tempRect, emptyWord);
			} else {
				GUI.Label(tempRect, texture2DBase);
				tempRect = rightQuad;
				tempRect.x += offset.x;
				tempRect.y += offset.y;
				GUI.Label(tempRect, GameBallers.instance.ballers[playerIndex].mugshot);
				tempRect = nameSpace;
				tempRect.x += offset.x;
				tempRect.y += offset.y;
				GUI.Label(tempRect, GameBallers.instance.ballers[playerIndex].ballerName);
				tempRect = leftQuad;
				tempRect.x += offset.x;
				tempRect.y += offset.y;
				if(stats)
					GUI.Label(tempRect, "STATS");
				else
					GUI.Label(tempRect, "ITEM");
				
				if(cover) {
					tempRect = iconRect;
					tempRect.x += offset.x;
					tempRect.y += offset.y;
					GUI.Box(tempRect, "SELECTED");
				}
				
			}
			GUI.skin = oldSkin;
		}
	}
}