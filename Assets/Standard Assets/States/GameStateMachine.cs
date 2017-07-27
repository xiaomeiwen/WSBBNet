using UnityEngine;
using System.Collections;

namespace WSBB.StateMachineBase
{
	public enum GameState
	{
		PREGAME,

		QUARTER_START,		//Game clock can answer this

		FIRST_QUARTER,	//control logic could calculate based on
		SECORD_QUARTER, //quarters per game and time per quarter
		THIRD_QUARTER,
		FOURTH_QUARTER,

		PAUSE,
		TIMEOUT,

		POSTGAME
	}

	public class GameStateMachine : StateMachine
	{
		/*
        public static State Pregame 		= new State (true, "1", "Pregame", "", 0.15f);

		public static State QuarterStart 	= new State(false, "2", "QuarterStart", "", 0.15f);

		public static State FirstQuarter	= new State(false, "3", "FirstQuarter", "", 0.15f);
		public static State SecondQuarter	= new State(false, "4", "SecondQuarter", "", 0.15f);
		public static State ThirdQuarter	= new State(false, "5", "ThirdQuarter", "", 0.15f);
		public static State FourthQuarter	= new State(false, "6", "FourthQuarter", "", 0.15f);

		public static State Pause 			= new State(false, "7", "Pause", "", 0.15f);
		public static State Timeout 		= new State(false, "8", "Timeout", "", 0.15f);

		public static State Postgame = new State (true, "1", "Postgame", "", 0.15f);
        */

        //** Game States relevant to the importFile **//
        public static State Quarter_Start = new State(false, "1", "Quarter_Start", "", 0.15f);

        public static State PLAYER_ON_OFFENSE = new State(true, "2", "PLAYER_ON_OFFENSE", "", 0.15f);
        public static State PLAYER_ON_DEFENSE = new State(false, "3", "PLAYER_ON_DEFENSE", "", 0.15f);

        public static State BALL_IN_AIR_PASS = new State(false, "4", "BALL_IN_AIR_PASS", "", 0.15f);
        public static State BALL_IN_AIR_SHOT = new State(false, "5", "BALL_IN_AIR_SHOT", "", 0.15f);
        public static State BALL_ON_RIM = new State(false, "6", "BALL_ON_RIM", "", 0.15f);
        public static State BALL_IN_BASKET = new State(false, "7", "BALL_IN_BASKET", "", 0.15f);

        public static State PAUSE_OR_TIME_OUT = new State(false, "8", "PAUSE_OR_TIME_OUT", "", 0.15f);

        //I chose to include having a Team owner for Game state machine for future
        //multiplayer where the there will be a player on either team
        public GameStateMachine(GameAdmin owner) : base(owner)
		{}
		
		public void Initialize()
		{
			base.Initialize();

            //TODO: determine correct default state
            /*
            this.AddState(Pregame, false);

			this.AddState(QuarterStart, false);

			this.AddState(FirstQuarter, true);
			this.AddState(SecondQuarter, false);
			this.AddState(ThirdQuarter, false);
			this.AddState(FourthQuarter, false);

			this.AddState(Pause, false);
			this.AddState(Timeout, false);

			this.AddState(Postgame, false);
            */
            this.AddState(Quarter_Start, false);
            this.AddState(PLAYER_ON_OFFENSE, true);
            this.AddState(PLAYER_ON_DEFENSE, false);
            this.AddState(BALL_IN_AIR_PASS, false);
            this.AddState(BALL_IN_AIR_SHOT, false);
            this.AddState(BALL_IN_BASKET, false);
            this.AddState(BALL_ON_RIM, false);
            this.AddState(PAUSE_OR_TIME_OUT, false);

        }
	}
}
