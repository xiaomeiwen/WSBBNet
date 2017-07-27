using UnityEngine;
using System;
using System.Collections;

namespace WSBB {
	public class GameAnims : MonoBehaviour {
		public static String Stand_Ball = "Offense Dribble Right";
		public static String Walk_Ball = "Offense Jog Dibble Right";                   //Need animation for walk with ball, set it to same as jog for now
		public static String Jog_Ball = "Offense Jog Dibble Right";
		public static String Run_Ball = "Offense Run Dibble Right";                    
        public static String Stand_NoBall = "Idle Offense 2";
        public static String Remain_Open = "Idle Offense 2";
		public static String Walk_NoBall = "Walk";
		public static String Jog_NoBall = "JogNoBall";
		public static String Run_NoBall = "RunNoBall";
		public static String GuardLeft = "Defense Left High";
		public static String GuardRight = "Defense Left High";    //Currently no animation for Defense Right, revisit this when necessary
		public static String GuardForward = "Defense Forward";
		public static String GuardBack = "Defense Backward";
		public static String Shot = "Shot1";
		public static String Pass = "Pass Chest";
		public static String ReceivePass = "Pass Recieve_Chest";
		public static String Steal = "Defense Steal Bounce Pass";
		public static String BlockShot = "Defense Block 1";
		public static String DefenseForward = "Defense Forward";
		public static String DefenseBackward = "Defense Backward";
		public static String DefenseLeft = "Defense Left";
		public static String DefenseRight = "Defense Right";
		public static String OneHandDunk = "DunkOneHand";
		public static String ClassicDunk = "DunkClassic";
        public static String Clown1 = "Clowning 1";             // empty animation
        public static String Clown2 = "Clowning 2";             // empty animation
        public static String Clown3 = "Clowning 3";             // empty animation
        public static String Bamboozled1 = "Bamboozled 1";
        public static String Bamboozled2 = "Bamboozled 2";
        public static String Rebound1 = "JumpRebound";
        public static String Rebound2 = "JumpRebound2";
        public static String Stag_Leap = "Stag Leap";
		
		void Awake() {
			//GameObject.DontDestroyOnLoad(this.gameObject);
		}

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}
	}
}
