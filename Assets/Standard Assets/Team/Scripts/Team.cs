using UnityEngine;
using System.Collections;
using WSBB.StateMachineBase;

namespace WSBB {
	public class Team : MonoBehaviour, ISMObject {

		#region VariableDefinitions

		#region Debug
		[HideInInspector]public bool EnableDebugMsgs = true;
		#endregion
		#region DataSetup&Initialization
		private bool loaded = false;
		public TeamDataSource teamDataSource;
		public TeamStateMachine stateMachine;
		public Texture2D teamLogo;
		public bool pureAI;
		public Team otherTeam;
		public CameraArm cameraArm;
		private TeamStat myTeamStat = null;
		private int mojoThreshold = 0;
		private int numberOfPlayers = 3;
		private Team_Manager teamManager = null; 
		#endregion
		#region FuncionalVar
		public string teamName;
		public BBaller [] bballers;
		public int playerBaller;
		public int currentBaller;
		private BBaller lastBallerWithBall;
		public int score;
		public Transform basketTarget;
		public Transform[] offenseTrans;
		public Transform[] defenseTrans;
        public bool isOffense;
		public SpotPicker[] positionPicker;
		public GameObject BallPrefab;		//I believe this is used by the animator

        public WSBBAI decisionAI;
        public WSBBAI.SuggestionName CurrentSuggestion;

		//for level of AI team
		public enum DifficultySettings{EASY,MEDIUM,HARD};
		public DifficultySettings difficulty = DifficultySettings.EASY;
		public double easyDifBase = 0.95;
		public double medDifBase = 1.0;
		public double hardDifBase = 1.1;
		public double difBase = 0.0;
		public double difVar = 0.05;

		//AI focus on power, speed, or agility 
		public PlayerStats AIStatFocus = PlayerStats.POWER;
		public double medAIStatFocusBoost = 0.15;
		public double hardAIStatFocusBoost = 0.3;

		//for powerup random draw
		public int powerupCount = 3;

		#endregion
		#region properties
		public TeamStat TeamStatScript
		{
			get{ return myTeamStat; }
		}
		public int MojoThreshold
		{
			get{ return mojoThreshold; }
		}

		#endregion

		#endregion
		#region ISMObject functions
		public void Initialize(WSBB.StateMachineBase.StateMachine stateMachine) {}
		

		
		public bool ChangeState(State newState) {
			return true;
		}
		
		public void PlayAnimation(string animationName, float fadeTime) {}
		
		public void Cleanup() {}
		
		#endregion

		#region Methods

		#region Initialization

		#region Awake
		public void Awake()
		{
			//setupVars();
		}
		#endregion
		#region Start
		public void Start()
		{
            setupVars();
            StartCoroutine("initDependentVars");

			decisionAI = FindObjectOfType<WSBBAI> ();
		}
		#endregion
		#region initDependentVars
		IEnumerator initDependentVars ()
		{
			while (cameraArm.playerTeam == null)
				yield return null;

			if(cameraArm.playerTeam == this)
			{
				this.resetOffense();
                this.isOffense = true;
				otherTeam.resetDefense();
                otherTeam.isOffense = false;
			}
			
			if (bballers.Length != 0)
				lastBallerWithBall = bballers[0];
			else
			{
				if(EnableDebugMsgs)
					Debug.LogError("Team.cs: BBallers not assigned");
			}

			//Rertrieve MojoThreshold from PlayerPreferences.
			mojoThreshold = PlayerPrefs.GetInt("MojoThreshold");
		}
		#endregion
		#region setupVars
		private void setupVars()
		{
			//StateMachine
			stateMachine = new TeamStateMachine(this);
			stateMachine.Initialize();
			//CameraArm
			cameraArm = GameObject.Find("CameraArm").GetComponent<CameraArm>();
			//OtherTeam
			teamManager = GetComponentInParent<Team_Manager>();
			Team[] teams = null;
			teams = teamManager.GetComponentsInChildren<Team>();
			if (teams[0] == this)
			{
				otherTeam = teams[1];
			}
			else
			{
				otherTeam = teams[0];
			}

			//My TeamStat
			myTeamStat = GetComponent<TeamStat>();

			//Set PositionPicker
			//SpotPicker[] tempPositionPicker = GetComponentsInChildren<SpotPicker>().Length;
			positionPicker = new SpotPicker[4];

			positionPicker[0] = transform.Find("PointMoveGuide").GetComponent<SpotPicker>();
			positionPicker[1] = transform.Find("WingMoveAreas").GetComponent<SpotPicker>();
			positionPicker[2] = transform.Find("CenterMoveAreas").GetComponent<SpotPicker>();
			positionPicker[3] = transform.Find("KeyMoveArea").GetComponent<SpotPicker>();

			//Set Offense/Defense Transforms
			offenseTrans = new Transform[3];
			defenseTrans = new Transform[3];

			Transform[] tempTransforms = null;

			tempTransforms = GetComponentsInChildren<Transform>();

			for (int i=0, cntrOffense = 0, cntrDefense = 0; i<tempTransforms.Length; ++i)
			{
				if (tempTransforms[i].CompareTag("Offense"))
				{
					offenseTrans[cntrOffense++] = tempTransforms[i];
				}
				else if (tempTransforms[i].CompareTag("Defense"))
				{
                    //Debug.Log("adding to defenseTrans");
					defenseTrans[cntrDefense++] = tempTransforms[i];
				}
			}

            //Debug.Log("after adding, defenseTrans size = " + defenseTrans.Length);
			/* MHS set AI ballers with semi-random stats.
			 * assumption: there are already pre-generated AI ballers
			 */
			GameObject[] bballerGO=null;
			GameObject[] bballerStatsOnly=null;

			double pTeamAvgPower = 0.0;
			double pTeamAvgSpeed = 0.0;
			double pTeamAvgAgility = 0.0;

			double AIStatFocusBoost = 0.0;

			if (pureAI)
			{
				bballerGO = GameObject.FindGameObjectsWithTag("AIControlBaller");

				//set stats for AI team 
				bballerStatsOnly = GameObject.FindGameObjectsWithTag("PlayerControlBaller");

				for (int i=0; i<bballerStatsOnly.Length; i++)
				{
					BBaller playerB = bballerStatsOnly[i].GetComponent<BBaller>();
					pTeamAvgPower += playerB.ballerStat.LatestStatList[PlayerStats.POWER];
					pTeamAvgSpeed += playerB.ballerStat.LatestStatList[PlayerStats.SPEED];
					pTeamAvgAgility += playerB.ballerStat.LatestStatList[PlayerStats.AGILITY];
				}
				pTeamAvgPower /= numberOfPlayers;
				pTeamAvgSpeed /= numberOfPlayers;
				pTeamAvgAgility /= numberOfPlayers;

				System.Random rand = new System.Random();
				int randRange = (int)difVar*100;
				double difRange = (double)rand.Next(-1*randRange, randRange);
	
				switch (difficulty){

				/* Easy: This AI would use a balanced team at or below the level of the player 
				 * with some random low level cards. You want to include the cards 
				 * so players have the benefit of seeing how they work. 
				 * Because the cards are random and low level and the team is balanced, 
				 * there are few mis-matches created.
				 */
				case DifficultySettings.EASY:
					difBase = easyDifBase;
					break;

				/* Medium: This AI would use a team that favors a specific Ability 
				 * at or above the level of the player with some mid level cards. 
				 * Because the cards are random and mid level, mismatches are unlikely, 
				 * but the increase in level should challenge the players.
				 */
				case DifficultySettings.MEDIUM:
					difBase = medDifBase;
					AIStatFocusBoost = medAIStatFocusBoost;
					break;
			
				/* Hard: This AI would use a team that favors a specific Ability at or 
				 * above the level of the player with high level cards. This will create 
				 * some mismatches and challenge the player. You want to create a point value 
				 * for each card so it becomes more likely that when randomly choosing cards, 
				 * it is weighted towards cards that complement the Ability archetype. 
				 * For example, if the player is facing a fast team, it should be more 
				 * likely the team will choose cards that enhance Speed.
				 */
				case DifficultySettings.HARD:
					difBase = hardDifBase;
					AIStatFocusBoost = hardAIStatFocusBoost;
					break;
				default:
					break;
				}
				drawTeamCards(difficulty);

				//MHS all 3 AI players created (mostly) equal for now.  
				//Boost given in one basic stat for medium and hard difficulty
				double powerFocusBoost = 0.0;
				double speedFocusBoost = 0.0;
				double agilityFocusBoost = 0.0;

				switch (AIStatFocus){
					case PlayerStats.POWER:
						powerFocusBoost = AIStatFocusBoost;
						break;
					case PlayerStats.SPEED:
						speedFocusBoost = AIStatFocusBoost;
						break;
					case PlayerStats.AGILITY:
						agilityFocusBoost = AIStatFocusBoost;
						break;
					default:
						break;
				}
				/*for (int i=0; i<bballerGO.Length; i++){ //ZACH: what kind of ludicrous mayhem is this?? this has already been done. it is by no means a team responsibility.
					BBaller AIB = bballerGO[i].GetComponent<BBaller>();
					AIB.ballerStat.LatestStatList[PlayerStats.POWER] = (float)pTeamAvgPower * (float)difBase + (float)difRange + (float)powerFocusBoost;
					AIB.ballerStat.LatestStatList[PlayerStats.SPEED] = (float)pTeamAvgSpeed * (float)difBase + (float)difRange + (float)speedFocusBoost;
					AIB.ballerStat.LatestStatList[PlayerStats.AGILITY] = (float)pTeamAvgAgility * (float)difBase + (float)difRange + (float)agilityFocusBoost;
					AIB.playerStat.calculateDerivedStats(this);
				}*/
			}
			else
			{
				bballerGO = GameObject.FindGameObjectsWithTag("PlayerControlBaller");
			}

			bballers = new BBaller[bballerGO.Length];

			for (int i=0; i<bballerGO.Length; ++i)
			{
				bballers[i] = bballerGO[i].GetComponent<BBaller>();
			}

			//Rearranging BBallers according to Index
			BBaller[] tempBallers = new BBaller[3];
			for (int i=0; i<bballers.Length; ++i)
			{
				int ballerIndex = 0;
				if( !int.TryParse(bballers[i].name[bballers[i].name.Length-1].ToString(), out ballerIndex) )
					Debug.LogError("Last Letter of BBaller GameObject name should be an index");

				tempBallers[ballerIndex-1] = bballers[i];
			}
			bballers = tempBallers;

		}
		#endregion
		#endregion

		// Update is called once per frame
		#region Update
		public void Update () {
			
			if (mojoThreshold == 0)
			{
				//Debug.LogError("Team.cs: mojoThreshold has not been set properly.");
				mojoThreshold = 3; //this should be -1, i changed to 3 for testing purposes
			}
			
			if(!loaded && GameData.instance)
			{
				switch(teamDataSource)
				{
				case TeamDataSource.localPlayer:
					teamLogo = GameLogo.instance.logos[GameData.instance.teamLogo].logo;
					break;
				case TeamDataSource.multiPlayer:
					teamLogo = GameLogo.instance.logos[GameData.instance.teamLogo].logo;
					break;
				case TeamDataSource.computer:
					teamLogo = GameLogo.instance.logos[GameData.instance.computerLogo].logo;
					break;
				}
				loaded = true;
			}

            //decisionAI.UpdateSuggestion(null);                       //uncomment out later 6/25
            CurrentSuggestion = decisionAI.GetCurrentTeamSuggestion();
			//Debug.Log ("Team Current suggestion: " + CurrentSuggestion.ToString ());

            switch(CurrentSuggestion)
            {
                case WSBBAI.SuggestionName.BALLER_GUARD:
                    //Assign Positions for Man-to-Man Guard
                    break;
                case WSBBAI.SuggestionName.BASKET_GUARD:
                    //Assign Positions for Basket Guard
                    break;
                case WSBBAI.SuggestionName.PICK:
                    //Assign non-dribbler to screen for dribbler.  This could be complex
                    break;
                case WSBBAI.SuggestionName.RUSH:
                    //Need to Disucss
                    break;
                case WSBBAI.SuggestionName.SPREAD:
                    //Assign Positions - offense
                    break;
                case WSBBAI.SuggestionName.ZONE_GUARD:
                    //Assign Positions to guard specific area - need to elaborate (base on player input?)
                    break;
                default:
                    Debug.Log("Invalid Suggestion Type: " + CurrentSuggestion.ToString());
                    break;
            }
		}
		#endregion
		#region BallerSelection
		//sets currentBaller to next baller
		public void selectNextBaller()
		{
			currentBaller++;
			if(currentBaller >= bballers.Length)
			{
				currentBaller = 0;
			}
		}
		//sets BOTH currentBaller and playerBaller to the next baller
		public void changeToNextBaller()
		{
			currentBaller++;
			if(currentBaller >= bballers.Length)
			{
				currentBaller = 0;
			}
			playerBaller = currentBaller;
		}
		#endregion
		#region SetAsCurrent
		//if the object baller is in the bballer array then
		//ONLY currentBaller will be assigned baller
		public void setAsCurrent(BBaller baller)
		{
			for(int i = 0; i < bballers.Length; i++)
			{
				if(bballers[i] == baller)
				{
					currentBaller = i;
				}
			}	
		}
		#endregion
		#region setAsPlayer
		//if the object baller is in the bballer array then
		//BOTH currentBaller and playerBaller will be assigned baller
		public void setAsPlayer(BBaller baller)
		{
			for(int i = 0; i < bballers.Length; i++)
			{
				if(bballers[i] == baller)
				{
					playerBaller = i;
					currentBaller = i;
                    bballers[i].GetComponentInParent<UnityEngine.AI.NavMeshAgent>().Stop();
                }
                else
                {

                }
			}
		}
		#endregion
		#region ScorePoint
		public void Score(int points)
		{
            //System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
            //Debug.Log(st.ToString());
			score += points;
			otherTeam.resetOffense();
            otherTeam.isOffense = true;
			resetDefense();
            isOffense = false;
		}
		#endregion
		#region ResetOffense
		public void resetOffense()
		{
			for(int i = 0; i < bballers.Length; i++)
			{
				bballers[i].transform.position = offenseTrans[i].position;
				bballers[i].transform.rotation = offenseTrans[i].rotation;
				bballers[i].faceDirection = offenseTrans[i].forward;
				bballers[i].HasBall = false;
                bballers[i].midAction = false;
                bballers[i].ballerAI.midAction = false;
                bballers[i].ballerAI.moveLocSet = false;
                positionPicker[i].lastPos = Vector3.zero; // Set to vector zero so that the ai will know to choose a new offensive position to go to at the start or posession

                // Adjust NavMeshAgent radius to minimize guard getting stuck while trying to get into guard position
                bballers[i].GetComponentInParent<UnityEngine.AI.NavMeshAgent>().radius = 2.5f;
            }
			setAsCurrent(bballers[0]);
			setAsPlayer(bballers[0]);
			bballers[currentBaller].HasBall = true;
			//Debug.Log("Resetting Offense");
		}
		#endregion
		#region ResetDefense
		public void resetDefense()
		{
            FillTrans();
            //Debug.Log("defenseTrans size = " + defenseTrans.Length);
			for(int i = 0; i < bballers.Length; i++)
			{
				bballers[i].transform.position = defenseTrans[i].position;
				bballers[i].transform.rotation = defenseTrans[i].rotation;
				bballers[i].faceDirection = defenseTrans[i].forward;
				bballers[i].HasBall = false;
                bballers[i].midAction = false;
                bballers[i].ballerAI.midAction = false;
                if (positionPicker.Length > 0)
                {
                    //Debug.Log("Resetting position picker last pos");
                    positionPicker[i].lastPos = Vector3.zero;
                }

                // Adjust NavMeshAgent radius to minimize guard getting stuck while trying to get into guard position
                bballers[i].GetComponentInParent<UnityEngine.AI.NavMeshAgent>().radius = 1.75f;
            }
			setAsCurrent(bballers[0]);
			setAsPlayer(bballers[0]);
		}
        #endregion
        #region FillPositionTransforms
        //putting this stuff into its own function - work around for error where defenseTrans has its size being reset to 0
        public void FillTrans()
        {
            //Set Offense/Defense Transforms
            offenseTrans = new Transform[3];
            defenseTrans = new Transform[3];

            Transform[] tempTransforms = null;

            tempTransforms = GetComponentsInChildren<Transform>();

            for (int i = 0, cntrOffense = 0, cntrDefense = 0; i < tempTransforms.Length; ++i)
            {
                if (tempTransforms[i].CompareTag("Offense"))
                {
                    offenseTrans[cntrOffense++] = tempTransforms[i];
                }
                else if (tempTransforms[i].CompareTag("Defense"))
                {
                    //Debug.Log("adding to defenseTrans");
                    defenseTrans[cntrDefense++] = tempTransforms[i];
                }
            }
        }
        #endregion
        #region Getters
        #region GetBallerWithBall
        public BBaller getBallerWithBall()
		{
			for(int i = 0; i < bballers.Length; i++)
			{
				if(bballers[i].HasBall)
				{
					lastBallerWithBall = bballers[i];
				}
			}
			return lastBallerWithBall;
		}
		#endregion
		#region GetCurrentBaller
		public BBaller getCurrentBaller()
		{
			return bballers[currentBaller];	
		}
		#endregion
		#region GetPlayerBaller
		public BBaller getPlayerBaller()
		{
			return bballers[playerBaller];
		}
		#endregion
		#region GetBallerTeamIndex
		public int getBallerTeamIndex(BBaller baller)
		{
			for(int i = 0; i < bballers.Length; i++)
			{
				if(bballers[i] == baller)
				{
					return i;
				}
			}
			return -1;
		}
		#endregion
		#region GetNearestBaller
		public BBaller getNearestBaller(Vector3 pos)
		{
			BBaller closestBaller = bballers[0];
			for(int i = 0; i < bballers.Length; i++)
			{
				closestBaller = Team.closerBaller(pos, closestBaller, bballers[i]);
			}
			return closestBaller;
		}
		#endregion
		#region GetNearestBlockingBaller
		public BBaller getNearestBlockingBaller(Vector3 pos)
		{
			BBaller closestBaller = bballers[0];
			for(int i = 0; i < bballers.Length; i++)
			{
				if(bballers[i].attemptingBlock)
				{
					if(closestBaller.attemptingBlock)
						closestBaller = Team.closerBaller(pos, closestBaller, bballers[i]);
					else
						closestBaller = bballers[i];
				}
			}
			if(closestBaller.attemptingBlock)
				return closestBaller;
			else
				return null;
		}
		#endregion


        //Methods for getting different roles, for stats & suggestions purposes
        #region RoleGetters
        #region GetPassRecipient
        public BBaller getPassRecipient(BBaller baller)
        {
            //For now, use method per BBaller.  MHS TODO: finalize decision for pass recipient in pass state mini-AI  
            return getCurrentBaller();
            
        }
        #endregion

        #region GetClosestOffenseman
        public BBaller getClosestOffenseman(BBaller baller)
        {
            BBaller closestBaller = bballers[0];
			for(int i = 0; i < bballers.Length; i++)
			{
                if (bballers[i].MyTeam.isOffense){
				    closestBaller = Team.closerBaller(baller.MyTransform.position, closestBaller, bballers[i]);
                }
			}
			return closestBaller;
        }
        #endregion

        #region GetClosestDefenseman
        public BBaller getClosestDefenseman(BBaller baller)
        {
            BBaller closestBaller = bballers[0];
			for(int i = 0; i < bballers.Length; i++)
			{
                if (!bballers[i].MyTeam.isOffense){
					closestBaller = Team.closerBaller(baller.MyTransform.position, closestBaller, bballers[i]);
                }
			}
			return closestBaller;
        }
        #endregion

        #endregion
        #endregion

        #region AmICurrentBaller
        public bool amICurrentBaller(BBaller baller)
		{
			return (baller == bballers[currentBaller]);
		}
		#endregion
		#region HasBall
		public bool HasBall()
		{
			for(int i = 0; i < bballers.Length; i++)
			{
				if(bballers[i] && bballers[i].HasBall)
				{
					return true;
				}
			}
			return false;
		}
		#endregion
		#region CloserBaller
		public static BBaller closerBaller(Vector3 pos, BBaller baller1, BBaller baller2 )
		{
			Vector3 vector1 = baller1.MyTransform.position - pos;
			Vector3 vector2 = baller2.MyTransform.position - pos;
			if(vector1.sqrMagnitude > vector2.sqrMagnitude)
			{
				return baller1;
			}
			else
			{
				return baller2;
			}
		}
		#endregion

		#region DrawTeamCards
		public void drawTeamCards(DifficultySettings difficulty){
			//TODO we need to be sure the list created in XMLDataParser script is accessible here
			//Change game object to the correct one once this stuff is refactored
			System.Random r = new System.Random();
			XMLDataParser xmlDataParser = (XMLDataParser)GetComponent("XMLDataParser"); 

            //commented out for testing purposes
            /*
			switch (difficulty){
				case DifficultySettings.EASY:
				for (int i = 0; i < powerupCount; i++){
					int rn = r.Next(1,xmlDataParser.powerUpList.Count);
					Powerup p = (Powerup)xmlDataParser.powerUpList[rn];
					if (p.cardLevel != Powerup.CardLevel.BRONZE){
						i--;
						continue;
					}
					myTeamStat.dunkCards.Add(p);
				}
					break;
				case DifficultySettings.MEDIUM:
					bool gotStatM = false;
					for (int i = 0; i < powerupCount; i++){
						int rn = r.Next(1,xmlDataParser.powerUpList.Count);
						Powerup p = (Powerup)xmlDataParser.powerUpList[rn];

						for  (int j = 0; j < p.attributesAffected.Length; j++){
							if (((PlayerStats)p.attributesAffected[j])== AIStatFocus ){
								gotStatM = true;
							}
						}
						if (p.cardLevel != Powerup.CardLevel.SILVER){
							i--;
							continue;
						}
						else if (i == (powerupCount - 1) && !gotStatM){
							i--;
							continue;
						}
						myTeamStat.dunkCards.Add(p);
					}
					break;
				case DifficultySettings.HARD:
					bool gotStatH = false;
					for (int i = 0; i < powerupCount; i++){
						int rn = r.Next(1,xmlDataParser.powerUpList.Count);
						Powerup p = (Powerup)xmlDataParser.powerUpList[rn];
						
						for  (int j = 0; j < p.attributesAffected.Length; j++){
							if (((PlayerStats)p.attributesAffected[j])== AIStatFocus ){
								gotStatH = true;
							}
						}
						if (p.cardLevel != Powerup.CardLevel.GOLD){
							i--;
							continue;
						}
						else if (i == (powerupCount - 1) && !gotStatH){
							i--;
							continue;
						}
						myTeamStat.dunkCards.Add(p);
					}
					break;
				default:
					break;
			}*/
		}
        
		#endregion
		#endregion
		public Team(){
			bballers = new BBaller[3];
		}
	}

}