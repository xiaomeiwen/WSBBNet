using UnityEngine;
using System;
using System.Collections;
using WSBB.StateMachineBase;

namespace WSBB {
	public enum PauseState {PauseMenu, ScoreInfo, QuarterInfo, BenchMenu, TeamIntro}

	public class GameAdmin : MonoBehaviour, ISMObject {

		private bool DebugModeActive = false;		//DebugMode
		public static GameStateMachine stateMachine;

		public String menuScreen;

		public int guiDrawOrder;

		/// <summary>
		/// The mojo threshold. Only for Testing. Make it static for release.
		/// </summary>
		[SerializeField]private int mojoThreshold = 10;

		public static bool GamePause = true;
		public bool changePause = false;
		public bool addQuarter = false;
		public bool clearTime = false;

		public PauseState state;

		public Rect pauseButtonRect;
		public Rect pauseMenuBackground;
		public Rect pauseMenuResumeRect;
		public Rect pauseMenuQuitRect;
		public Rect scoreMenuBackground;
		public Rect homeScoreRect;
		public Rect awayScoreRect;
		public Rect wordScoreRect;
		public Rect quarterEndRect;
		public Rect continueCenterRect;
		public Rect continueRightRect;
		public Rect benchMenuTitle;
		public Rect benchMenuLeftRect;
		public Rect benchMenuBackground;
		public Rect benchMenuPlayerButton;
		public Rect benchMenuPortrait;
		public Rect benchMenuPosLabel;
		public int benchMenuXOffset;
		public int benchMenuYOffset;
		public Rect benchMenuExit;


		public Team playerTeam;

		public int firstBenchIndex;

		public String scoreWord;
		public String quarterEndWord;
		public String continueWord;
		public String benchMenuWord;
		public String benchMenuExitWord;

		public static GameAdmin instance;

		public float scoreMenuDelay;
		public float displayDelay;

		#region ISMObject functions
		public void Initialize(StateMachineBase.StateMachine stateMachine) {}
		
		// Update is called once per frame
		public void Update() {
			
			//??? Do we need this?
			//DebugModeActive = GameData.instance.DebugModeActive;
			
			if(changePause) {
				changePause = false;
//				GameAdmin.GamePause = !GameAdmin.GamePause;
			}
			
			if(addQuarter) {
				addQuarter = false;
				Scoreboard.quarterNumber += 1;
				if(Scoreboard.quarterNumber > 4) {
					Scoreboard.quarterNumber = 4;
				}
			}
			
			if(clearTime) {
				clearTime = false;
				Scoreboard.gameTime = 0;
			}
			
			if(GameAdmin.GamePause)
				Time.timeScale = 1;
			else
				Time.timeScale = 1;
			
			if(Scoreboard.gameTime <= 0 && !GameAdmin.GamePause) {
				changePause = true;
				state = PauseState.QuarterInfo;
			}

			stateMachine.Update(this);
		}

		public bool ChangeState(State newState) {
			return true;
		}
		
		public void PlayAnimation(string animationName, float fadeTime) {}
		
		public void Cleanup() {}

		#endregion

		void Awake() {
			GameAdmin.instance = this;
			stateMachine = new GameStateMachine(this);
			stateMachine.Initialize ();
			firstBenchIndex = -1;
			//GameAdmin.GamePause = true;
			if(Screen.height != ScreenHelpers.idealScreenHeight) {
				pauseButtonRect = ScreenHelpers.scaleRectToScreen(pauseButtonRect);
				pauseMenuBackground = ScreenHelpers.scaleRectToScreen(pauseMenuBackground);
				pauseMenuResumeRect = ScreenHelpers.scaleRectToScreen(pauseMenuResumeRect);
				pauseMenuQuitRect = ScreenHelpers.scaleRectToScreen(pauseMenuQuitRect);
				scoreMenuBackground = ScreenHelpers.scaleRectToScreen(scoreMenuBackground);
				homeScoreRect = ScreenHelpers.scaleRectToScreen(homeScoreRect);
				awayScoreRect = ScreenHelpers.scaleRectToScreen(awayScoreRect);
				wordScoreRect = ScreenHelpers.scaleRectToScreen(wordScoreRect);
				quarterEndRect = ScreenHelpers.scaleRectToScreen(quarterEndRect);
				continueCenterRect = ScreenHelpers.scaleRectToScreen(continueCenterRect);
				continueRightRect = ScreenHelpers.scaleRectToScreen(continueRightRect);
				benchMenuLeftRect = ScreenHelpers.scaleRectToScreen(benchMenuLeftRect);
				benchMenuBackground = ScreenHelpers.scaleRectToScreen(benchMenuBackground);
				benchMenuPlayerButton = ScreenHelpers.scaleRectToScreen(benchMenuPlayerButton);
				benchMenuPortrait = ScreenHelpers.scaleRectToScreen(benchMenuPortrait);
				benchMenuXOffset *= (int) (Screen.height / (1.0 * ScreenHelpers.idealScreenHeight));
				benchMenuYOffset *= (int) (Screen.height / (1.0 * ScreenHelpers.idealScreenHeight));
				benchMenuExit = ScreenHelpers.scaleRectToScreen(benchMenuExit);
			}

			//Change MojoThreshold
			GameData.ChangeOption(SaveTarget.MojoThreshold, mojoThreshold);
		}

		// Use this for initialization
		void Start () {

		}


		public void OnGUI() {
			if(!GameAdmin.GamePause) {
				if(GUI.Button(pauseButtonRect, "Pause")) {
					changePause = true;
					state = PauseState.PauseMenu;
				}
			} else {
				GUI.depth = guiDrawOrder;
				GUI.Box(new Rect(-5,-5, Screen.width + 10, Screen.height + 10), "");
				//GUI.Box(new Rect(-5,-5, Screen.width + 10, Screen.height + 10), "");
				switch(state) {
					case PauseState.PauseMenu:
						PauseMenuGUI();
						break;
					case PauseState.BenchMenu:
						GameAdmin.GamePause = false;
						//BenchGUI();
						break;
					case PauseState.ScoreInfo:
						ScoreGUI();
						break;
					case PauseState.TeamIntro:
						TeamIntroGUI();
						break;
					case PauseState.QuarterInfo:
						GameAdmin.GamePause = false;
						//QuarterOver();
						break;
				}
			}
		}

		public void TeamIntroGUI() {
			GUI.Box(scoreMenuBackground, "");
			GUI.Box(homeScoreRect, Scoreboard.instance.teamManager.homeTeam.teamLogo);
			GUI.Box(awayScoreRect, Scoreboard.instance.teamManager.awayTeam.teamLogo);
			GUI.Box(quarterEndRect, scoreWord);
			if(GUI.Button(continueCenterRect, continueWord)) {
				Scoreboard.shotTime = 45;
				changePause = true;
			}
		}

		public void BenchGUI() {
			/*
			Rect benchMenuBackground;
			Rect benchMenuPlayerButton;
			Rect benchMenuPortrait;
			Rect benchMenuXOffset;
			Rect benchMenuYOffset;
			Rect benchMenuChange;
			Rect benchMenuExit;
			*/
			GUI.Box(benchMenuBackground, "");
			GUI.Box(benchMenuBackground, "");
			Rect tempRect; 
			for(int i = 0; i < 6; i++) {
				tempRect = benchMenuPlayerButton;
				if(i > 2) {
					tempRect.x += benchMenuXOffset * (i - 3);
					tempRect.y += benchMenuYOffset;
				} else {
					tempRect.x += benchMenuXOffset * i;
				}
				if(i == firstBenchIndex) {
					GUI.BeginGroup(tempRect);
					tempRect.x = 0;
					tempRect.y = 0;
					GUI.Box(tempRect, "Player");
					GUI.Label(benchMenuPortrait, GameBallers.instance.ballers[GameData.instance.teamBallers[i]].mugshot);
					GUI.EndGroup();
				} else {
					GUI.BeginGroup(tempRect);
					tempRect.x = 0;
					tempRect.y = 0;
					if(GUI.Button(tempRect, "Player")) {
						if(firstBenchIndex == -1) {
							firstBenchIndex = i;
						} else {
							int firstStarter = -1;
							int secondStarter = -1;
							for(int j = 0; j < 3; j++) {
								if(firstBenchIndex == GameData.instance.teamStarters[j])
									firstStarter = j;
								else if(i == GameData.instance.teamStarters[j])
									secondStarter = j;
							}
							if(firstStarter != -1 && secondStarter != -1) {
								GameData.instance.teamStarters[firstStarter] = i;
								GameData.instance.teamStarters[secondStarter] = firstBenchIndex;
							} else if(firstStarter != -1) {
								GameData.instance.teamStarters[firstStarter] = i;
							} else if(secondStarter != -1) {
								GameData.instance.teamStarters[secondStarter] = firstBenchIndex;
							}
							firstBenchIndex = -1;
						}
					}
					if(GameBallers.instance) {

						//Debug Settings:
						if (GameData.instance.DebugModeActive)
						{
							//GameData.instance.teamBallers[i]

							for (int index=0; index<6; ++index)
							{
								GameData.instance.teamBallers[index] = index;
								PlayerPrefs.SetInt("Team_P" + index, GameData.instance.teamBallers[index]);
							}

							for (int index=0; index<3; ++index)
							{
								GameData.instance.teamStarters[index] = index;
								PlayerPrefs.SetInt("Starter_" + index, GameData.instance.teamStarters[index]);
							}

						}

						GUI.Label(benchMenuPortrait, GameBallers.instance.ballers[GameData.instance.teamBallers[i]].mugshot);
						if(i ==  GameData.instance.teamStarters[0])
							GUI.Label(benchMenuPosLabel, "Point");
						else if(i ==  GameData.instance.teamStarters[1])
							GUI.Label(benchMenuPosLabel, "Wing");
						else if(i ==  GameData.instance.teamStarters[2])
							GUI.Label(benchMenuPosLabel, "Center");
					}
					GUI.EndGroup();
				}				
			}
			if(GUI.Button(benchMenuExit, benchMenuExitWord)) {
				//Debug.Log("Reloading Players");
				for(int k = 0; k < 3; k++) {
					playerTeam.bballers[k].SkinAndData.fetchData();
					playerTeam.bballers[k].SkinAndData.refreshUVPos();
				}
				changePause = true;
			}
		}

		public void ScoreGUI() {
			GUI.Box(scoreMenuBackground, "");
			GUI.Box(homeScoreRect, Scoreboard.instance.teamManager.homeTeam.score + "");
			GUI.Box(awayScoreRect, Scoreboard.instance.teamManager.awayTeam.score + "");
			GUI.Box(quarterEndRect, scoreWord);
			if(GUI.Button(continueRightRect, continueWord)) {
				Scoreboard.shotTime = 45;
				changePause = true;
			}
			if(GUI.Button(benchMenuLeftRect, benchMenuWord)) {
				Scoreboard.shotTime = 45;
				state = PauseState.BenchMenu;
				firstBenchIndex = -1;
			} 
		}

		public void QuarterOver() {
			GUI.Box(scoreMenuBackground, "");
			GUI.Box(homeScoreRect, Scoreboard.instance.teamManager.homeTeam.score + "");
			GUI.Box(awayScoreRect, Scoreboard.instance.teamManager.awayTeam.score + "");
			GUI.Box(quarterEndRect, quarterEndWord);
			if(GUI.Button(continueRightRect, continueWord)) {
				Scoreboard.quarterNumber += 1;
				//CODE TO END GAME
				Scoreboard.gameTime = GameData.quarterLength  * 60;
				Scoreboard.shotTime = 45;
				changePause = true;

            }
			if(Scoreboard.quarterNumber < 5) {
				if(GUI.Button(benchMenuLeftRect, benchMenuWord)) {
					Scoreboard.quarterNumber += 1;
					Scoreboard.gameTime = GameData.quarterLength  * 60;
					Scoreboard.shotTime = 45;
					state = PauseState.BenchMenu;
					firstBenchIndex = -1;
				}
			}
		}

		public void PauseMenuGUI() {
			GUI.Box(pauseMenuBackground, "Pause Menu");
			if(GUI.Button(pauseMenuResumeRect, "RESUME")) {
				GameAdmin.GamePause = false;
			}
			if(GUI.Button(pauseMenuQuitRect, "QUIT")) {
				Time.timeScale = 1;
				GameAdmin.GamePause = false;
				GameData.loadScreenTarget = menuScreen;
				GameData.loadCourt = false;	
				Application.LoadLevel("LoadScreen");
			}
		}
	}
}