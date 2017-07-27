using UnityEngine;
using System;
using System.Collections;

namespace WSBB {
	public enum GeneralTeamStates {baseScreen, logoSelect, dunkSelect, PopUp}

	public class GeneralTeamScreen : MonoBehaviour {
		public GeneralTeamStates state;
		private GeneralTeamStates lastState;
		public MenuScreen menuBase;
		public PurchasePopUp popUpScreen;
		public Rect teamNameLabel;
		public String teamNameWord;
		public Rect teamNameRect;
		public Rect teamLogoRect;
		public Rect teamDunkRect;
		public GUISkin mainSkin;
		public GUISkin mainSkinLarge;
		
		//Logo Displays
		public Rect logoDisplayBack;
		public Rect logoOption;
		public Vector2 offset;
		public int logosPerColumn;
		public int logosToDisplay;
		public Rect descriptRect;
		public Rect cancelButton;
		public String cancelWord;
		public int selectionOffset;
		public Rect rightArrow;
		public Rect leftArrow;
		private int purchaseIndex;
		
		//Dunk Displays
		public Rect dunkDisplayBack;
		
		void Awake() {
			if(Screen.height != ScreenHelpers.idealScreenHeight) {
				mainSkin = mainSkinLarge;
				teamNameLabel = ScreenHelpers.scaleRectToScreen(teamNameLabel);
				teamNameRect = ScreenHelpers.scaleRectToScreen(teamNameRect);
				teamLogoRect = ScreenHelpers.scaleRectToScreen(teamLogoRect);
				teamDunkRect = ScreenHelpers.scaleRectToScreen(teamDunkRect);
				logoDisplayBack = ScreenHelpers.scaleRectToScreen(logoDisplayBack);
				logoOption = ScreenHelpers.scaleRectToScreen(logoOption);
				offset = ScreenHelpers.scaleVector2ToScreen(offset);
				descriptRect = ScreenHelpers.scaleRectToScreen(descriptRect);
				cancelButton = ScreenHelpers.scaleRectToScreen(cancelButton);
				rightArrow = ScreenHelpers.scaleRectToScreen(rightArrow);
				leftArrow = ScreenHelpers.scaleRectToScreen(leftArrow);
				
			}
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
			GUI.skin = mainSkin;
			int i = 0;
			Rect tempRect;
			int index;
			if(state == GeneralTeamStates.logoSelect || state == GeneralTeamStates.dunkSelect) {
				GUI.Box(logoDisplayBack, "");	
				GUI.Box(logoDisplayBack, "");
			}
			switch(state) {
				case GeneralTeamStates.logoSelect:
					for(i = 0 + (selectionOffset * logosPerColumn); i < GameLogo.instance.logos.Length; i++) {
						index = i - (selectionOffset * logosPerColumn);
						tempRect = logoOption;
						if(index >= logosToDisplay)
							continue;
						tempRect.y += offset.y * (index % logosPerColumn);
						tempRect.x += offset.x * (index - (index % logosPerColumn )) / logosPerColumn ;
						if(i == GameData.instance.teamLogo) {
							GUI.Label(tempRect, GameLogo.instance.logos[i].logo);
							continue;
						}
						if(GUI.Button(tempRect, GameLogo.instance.logos[i].logo)) {
							if(GameLogo.instance.logos[i].unlocked) {
								GameData.instance.teamLogo = i;
								GameData.save(SaveTarget.TeamIcon);
							} else {
								lastState = state;
								popUpScreen.PopUpImage = GameLogo.instance.logos[i].logo;
								popUpScreen.PopUpMessage = GameLogo.instance.logos[i].popUpMessage;
								popUpScreen.PopUpCost = GameLogo.instance.logos[i].cost;
								popUpScreen.PopUpLevel = 0;
								purchaseIndex = i;
								state = GeneralTeamStates.PopUp;
								
							}
						}
						if(!GameLogo.instance.logos[i].unlocked)
							GUI.Label(tempRect, GameLogo.instance.lockedLogo);
					}
					SelectionBasics();
					break;
				case GeneralTeamStates.dunkSelect:
					for(i = 0 + (selectionOffset * logosPerColumn); i < GameDunks.instance.dunks.Length; i++) {
						index = i - (selectionOffset * logosPerColumn);
						tempRect = logoOption;
						if(index >= logosToDisplay)
							continue;
						tempRect.y += offset.y * (index % logosPerColumn);
						tempRect.x += offset.x * (index - (index % logosPerColumn )) / logosPerColumn ;
						if(i == GameData.instance.teamDunk) {
							GUI.Label(tempRect, GameDunks.instance.dunks[i].selectionImage);
							continue;
						}
						if(GUI.Button(tempRect, GameDunks.instance.dunks[i].selectionImage)) {
							if(GameDunks.instance.dunks[i].unlocked) {
								GameData.instance.teamDunk = i;
								GameData.save(SaveTarget.TeamDunk);
							} else {
								lastState = state;
								popUpScreen.PopUpImage = GameDunks.instance.dunks[i].selectionImage;
								popUpScreen.PopUpMessage = GameDunks.instance.dunks[i].popUpMessage;
								popUpScreen.PopUpCost = GameDunks.instance.dunks[i].cost;
								popUpScreen.PopUpLevel = GameDunks.instance.dunks[i].levelNeeded;
								purchaseIndex = i;
								state = GeneralTeamStates.PopUp;
								
							}
						}
						if(!GameDunks.instance.dunks[i].unlocked)
							GUI.Label(tempRect, GameDunks.instance.lockedDunk);
					}
					SelectionBasics();
					break;
				case GeneralTeamStates.PopUp:
					switch(popUpScreen.DisplayPurchasePopUp()) {
						case PopUpReturn.Success:
							state = lastState;
							Debug.Log("Success");
							if(lastState != GeneralTeamStates.dunkSelect) {
								GameLogo.instance.logos[purchaseIndex].unlocked = true;
								GameData.instance.teamLogo = purchaseIndex;
								GameData.save(SaveTarget.LogoUnlock);
								GameData.save(SaveTarget.TeamIcon);
							} else {
								GameDunks.instance.dunks[purchaseIndex].unlocked = true;
								GameData.instance.teamDunk = purchaseIndex;
								GameData.save(SaveTarget.DunkUnlock);
								GameData.save(SaveTarget.TeamDunk);
							}
							break;
						case PopUpReturn.Failure:
							state = lastState;
							break;
						default:
							break;
					}
					break;
				case GeneralTeamStates.baseScreen:
					GUI.skin = mainSkin;
					GUI.Label(teamNameLabel, teamNameWord);
					GameData.instance.teamName = GUI.TextField(teamNameRect, GameData.instance.teamName, 30);
					if(GUI.Button(teamLogoRect, GameLogo.instance.logos[GameData.instance.teamLogo].logo))
						state = GeneralTeamStates.logoSelect;
					if(GUI.Button(teamDunkRect, GameDunks.instance.dunks[GameData.instance.teamDunk].selectionImage))
						state = GeneralTeamStates.dunkSelect;
					menuBase.ScreenOnGUI();
					break;
				default:
					break;
			}
		}
		
		private void SelectionBasics() {
			if(GUI.Button(cancelButton, cancelWord))
				state = GeneralTeamStates.baseScreen;
			if(state != GeneralTeamStates.dunkSelect) {
				GUI.Box(descriptRect, GameLogo.instance.logos[GameData.instance.teamLogo].popUpMessage);
				if((selectionOffset * logosPerColumn) + logosToDisplay < GameLogo.instance.logos.Length) {
					if(GUI.Button(rightArrow, ">"))
						selectionOffset ++;
				}
			} else {
				GUI.Box(descriptRect, GameDunks.instance.dunks[GameData.instance.teamDunk].popUpMessage);
				if((selectionOffset * logosPerColumn) + logosToDisplay < GameDunks.instance.dunks.Length) {
					if(GUI.Button(rightArrow, ">"))
						selectionOffset ++;
				}
				
			}
			
			if(selectionOffset > 0) {
				if(GUI.Button(leftArrow, "<")) {
					selectionOffset --;
				}
			}
		}
	}
}