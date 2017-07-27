using UnityEngine;
using System;
using System.Collections;

namespace XMLSpace
{
	public class TeamLoader : MonoBehaviour {

	    private Team myTeam;
//	    public bool test = false;
		public DefaultPlayer[] players;
	    public int player1_t_uid;
	    public String player1_t_firstName;
	    public String player1_t_nickName;
	    public String player1_t_lastName;
	    public int player1_t_speedPercentage;
	    public int player1_t_agilityPercentage;
	    public int player1_t_powerPercentage;

	    public int player2_t_uid;
	    public String player2_t_firstName;
	    public String player2_t_nickName;
	    public String player2_t_lastName;
	    public int player2_t_speedPercentage;
	    public int player2_t_agilityPercentage;
	    public int player2_t_powerPercentage;

	    public int player3_t_uid;
	    public String player3_t_firstName;
	    public String player3_t_nickName;
	    public String player3_t_lastName;
	    public int player3_t_speedPercentage;
	    public int player3_t_agilityPercentage;
	    public int player3_t_powerPercentage;

		public TeamLoader() {
			players = new DefaultPlayer[3];
		}

	    public Team MyTeam
	    {
	        get { return myTeam; }
	        set { myTeam = value; }
	    }


		// Use this for initialization
		void Start () {
		}
		
		// Update is called once per frame
		void Update () {
		
		}

	    void LoadTeam(Team defaultTeam)
	    {
	        myTeam = defaultTeam;

	        // just to see values in editor, can be deleted later
	        player1_t_uid = myTeam.players[0].uid;
	        player1_t_firstName = myTeam.players[0].firstName;
	        player1_t_nickName = myTeam.players[0].nickName;
	        player1_t_lastName = myTeam.players[0].lastName;
	        player1_t_speedPercentage = myTeam.players[0].speedPercentage;
	        player1_t_agilityPercentage = myTeam.players[0].agilityPercentage;
	        player1_t_powerPercentage = myTeam.players[0].powerPercentage;

	        player2_t_uid = myTeam.players[1].uid;
	        player2_t_firstName = myTeam.players[1].firstName;
	        player2_t_nickName = myTeam.players[1].nickName;
	        player2_t_lastName = myTeam.players[1].lastName;
	        player2_t_speedPercentage = myTeam.players[1].speedPercentage;
	        player2_t_agilityPercentage = myTeam.players[1].agilityPercentage;
	        player2_t_powerPercentage = myTeam.players[1].powerPercentage;

	        player3_t_uid = myTeam.players[2].uid;
	        player3_t_firstName = myTeam.players[2].firstName;
	        player3_t_nickName = myTeam.players[2].nickName;
	        player3_t_lastName = myTeam.players[2].lastName;
	        player3_t_speedPercentage = myTeam.players[2].speedPercentage;
	        player3_t_agilityPercentage = myTeam.players[2].agilityPercentage;
	        player3_t_powerPercentage = myTeam.players[2].powerPercentage;
	    }
	}
}