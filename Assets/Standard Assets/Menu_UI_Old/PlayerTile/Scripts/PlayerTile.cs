using UnityEngine;
using System.Collections;

namespace WSBB {
	public class PlayerTile : MonoBehaviour {
		public static int tileOffsetX;
		public static int tileOffsetY;
		public static int barAreaOffset;
		public static int barAreaXPos;
		public static int barAreaYPos;
		public static int barBottomY ;

		public static Rect tileArea;
		public static Rect picRect;
		public static Rect wammyRect;
		public static Rect shotBar;
		public static Rect passBar;
		public static Rect blockBar;
		public static Rect stealBar;

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		public static void Init() {
			if(Screen.height == 320) {
				tileArea = new Rect(3,0, 90, 60);
				picRect = new Rect(22,0,35,60);
				wammyRect = new Rect(0,0,22,60);
				shotBar = new Rect(59,2,5,50);
				passBar = new Rect(66,2,5,50);
				blockBar = new Rect(73,2,5,50);
				stealBar = new Rect(80,2,5,50);
				tileOffsetX = -3;
				tileOffsetY = -3;
				barAreaXPos = -2;
				barAreaYPos = -2;
				barBottomY = 47;
				barAreaOffset = -10;
			} else if(Screen.height == 640) {		
				tileArea = new Rect(4,0, 180, 120);
				picRect = new Rect(45,0,70,120);
				wammyRect = new Rect(0,0,45, 120);
				shotBar = new Rect(118,4,10,100);
				passBar = new Rect(134,4,10,100);
				blockBar = new Rect(148,4,10,100);
				stealBar = new Rect(164,4,10,100);
				tileOffsetX = -4;
				tileOffsetY = -4;
				barAreaXPos = 0;
				barAreaYPos = 0;
				barBottomY = 98;
				barAreaOffset = -19;
			}
		}
		
		public static void DrawTile(Vector2 pos, BBallerData data) {
			PlayerTile.DrawTile(pos, data, true);
		}

		public static void DrawTile(Vector2 pos, BBallerData data, bool face) {
			var tempRect = tileArea;
			tempRect.x = pos.x + tileOffsetX;
			tempRect.y = pos.y + tileOffsetY;
			if(GameData.instance) {
				GUI.BeginGroup(tempRect);
				GUI.Label(tileArea, GameData.instance.playerTileBackdrop);
				if(face)
					GUI.Label(picRect, data.mugshot);
				GUI.BeginGroup(new Rect(barAreaXPos,barAreaYPos,tileArea.width, tileArea.height + barAreaOffset));
				var barRect = shotBar;
				barRect.y = Mathf.Lerp(barBottomY, shotBar.y, data.getShoot() / 100.0f);
				GUI.Label(barRect, GameData.instance.playerTileShotBar);
				barRect = passBar;
				barRect.y = Mathf.Lerp(barBottomY, passBar.y, data.getPass() / 100.0f);
				GUI.Label(barRect, GameData.instance.playerTilePassBar);
				barRect = blockBar;
				barRect.y = Mathf.Lerp(barBottomY, blockBar.y, data.getBlock() / 100.0f);
				GUI.Label(barRect, GameData.instance.playerTileBlockBar);
				barRect = stealBar;
				barRect.y = Mathf.Lerp(barBottomY, stealBar.y, data.getSteal() / 100.0f);
				GUI.Label(barRect, GameData.instance.playerTileStealBar);
				GUI.EndGroup();
				GUI.EndGroup();
			}
		}
		
		public static Rect GetRectArea(Vector2 pos) {
			Rect tempRect = tileArea;
			tempRect.x = pos.x + tileOffsetX;
			tempRect.y = pos.y + tileOffsetY;
			return tempRect;
		}
	}
}