using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using XMLSpace;

public class ChangeExpValue : MonoBehaviour {
//	public Text speedTxt, agilityTxt, powerTxt;
	private static int speedCount, agilityCount, powerCount;
	private float block, shoot, frThrow, steal, pass, drive, ball, guard;
	// to check whether the expvalue has changed, for further use in DataFromUI.cs
	public static bool isEverEnable;
	TeamLoader selectedTeam;
	public GameObject player_speed_img, player_agility_img, player_power_img, block_img, shoot_img, frThrow_img, steal_img, pass_img, drive_img, ball_img, guard_img;

	// In the array, 0-speed, 1-agility, 2-power
	public static int[] player1Exp = new int[3];
	public static int[] player2Exp = new int[3];
	public static int[] player3Exp = new int[3];
	private float expBarLen;
	private float skillBarLen;
	public Text[] playerName;

	// Use this for initialization
	void Start () {
		expBarLen = 360;
		skillBarLen = 300;
		isEverEnable = true;
		selectedTeam = GameObject.Find(SelectTeam.selectedTeamName).GetComponent<TeamLoader>();
		// Debug.Log(selectedTeam.player3_t_powerPercentage);
		if (SwitchPlayer.states == 0) {
			// get front player's speed, agility and power - player1
			speedCount = selectedTeam.player1_t_speedPercentage;
			agilityCount = selectedTeam.player1_t_agilityPercentage;
			powerCount = selectedTeam.player1_t_powerPercentage;
			playerName[0].text = selectedTeam.player1_t_firstName;
			playerName[1].text = selectedTeam.player1_t_firstName + "'s Skills:";
		}
		if (SwitchPlayer.states == 1) {
			// get left player's speed, agility and power - player2
			speedCount = selectedTeam.player2_t_speedPercentage;
			agilityCount = selectedTeam.player2_t_agilityPercentage;
			powerCount = selectedTeam.player2_t_powerPercentage;
			playerName[0].text = selectedTeam.player2_t_firstName;
			playerName[1].text = selectedTeam.player2_t_firstName + "'s Skills:";
		}
		if (SwitchPlayer.states == -1) {
			// get right player's speed, agility and power - player3
			speedCount = selectedTeam.player3_t_speedPercentage;
			agilityCount = selectedTeam.player3_t_agilityPercentage;
			powerCount = selectedTeam.player3_t_powerPercentage;
			playerName[0].text = selectedTeam.player3_t_firstName;
			playerName[1].text = selectedTeam.player3_t_firstName + "'s Skills:";
		}
		getSkillBaseVal ();
		updateExpVal();
//		Debug.Log (block + " " + shoot + " " + frThrow + " " + steal + " " + pass + " " + drive + " " + ball + " " + guard);
	}
	
	// Update is called once per frame
	void Update () {
//		speedTxt.text = speedCount + " EXP";
//		agilityTxt.text = agilityCount + " EXP";
//		powerTxt.text = powerCount + " EXP";	
	}

	public void addSpeed() {
		if (speedCount < 100) {
			speedCount += 1; 
		}
		updateExpVal();
	}

	public void minusSpeed() {
		if (speedCount > 0) {
			speedCount -= 1; 
		}
		updateExpVal();
	}

	public void addAgility() {
		if (agilityCount < 100) {
			agilityCount += 1;
		}
		updateExpVal();
	}

	public void minusAgility() {
		if (agilityCount > 0) {
			agilityCount -= 1; 
		}
		updateExpVal();
	}

	public void addPower() {
		if (powerCount < 100) {
			powerCount += 1; 
		}
		updateExpVal();
	}

	public void minusPower() {
		if (powerCount > 0) {
			powerCount -= 1;
		}
		updateExpVal();
	}

	public void getSkillBaseVal() {
		block = (powerCount + agilityCount / 2) / 2;
		shoot = (agilityCount + speedCount / 2) / 2;
		frThrow = (agilityCount + speedCount / 2) / 2;
		steal = (speedCount + powerCount / 2) / 2;
		pass = (agilityCount + speedCount / 2) / 2;
		drive = (powerCount + agilityCount / 2) / 2;
		ball = (agilityCount + powerCount / 2) / 2;
		guard = (powerCount + agilityCount / 2) / 2;
	}

	public void updateExpVal() {
		Formula.changePlayerExpBarLength (player_speed_img.GetComponent<RectTransform> (), expBarLen, 100f, speedCount);
		Formula.changePlayerExpBarLength (player_agility_img.GetComponent<RectTransform> (), expBarLen, 100f, agilityCount);
		Formula.changePlayerExpBarLength (player_power_img.GetComponent<RectTransform> (), expBarLen, 100f, powerCount);

		Formula.changePlayerExpBarLength (block_img.GetComponent<RectTransform> (), skillBarLen, 75f, block);
		Formula.changePlayerExpBarLength (shoot_img.GetComponent<RectTransform> (), skillBarLen, 75f, shoot);
		Formula.changePlayerExpBarLength (frThrow_img.GetComponent<RectTransform> (), skillBarLen, 75f, frThrow);
		Formula.changePlayerExpBarLength (steal_img.GetComponent<RectTransform> (), skillBarLen, 75f, steal);
		Formula.changePlayerExpBarLength (pass_img.GetComponent<RectTransform> (), skillBarLen, 75f, pass);
		Formula.changePlayerExpBarLength (drive_img.GetComponent<RectTransform> (), skillBarLen, 75f, drive);
		Formula.changePlayerExpBarLength (ball_img.GetComponent<RectTransform> (), skillBarLen, 75f, ball);
		Formula.changePlayerExpBarLength (guard_img.GetComponent<RectTransform> (), skillBarLen, 75f, guard);
	}

	public void player1ToPlayer2() {   // front player -> right player
		selectedTeam.player1_t_speedPercentage = speedCount;
		selectedTeam.player1_t_agilityPercentage = agilityCount;
		selectedTeam.player1_t_powerPercentage = powerCount;
		speedCount = selectedTeam.player2_t_speedPercentage;
		agilityCount = selectedTeam.player2_t_agilityPercentage;
		powerCount = selectedTeam.player2_t_powerPercentage;
		playerName[0].text = selectedTeam.player2_t_firstName;
		playerName[1].text = selectedTeam.player2_t_firstName + "'s Skills:";
		getSkillBaseVal ();
		updateExpVal();
	}

	public void player2ToPlayer3() {   // front player -> right player
		selectedTeam.player2_t_speedPercentage = speedCount;
		selectedTeam.player2_t_agilityPercentage = agilityCount;
		selectedTeam.player2_t_powerPercentage = powerCount;
		speedCount = selectedTeam.player3_t_speedPercentage;
		agilityCount = selectedTeam.player3_t_agilityPercentage;
		powerCount = selectedTeam.player3_t_powerPercentage;
		playerName[0].text = selectedTeam.player3_t_firstName;
		playerName[1].text = selectedTeam.player3_t_firstName + "'s Skills:";
		getSkillBaseVal ();
		updateExpVal();
	}

	public void player3ToPlayer1() {   // front player -> right player
		selectedTeam.player3_t_speedPercentage = speedCount;
		selectedTeam.player3_t_agilityPercentage = agilityCount;
		selectedTeam.player3_t_powerPercentage = powerCount;
		speedCount = selectedTeam.player1_t_speedPercentage;
		agilityCount = selectedTeam.player1_t_agilityPercentage;
		powerCount = selectedTeam.player1_t_powerPercentage;
		playerName[0].text = selectedTeam.player1_t_firstName;
		playerName[1].text = selectedTeam.player1_t_firstName + "'s Skills:";
		getSkillBaseVal ();
		updateExpVal();
	}

	public void getPlayerFinalExpVal() {
		Debug.Log ("yes");
		if (SwitchPlayer.states == 0) {
			// front player - player1
			player1Exp [0] = speedCount;
			player1Exp [1] = agilityCount;
			player1Exp [2] = powerCount;

			player2Exp [0] = selectedTeam.player2_t_speedPercentage;
			player2Exp [1] = selectedTeam.player2_t_agilityPercentage;
			player2Exp [2] = selectedTeam.player2_t_powerPercentage;

			player3Exp [0] = selectedTeam.player3_t_speedPercentage;
			player3Exp [1] = selectedTeam.player3_t_agilityPercentage;
			player3Exp [2] = selectedTeam.player3_t_powerPercentage;
		}
		if (SwitchPlayer.states == 1) {
			// left player - player2
			player1Exp [0] = selectedTeam.player1_t_speedPercentage;
			player1Exp [1] = selectedTeam.player1_t_agilityPercentage;
			player1Exp [2] = selectedTeam.player1_t_powerPercentage;

			player2Exp [0] = speedCount;
			player2Exp [1] = agilityCount;
			player2Exp [2] = powerCount;

			player3Exp [0] = selectedTeam.player3_t_speedPercentage;
			player3Exp [1] = selectedTeam.player3_t_agilityPercentage;
			player3Exp [2] = selectedTeam.player3_t_powerPercentage;
		}
		if (SwitchPlayer.states == -1) {
			// right player - player3
			player1Exp [0] = selectedTeam.player1_t_speedPercentage;
			player1Exp [1] = selectedTeam.player1_t_agilityPercentage;
			player1Exp [2] = selectedTeam.player1_t_powerPercentage;

			player2Exp [0] = selectedTeam.player2_t_speedPercentage;
			player2Exp [1] = selectedTeam.player2_t_agilityPercentage;
			player2Exp [2] = selectedTeam.player2_t_powerPercentage;

			player3Exp [0] = speedCount;
			player3Exp [1] = agilityCount;
			player3Exp [2] = powerCount;
		}
			
//		Debug.Log ("firstplayer: ");
//		foreach (int val in player1Exp) {
//			Debug.Log(val);
//		}
//
//		Debug.Log ("secondplayer: ");
//		foreach (int val in player2Exp) {
//			Debug.Log(val);
//		}
//
//		Debug.Log ("thirdplayer: ");
//		foreach (int val in player3Exp) {
//			Debug.Log(val);
//		}
	}
}
