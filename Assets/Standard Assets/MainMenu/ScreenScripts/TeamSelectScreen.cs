using UnityEngine;
using System;
using System.Collections;

namespace WSBB {
	public enum TeamSelectStates {Base, PopUp, TeamSelect, StarterSelect}

	public class TeamSelectScreen : MonoBehaviour {
		public int index;
		public int playersPerRow;
		public int rowOffset;
		
		public String recruitWordEmpty;
		public String recruitWordOccupied;
		public String playerCancelWord;
		public String starterWord;
		public String benchWord;
		public String pointWord;
		public String wingWord;
		public String centerWord;

		public TeamSelectStates state;
		public TeamSelectStates lastState;
		public MenuScreen menuBase;
		public PlayerPicker playerPicker;
		public MannequinManager mannequins;
		public BallerIcon ballerIcon;
		
		public Rect recruitRect;
		//Team Select Icons 
		public Rect playerSelectBackground;
		public Vector2 playerStartPos;
		public Vector2 playerOffset;
		public Rect playerCancelRect;
		
		//Team Start GUI
		public Rect startSelectBackground;
		public Rect starterButton;
		public Rect makePointButton;
		public Rect makeWingButton;
		public Rect makeCenterButton;
		
		void Awake() {
			if(Screen.height != ScreenHelpers.idealScreenHeight) {
				recruitRect = ScreenHelpers.scaleRectToScreen(recruitRect);
				playerSelectBackground = ScreenHelpers.scaleRectToScreen(playerSelectBackground);
				playerStartPos = ScreenHelpers.scaleVector2ToScreen(playerStartPos);
				playerOffset = ScreenHelpers.scaleVector2ToScreen(playerOffset);
				playerCancelRect = ScreenHelpers.scaleRectToScreen(playerCancelRect);
				startSelectBackground = ScreenHelpers.scaleRectToScreen(startSelectBackground);
				starterButton = ScreenHelpers.scaleRectToScreen(starterButton);
				makePointButton = ScreenHelpers.scaleRectToScreen(makePointButton);
				makeWingButton = ScreenHelpers.scaleRectToScreen(makeWingButton);
				makeCenterButton = ScreenHelpers.scaleRectToScreen(makeCenterButton);
			}
		}

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		public void ScreenUpdate () {
			menuBase.ScreenUpdate();
		}
		
		public void ScreenOnGUI() {
			switch(state) {
				case TeamSelectStates.Base: 
					DrawBaseMenu();
					menuBase.ScreenOnGUI();
					break;
				case TeamSelectStates.TeamSelect:
					DrawTeamSelection();
					break;
				case TeamSelectStates.StarterSelect:
					DrawStarterSelect();
					break;
				case TeamSelectStates.PopUp:
					DrawStarterSelect();
					break;
				default:
					break;
			}
		}
		
		public void DrawBaseMenu() {
			if(playerPicker.DrawTeamPicker(index)) {
				index = PlayerPicker.selectedPlayer;
			}

			if(GameData.instance.teamBallers[index] == -1) {
				if(GUI.Button(recruitRect, recruitWordEmpty))
					state = TeamSelectStates.TeamSelect;
			} else {
				if(GUI.Button(recruitRect, recruitWordOccupied))
					state = TeamSelectStates.TeamSelect;
			}
			
			bool alreadyStarter = false;
			int starterIndex = -1;
			for(int j = 0; j < GameData.instance.teamStarters.Length; j++) {
				if(GameData.instance.teamStarters[j] == index) {
					alreadyStarter = true;
					starterIndex = j;
				}
			}
			
			if(alreadyStarter) {
				if(GUI.Button(starterButton, benchWord))
					GameData.instance.teamStarters[starterIndex] = -1;
			} else {
				if(GUI.Button(starterButton, starterWord))
					state = TeamSelectStates.StarterSelect;
			}
			
		}
		
		public void DrawStarterSelect() {
			GUI.Box(startSelectBackground, "");
			GUI.Box(startSelectBackground, "");
			if(GUI.Button(starterButton, playerCancelWord))
				state = TeamSelectStates.Base;
			if(GUI.Button(makePointButton, pointWord)) {
				GameData.instance.teamStarters[0] = index;
				GameData.save(SaveTarget.Starters);
				state = TeamSelectStates.Base;
			}
			if(GUI.Button	(makeWingButton, wingWord)) {
				GameData.instance.teamStarters[1] = index;
				GameData.save(SaveTarget.Starters);
				state = TeamSelectStates.Base;
			}
			if(GUI.Button(makeCenterButton, centerWord)) {
				GameData.instance.teamStarters[2] = index;
				GameData.save(SaveTarget.Starters);
				state = TeamSelectStates.Base;
			}
		}
		
		public void DrawTeamSelection() {
			GUI.Box(playerSelectBackground, "");
			GUI.Box(playerSelectBackground, "");
			
			if(GUI.Button(playerCancelRect, playerCancelWord))
				state = TeamSelectStates.Base;
			
			Vector2 tempPos;
			Rect tempRect = ballerIcon.iconRect;
			for(int i = 0; i < GameBallers.instance.ballers.Length; i++)
			{
				tempPos = playerStartPos;
				int columnPos = i % playersPerRow;
				int rowPos = ((i - columnPos) / playersPerRow) - rowOffset;
				tempPos.x += columnPos * playerOffset.x;
				tempPos.y += rowPos * playerOffset.y;
				bool ballerSelected = false;
				foreach(int ballerNum in GameData.instance.teamBallers) {
					if(ballerNum == i) {
						ballerSelected = true;
						Debug.Log("Baller Found");
					}
				}

				if(!ballerSelected) {
					tempRect.x = tempPos.x;
					tempRect.y = tempPos.y;
					if(GUI.Button(tempRect, "")) {
						GameData.instance.teamBallers[index] = i;
						state = TeamSelectStates.Base;
						ReloadBaller(index);
						GameData.save(SaveTarget.TeamLayout, index);
						for(int j = 0; j < GameData.instance.teamBallers.Length; j++) {
							if(GameData.instance.teamBallers[j] == -1) {
								index = j;
								break;
							}
						}
					}
				}
				ballerIcon.DrawBallerIcon(i, tempPos, true, ballerSelected);
			}
		}
		
		public void ReloadBaller(int index) {
			mannequins.skins[index].teamDataSource = TeamDataSource.localPlayer;
			mannequins.skins[index].awayTeam = false;
			mannequins.skins[index].fetchData();
			mannequins.skins[index].refreshUVPos();
			mannequins.playerRenders[index].enabled = true;
			mannequins.ballRenders[index].enabled = true;
		}
	}
}