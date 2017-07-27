using UnityEngine;
using System.Collections;

namespace WSBB {
	public class PlayerBars : MonoBehaviour {
		public static int barXReduction;
		public static Vector2 shotBarCorner;
		public static Vector2 passBarCorner;
		public static Vector2 blockBarCorner;
		public static Vector2 stealBarCorner;
		public static Vector2 barOffset;
		public static Rect backdropArea;
		public static Rect barRect;

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		public static void Init() {
			if(Screen.height == 320) {
				backdropArea = new Rect(0, 0, 160, 65);
				barRect = new Rect(0, 0, 125, 15);
				shotBarCorner = new Vector2(14, 5);
				passBarCorner = new Vector2(14, 16);
				blockBarCorner = new Vector2(14, 29);
				stealBarCorner = new Vector2(14, 40);
				barXReduction = 119;
			} else if(Screen.height == 640) {
				backdropArea = new Rect(0, 0, 320, 130);
				barRect = new Rect(0, 0, 250, 30);
				shotBarCorner = new Vector2(40, 11);
				passBarCorner = new Vector2(40, 35);
				blockBarCorner = new Vector2(40, 60);
				stealBarCorner = new Vector2(40, 84);
				barXReduction = 250;
			}
		}
		
		public static void DrawStatBar(Vector2 pos, BBallerData data) {
			GUI.skin = null;
			var tempRect = backdropArea;
			tempRect.x = pos.x + barOffset.x;
			tempRect.y = pos.y + barOffset.y;
			if(!GameData.instance)
				return;
			
			GUI.BeginGroup(tempRect);
			GUI.Label(backdropArea, GameData.instance.playerBarBackdrop);
			PlayerBars.DrawBarLine(shotBarCorner, (int) data.getShoot(), GameData.instance.playerBarShotBar);
			PlayerBars.DrawBarLine(passBarCorner, (int) data.getPass(), GameData.instance.playerBarPassBar);
			PlayerBars.DrawBarLine(blockBarCorner, (int) data.getBlock(), GameData.instance.playerBarBlockBar);
			PlayerBars.DrawBarLine(stealBarCorner, (int) data.getSteal(), GameData.instance.playerBarStealBar);
			GUI.Label(backdropArea, GameData.instance.playerBarCover);
			
			GUI.EndGroup();
		}
		
		public static void DrawBarLine(Vector2 barCorner, int barValue, Texture2D barTexture) {
			Rect tempRect;
			tempRect = barRect;
			tempRect.x = barCorner.x;
			tempRect.y = barCorner.y;
			tempRect.x -= Mathf.Lerp(0, barXReduction, ((100 - barValue) / 100.0f));
			GUI.Label(tempRect, barTexture);	
		}
	}
}