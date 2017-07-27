using UnityEngine;
using System.Collections;

namespace WSBB {
	public class PlayerPicker : MonoBehaviour {
		public static int selectedPlayer;
		
		public BallerIcon ballerIcon;
		public Rect teamButtonRect;
		public Rect playerLabelRect;
		public Rect playerSelectRect;
		public Vector2 offset;
		public GUISkin mainSkin;
		public GUISkin mainSkinLarge;
		
		//Team Select Tags
		public Texture2D benchTag;
		public Texture2D postTag;
		public Texture2D wingTag;
		public Texture2D pointGuardTag;
		public Texture2D playerSelectImage;
		
		void Awake() {
			if(Screen.height != ScreenHelpers.idealScreenHeight) {
				mainSkin = mainSkinLarge;
				teamButtonRect = ScreenHelpers.scaleRectToScreen(teamButtonRect);
				playerLabelRect = ScreenHelpers.scaleRectToScreen(playerLabelRect);
				playerSelectRect = ScreenHelpers.scaleRectToScreen(playerSelectRect);
				offset = ScreenHelpers.scaleVector2ToScreen(offset);
			}
		}
		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}
		
		public bool DrawTeamPicker(int selectedPlayer){
			GUISkin oldSkin = GUI.skin;
			GUI.skin = mainSkin;
			Rect tempRect;
			for(int i = 0; i < 6; i++) {
				if(i == selectedPlayer) {
					tempRect = playerSelectRect;
					tempRect.x += offset.x * i;
					tempRect.y += offset.y * i;	
					GUI.Label(tempRect, playerSelectImage);
				}
				tempRect = teamButtonRect;
				tempRect.x += offset.x * i;
				tempRect.y += offset.y * i;
				if(GUI.Button(tempRect, "")) {
					PlayerPicker.selectedPlayer = i;
					GUI.skin = oldSkin;
					return true;
				}
				Vector2 tempOffset;
				tempOffset.x = tempRect.x;
				tempOffset.y = tempRect.y;
				ballerIcon.DrawTeamPlayerIcon(i, tempOffset, true);
				tempRect = playerLabelRect;
				tempRect.x += offset.x * i;
				tempRect.y += offset.y * i;
				if(GameData.instance.teamStarters[0] == i)
					GUI.Label(tempRect, postTag);
				else if(GameData.instance.teamStarters[1] == i)
					GUI.Label(tempRect, wingTag);
				else if(GameData.instance.teamStarters[2] == i)
					GUI.Label(tempRect, pointGuardTag);
				else
					GUI.Label(tempRect, benchTag);
				
				
			}
			GUI.skin = oldSkin;
			return false;
		}
	}
}