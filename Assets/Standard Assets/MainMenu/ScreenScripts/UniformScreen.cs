using UnityEngine;
using System;
using System.Collections;

namespace WSBB {
	public enum EquipState {Shorts, Shoes, Jersey, PopUp}

	public class UniformScreen : MonoBehaviour {
		public bool awayColors;
		public bool dataLoaded;
		
		public int itemsInRow;
		public int maxRows;
		public int rowOffset;
		public int storedLength;
		private int storedIndex;
		
		public String homeTeamButtonMessage;
		public String awayTeamButtonMessage;
		public String homeTeamLabelWord;
		public String awayTeamLabelWord;
		public String nextWord;
		public String previousWord;
		public String teamUnifornWord;
		
		public Rect awayButton;		
		public Rect jerseyButton;
		public Rect shortsButton;
		public Rect shoesButton;
		public Rect selectionUpRect;
		public Rect selectionDownRect;
		public Rect selectBackground;
		public Rect itemRect;		
		public Rect titleBarRect;
		public Rect primaryRect;
		public Rect secondaryRect;

		public Vector2 itemSpacing;
		
		public Texture2D homeTexture;
		public Texture2D awayTexture;

		public EquipState state;
		public EquipState lastState;
		
		public GUISkin mainSkin;
		public GUISkin mainSkinLarge;

		public MenuScreen menuBase;
		public PurchasePopUp purchaseScreen;
		public BBaller_skin [] dummy;
		
		void Awake() {
			if(Screen.height != ScreenHelpers.idealScreenHeight) {
				mainSkin = mainSkinLarge;
				titleBarRect = ScreenHelpers.scaleRectToScreen(titleBarRect);
				primaryRect = ScreenHelpers.scaleRectToScreen(primaryRect);
				secondaryRect = ScreenHelpers.scaleRectToScreen(secondaryRect);
				awayButton = ScreenHelpers.scaleRectToScreen(awayButton);
				jerseyButton = ScreenHelpers.scaleRectToScreen(jerseyButton);
				shortsButton = ScreenHelpers.scaleRectToScreen(shortsButton);
				shoesButton = ScreenHelpers.scaleRectToScreen(shoesButton);
				selectBackground = ScreenHelpers.scaleRectToScreen(selectBackground);
				itemRect = ScreenHelpers.scaleRectToScreen(itemRect);
				itemSpacing = ScreenHelpers.scaleVector2ToScreen(itemSpacing);
				selectionUpRect = ScreenHelpers.scaleRectToScreen(selectionUpRect);
				selectionDownRect = ScreenHelpers.scaleRectToScreen(selectionDownRect); 
			}
			dataLoaded = false;
		}

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}
		
		public void Init(bool forceInit) {
			if(!dataLoaded || forceInit) {
				if(GameData.instance != null) {
					awayColors = false;
					SetBallerMaterials();
					dummy[0].teamDataSource = TeamDataSource.localPlayer;
					dummy[0].awayTeam = false;
					dummy[1].teamDataSource = TeamDataSource.localPlayer;
					dummy[1].awayTeam = true;
					dummy[0].refreshUVPos();
					dummy[1].refreshUVPos();
					dataLoaded = true;
				}
			}
		}
		
		public void ScreenUpdate() {
			menuBase.ScreenUpdate();
		}
		
		public void ScreenOnGUI() {
			if(state == EquipState.PopUp) {
				switch(purchaseScreen.DisplayPurchasePopUp()) {
					case PopUpReturn.Success:
						state = lastState;
						Debug.Log("Success");
						UnlockUniformItem();
						break;
					case PopUpReturn.Failure:
						state = lastState;
						break;
					default:
						break;
				}
				return;
			}
			
			GUI.skin = mainSkin;
			GUI.color = Color.white;
			if(state != EquipState.Jersey) {
				if(GUI.Button(jerseyButton, "")) {
					state = EquipState.Jersey;
					rowOffset = 0;
				}
			} else
				GUI.Box(jerseyButton, "");
			GUI.color = Color.white;
			GUI.Label(jerseyButton, GameUniforms.instance.baseJerseyIcon);
			
			GUI.color = Color.white;
			if(state != EquipState.Shorts) {
				if(GUI.Button(shortsButton, "")) {
					state = EquipState.Shorts;
					rowOffset = 0;
				}
			} else
				GUI.Box(shortsButton, "");
			GUI.color = Color.blue;
			GUI.Label(shortsButton, GameUniforms.instance.baseJerseyIcon);
			
			
			GUI.color = Color.white;
			if(state != EquipState.Shoes) {
				if(GUI.Button(shoesButton, "")) {
					state = EquipState.Shoes;
					rowOffset = 0;
				}
			} else
				GUI.Box(shoesButton, "");
			GUI.color = Color.red;
			GUI.Label(shoesButton, GameUniforms.instance.baseJerseyIcon);
			
			GUI.color = Color.white;
			GUI.Box(selectBackground, "");
			if(rowOffset != 0)
				if(GUI.Button(selectionUpRect, previousWord + itemsInRow))
					rowOffset --;
			int i;
			switch(state) {
				case EquipState.Jersey:
					if(awayColors) {
						for(i = 0; i < GameUniforms.instance.awayJerseys.Length; i++)
							if(DisplayItemIcon(i, GameUniforms.instance.awayJerseys[i].previewColor,
							                   GameUniforms.instance.baseJerseyIcon,
							                   GameUniforms.instance.awayJerseys[i].unlocked,
							                   GameUniforms.instance.awayJerseys[i].unlockMessage,
							                   GameUniforms.instance.awayJerseys[i].cost) != -1) {
							GameData.instance.teamJerseyAway = i;
							dummy[0].refreshUVPos();
							GameData.save(SaveTarget.TeamColors);
						}
						storedLength = GameUniforms.instance.awayJerseys.Length;
					} else {
						for(i = 0; i < GameUniforms.instance.homeJerseys.Length; i++)
							if(DisplayItemIcon(i, GameUniforms.instance.homeJerseys[i].previewColor, 
							                   GameUniforms.instance.baseJerseyIcon,
							                   GameUniforms.instance.homeJerseys[i].unlocked,
							                   GameUniforms.instance.homeJerseys[i].unlockMessage,
							                   GameUniforms.instance.homeJerseys[i].cost) != -1) {
							GameData.instance.teamJersey = i;
							dummy[0].refreshUVPos();
							GameData.save(SaveTarget.TeamColors);
						}
						storedLength = GameUniforms.instance.homeJerseys.Length;
					}
					break;
				case EquipState.Shorts:
					if(awayColors) {
						for(i = 0; i < GameUniforms.instance.awayShorts.Length; i++) {
							if(DisplayItemIcon(i, GameUniforms.instance.awayShorts[i].previewColor,
							                   GameUniforms.instance.baseJerseyIcon,
							                   GameUniforms.instance.awayShorts[i].unlocked,
							                   GameUniforms.instance.awayShorts[i].unlockMessage,
							                   GameUniforms.instance.awayShorts[i].cost) != -1) {
								GameData.instance.teamShortsAway = i;
								dummy[0].refreshUVPos();
								GameData.save(SaveTarget.TeamColors);
							}
						}
						storedLength = GameUniforms.instance.awayShorts.Length;
					} else {
						for(i = 0; i < GameUniforms.instance.homeShorts.Length; i++) {
							if(DisplayItemIcon(i, GameUniforms.instance.homeShorts[i].previewColor, 
							                   GameUniforms.instance.baseJerseyIcon,
							                   GameUniforms.instance.homeShorts[i].unlocked,
							                   GameUniforms.instance.homeShorts[i].unlockMessage,
							                   GameUniforms.instance.homeShorts[i].cost) != -1) {
								GameData.instance.teamShorts = i;
								dummy[0].refreshUVPos();
								GameData.save(SaveTarget.TeamColors);
							}
						}
						storedLength = GameUniforms.instance.homeShorts.Length;
					}
					break;
				case EquipState.Shoes:
					if(awayColors) {
						for(i = 0; i < GameUniforms.instance.awayShoes.Length; i++) {
							if(DisplayItemIcon(i, GameUniforms.instance.awayShoes[i].previewColor,
							                   GameUniforms.instance.baseJerseyIcon,
							                   GameUniforms.instance.awayShoes[i].unlocked,
							                   GameUniforms.instance.awayShoes[i].unlockMessage,
							                   GameUniforms.instance.awayShoes[i].cost) != -1) {
								GameData.instance.teamShoesAway = i;
								dummy[0].refreshUVPos();
								GameData.save(SaveTarget.TeamColors);
							}
						}
						storedLength = GameUniforms.instance.awayShoes.Length;
					} else {
						for(i = 0; i < GameUniforms.instance.homeShoes.Length; i++) {
							if(DisplayItemIcon(i, GameUniforms.instance.homeShoes[i].previewColor, 
							                   GameUniforms.instance.baseJerseyIcon,
							                   GameUniforms.instance.homeShoes[i].unlocked,
							                   GameUniforms.instance.homeShoes[i].unlockMessage,
							                   GameUniforms.instance.homeShoes[i].cost) != -1) {
								GameData.instance.teamShoes = i;
								dummy[0].refreshUVPos();
								GameData.save(SaveTarget.TeamColors);
							}
						}
						storedLength = GameUniforms.instance.homeShoes.Length;
					}
					break;
				default:
					break;
			}
			int filledRows = (storedLength + (itemsInRow - (storedLength % itemsInRow))) / itemsInRow;
			if((rowOffset + maxRows + 1) < filledRows) {
				if(GUI.Button(selectionDownRect, nextWord + itemsInRow)) {
					rowOffset++;
				}
			}			
			
			GUI.color = Color.white;
			String buttonLabel = awayTeamButtonMessage;
			if(awayColors) {
				buttonLabel =  homeTeamButtonMessage;
			}
			if(GUI.Button(awayButton, buttonLabel)) {
				if(awayColors) {
					awayColors = false;
					SetBallerMaterials();
					dummy[0].awayTeam = false;
					dummy[1].awayTeam = true;
					dummy[0].refreshUVPos();
					dummy[1].refreshUVPos();
					return;
				} else {
					awayColors = true;
					SetBallerMaterials();
					dummy[0].awayTeam = true;
					dummy[1].awayTeam = false;
					dummy[0].refreshUVPos();
					dummy[1].refreshUVPos();
					return;
				}
			}
			
			GUI.Box(titleBarRect, teamUnifornWord);
			if(awayColors) {
				GUI.Box(primaryRect, awayTeamLabelWord);
				GUI.Box(secondaryRect, homeTeamLabelWord );
			} else {
				GUI.Box(primaryRect, homeTeamLabelWord );
				GUI.Box(secondaryRect, awayTeamLabelWord);
			}
			menuBase.ScreenOnGUI();
		}
		
		private int DisplayItemIcon(int i, Color tint, Texture2D texture2DBase, bool unlocked, String unlockMess, int unlockCost) {
			int valueToReturn = -1;
			int columnPos = i % itemsInRow;
			int rowPos = ((i - columnPos) / itemsInRow) - rowOffset;
			if(rowPos >= maxRows || rowPos < 0)
				return valueToReturn;
			Rect tempRect = itemRect;
			tempRect.x += itemSpacing.x * columnPos;
			tempRect.y += itemSpacing.y * rowPos;
			GUI.color = Color.white;
			if(GUI.Button(tempRect, "")) {
				if(unlocked) {
					valueToReturn = i;
				} else {
					purchaseScreen.PopUpImage = texture2DBase;
					purchaseScreen.PopUpMessage = unlockMess;
					purchaseScreen.PopUpCost = unlockCost;
					purchaseScreen.PopUpLevel = 0;
					storedIndex = i;
					lastState = state;
					state = EquipState.PopUp;
				}
			}
			GUI.color = tint;
			if(unlocked)
				GUI.Label(tempRect, texture2DBase);
			else
				GUI.Label(tempRect, GameUniforms.instance.lockedIcon);
			GUI.color = Color.white;
			return valueToReturn;
		}
		
		private void UnlockUniformItem() {
			switch(state) {
				case EquipState.Jersey:
					if(awayColors) {
						GameUniforms.instance.awayJerseys[storedIndex].unlocked = true;
					} else {
						GameUniforms.instance.homeJerseys[storedIndex].unlocked = true;		
					}
					break;
				case EquipState.Shorts:
					if(awayColors) {
						GameUniforms.instance.awayShorts[storedIndex].unlocked = true;
					} else {
						GameUniforms.instance.homeShorts[storedIndex].unlocked = true;		
					}
					break;
				case EquipState.Shoes:
					if(awayColors) {
						GameUniforms.instance.awayShoes[storedIndex].unlocked = true;
					} else {
						GameUniforms.instance.homeShoes[storedIndex].unlocked = true;		
					}
					break;
				default:
					break;
			}
			GameData.save(SaveTarget.UniformUnlock);
			PlayerPrefs.Save();
		}
		
		private void SetBallerMaterials() {
			if(awayColors) {
				foreach(Material homeMat in dummy[1].myRenderer.materials)
					homeMat.mainTexture = homeTexture;
				foreach(Material awayMat in dummy[0].myRenderer.materials)
					awayMat.mainTexture = awayTexture;
			} else {
				foreach(Material homeMat in dummy[0].myRenderer.materials)
					homeMat.mainTexture = homeTexture;
				foreach(Material awayMat in dummy[1].myRenderer.materials)
					awayMat.mainTexture = awayTexture;
			}
		}
	}
}