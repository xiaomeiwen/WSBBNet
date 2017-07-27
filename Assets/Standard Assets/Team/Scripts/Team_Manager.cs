using UnityEngine;
using System.Collections;

namespace WSBB {
	public class Team_Manager : MonoBehaviour {
		public Team homeTeam = null;
		public Team awayTeam = null;

		private Team teamAI = null;
		private Team teamPlayer = null;

		public Team TeamAI
		{
			get{ return teamAI; }
		}
		public Team TeamPlayer
		{
			get{ return teamPlayer; }
		}

		[HideInInspector]public bool EnableDebugMsgs = true;

		// Use this for initialization
		void Awake () {

			if (homeTeam != null && awayTeam != null)
			{
				if (homeTeam.pureAI)
				{
					teamAI = homeTeam;
					teamPlayer = awayTeam;
				}
				else
				{
					teamAI = awayTeam;
					teamPlayer = homeTeam;
				}
			}


			//DebugMsg
			if(EnableDebugMsgs)
			{
				if(awayTeam == null || homeTeam == null)
					Debug.LogError("Team_Manager: Either AwayTeam or HomeTeam or both have not been set properly");
			}
		
		}

	}
}