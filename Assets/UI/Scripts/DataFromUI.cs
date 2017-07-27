using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using XMLSpace;
using WSBB;

public class DataFromUI : MonoBehaviour {
	public string teamName;
	public string[] basketballers;
	// in the array, 0-speed, 1-agility, 2-power
	public int[] player1Exp;  
	public int[] player2Exp;
	public int[] player3Exp;
//	public GameObject selectedCards;
//	public Sprite[] firstQuar;
//	SelectedCardsPanel selectedCards;
	public string[] firstCards;
	public string[] secCards;
	public string[] thirdCards;
	public string[] fourthCards;
	TeamLoader teamObj;

	// court models and materials
	public Mesh selectedCourtMesh;
	public Material selectedCourtMat;
	public Material selectedCourtFence;
	public int selectedCourtIdx;

	public static List<Powerup> firstQuarCards;
	void Awake() {
		DontDestroyOnLoad (this.gameObject);
	}

	void Start() {
//		bballer = GameObject.Find ("BBallerFloor").transform;
	}

	public void getFinalData() {
//		bballer.gameObject.SetActive (false);
		teamName = SelectTeam.selectedTeamName;
		teamObj = GameObject.Find (SelectTeam.selectedTeamName).GetComponent<TeamLoader> ();
		if (ChangeExpValue.isEverEnable) {
			player1Exp = ChangeExpValue.player1Exp;
			player2Exp = ChangeExpValue.player2Exp;
			player3Exp = ChangeExpValue.player3Exp;
		} else {
			player1Exp [0] = teamObj.player1_t_speedPercentage;
			player2Exp [0] = teamObj.player2_t_speedPercentage;
			player3Exp [0] = teamObj.player3_t_speedPercentage;
			player1Exp [1] = teamObj.player1_t_agilityPercentage;
			player2Exp [1] = teamObj.player2_t_agilityPercentage;
			player3Exp [1] = teamObj.player3_t_agilityPercentage;
			player1Exp [2] = teamObj.player1_t_powerPercentage;
			player2Exp [2] = teamObj.player2_t_powerPercentage;
			player3Exp [2] = teamObj.player3_t_powerPercentage;
		}
			
		int i = 0;
		foreach (string cardsName in SelectedCardsPanel.firstQuar) {
			firstCards [i++] = cardsName; 
		}
		int j = 0;
		foreach (string cardsName in SelectedCardsPanel.secondQuar) {
			secCards [j++] = cardsName; 
		}
		int k = 0;
		foreach (string cardsName in SelectedCardsPanel.thirdQuar) {
			thirdCards [k++] = cardsName; 
		}
		int m = 0;
		foreach (string cardsName in SelectedCardsPanel.fourthQuar) {
			fourthCards [m++] = cardsName; 
		}

		firstQuarCards = SelectedCardsPanel.firstQuarCards;
//		TeamStat.powerUpCards = firstQuarCards;
//		text.text = ((Powerup)firstQuarCards [0]).Name;
		foreach (Powerup first in firstQuarCards) {
			Debug.Log ("first quarter: " + first.Name + " " + first.cardLevel + " " + first.Description + " buffBase0: " + first.buffBases[0]);
		}

//		for (int x = 0; x < bballer.childCount; x++) {
//			for (int y = 0; y < bballer.GetChild (x).childCount; y++) {
//				if (bballer.GetChild(0).GetChild(y).gameObject.activeSelf) {
//					passedFront = bballer.GetChild (0).GetChild (y).gameObject;
//				}
//				if (bballer.GetChild(1).GetChild(y).gameObject.activeSelf) {
//					passedLeft = bballer.GetChild (1).GetChild (y).gameObject;
//				}
//				if (bballer.GetChild(2).GetChild(y).gameObject.activeSelf) {
//					passedRight = bballer.GetChild (2).GetChild (y).gameObject;
//				}
//			}
//		}
	}
}
