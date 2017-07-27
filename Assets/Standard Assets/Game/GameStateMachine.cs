using UnityEngine;
using System.Collections;
using System;

namespace Test {

	public class GameStateMachine : MonoBehaviour {
	/* TODO: answer the following
	 * Components needed for the info needed for the state machines
	 * GameData: quarterLength  (WHAT UNIT IS USED WITH THIS VAR? minutes or seconds
	 * What is the current gametime?
	 * position of BBall hoops?
	 * position of active ball?
	 * does players team possess the ball?
	 */
		public enum GameState{
			PAUSE,
			QUARTER_START,
			// Player In Possession
			PLAYER_ON_OFFENSE, //where player is the human player
			PLAYER_ON_DEFENSE,
			// No Player in Possession
			BALL_IN_AIR,
			BALL_ON_RIM,
			BALL_IN_BASKET
		}
		public GameState activeState;
		//booleans for updating the state Machine
		private bool gamePaused 		= false;
		private bool quarterStart 		= false;
		private bool playerOnOffense 	= true;
		private bool playerOnDefense 	= false;
		private bool ballInBasket 		= false;
		private bool ballOnRim 			= false;
		private bool ballInAir 			= false;
		

		private WSBB.Team playerTeam;	//needed for tracking offense & defense
		private WSBB.Team aiTeam; 		//to simplify playerOnDefense bool calculation

		//constructor
		public GameStateMachine(){
			
		}

		void Start(){
			//quarterStart is always the first state
			quarterStart = true;
			activeState = GameState.QUARTER_START;

			//get team manager component of Team Manager object
			WSBB.Team_Manager teamManager = GameObject.Find ("Team_Manager").GetComponent<WSBB.Team_Manager>();
			playerTeam = teamManager.TeamPlayer;
            aiTeam = teamManager.TeamAI;
		}

		void Update(){
			updateStateBools();
			updateState ();
		}

		/*! 
		 * \brief get the active state in this instance of TeamAIStateMachine
		 * \return a string relresentation of the active state
		 */
		public GameState GetActiveState(){
			return activeState;
		}

		private void updateStateBools(){
			//setting state transition condition booll gamePaused
			gamePaused = WSBB.GameAdmin.GamePause;
			
			//setting state transition condition bool quarterStart
			quarterStart  = (WSBB.Scoreboard.gameTime <= 0);
			quarterStart |= (WSBB.Scoreboard.gameTime >= WSBB.GameData.quarterLength  * 60 );
			
			//setting state transition condition bool playerOnOffense
			//if a player on Player_Team possesses the ball then the player is on offense
			playerOnOffense = playerTeam.HasBall ();
			
			//setting state transition condition bool playerOnDefense
			playerOnDefense = aiTeam.HasBall ();
			
			//setting state transition condition bool ballInAir
			ballInAir  = !(playerOnDefense || playerOnOffense); //if the player is not on offense or defense
			ballInAir &= !quarterStart; 						//and its not the start of the quarter
																//then the ball is airborne
			//setting state transition condition bool ballOnRim
			ballOnRim = isBallOnRim ();
			
			//setting state transition condition bool ballInBasket
			ballInBasket = isBallInBasket ();
			
		}

		private void updateState(){
			
			if (gamePaused) {						//note that this if is only included for clarity/readability
				activeState = GameState.PAUSE;
			}else if (quarterStart) {
				activeState = GameState.QUARTER_START;
			}else if (playerOnOffense) {
				activeState = GameState.PLAYER_ON_OFFENSE;
                playerTeam.isOffense = true;
                aiTeam.isOffense = false;
			}else if (playerOnDefense) {
				activeState = GameState.PLAYER_ON_DEFENSE;
                playerTeam.isOffense = false;
                aiTeam.isOffense = true;
			} // note that order maters on the next three if statements due to "short circuiting"
			else if (ballInBasket) {
				activeState = GameState.BALL_IN_BASKET;
			}else if (ballOnRim) {
				activeState = GameState.BALL_ON_RIM;
			}else if (ballInAir) {
				activeState = GameState.BALL_IN_AIR;
			}else {
				Debug.Log ("Error: all game state bools evaluated to false");
			}
		}

	//BEGIN: TODO
		//Find proper radius values
		//Note: Used in ballOnRim()
		//Note: Used in ballInBasket()
		static float hoopRadius = 1.5f;
		static float ballRadius = 1f;
		static float fudgeFactor = 1.05f;

		static float pecentRadiusInBasketForGoal = 0.75f; 										//Note: Used in ballInBasket() only
		float maxProximityForGoal = (hoopRadius - ballRadius) * fudgeFactor;				//Note: Used in ballInBasket() only
		float maxHeightDifferenceForGoal = ballRadius * (1 - pecentRadiusInBasketForGoal);	//Note: Used in ballInBasket() only
	//END: TODO

		private bool isBallOnRim(){

			bool ballOnRim = false;
			Vector3 ballPos = WSBB.Ball.currentPosition;
			Vector3 hoopPos = getNearestHoopPosition();
			Vector3 hoopToBallVector = ballPos - hoopPos;
			double theta = Math.Atan2 (hoopToBallVector.z, hoopToBallVector.x);//angle on the xz plane from +x-axis

			Vector3 nearestPointOnRim = hoopPos; //we will find the nearest point on rim to the ball position by
			nearestPointOnRim.z += hoopRadius * (float)Math.Sin(theta); //adjust the z value
			nearestPointOnRim.x += hoopRadius * (float)Math.Cos(theta);

			Vector3 rimToBallVector = ballPos - nearestPointOnRim;
			//if the ball is about a ball radius away from the rim its touching the rim
			ballOnRim = rimToBallVector.magnitude < ballRadius * fudgeFactor;

			return ballOnRim;
		}

		private Vector3 getNearestHoopPosition(){
			Vector3 playerTeamHoop = playerTeam.basketTarget.position;
			Vector3 aiTeamHoop = aiTeam.basketTarget.position;
			float distanceToTeamHoop = (playerTeamHoop - WSBB.Ball.currentPosition).magnitude;
			float distanceToAiHoop = (aiTeamHoop - WSBB.Ball.currentPosition).magnitude;
			
			return distanceToTeamHoop <= distanceToAiHoop ? playerTeamHoop : aiTeamHoop;
		}

		private bool isBallInBasket(){
			bool ballInBasket = false;
			Vector3 hoopPos = getNearestHoopPosition();
			Vector3 hoopToBallVector = WSBB.Ball.currentPosition - hoopPos;

			float distanceFromHoop = hoopToBallVector.magnitude;

			bool ballIsWithinScoringSphere = distanceFromHoop <= maxProximityForGoal;
			bool ballIsDeepEnoughInBasket = Math.Abs (hoopToBallVector.y) >= maxHeightDifferenceForGoal;

			if (ballIsWithinScoringSphere && ballIsDeepEnoughInBasket) {
				ballInBasket = true;
			}

			return ballInBasket;
		}


	//	/*! 
	//	 * \brief set the active state in this instance of GameAIStateMachine
	//	 * if calling function attepts to set gameState to the currently active gamestate
	//	 * a value of false is returned
	//	 * \param gs the new game state
	//	 * \return a bool of if the active state was successfully set to ps 
	//	 */
	//	public bool SetActiveState(GameState state){
	//		bool isSuccess = false;
	//		//if state preconditions are valid
	//		if(transitionIsValid(state) ){
	//			activeState = state;
	//			isSuccess = true;
	//		}else{
	//			Debug.Log("Error:Setting GameState failed");
	//		}
	//		return isSuccess;
	//	}
		

		

	// The below check isn't really needed
	//	/*!
	//	 * \brief Returns true if the state transition is valid
	//	 *  a transition is only valid if it is between two different states
	//	 * \param state the state we are attemptinc to transition to
	//	 * \return whether the transition is valid
	//	 */
	//	private bool transitionIsValid(GameState state){
	//		bool isValid = false;
	//		switch (state) {
	//			case GameState.PAUSE:						// valid transitioning from any
	//				isValid = (state != GameState.PAUSE);	// state except itself
	//				break;
	//			case GameState.QUARTER_START:						// valid transitioning from any
	//				isValid = (state != GameState.QUARTER_START);	//  state except itself
	//				break;
	//			case GameState.PLAYER_ON_OFFENSE:						// valid transitioning from any
	//				isValid = (state != GameState.PLAYER_ON_OFFENSE);	// state except itself
	//				break;
	//			case GameState.PLAYER_ON_DEFENSE:						// valid transitioning from 
	//				isValid = (state != GameState.PLAYER_ON_DEFENSE);	// state except itself
	//				break;
	//			case GameState.BALL_IN_AIR:								// valid if transitioning from
	//				isValid  = (state == GameState.PLAYER_ON_OFFENSE);	// PLAYER_ON_OFFENSE state
	//				isValid |= (state == GameState.PLAYER_ON_DEFENSE);	// PLAYER_ON_DEFENSE state
	//				isValid &= (state != GameState.BALL_IN_AIR);		// and not itself
	//				break;
	//			case GameState.BALL_ON_RIM:								// valid if transitioning from
	//				isValid  = (state == GameState.PLAYER_ON_OFFENSE);	// PLAYER_ON_OFFENSE state
	//				isValid |= (state == GameState.PLAYER_ON_DEFENSE);	// or PLAYER_ON_DEFENSE state
	//				isValid |= (state == GameState.BALL_IN_AIR);		// or BALL_IN_AIR
	//				isValid &= (state != GameState.BALL_ON_RIM); 		// and not itself
	//				break;
	//			case GameState.BALL_IN_BASKET:								// valid if transitioning from
	//				isValid  = (state == GameState.BALL_IN_AIR);		// or BALL_IN_AIR
	//				isValid |= (state != GameState.BALL_ON_RIM);		// or BALL_ON_RIM
	//				isValid &= (state != GameState.BALL_IN_BASKET);		// and not itself
	//				break;
	//			default:
	//				Debug.Log ("Error: state not recognized as a valid gamestate");
	//				break;
	//		}
	//		return isValid;
	//	}
	}	
}