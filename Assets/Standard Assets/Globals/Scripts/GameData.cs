using UnityEngine;
using System;
using System.Collections;

namespace WSBB {
	public enum SaveTarget{TeamColors, PlayerNames, TeamName, TeamIcon, SoundVolume, MusicVolume, QuarterLength, MojoThreshold, TeamLayout, Starters, WorldTour, Courts, ItemConfig, ItemUnlock, BallerUnlock, BlingAmount, UniformUnlock, LogoUnlock, DunkUnlock, TeamDunk, Level}

	public class GameData : MonoBehaviour {
		public static GameData instance;
		public static bool present = false;
		//public static GameItems itemList;
		public static String loadScreenTarget;
		public static bool loadCourt;
		public static String loadCourtTarget;
		public static Matrix4x4 gameGUIMatrix;
		public static Array lastScreen;
		
		public static int quarterLength;
		public static int MojoThreshold;

		[HideInInspector]public bool DebugModeActive = false;		//DebugMode
		
		public float dunkRange;
        public float stagLeapRange;
		public String teamName;
		public int teamLogo;
		public int teamMoney;
		public int teamLevel;
		public int teamExp;
		public int [] teamBallers;
		public int [] teamStarters;
		public int teamJersey;
		public int teamShorts;
		public int teamShoes;
		public int teamJerseyAway;
		public int teamShortsAway;
		public int teamShoesAway;
		public int teamDunk;
		public bool playerHomeTeam;
		public int [] computerBallers;
		public int [] computerStarters;
		public int computerJersey;
		public int computerShorts;
		public int computerShoes;
		public int computerLogo;
		public String computerTeamName;
		public bool computerHomeTeam;
		public CourtItem [] courts;
		public CourtItem [] worldTour;
		
		public int courtToUnlock;
		public int worldTourToUnlock;
		public bool unlockNextWorldTour;
		
		public Texture2D playerTileBackdrop;
		public Texture2D playerTileShotBar;
		public Texture2D playerTilePassBar;
		public Texture2D playerTileStealBar;
		public Texture2D playerTileBlockBar;
		
		public Texture2D playerBarBackdrop;
		public Texture2D playerBarCover;
		public Texture2D playerBarShotBar;
		public Texture2D playerBarPassBar;
		public Texture2D playerBarStealBar;
		public Texture2D playerBarBlockBar;
		
		private bool itemsLoaded;
		
		void Awake() {
			//DontDestroyOnLoad(this.gameObject);
			instance = this;
			instance.DebugModeActive = (PlayerPrefs.GetInt("DebugMode") != 0) ? true : false;

			//Debug.Log("Game Data being set to instance");
			loadCourt = false;
			PlayerTile.Init();
			PlayerBars.Init();
			//var scaleValues : Vector3; 
			//scaleValues.x = Screen.width / 480.0;
			//scaleValues.y = Screen.height / 320.0;
			//scaleValues.z = 1;
			//gameGUIMatrix.SetTRS(Vector3.zero, Quaternion.identity, scaleValues);
		}

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
			if(!itemsLoaded && SharkTankRessurection.GameItems.instance) {
				GameData.init();
				GameData.present = true;
				itemsLoaded = true;
			}
		}

		public static void init() {
			if(PlayerPrefs.HasKey("SaveCreated")) {
				load();
			} else {
				create();
				load();
			}
		}
		
		public static void create() {
			PlayerPrefs.SetInt("Version Number", 1);
			PlayerPrefs.SetInt("SaveCreated", 1);
			PlayerPrefs.SetString("TeamName", "Ballers");
			PlayerPrefs.SetInt("TeamLogo", 0);
			PlayerPrefs.SetInt("TeamDunk", 0);
			PlayerPrefs.SetInt("JerseyColor", 0);
			PlayerPrefs.SetInt("ShortsColor", 0);
			PlayerPrefs.SetInt("ShoesColor", 0);
			PlayerPrefs.SetInt("JerseyColorAway", 0);
			PlayerPrefs.SetInt("ShortsColorAway", 0);
			PlayerPrefs.SetInt("ShoesColorAway", 0);
			for(int i = 0; i < 6; i++)
				PlayerPrefs.SetInt("Team_P" + i, -1);
			for(int j = 0; j < 3; j++)
				PlayerPrefs.SetInt("Starter_" + j, -1);
			PlayerPrefs.SetInt("SoundVolume", 5);
			PlayerPrefs.SetInt("MusicVolume", 5);
			PlayerPrefs.SetInt("QuarterLength", 5);
			PlayerPrefs.SetInt("MojoThreshold", 10);
			PlayerPrefs.SetInt("BlingAmount", 100);
			PlayerPrefs.SetInt("TeamExp", 0);
			PlayerPrefs.SetInt("TeamLevel", 1);
			saveCourtLocks();
			savePlayerLocks();
			SharkTankRessurection.GameItems.instance.saveItemConfig();
			SharkTankRessurection.GameItems.instance.saveItemUnlock();
			GameUniforms.instance.saveUniformUnlock();
			GameLogo.instance.saveLogoUnlocks();
			GameDunks.instance.saveDunkUnlock();
			PlayerPrefs.Save();
		}
		
		public static void load() {
			GameData.instance.teamJersey = PlayerPrefs.GetInt("JerseyColor");
			GameData.instance.teamShorts = PlayerPrefs.GetInt("ShortsColor");
			GameData.instance.teamShoes = PlayerPrefs.GetInt("ShoesColor");
			GameData.instance.teamJerseyAway = PlayerPrefs.GetInt("JerseyColorAway");
			GameData.instance.teamShortsAway = PlayerPrefs.GetInt("ShortsColorAway");
			GameData.instance.teamShoesAway = PlayerPrefs.GetInt("ShoesColorAway");
			GameData.instance.teamName = PlayerPrefs.GetString("TeamName");
			GameData.instance.teamLogo = PlayerPrefs.GetInt("TeamLogo");
			GameData.instance.teamDunk = PlayerPrefs.GetInt("TeamDunk");
			GameSounds.effectVolume = PlayerPrefs.GetInt("SoundVolume");
			GameSounds.gameMusicVolume = PlayerPrefs.GetInt("MusicVolume");
			GameData.quarterLength = PlayerPrefs.GetInt("QuarterLength");
			GameData.MojoThreshold = PlayerPrefs.GetInt ("MojoThreshold");
			GameData.instance.teamMoney = PlayerPrefs.GetInt("BlingAmount");
			GameData.instance.teamExp = PlayerPrefs.GetInt("TeamExp");
			GameData.instance.teamLevel = PlayerPrefs.GetInt("TeamLevel");
			for(int i = 0; i < 6; i++)
				loadPlayerBaller(i);
			for(int j = 0; j < 3; j++)
				GameData.instance.teamStarters[j] = PlayerPrefs.GetInt("Starter_" + j);
			
			loadCourtLocks();
			loadPlayerLocks();
			SharkTankRessurection.GameItems.instance.loadItemUnlock();
			SharkTankRessurection.GameItems.instance.loadItemConfig();
			GameUniforms.instance.loadUniformUnlock();
			GameLogo.instance.loadLogoUnlocks();
			GameDunks.instance.loadDunkUnlock();
		}
		
		public static void save(SaveTarget saveTarget) {
			int debug = (int)saveTarget;
			switch(saveTarget) {
				case SaveTarget.TeamColors:
					PlayerPrefs.SetInt("JerseyColor", GameData.instance.teamJersey);
					PlayerPrefs.SetInt("ShortsColor", GameData.instance.teamShorts);
					PlayerPrefs.SetInt("ShoesColor", GameData.instance.teamShoes);
					PlayerPrefs.SetInt("JerseyColorAway", GameData.instance.teamJerseyAway);
					PlayerPrefs.SetInt("ShortsColorAway", GameData.instance.teamShortsAway);
					PlayerPrefs.SetInt("ShoesColorAway", GameData.instance.teamShoesAway);
					break;
				case SaveTarget.TeamName:
					PlayerPrefs.SetString("TeamName", GameData.instance.teamName);
					break;
				case SaveTarget.TeamIcon:
					PlayerPrefs.SetInt("TeamLogo", GameData.instance.teamLogo);
					break;
				case SaveTarget.TeamDunk:
					PlayerPrefs.SetInt("TeamDunk", GameData.instance.teamDunk);
					break;
				case SaveTarget.Level:
					PlayerPrefs.SetInt("TeamExp", GameData.instance.teamExp);
					PlayerPrefs.SetInt("TeamLevel", GameData.instance.teamLevel);
					break;
				case SaveTarget.SoundVolume:
					PlayerPrefs.SetInt("SoundVolume", (int) GameSounds.effectVolume);
					break;
				case SaveTarget.MusicVolume:
					PlayerPrefs.SetInt("MusicVolume", (int) GameSounds.gameMusicVolume);
					break;
				case SaveTarget.QuarterLength:
					PlayerPrefs.SetInt("QuarterLength", GameData.quarterLength);
					break;
				case SaveTarget.MojoThreshold:
					PlayerPrefs.SetInt("MojoThreshold", GameData.MojoThreshold);
					break;
				case SaveTarget.TeamLayout:
					for(int i = 0; i < 6; i++) {
						savePlayerBaller(i);
					}
					break;
				case SaveTarget.Starters:
					for(int j = 0; j < 3; j++)
						PlayerPrefs.SetInt("Starter_" + j, GameData.instance.teamStarters[j]);
					break;
				case SaveTarget.Courts:
					GameData.saveCourtLocks();
					break;
				case SaveTarget.ItemConfig:
				SharkTankRessurection.GameItems.instance.saveItemConfig();
					break;
				case SaveTarget.ItemUnlock:
				SharkTankRessurection.GameItems.instance.saveItemUnlock();
					break;
				case SaveTarget.BallerUnlock:
					GameData.savePlayerLocks();
					break;
				case SaveTarget.UniformUnlock:
					GameUniforms.instance.saveUniformUnlock();
					break;
				case SaveTarget.LogoUnlock:
					GameLogo.instance.saveLogoUnlocks();
					break;
				case SaveTarget.DunkUnlock:
					GameDunks.instance.saveDunkUnlock();
					break;
			}
			if(GameData.instance != null)
				PlayerPrefs.SetInt("BlingAmount", GameData.instance.teamMoney);
		}
		
		public static void save(SaveTarget saveTarget, int num) {
			switch(saveTarget) {
				case SaveTarget.TeamLayout:
					if(num >= 0 && num < 6)
						PlayerPrefs.SetInt("Team_P" + num, GameData.instance.teamBallers[num]);
					break;
				case SaveTarget.ItemUnlock:
					SharkTankRessurection.GameItems.instance.saveItemUnlockGroup(num);
					break;
			}
		}
		
		public static void savePlayerBaller(int num) {
			if(num < 0 || num > 5) {
				Debug.LogError("Save Player Number Out Of Range: " + num);
				return;
			}
			PlayerPrefs.SetInt("Team_P" + num, GameData.instance.teamBallers[num]);
		}
		
		public static void loadPlayerBaller(int num) {
			if(num < 0 || num > 5) {
				Debug.LogError("Save Player Number Out Of Range: " + num);
			}
			GameData.instance.teamBallers[num] = PlayerPrefs.GetInt("Team_P" + num);
		}
		
		public static void saveCourtLocks() {
			for(int i = 0; i < GameData.instance.courts.Length; i++) {
				if(GameData.instance.courts[i].unlocked)
					PlayerPrefs.SetInt("Court" + i, 1);
				else
					PlayerPrefs.SetInt("Court" + i, 0);
			}
		}
		
		public static void loadCourtLocks() {
			for(int i = 0; i < GameData.instance.courts.Length; i++) {
				if(PlayerPrefs.GetInt("Court" + i) == 1)
					GameData.instance.courts[i].unlocked = true;
				else
					GameData.instance.courts[i].unlocked = false;
			}
		}
		
		public static void savePlayerLocks() {
			for(int i = 0; i < GameBallers.instance.ballers.Length; i++) {
				if(GameBallers.instance.ballers[i].unlocked)
					PlayerPrefs.SetInt("PlayerLock" + i, 1);
				else
					PlayerPrefs.SetInt("PlayerLock" + i, 0);
			}
		}
		
		public static void loadPlayerLocks() {
			for(int i = 0; i < GameBallers.instance.ballers.Length; i++) {
				if(PlayerPrefs.GetInt("PlayerLock" + i) == 1)
					GameBallers.instance.ballers[i].unlocked = true;
				else
					GameBallers.instance.ballers[i].unlocked = false;
			}
		}
		
		public static void ChangeOption(SaveTarget option, int newValue) {
			if(newValue == -1) {
				return;
			}
			switch(option) {
				case SaveTarget.SoundVolume:
					if(newValue != GameSounds.effectVolume) {
						GameSounds.effectVolume = newValue;
						save(option);
					}
					break;
				case SaveTarget.MusicVolume:
					if(newValue != GameSounds.gameMusicVolume) {
						GameSounds.gameMusicVolume = newValue;
						save(option);
					}
					break;
				case SaveTarget.QuarterLength:
					if(newValue != quarterLength) {
						quarterLength = newValue;
						save(option);
					}
					break;
				case SaveTarget.MojoThreshold:
					if(newValue != MojoThreshold) {
						MojoThreshold = newValue;
						save(option);
					}
					break;
			}
		}
		
		public void OnApplicationPause(bool pause) {
			PlayerPrefs.Save();
		}
		
		public void runLevelCheck() {
			if(teamExp >= 100) {
				teamLevel += 1; //Then Do MAX Level Check
				teamExp -= 100;
			}
			GameData.save(SaveTarget.Level);
			PlayerPrefs.Save();
		}
	}
}