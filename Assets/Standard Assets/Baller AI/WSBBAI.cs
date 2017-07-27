using UnityEngine;
using System.Collections;
using System.Linq;
using System;
using WSBB.StateMachineBase;

//using System.Reflection;

/*
   WSBBAI script: game object, methods and supporting classes.  If loading from CSV/XML/database, any changes to headings, or to the range of permitted cell formulas/values should also be changed here (and vice versa).  Cell value changes within permitted ranges do not need to be addressed by development team.

   The script runs for each AI baller over a given match, and is updated regularly during gameplay (not necessarily every frame).  
   
   At the start of a match, the full range of factors, suggestions, and each factor's influence on each suggestion are loaded.  
   
   At every update, the factor values are polled from the game.  These include state machine values for: Game, Team, Opposing Team, Dribbler, and Player (baller).
   
   The factor values are calculated to get the strength of all permitted suggestions, and the strongest one is chosen.
   
   There are two constant mitigating factors (could be variables in later versions).  These are continuity: current strategy or tactic is favored), and chaos: the "right" suggestion may not be followed
   
   Commit 9/18/15: 
        -Added stats to suggestions spreadsheet and formulas
        -Added teammate / opponent stats knowledge level (constant factor for now).  These require identification of various ballers besides the one whose AI this is: dribbler, pass recipient, closest offenseman, closest defenseman, closest defenseman to dribbler, closest defenseman to pass recipient
   MHS TODO:
        -Add factor hierarchy to certain suggestion formulas.  V2?
*/

namespace WSBB
{
    //Game object
    public class WSBBAI : MonoBehaviour
    {
        //VARIABLES
        string importFile;
		ArrayList factorContainers;
        public Team myTeam;
        public BBaller myBaller;
        //There can be two suggestions (i.e., state machine changes) every update: team strategy and player tactics
		ArrayList teamSuggestions;
        ArrayList playerSuggestions;

        #region CSV Parsing Variables
		ArrayList factorTypes;
        ArrayList ordinals;
        SuggestionType sType;
        SuggestionName sName;
        #endregion
        #region Constant factor variables for suggestion selection
        //Constant factor values for any suggestion choice
        double teamChaosFactor = .1;
        double teamContinuityFactor = 4;
        double playerChaosFactor = .1; // was 0.2
        double playerContinuityFactor = 1.5;
        double totalTeamSuggestionStrength = 0;
        double totalPlayerSuggestionStrength = 0;
        double teammateStatsKnowledgeLevel = .9;
        double opponentStatsKnowledgeLevel = .7;
        double normalizationFactor = 1.1;

        #endregion
        #region Output
        SuggestionName newTeamSuggestion;
        SuggestionName newPlayerSuggestion;
        Data data;
        #endregion
        /* 
    Enumerations:
    	for states and factor and suggestion variables  
	*/
        #region Enumerations
        enum FactorType
        {
            GAME_STATE,
            GAME_VARIABLE,
            TEAM_STATE,
            OPPOSING_TEAM_STATE,
            DRIBBLER_STATE,
            PLAYER_STATE,
            STATS
        }
	;

        enum GameState
        {
            QUARTER_START,
            PLAYER_ON_OFFENSE,
            PLAYER_ON_DEFENSE,
            BALL_IN_AIR_PASS,
            BALL_IN_AIR_SHOT,
            BALL_ON_RIM,
            BALL_IN_BASKET,
            PAUSE_OR_TIMEOUT
        }
	;

        enum GameVariable
        {
            DISTANCE_DRIBBLER_BASKET,
            DISTANCE_BALL_BASKET,
            DRIBBLER_SPEED,
            TOTAL_DISTANCE_DEFENSE_DRIBBLER,
            SHOT_CLOCK_TIME,
            GAME_TIME
        }
	;

        enum TeamState
        {
            OFFENSE,
            DEFENSE,
            SPREAD,
            PICK,
            RUSH,
            BASKET_GUARD,
            ZONE_GUARD,
            BALLER_GUARD
        }
	;

        enum OpposingTeamState
        {
            OFFENSE,
            DEFENSE,
            SPREAD,
            PICK,
            RUSH,
            BASKET_GUARD,
            ZONE_GUARD,
            BALLER_GUARD
        }
	;

        enum DribblerState
        {
            PASS,
            CLOWN,
            DUNK,
            SHOOT,
            REBOUND,
            IDLE,
            MOVE
        }
	;

        enum PlayerState
        {
            STEAL,
            BLOCK,
            GUARD,
            INTERCEPT,
            REMAIN_OPEN,
            RECEIVE,
            STAG_LEAP,
            PASS,
            CLOWN,
            DUNK,
            SHOOT,
            REBOUND,
            IDLE,
            MOVE
        }
	;

        public enum SuggestionType
        {
            TEAM_OFFENSE,
            TEAM_DEFENSE,
            PLAYER_DEFENSE,
            PLAYER_OFFENSE,
            PLAYER_DRIBBLER,
            PLAYER_GENERAL
        }
	;

        public enum SuggestionName
        {
            SPREAD,
            PICK,
            RUSH,
            BASKET_GUARD,
            ZONE_GUARD,
            BALLER_GUARD,
            STEAL,
            BLOCK,
            GUARD,
            INTERCEPT,
            REMAIN_OPEN,
            RECEIVE,
            STAG_LEAP,
            PASS,
            CLOWN,
            DUNK,
            SHOOT,
            REBOUND,
            IDLE,
            MOVE
        }
	;

        enum RowType
        {
            FACTOR_TYPE,
            FACTOR,
            MIN,
            MAX,
            SUGGESTION_TYPE,
            SUGGESTION
        }
	;
        #endregion


        /*
    Helper classes:
		Data: Interface class for accessing WSBBAI funtional game data dependencies

	    FactorContainer: Container class for suggestion factor values
	    Factor: Class for factor value per each suggestion
	    Suggestion: Suggestion Class Definition
    */
        #region DataClassDefinition
        //dummy data class
        ////NData
        //Team TODO: These states and variables should be input from baller and other game objects.
        //Currently all states are in enums, and all variables are doubles
        class Data
        {

            public WSBB.Team_Manager teamManager;
            public WSBB.Team ballerTeam;
            public WSBB.Team opposingTeam;
            public WSBB.BBaller baller;
            public WSBB.BBaller dribbler;

            //Added to match sets of stats in spreadsheet
            public WSBB.BBaller passRecipient;
            public WSBB.BBaller closestOffenseman;
            public WSBB.BBaller closestDefenseman;
            public WSBB.BBaller closestDefensemanToDribbler;
            public WSBB.BBaller closestDefensemanToPassRecipient;

            #region StateMachines
            //Game
            private GameStateMachine gameStateMachine;
            //Teams
            private TeamStateMachine teamStateMachine;
            private TeamStateMachine opposingTeamStateMachine;
            //Ballers
            private BBallerStateMachine ballerStateMachine;
            private BBallerStateMachine dribblerStateMachine;
            //Ball
            private BallStateMachine ballStateMachine;
            #endregion

            #region States
            public string POSSESSION;
            public string GAME_STATE;
            public string TEAM_STATE;
            public string OPPOSING_TEAM_STATE;
            public string PLAYER_STATE;
            public string DRIBBLER_STATE;
            public string STATS;

            #endregion

            #region GameStats
            public double GAME_TIME;
            public double SHOT_CLOCK_TIME;
            //public double	DRIBBLER_SPEED;
            public double DISTANCE_DRIBBLER_BASKET;
            public double TOTAL_DISTANCE_DEFENSE_DRIBBLER;
            public double DISTANCE_BALL_BASKET;

            #endregion


            //Match between current suggestion and previous suggestion?
            public SuggestionName previousTeamSuggestion;
            public SuggestionName previousPlayerSuggestion;

            public Data()
            {
            }//blank constructor
            //Debug Note:: note that issues may arise if this is called before relevant values are set
            public Data(WSBB.BBaller baller) //Constructor
            {
                this.baller = baller;
                ballerTeam = baller.MyTeam;
                //baller.GetComponent<SharkTankRessurection.BBaller>()
                ballerStateMachine = this.baller.ballerAI.stateMachine;                 //!//  I wan
                //Debug.Log("NAME: " + baller.name);
                teamStateMachine = ballerTeam.stateMachine;
                //Debug.Log(teamStateMachine);
                teamManager = GameObject.Find("Team_Manager").GetComponent<Team_Manager>();

                if ((this.ballerTeam).pureAI)
                { //if this is the pure AiTeam 
                    opposingTeam = teamManager.TeamPlayer;
                }
                else
                {
                    opposingTeam = teamManager.TeamAI;
                }
                if (opposingTeam.stateMachine.CurrentState == null)
                    Debug.LogError("OpposingTeam statemachine currentstate is null");
                opposingTeamStateMachine = opposingTeam.stateMachine;
                gameStateMachine = WSBB.GameAdmin.stateMachine;

                Update();
            }

            /*!
     * \brief Updates the data values for all dynamic data variables
     */
            //TODO: optimize. This doesn't need to be called every frame*/

            public void Update()
            {
                
                //TODO: nothing is changing the team's current state!
				if (teamStateMachine.CurrentState == null) {
					teamStateMachine.CurrentState = teamStateMachine.DefaultState;
				}
                TEAM_STATE = teamStateMachine.CurrentState.StateName;
                OPPOSING_TEAM_STATE = opposingTeamStateMachine.CurrentState.StateName;
                PLAYER_STATE = ballerStateMachine.CurrentState.StateName;
                SHOT_CLOCK_TIME = WSBB.Scoreboard.shotTime;
                GAME_TIME = WSBB.Scoreboard.gameTime;

                if (gameStateMachine.CurrentState == null)
                {
                    Debug.Log("GameStateMachine.current state == null");
                    GAME_STATE = gameStateMachine.DefaultState.StateName;
                }
                else {
                    if (baller.MyTeam.isOffense)
                        GAME_STATE = "PLAYER_ON_OFFENSE";
                    else
                        GAME_STATE = "PLAYER_ON_DEFENSE";
                    //GAME_STATE = gameStateMachine.CurrentState.StateName;
                }
				
                //if someone possesses the ball 
                if (true /*GAME_STATE == "PLAYER_ON_DEFENSE" || GAME_STATE == "PLAYER_ON_OFFENSE"*/)
                {
                    //update dribbler: last active baller: only do after Ball changes state
                    //team on offense
                    if (baller.MyTeam.isOffense)
                    {//my team possesses the ball
                        dribbler = ballerTeam.getBallerWithBall();
                    }
                    else
                    { //other team possesses the ball
                        dribbler = opposingTeam.getBallerWithBall();
                    }
                    //Debug.Log(dribbler.firstName);
                    dribblerStateMachine = dribbler.ballerAI.stateMachine;
                    DRIBBLER_STATE = dribblerStateMachine.CurrentState.StateName;
                    //DRIBBLER_SPEED = dribbler.ActualMoveSpeed;
                    DISTANCE_DRIBBLER_BASKET = getDistanceBetweenDribblerAndBasket(false);
                    TOTAL_DISTANCE_DEFENSE_DRIBBLER = (baller.MyTransform.position - dribbler.MyTransform.position).magnitude;
                    //Debug.Log(baller.firstName + "-TDDD: " + TOTAL_DISTANCE_DEFENSE_DRIBBLER);
                    

                }

                DISTANCE_BALL_BASKET = getShortestBallGoalDistance(false);
                //Debug.Log(baller.firstName + " team state: " + TEAM_STATE);
                if (baller.MyTeam.isOffense)
                    POSSESSION = "PLAYER_ON_OFFENSE";
                else
                    POSSESSION = "PLAYER_ON_DEFENSE";

                /*PlayerRoles:-------Get current player roles as factors for stats methods
                 */
                getPlayerRoles();
            }

            /*!
     * \brief calculates and returns the distance between this player and the dribbler
     * ASSUMPTION: The distance is meant to be between the dribbler and their target basket
     * \param heightUsedInCalc true if we are including height in the calculation
     * \return the distance to the dribbler
     */
            private float getDistanceBetweenDribblerAndBasket(bool heightUsedInCalc)  //changed meaning of "dribbler" to just mean "this baller"
            {
                Vector3 distanceVector;
                //distanceVector = dribbler.MyTransform.position - opposingTeam.basketTarget.position;
                distanceVector = baller.MyTransform.position - ballerTeam.basketTarget.position;
                //Debug.Log("Dribbler position: " + dribbler.MyTransform.position + "...Target basket pos: " + opposingTeam.basketTarget.position);

                if (!heightUsedInCalc)
                {
                    distanceVector.y = 0;
                }

                return distanceVector.magnitude;
            }


            /*!
 * get stats based on role and relative target
 */
            private void getPlayerRoles()
            {
                /*
                  These methods determine which baller is in which
                  role at the time of update.
                         
                  These functions could also help in the state machine mini-AIs.  
                 */
                passRecipient = baller.MyTeam.getPassRecipient(baller);
                closestOffenseman = baller.MyTeam.getClosestOffenseman(baller);
                closestDefenseman = baller.MyTeam.getClosestDefenseman(baller);
				if (dribbler == null) {
					Debug.Log ("dribller is null!!");
				}
                closestDefensemanToDribbler = baller.MyTeam.getClosestDefenseman(dribbler);
                closestDefensemanToPassRecipient = baller.MyTeam.getClosestDefenseman(passRecipient);
            }




            /*!
     * \brief calculates and returns the shortest distance between the ball and a goal
     * SpecialCase: if distance is equal we assume the ball is heading to the player's 
     * goal net because I assue the player will mostly be beating the AI
     * \param heightUsedInCalc true if we are including height in the calculation
     * \return the distance to the nearest goal
     */
            private float getShortestBallGoalDistance(bool heightUsedInCalc)
            {
                Vector3 ball = WSBB.Ball.currentPosition;
                /* 
                Vector3 aiGoalHoop = teamManager.TeamAI.basketTarget.position;
                 Vector3 playerHoop = teamManager.TeamPlayer.basketTarget.position;

                 if (!heightUsedInCalc)
                 {
                     ball.y = 0;
                     aiGoalHoop.y = 0;
                     playerHoop.y = 0;
                 }
                 //now we set distance to the closest hoop
                 float ballAiGoalDistance = (aiGoalHoop - ball).magnitude;
                 float ballPlayerGoalDistance = (playerHoop - ball).magnitude;

                 return ballPlayerGoalDistance <= ballAiGoalDistance ? ballPlayerGoalDistance : ballAiGoalDistance;
                 */
                Vector3 hoopPosition;
                if (baller.MyTeam.isOffense)
                    hoopPosition = baller.MyTeam.basketTarget.position;
                else
                    hoopPosition = opposingTeam.basketTarget.position;

                if (!heightUsedInCalc)
                {
                    ball.y = 0;
                    hoopPosition.y = 0;
                }

                return (ball - hoopPosition).magnitude;
            }

            private SuggestionName getEquivalentSuggestionEnum(string state)
            {
                string[] suggestionNames = System.Enum.GetNames(typeof(SuggestionName));
                for (int i = 0; i < suggestionNames.Length; i++)
                {
                    if (suggestionNames[i].Equals(state))
                    {
                        return (SuggestionName)i;
                    }
                }
                //Debug.Log("ERROR: No Enum match");
                return SuggestionName.IDLE;
            }


        }
        #endregion
        #region FactorContainer
        //Container class for suggestion factor values
        class FactorContainer
        {
            public FactorType factorType;
            public string factorName;
            public string value;
            public ArrayList factors = new ArrayList();

            //for bookkeeping
            public int ordinal;

            //Some factors have min and max values that impact calculation.  There may be other variables involved in later versions.
            public double min;
            public double max;

            //Method for calculating impacts from factor values
            public void calculateImpacts(WSBBAI ai)
            {
                double d;
                foreach (Factor f in factors)
                {
                    /* 
        Currently, there are only a few types of impact relations:
           1)  "X" for contradictory factor + suggestion
           2)  Numerical values for state impacts
           3)  Linear/Exponential relationship (direct or inverse, with varying degree)
           4)  Special case stats factor relationships (currently 6 of them: A-F)
                    
     */
                    if (f.impactRelation.Equals("X"))
                    {
                        f.impact = "X";
                    }
                    else if (double.TryParse(f.impactRelation, out d))
                    {
                        f.impact = f.impactRelation;
                    }
                    else if (f.impactRelation.Length > 1)
                    {
                        //Debug.Log(this.factorName + "--" + f.suggestionName + ": " + f.impactRelation);
                        f.impact = f.parseRelationship(this, ai);
                        //slight hack for now - single letters for special stats equations
                    }
                    else
                    {
                        Debug.Log("Herrreeee!!!");
                        f.impact = f.parseStatsRelationship(f.impactRelation, ai);
                    }
                }
            }
        }
        #endregion
        #region Factor
        //Class for factor value per each suggestion
        class Factor
        {
            /*
        MHS TODO: For next iteration, factors can impact each other.  For each suggestion, each factor value would have parent/child factor values,  and an evaluation function to calculate influences.  Initialization will use reflection on CSV file input strings for evaluation functions.
    */
            public SuggestionName suggestionName;
            public string impactRelation;
            public string impact;

            public string parseRelationship(FactorContainer fc, WSBBAI ai)
            {
                string returnValue;

                //The pieces of the impact relation. 
                //MHS TODO make this piece less hacky
                
                double strength = Convert.ToDouble((int)impactRelation.ElementAt(0)) - '0';
                char type = impactRelation.ElementAt(1);
                double exponent = Convert.ToDouble((int)impactRelation.ElementAt(2)) - '0';

                
                //with normalization
                switch (type)
                {
                    //direct
                    case 'D':
                        returnValue = Convert.ToString(ai.normalizationFactor * Math.Pow(strength * ((Convert.ToDouble(fc.value) - fc.min) / (fc.max - fc.min)), exponent));
                        break;
                    //inverse
                    case 'I':
                        returnValue = Convert.ToString(ai.normalizationFactor * Math.Pow(strength * ((fc.max - Convert.ToDouble(fc.value)) / (fc.max - fc.min)), exponent));
                        break;
                    default:
                        returnValue = "";
                        break;
                }
                //Debug.Log(suggestionName + ": " + returnValue);
                return returnValue;
            }
            #region StatsRelationships
            public string parseStatsRelationship(String rel, WSBBAI ai)
            {
                //These cases are in order per spreadsheet
                string returnValue;
                switch (rel[0])
                {
                    case 'A':
                        returnValue = stealStatImpact(ai);
                        break;
                    case 'B':
                        returnValue = guardStatImpact(ai);
                        break;
                    case 'C':
                        returnValue = passStatImpact(ai);
                        break;
                    case 'D':
                        returnValue = dunkStatImpact(ai);
                        break;
                    case 'E':
                        returnValue = shootStatImpact(ai);
                        break;
                    case 'F':
                        returnValue = powerupStatImpact(ai);
                        break;
                    default:
                        returnValue = "";
                        break;
                }
                Debug.Log("Return value - WSBBAI.parseRelationship: " + returnValue);
                return returnValue;

            }

            /* here are the stats based equations from the spreadsheet.  They're all
             * unique but there are only a few so they are explicitly handled in code
             */
            //Attempt Steal?
            string stealStatImpact(WSBBAI ai)
            {
                string returnValue;
                //baller speed and power
                //dribbler agility and power, +/- opponent knowledge level gap
                //include shot clock time 
                Debug.Log(ai.data.baller.ballerStat.getStealBase() + " - " + knowledgeGap(ai.data.dribbler.ballerStat.getHandlingBase(), ai.opponentStatsKnowledgeLevel) + " - " + (ai.data.SHOT_CLOCK_TIME / 0.24).ToString());
                returnValue = (ai.data.baller.ballerStat.getStealBase() - knowledgeGap(ai.data.dribbler.ballerStat.getHandlingBase(), ai.opponentStatsKnowledgeLevel) - (ai.data.SHOT_CLOCK_TIME / 0.24)).ToString();

                return returnValue;
            }

            //Man to Man Defense?  Note: not just on change of possession
            string guardStatImpact(WSBBAI ai)
            {
                string returnValue;
                //baller agility and power
                //closest defenseman agility and power, +/- opponent knowledge level gap
                //include distance to basket
                returnValue = ((ai.data.baller.ballerStat.getGuardBase() - knowledgeGap(ai.data.closestOffenseman.ballerStat.getHandlingBase(), ai.opponentStatsKnowledgeLevel)) - (ai.data.DISTANCE_BALL_BASKET * 2)).ToString();

                return returnValue;
            }

            //Shot on Goal?
            string shootStatImpact(WSBBAI ai)
            {
                string returnValue;
                //baller agility and speed
                //closest defenseman agility and power, +/- opponent knowledge level gap
                //include distance to basket and shot clock time
                returnValue = (ai.data.baller.ballerStat.getShootBase() - (Math.Pow(ai.data.DISTANCE_BALL_BASKET, (1 + (ai.data.baller.ballerStat.getShootBase() / 140))) - knowledgeGap(ai.data.closestDefenseman.ballerStat.getBlockBase(), ai.opponentStatsKnowledgeLevel) + (24 - ai.data.SHOT_CLOCK_TIME))).ToString();
                return returnValue;
            }


            //Pass?  NOTE: this returns a NEGATIVE impact towards passing (i.e., keep the ball if high)
            //MHS: Should knowledge of pass recipient's (and their guard's) stats affect this impact?
            string passStatImpact(WSBBAI ai)
            {
                string returnValue;
                //baller agility and power
                //closest defenseman agility and power, +/- opponent knowledge level gap
                //include shot clock time, twice for some reason
                returnValue = (-((ai.data.baller.ballerStat.getHandlingBase() - knowledgeGap(ai.data.closestDefenseman.ballerStat.getGuardBase(), ai.opponentStatsKnowledgeLevel)) - (ai.data.SHOT_CLOCK_TIME / 0.24) - (24 - ai.data.SHOT_CLOCK_TIME))).ToString();
                return returnValue;
            }

            //Drive to Basket (aka dunk)
            /*MHS TODO should there be a difference between dunk and layup here?
             *If so, should knowledge of own mojo be a factor in decision to dunk?  Should equation be completely different 
             *for dunk?  Also we need a stag leap equation.
             */
            string dunkStatImpact(WSBBAI ai)
            {
                string returnValue;
                //baller agility and speed
                //closest defenseman agility and power, +/- opponent knowledge level gap
                //include distance to basket 
                returnValue = ((ai.data.baller.ballerStat.getDriveBase() - knowledgeGap(ai.data.closestDefenseman.ballerStat.getGuardBase(), ai.opponentStatsKnowledgeLevel)) - (ai.data.DISTANCE_BALL_BASKET * 5)).ToString();
                return returnValue;
            }

            //MHS TODO need an equation for powerup use
            string powerupStatImpact(WSBBAI ai)
            {
                string returnValue = "0";

                return returnValue;
            }

            double knowledgeGap(double stat, double gap)
            {
                System.Random rand = new System.Random();
                //bit hacky but probably won't need to go past 2 decimal places for gap
                int intGap = 100 * (int)(1 - gap);
                double range = rand.Next(-1 * intGap, intGap);
                double gapImpact = stat + (stat * range / 100);

                return gapImpact;
            }
            #endregion
        }
        #endregion
        #region Suggestion
        //Suggestion
        class Suggestion
        {
            public SuggestionType suggestionType;
            public SuggestionName suggestionName;
            public bool invalid = false;
            public double rawStrength = 0;
            public int roundedStrength = 0;
        }
        #endregion

        /*
	MonoBehavior Methods:
		Start: called for initialization
		Update: called on each monobehavior update
	*/
        #region Start
        void Awake()
        {
            /*
        When match starts, read in CSV file (or use other method) to inform suggestions.  If using CSV input file, factor type is row 0, factor name is row 1.  Then, parse each body row (2+) and create new factor value per suggestion: suggestion type is column 0, suggestion name is column 1.
    */
            
            
            importFile = System.IO.File.ReadAllText(Application.dataPath + "/importFile.csv");
            ReadCSVFile();
        }
        #endregion
        #region Update
        public void UpdateSuggestion(BBaller theBaller)
        {
            //Clear everything
//            if(data == null)
//            {
//                if(myBaller != null)
//                {
//                    data = new Data(myBaller);
//                }
//            }

            Initialize();
            
			if (theBaller != null) { //IF BALLER
                //Debug.Log("updating: im a baller!  " + theBaller.HasBall);
				//update the data used in calculations
				if (data == null) {
					data = new Data (theBaller);
				}
				data.Update ();
				//Note: the following routine could be called after N Unity updates, as opposed to every time
				CalculateFactors ();
				//Check for player tactical state change using suggestion model
				PlayerTactics ();
			} else { //IF TEAM
               // Debug.Log("updating: im a team!");
                //Note: the following routine could be called after N Unity updates, as opposed to every time
                if (data == null) {
					data = new Data (myTeam.bballers [0]);
				}
				data.Update ();
				CalculateFactors ();
				//Check for team strategy change using suggestion model
				TeamStrategy();
			}
        }
        #endregion

        /* 
	START() HELPER METHODS
		ReadCSVFile: Standard text parse
			ReadCSVFile's helper methods: ParseFactorTypeRow,ParseFactorRow, ParseMinRow, ParseMaxRow, ParseSuggestionRow
			
        XML or database would make parsing easier on developers.
        It might also eventually need a routine for non-technical designers to be able to use.
    */
        #region ReadCSVFile
        //Standard text parse
        void ReadCSVFile()
        {
            //Debug.Log("File is " + importFile.Length);
            string[] rows = importFile.Split("\n"[0]);
			teamSuggestions = new ArrayList ();
			playerSuggestions = new ArrayList ();

            RowType row = RowType.FACTOR_TYPE;

            for (int i = 0; i < rows.Length; i++)
            {
                switch (row)
                {
                    case RowType.FACTOR_TYPE:
                        ParseFactorTypeRow(rows[i]);
                        row = RowType.FACTOR;
                        break;
                    case RowType.FACTOR:
                        ParseFactorRow(rows[i]);
                        row = RowType.MIN;
                        break;
                    case RowType.MIN:
                        ParseMinRow(rows[i]);
                        row = RowType.MAX;
                        break;
                    case RowType.MAX:
                        ParseMaxRow(rows[i]);
                        row = RowType.SUGGESTION;
                        break;
                    case RowType.SUGGESTION:
                        ParseSuggestionRow(rows[i]);
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion
        /*
	ReadCSVFile's Helper Methods:
			-These methods parse the various row types in the CSV, and 
		    populate factor containers with factors and suggestions. 
				ParseFactorTypeRow:--Get all types of factors
	            ParseFactorRow:------Set up factor container, assigning appropriate type from previous row
	            ParseMinRow:---------(description = null)
	            ParseMaxRow:---------(delcription = null)
	            ParseSuggestionRow:--Suggestions and suggestion factors
	  */
        #region ParseFactorTypeRow
        //Get all types of factors
        void ParseFactorTypeRow(string line)
        {
            string[] elements = line.Split(","[0]);
			factorTypes = new ArrayList ();
			ordinals = new ArrayList ();

            for (int i = 0; i < elements.Length; i++)
            {
                if (!elements[i].Trim().Equals(""))
                {
                    FactorType factorType = (FactorType)Enum.Parse(typeof(FactorType), elements[i]);
					//if (factorTypes == null)
					//	Debug.Log ("factorTypes is null!!");
					//if (factorType == null)
					//	Debug.Log ("factorType is null!!");
                    factorTypes.Add(factorType);
                    //Debug.Log(factorType.ToString());
                    ordinals.Add(i);
                }
            }
        }
        #endregion
        #region ParseFactorRow
        //Set up factor container, assigning appropriate type from previous row
        void ParseFactorRow(string line)
        {
            string[] elements = line.Split(","[0]);
			factorContainers = new ArrayList ();

            for (int i = 0; i < elements.Length; i++)
            {
                if (!elements[i].Trim().Equals(""))
                {
                    FactorContainer fc = new FactorContainer();
                    factorContainers.Add(fc);
                    fc.ordinal = i;
                    
                    for (int j = 0; j < ordinals.Count; j++)
                    {
                        if (fc.ordinal >= (int)ordinals[j])
                        {
                            fc.factorType = (FactorType)factorTypes[j];
                        }
                    }
                    fc.factorName = elements[i];
                    
                }
            }
        }
        #endregion
        #region ParseMinRow
        void ParseMinRow(string line)
        {
            string[] elements = line.Split(","[0]);

            for (int i = 0; i < elements.Length; i++)
            {
                if (!elements[i].Trim().Equals(""))
                {
                    int min = Convert.ToInt16(elements[i]);
                    foreach (FactorContainer fc in factorContainers)
                    {
                        if (fc.ordinal == i)
                        {
                            fc.min = min;
                            break;
                        }
                    }
                }
            }
        }
        #endregion
        #region ParseMaxRow
        void ParseMaxRow(string line)
        {
            string[] elements = line.Split(","[0]);

            for (int i = 0; i < elements.Length; i++)
            {
                if (!elements[i].Trim().Equals(""))
                {
                    int max = Convert.ToInt16(elements[i]);
                    foreach (FactorContainer fc in factorContainers)
                    {
                        if (fc.ordinal == i)
                        {
                            fc.max = max;
                            break;
                        }
                    }
                }
            }
        }
        #endregion
        #region ParseSuggestionRow
        //Suggestions and suggestion factors
        void ParseSuggestionRow(string line)
        {
            string[] elements = line.Split(","[0]);

            for (int i = 0; i < elements.Length; i++)
            {
                if (!elements[i].Trim().Equals(""))
                {
                    switch (i)
                    {
                        case 0:
                            sType = (SuggestionType)Enum.Parse(typeof(SuggestionType), elements[i]);
                            break;
                        case 1:
                            sName = (SuggestionName)Enum.Parse(typeof(SuggestionName), elements[i]);
                            Suggestion s = new Suggestion();
                            s.suggestionType = sType;
                            s.suggestionName = sName;
                            if ((sType == SuggestionType.TEAM_OFFENSE) || (sType == SuggestionType.TEAM_DEFENSE))
                            {
                                teamSuggestions.Add(s);
                                //Debug.Log("adding a team suggestion: " + s.suggestionName);
                            }
                            else
                            {
                                playerSuggestions.Add(s);
								//Debug.Log("adding a player suggestion: " + s.suggestionName);
                            }
                            break;
                        default:
                            foreach (FactorContainer fc in factorContainers)
                            {
                                if (fc.ordinal == i)
                                {
                                    Factor f = new Factor();
                                    f.suggestionName = sName;
                                    f.impactRelation = elements[i];
                                    fc.factors.Add(f);
                                }
                            }
                            break;
                    }
                }
            }
        }
        #endregion

        /* 
	UPDATE() HELPER METHODS
			Initialize:--------Clear the previous update's Suggestions 
			CalculateFactors:--(description = null)
            TeamStrategy:------Method for team suggestion choice //helper method: DetermineResult()
			PlayerTactics:-----Method for player suggestion choice//helper method: DetermineResult()
            
	*/
        #region Initialize
        //Clear the previous update's Suggestions 
        void Initialize()
        {
            totalTeamSuggestionStrength = 0;
            totalPlayerSuggestionStrength = 0;
            if (teamSuggestions != null)
            {
                foreach (Suggestion s in teamSuggestions)
                {
                    s.invalid = false;
                    s.rawStrength = 0;
                    s.roundedStrength = 0;
                }
            }

            if (playerSuggestions != null)
            {
                foreach (Suggestion s in playerSuggestions)
                {
                    s.invalid = false;
                    s.rawStrength = 0;
                    s.roundedStrength = 0;
                }
            }
        }
        #endregion
        #region CalculateFactors
        void CalculateFactors()
        {
            /* 
        1. Check for X cases for factor vs. suggestion. If value is True, set suggestion strength 0 and continue
        2. Get values for other suggestions
            a. Add total numerical values together into suggestion point value
            b. Run direct and inverse relationship equations and add to suggestion point value
                1) Linear--> proportion from min-max: from -10 to 10
                2) Exponential--> normalize proportion from -10 to 10
        3. Concatenate all suggestions with more than 0 points and normalize to % ranges, incorporating continuity factor.  
        4. Random % (+/- chaos factor) chooses suggestion.
    */

            //Traverse current states and variable values
            foreach (FactorContainer fc in factorContainers)
            {
                switch (fc.factorType)
                {
					case (FactorType.GAME_STATE):
						//TODO: game state is null! - declared as current state of team
                        fc.value = data.GAME_STATE.ToString();
                        break;
                    case (FactorType.GAME_VARIABLE):
                        if (fc.factorName.Equals(GameVariable.DISTANCE_DRIBBLER_BASKET.ToString()))
                        {
                            fc.value = data.DISTANCE_DRIBBLER_BASKET.ToString();
                            //Debug.Log("-Total distance dribbler-basket: " + data.DISTANCE_DRIBBLER_BASKET.ToString());
                        }
                        else if (fc.factorName.Equals(GameVariable.DISTANCE_BALL_BASKET.ToString()))
                        {
                           //Debug.Log("Total distance defence-dribbler: " + data.TOTAL_DISTANCE_DEFENSE_DRIBBLER.ToString());
                            fc.value = data.DISTANCE_BALL_BASKET.ToString();
                        } /*else if (fc.factorName.Equals (GameVariable.DRIBBLER_SPEED.ToString ())) {
										fc.value = data.DRIBBLER_SPEED.ToString ();
								}*/
                        else if (fc.factorName.Equals(GameVariable.TOTAL_DISTANCE_DEFENSE_DRIBBLER.ToString()))
                        {
                           // if(!myTeam.isOffense) // only checking defense for now
                               //Debug.Log("-Total distance defence-dribbler: " + data.TOTAL_DISTANCE_DEFENSE_DRIBBLER.ToString());
                            fc.value = data.TOTAL_DISTANCE_DEFENSE_DRIBBLER.ToString();

                        }
                        else if (fc.factorName.Equals(GameVariable.SHOT_CLOCK_TIME.ToString()))
                        {
                            fc.value = data.SHOT_CLOCK_TIME.ToString();
                        }
                        else if (fc.factorName.Equals(GameVariable.GAME_TIME.ToString()))
                        {
                            fc.value = data.GAME_TIME.ToString();
                        }
                        break;
                    case (FactorType.TEAM_STATE):
                        fc.value = data.TEAM_STATE.ToString();
                        break;
                    case (FactorType.OPPOSING_TEAM_STATE):
                        fc.value = data.OPPOSING_TEAM_STATE.ToString();
                        break;
                    case (FactorType.DRIBBLER_STATE):
                        if (data.DRIBBLER_STATE == null)///skip this for the initialization, DRIBBLER_STATE is set in update.
                            break;
                        fc.value = data.DRIBBLER_STATE.ToString();
                        break;
                    case (FactorType.PLAYER_STATE):
                        fc.value = data.PLAYER_STATE.ToString();
                        break;

                    //Stats
                    case (FactorType.STATS):
                        //MHS TODO: Major player, major stat check
                        fc.value =

                            fc.value = data.STATS.ToString();
                        break;
                    default:
                        break;
                }
                fc.calculateImpacts(this);
            }
        }
        #endregion
        #region TeamStrategy
        //Function for team suggestion choice 
        void TeamStrategy()
        {
            foreach (Suggestion s in teamSuggestions)
            {
                foreach (FactorContainer fc in factorContainers)
                {
                    foreach (Factor f in fc.factors)
                    {
                        if (f.suggestionName == s.suggestionName)
                        {
                            if (f.impact.Equals("X"))
                            {
                                s.invalid = true;
                                break;
                            }
                            else
                            {
                                s.rawStrength += Convert.ToDouble(f.impact);
                            }

                            if (f.suggestionName == data.previousTeamSuggestion)
                            {
                                s.rawStrength *= teamContinuityFactor;
                            }

                            s.roundedStrength = (int)s.rawStrength;
                            totalTeamSuggestionStrength += s.roundedStrength;
                        }
                    }
                }
                //Debug.Log(s.suggestionName + ": " + s.roundedStrength);
            }
            DetermineResult(true);
        }
        #endregion
        #region PlayerTactics
        //Method for player suggestion choice
        void PlayerTactics()
        {
            foreach (Suggestion s in playerSuggestions)
            {
                //Debug.Log(s.suggestionName + "***");
                foreach (FactorContainer fc in factorContainers)
                {
                    //Debug.Log("fc.value = " + fc.value);
                    foreach (Factor f in fc.factors)
                    {
                        if (f.suggestionName == s.suggestionName)
                        {
                            if (f.impact.Equals("X"))
                            {
                                if (fc.value == fc.factorName) 
                                {
                                    s.invalid = true;
                                    continue;
                                }
                            }
                            else
                            {
                                s.rawStrength += Convert.ToDouble(f.impact);
                            }

                            if (f.suggestionName == data.previousPlayerSuggestion)
                            {
                                s.rawStrength *= playerContinuityFactor;
                            }
                            
                            s.roundedStrength = (int)s.rawStrength;
                            totalPlayerSuggestionStrength += s.roundedStrength;

                        }
                    }
                }

              //  if (!s.invalid /*&& myBaller.MyTeam.isOffense*/ && myBaller.firstName == "p3") 
                    //Debug.Log(s.suggestionName + ": " + s.roundedStrength);
            }
            DetermineResult(false);
        }
        #endregion

        /* 
	TeamStrategy()'s && PlayerTactics()'s shared helper Methods
			DetermineResult:--Helper function for determining team strategy and player tactics
	*/
        #region DetermineResult
        void DetermineResult(bool isTeam)
        {
            double chaoFac;
            double totalStrength;
            ArrayList suggestions;
            SuggestionName newSuggestion;
            
            if (isTeam)
            {
                //newSuggestion = data.previousTeamSuggestion;
                if (myTeam == null)
                    Debug.Log("myTeam is null");
                newSuggestion = myTeam.CurrentSuggestion;
                chaoFac = teamChaosFactor;
                totalStrength = totalTeamSuggestionStrength;
                suggestions = teamSuggestions;
            }
            else
            {
                newSuggestion = data.previousPlayerSuggestion;
                chaoFac = playerChaosFactor;
                totalStrength = totalPlayerSuggestionStrength;
                suggestions = playerSuggestions;
            }

            System.Random rnd = new System.Random();
            double check = rnd.NextDouble() * totalStrength;
            //int counterCheck = 0;
            double chaosCheck = rnd.Next(0, 100);
            chaosCheck = chaosCheck / 100.0f;
           if (playerChaosFactor > chaosCheck)
          // if(false)                                //Ignore random chance until more suggestions are implemented  -- Adam
            {
               // Debug.Log("Chaos Factor! Choosing random state for " + myBaller.firstName);
               // Debug.Log("chaoFac = " + playerChaosFactor + " :: Chaos = " + chaosCheck);
                bool validRandomChoice = false;
                while (!validRandomChoice)
                {
                    int randomSuggestion = rnd.Next(0, suggestions.Count);
                    Suggestion suggestion = (Suggestion)suggestions[randomSuggestion];
                    if (IsValidSuggestion(suggestion))
                    {
                        newSuggestion = suggestion.suggestionName;
                        validRandomChoice = true;
                    }
                }
            }
            else
            {
                int highest = 0;
                foreach (Suggestion s in suggestions)
                {

                    if (!IsValidSuggestion(s))
                    {
                        continue;
                    }

                    if (s.roundedStrength > highest)
                    {
                        highest = s.roundedStrength;
                        newSuggestion = s.suggestionName;
                    }
                }
            }


            //Final suggestion choice made

            /* TEAM TODO These are the outputs to the state machines.  Each strategy / tactic has its own details and directives, which we can work on.  The other serious piece of work is to populate the spreadsheet (or XML file, etc.) with data that balances out the suggestions realistically: in other words, a lot of playtesting.
     */
            if (isTeam)
            {
                newTeamSuggestion = newSuggestion;
                //Debug.Log(newTeamSuggestion.ToString());
            }
            else
            {
                newPlayerSuggestion = newSuggestion;
                //Debug.Log(newPlayerSuggestion.ToString());
            }
        }


        //** Ignore invalid suggestions based on baller status (offense/defense/dribbler) **//
        private bool IsValidSuggestion(Suggestion s)
        {
            //bool isBlockSuggestion = false;
            //if(s.suggestionName == SuggestionName.BLOCK)
            //{
            //    isBlockSuggestion = true;
            //}

            if (s.invalid)
                return false;
            else if (s.suggestionType.Equals(SuggestionType.PLAYER_OFFENSE) && !myBaller.MyTeam.isOffense)
            {
                return false;
            }
            else if (s.suggestionType.Equals(SuggestionType.PLAYER_DEFENSE) && myBaller.MyTeam.isOffense)
            {
                return false;
            }
            else if (s.suggestionType.Equals(SuggestionType.PLAYER_DRIBBLER) && !myBaller.HasBall)
            {
                return false;
            }
            else if (s.suggestionName == SuggestionName.STAG_LEAP && myBaller.HasBall)
                return false;

            return true;
        }
        #endregion

        public SuggestionName GetCurrentPlayerSuggestion()
        {
            return newPlayerSuggestion;
        }

        public SuggestionName GetCurrentTeamSuggestion()
        {
            return newTeamSuggestion;
        }
    }
}