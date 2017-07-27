using UnityEngine;
using System.Collections;

namespace WSBB {
	public enum MenuStates{StartScreen, FrontMenu, OptionRoot, OptionSound, OptionGame, LockerRoom, StoreRoot, StoreATM, StoreContent, Uniform, Team, CourtScreen, WorldTour, Transition, TeamGeneral, TeamSelect, ItemSelect}

	public class MainMenuState : MonoBehaviour {
		public static MainMenuState instance;

		public bool worldTourMode;

		public int cameraPathIndex;
		public int testVariable;
		
		public float pathSpeed;
		public float cameraPathPercent;
		
		public Transform [] cameraPath;
		public Transform cameraTransform;

		private MenuStates lastState;
		public MenuStates currentState;
		public MenuStates nextState;

		public StartScreen startScreen;
		public MenuScreen frontMenuScreen;
		public MenuScreen optionRootScreen;
		public OptionSoundScreen optionSoundScreen;
		public OptionGameScreen optionGameScreen;
		public MenuScreen lockerRoomScreen;
		public UniformScreen uniformScreen;
		public GeneralTeamScreen generalTeamScreen;
		public TeamSelectScreen teamSelectScreen;
		public SharkTankRessurection.ItemScreen itemSelectScreen;
		public CourtScreen courtScreen;
		
		void Awake() {
			instance = this;
			cameraPathIndex = 1;
			Time.timeScale = 1;
			
			testVariable = 1;//test submitting
		}

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
			if(currentState == MenuStates.Transition) {
				if(currentState != lastState)
					InitSwitch();
				cameraPathPercent += Time.deltaTime * pathSpeed;
				if(cameraPathPercent > 1)
					cameraPathPercent = 1;
				cameraTransform.position = Vector3.Lerp(cameraPath[cameraPathIndex - 1].position, cameraPath[cameraPathIndex].position, cameraPathPercent);
				cameraTransform.rotation = Quaternion.Lerp(cameraPath[cameraPathIndex - 1].rotation, cameraPath[cameraPathIndex].rotation, cameraPathPercent);
				if(cameraPathPercent == 1) {
					cameraPathIndex++;
					if(cameraPathIndex >= cameraPath.Length) {
						cameraPathIndex = 1;
						currentState = nextState;
						
					}
					cameraPathPercent = 0;
				}
			}
			switch(currentState) {
				case MenuStates.StartScreen:
					startScreen.ScreenUpdate();
					break;
				case MenuStates.FrontMenu:
					frontMenuScreen.ScreenUpdate();
					break;
				case MenuStates.OptionRoot:
					optionRootScreen.ScreenUpdate();
					break;
				case MenuStates.OptionSound:
					optionSoundScreen.ScreenUpdate();
					break;
				case MenuStates.OptionGame:
					optionGameScreen.ScreenUpdate();
					break;
				case MenuStates.LockerRoom:
					lockerRoomScreen.ScreenUpdate();
					break;
				case MenuStates.Uniform:
					uniformScreen.ScreenUpdate();
					break;
				case MenuStates.TeamGeneral:
					generalTeamScreen.ScreenUpdate();
					break;
				case MenuStates.TeamSelect:
					teamSelectScreen.ScreenUpdate();
					break;
				case MenuStates.ItemSelect:
					itemSelectScreen.ScreenUpdate();
					break;
				case MenuStates.CourtScreen:
					courtScreen.ScreenUpdate();
					break;
				default:
					break;
			}
			lastState = currentState;
		}
		
		public void OnGUI() {
			switch(currentState) {
				case MenuStates.StartScreen:
					startScreen.ScreenOnGUI();
					break;
				case MenuStates.FrontMenu:
					frontMenuScreen.ScreenOnGUI();
					break;
				case MenuStates.OptionRoot:
					optionRootScreen.ScreenOnGUI();
					break;
				case MenuStates.OptionSound:
					optionSoundScreen.ScreenOnGUI();
					break;
				case MenuStates.OptionGame:
					optionGameScreen.ScreenOnGUI();
					break;
				case MenuStates.LockerRoom:
					lockerRoomScreen.ScreenOnGUI();
					break;
				case MenuStates.Uniform:
					uniformScreen.ScreenOnGUI();
					break;
				case MenuStates.TeamGeneral:
					generalTeamScreen.ScreenOnGUI();
					break;
				case MenuStates.TeamSelect:
					teamSelectScreen.ScreenOnGUI();
					break;
				case MenuStates.ItemSelect:
					itemSelectScreen.ScreenOnGUI();
					break;
				case MenuStates.CourtScreen:
					courtScreen.ScreenOnGUI();
					break;
				default:
					break;
			}
		}
		
		public void InitSwitch() {
			switch(nextState) {
				case MenuStates.Uniform:
					uniformScreen.Init(true);
					break;
				default:
					break;
			}
		}
	}
}