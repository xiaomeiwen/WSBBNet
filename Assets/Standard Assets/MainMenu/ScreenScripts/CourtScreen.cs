using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

namespace WSBB {
	public class CourtScreen : MonoBehaviour {

		//Adrian Testing UI Variable
		public Button[] sourceButtons;
		public Image bigImg;
		public Text courtName;

		public GUISkin mySkin;
		public GUISkin mySkin_Large;
		public MenuScreen baseMenu;
		public String gameScreenName;
		public String loadScreenName;
		public bool displayExpBuy;
		public BlingPopUp credPopUp;
		
		public int index;
		public Texture2D sliderBackdrop;
		public Texture2D lockedTexture;
		public Rect centerCourtRect;
		public Rect courtNameRect;
		public Rect courtDescRect;
		public float spacing;
		public float offset;
		public float springStrength;
		public Rect touchArea;
		public Rect buttonArea;
		public Touch currentTouch;
		public int currentTouchIndex;
		public String selectWord;
		public String needUnlockWord;
		public String lockedWord;
		public String unlockMessage;
		public Texture2D progressBarFrame;
		public Texture2D progressBarImage;
		public Rect progressBarRect;
		public Rect levelDisplay;
		public Rect requiredLevelDisplay;
		private bool dataLoaded = false;

		//DEBUG STUFF
		public Rect unlockButton;
		
		void Awake() {
			currentTouchIndex = -1;	
			if(Screen.height != ScreenHelpers.idealScreenHeight) {
				centerCourtRect = ScreenHelpers.scaleRectToScreen(centerCourtRect);
				courtNameRect = ScreenHelpers.scaleRectToScreen(courtNameRect);
				courtDescRect = ScreenHelpers.scaleRectToScreen(courtDescRect);
				unlockButton = ScreenHelpers.scaleRectToScreen(unlockButton);
				touchArea = ScreenHelpers.scaleRectToScreen(touchArea);
				buttonArea = ScreenHelpers.scaleRectToScreen(buttonArea);
				spacing = spacing * (Screen.width / ScreenHelpers.idealScreenWidth);
				springStrength = springStrength * (Screen.width / ScreenHelpers.idealScreenWidth);
				progressBarRect = ScreenHelpers.scaleRectToScreen(progressBarRect);
				levelDisplay = ScreenHelpers.scaleRectToScreen(levelDisplay);
				requiredLevelDisplay = ScreenHelpers.scaleRectToScreen(requiredLevelDisplay);
				mySkin = mySkin_Large;
			}
			displayExpBuy = false;
		}

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		public void ScreenUpdate() {
			baseMenu.ScreenUpdate();
			if(GameData.instance == null) {
				return;
			}
			
			if(Input.touches.Length == 0) {
				currentTouchIndex = -1;
			}
			foreach(Touch touch in Input.touches) {
				Touch tempTouch = touch;
				if(ScreenHelpers.checkTouch(tempTouch.position, touchArea)) {
					if(-1 == currentTouchIndex) {
						currentTouch = tempTouch;
						currentTouchIndex = currentTouch.fingerId;
						break;
					}
				}
				if(currentTouch.fingerId == tempTouch.fingerId) {
					if(tempTouch.phase == TouchPhase.Ended || tempTouch.phase == TouchPhase.Canceled) {
						currentTouchIndex = -1;
						break;
					} else {
						currentTouch = tempTouch;
						break;
					}
				}
			}
			
			if(currentTouchIndex != -1) {
				if (currentTouch.phase == TouchPhase.Moved) {
					offset += currentTouch.deltaPosition.x * 1.5f;
				}
			} else if(Mathf.Abs(offset) > 0) {
				if(offset > 0) {
					offset -= Time.deltaTime * springStrength;
					if(offset < 0)
						offset = 0;
				} else {
					offset += Time.deltaTime * springStrength;
					if(offset > 0)
						offset = 0;
				}
			}
			
			if(Mathf.Abs(offset) > (spacing / 2)) {
				if(offset > 0) {
					index--;
					offset -= spacing;
					IndexChanged();
				} else {
					index++;
					offset += spacing;
					IndexChanged();
				}
			}		
		}
		
		public void ScreenOnGUI() {
			baseMenu.ScreenOnGUI();
			if(GameData.instance == null) {
				return;
			}
			if(displayExpBuy) {
				if(credPopUp.DisplayPopUp())
					displayExpBuy = false;
				return;
			}
			GUI.skin = mySkin;
			
			if(MainMenuState.instance.worldTourMode) {
				WorldTourDisplay();
			} else {
				CourtsDisplay();
			}
			
			GUI.Box(touchArea, "");
			
			if(MainMenuState.instance.worldTourMode) {
				if(GameData.instance.teamLevel >= GameData.instance.worldTour[index].requiredLevel) {
					if(GUI.Button(buttonArea, selectWord))
						WorldTourSelected();
				} else {
					if(GUI.Button(buttonArea, needUnlockWord))
						displayExpBuy = true;
				}
				GUI.BeginGroup(progressBarRect);
				Rect tempRect = progressBarRect;
				tempRect.y = 0;
				tempRect.x = -(tempRect.width * (100.0f - GameData.instance.teamExp) / 100.0f);
				GUI.Label(tempRect, progressBarImage);
				tempRect.x = 0;
				GUI.Label(tempRect, progressBarFrame);
				GUI.EndGroup();
				
				GUI.Box(levelDisplay, "" + GameData.instance.teamLevel);
				GUI.Box(requiredLevelDisplay, "" + GameData.instance.worldTour[index].requiredLevel);
			} else {
				if(GameData.instance.courts[index].unlocked) {
					if(GUI.Button(buttonArea, selectWord))
						CourtSelected();
				} else {
					GUI.Button(buttonArea, lockedWord);
				}
				if(GameData.instance.courts[index].unlocked)
					GUI.Box(courtDescRect, GameData.instance.courts[index].courtDesc);
				else
					GUI.Box(courtDescRect,  unlockMessage);
			}
		}

		public void IndexChanged() {
			if(MainMenuState.instance.worldTourMode) {
				if(index >= GameData.instance.worldTour.Length)
					index = 0;
				else if(index < 0)
					index = GameData.instance.worldTour.Length - 1;	
			} else {
				if(index >= GameData.instance.courts.Length)
					index = 0;
				else if (index < 0)
					index = GameData.instance.courts.Length - 1;
			}
		}
		
		public void WorldTourDisplay() {
			int curIndex;
			Rect tempRect;
			for(int i = -2; i < 3; i++) {
				curIndex = index + i;
				if(curIndex >= GameData.instance.worldTour.Length) {
					curIndex -= GameData.instance.worldTour.Length;
				}
				if (curIndex < 0) {
					curIndex += GameData.instance.worldTour.Length;
				}
				tempRect = centerCourtRect;
				tempRect.x = tempRect.x + (spacing * i) + offset;
				//Debug.Log(i + ":" + curIndex);
				if(GameData.instance.teamLevel >= GameData.instance.worldTour[curIndex].requiredLevel) {
					GUI.Label(tempRect, GameData.instance.worldTour[curIndex].courtPic);
				} else {
					GUI.Label(tempRect, lockedTexture);
				}
			}
			if(GameData.instance.teamLevel >= GameData.instance.worldTour[index].requiredLevel) {
				GUI.Box(courtNameRect, GameData.instance.worldTour[index].courtName);
			} else {
				GUI.Box(courtNameRect, "!LOCKED!");
			}
		}
		
		public void CourtsDisplay() {
			int curIndex;
			Rect tempRect;
			for(int i = -2; i < 3; i++) {
				curIndex = index + i;
				if(curIndex >= GameData.instance.courts.Length) {
					curIndex -= GameData.instance.courts.Length;
				}
				if (curIndex < 0) {
					curIndex += GameData.instance.courts.Length;
				}
				tempRect = centerCourtRect;
				tempRect.x = tempRect.x + (spacing * i) + offset;
				//Debug.Log(i + ":" + curIndex);
				if(GameData.instance.courts[curIndex].unlocked) {
					GUI.Label(tempRect, GameData.instance.courts[curIndex].courtPic);
				} else {
					GUI.Label(tempRect, lockedTexture);
				}
			}
			if(GameData.instance.courts[index].unlocked) {
				GUI.Box(courtNameRect, GameData.instance.courts[index].courtName);
			} else {
				GUI.Box(courtNameRect, "!LOCKED!");
			}
		}
		
		public void CourtSelected() {
			Debug.Log("Court Start");
			GameData.loadScreenTarget =  gameScreenName;
			GameData.loadCourt = true;
			GameData.loadCourtTarget = GameData.instance.courts[index].courtScene;
			Application.LoadLevel(loadScreenName);
			//NEED TO DEVELOPE TEAM RANDOMIZER FUNCTION
		}
		
		public void WorldTourSelected() {


			Debug.Log("Court Start");
			GameData.loadScreenTarget =  "GameScreen";
			GameData.loadCourt = true;
			GameData.loadCourtTarget = "AustraliaAdditive2";
			//GameData.loadCourtTarget = GameData.instance.worldTour[index].courtScene;
			//GameData.instance.computerBallers = GameData.instance.worldTour[index].teamPlayers;
			//GameData.instance.computerStarters = GameData.instance.worldTour[index].teamStarters;
			//GameData.instance.computerJersey = GameData.instance.worldTour[index].teamJerseyColor;
			//GameData.instance.computerShorts = GameData.instance.worldTour[index].teamShortsColor;
			//GameData.instance.computerShoes = GameData.instance.worldTour[index].teamShoesColor;
			//GameData.instance.computerLogo = GameData.instance.worldTour[index].teamLogo;
			//GameData.instance.computerHomeTeam = GameData.instance.worldTour[index].isHomeTeam;
			//GameData.instance.playerHomeTeam = !GameData.instance.worldTour[index].isHomeTeam;
			Application.LoadLevel(loadScreenName);
		}
		public void WorldTourSelected(int index) {
			
			bigImg.GetComponent<Image>().sprite = sourceButtons [index].image.sprite;
			courtName.text = sourceButtons [index].GetComponentInChildren<Text> ().text;
			Debug.Log("Court Start");
			GameData.loadScreenTarget =  "GameScreen";
			GameData.loadCourt = true;
			GameData.loadCourtTarget = sourceButtons [index].name;
			//GameData.loadCourtTarget = GameData.instance.worldTour[index].courtScene;
			//GameData.instance.computerBallers = GameData.instance.worldTour[index].teamPlayers;
			//GameData.instance.computerStarters = GameData.instance.worldTour[index].teamStarters;
			//GameData.instance.computerJersey = GameData.instance.worldTour[index].teamJerseyColor;
			//GameData.instance.computerShorts = GameData.instance.worldTour[index].teamShortsColor;
			//GameData.instance.computerShoes = GameData.instance.worldTour[index].teamShoesColor;
			//GameData.instance.computerLogo = GameData.instance.worldTour[index].teamLogo;
			//GameData.instance.computerHomeTeam = GameData.instance.worldTour[index].isHomeTeam;
			//GameData.instance.playerHomeTeam = !GameData.instance.worldTour[index].isHomeTeam;

		}

		public void loadLevel()
		{
			Application.LoadLevel(loadScreenName);
		}
	}
}