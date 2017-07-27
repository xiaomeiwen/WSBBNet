using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using WSBB;

public class SelectedCardsPanel : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other) {
		Debug.Log ("Card entered the trigger");
	}

	void OnTriggerStay2D(Collider2D other) {
		Debug.Log ("Card is within trigger");
	}

	void OnTriggerExit2D(Collider2D other) {
		Debug.Log ("Card exited the trigger");
	}

	Powerup powerupCards;
	public static int quarterSize;
	public static int cardsEachQuarter;

	public static ArrayList firstQuar;
	public static ArrayList secondQuar;
	public static ArrayList thirdQuar;
	public static ArrayList fourthQuar;
	public static List<Powerup> firstQuarCards;
	public static List<Powerup> secondQuarCards;
	public static List<Powerup> thirdQuarCards;
	public static List<Powerup> fourthQuarCards;

	public string cardsLevel;
	public string cardName;

	void Start() {
		quarterSize = 4;
		cardsEachQuarter = 3;
		cardsLevel = "";
		firstQuar = new ArrayList();
		secondQuar = new ArrayList ();
		thirdQuar = new ArrayList ();
		fourthQuar = new ArrayList ();
		firstQuarCards = new List<Powerup>();
		secondQuarCards = new List<Powerup> ();
		thirdQuarCards = new List<Powerup> ();
		fourthQuarCards = new List<Powerup> ();
	}

	void Update() {
		
	}


	// run this method when "Play" button in the Cards page is pressed
	public void playGame() {
		cardsLevel = AssignCards.cardsLevel.Trim().ToUpper();

		for (int i = 0; i < cardsEachQuarter; i++) {
			Transform firstQuarCards = this.gameObject.transform.GetChild (0).GetChild (i);
			if (firstQuarCards.transform.childCount > 0) {
//				Debug.Log (firstQuarCards.GetChild (0).GetComponent<Image> ().sprite.name);
				cardName = firstQuarCards.GetChild (0).GetComponent<Image> ().sprite.name.Trim();
				firstQuar.Add (cardName);
			}

			Transform secondQuarCards = this.gameObject.transform.GetChild (1).GetChild (i);
			if (secondQuarCards.transform.childCount > 0) {
//				Debug.Log (secondQuarCards.GetChild (0).GetComponent<Image> ().sprite.name);
				cardName = secondQuarCards.GetChild (0).GetComponent<Image> ().sprite.name.Trim();
				secondQuar.Add (cardName);
			}

			Transform thirdQuarCards = this.gameObject.transform.GetChild (2).GetChild (i);
			if (thirdQuarCards.transform.childCount > 0) {
//				Debug.Log (thirdQuarCards.GetChild (0).GetComponent<Image> ().sprite.name);
				cardName = thirdQuarCards.GetChild (0).GetComponent<Image> ().sprite.name.Trim();
				thirdQuar.Add (cardName);
			}

			Transform fourthQuarCards = this.gameObject.transform.GetChild (3).GetChild (i);
			if (fourthQuarCards.transform.childCount > 0) {
//				Debug.Log (fourthQuarCards.GetChild (0).GetComponent<Image> ().sprite.name);
				cardName = fourthQuarCards.GetChild (0).GetComponent<Image> ().sprite.name.Trim();
				fourthQuar.Add (cardName);
			}
		}

		// for debugging
		/*
		foreach (string cardsName in firstQuar) {
			Debug.Log (cardsName + " in firstQuar");
		}

		foreach (string cardsName in secondQuar) {
			Debug.Log (cardsName + " in secondQuar");
		}

		foreach (string cardsName in thirdQuar) {
			Debug.Log (cardsName + " in thirdQuar");
		}

		foreach (string cardsName in fourthQuar) {
			Debug.Log (cardsName + " in fourthQuar");
		}
		*/

		// if the card is selected in the specific quarter, get the card's information and add the card's to the corresponding card arraylist
		foreach (Powerup allCards in XMLDataParser.powerUpList) {
			if (firstQuar.Count > 0) {
				foreach (string cardName in firstQuar) {
					if (cardsLevel == allCards.cardLevel.ToString() && cardName == allCards.Name) {
//						Debug.Log ("first: " + cardName + " CardLevel: " + cardsLevel + " Description: " + allCards.Description + " BuffBase: " + allCards.buffBasesString);
						powerupCards = allCards;
						firstQuarCards.Add (powerupCards);				
					}
				}
			}
			if (secondQuar.Count > 0) {
				foreach (string cardName in secondQuar) {
					if (cardsLevel == allCards.cardLevel.ToString() && cardName == allCards.Name) {
//						Debug.Log ("second: " + cardName + " CardLevel: " + cardsLevel + " Description: " + allCards.Description + " BuffBase: " + allCards.buffBasesString);
						powerupCards = allCards;
						secondQuarCards.Add (powerupCards);	
					}
				}
			}
			if (thirdQuar.Count > 0) {
				foreach (string cardName in thirdQuar) {
					if (cardsLevel == allCards.cardLevel.ToString() && cardName == allCards.Name) {
//						Debug.Log ("third: " + cardName + " CardLevel: " + cardsLevel + " Description: " + allCards.Description + " BuffBase: " + allCards.buffBasesString);
						powerupCards = allCards;
						thirdQuarCards.Add (powerupCards);	
					}
				}
			}
			if (fourthQuar.Count > 0) {
				foreach (string cardName in fourthQuar) {
					if (cardsLevel == allCards.cardLevel.ToString() && cardName == allCards.Name) {
//						Debug.Log ("fourth: " + cardName + " CardLevel: " + cardsLevel + " Description: " + allCards.Description + " BuffBase: " + allCards.buffBasesString);
						powerupCards = allCards;
						fourthQuarCards.Add (powerupCards);	
					}
				}
			}
		}

		// For debugging
		/*
		foreach (Powerup first in firstQuarCards) {
			Debug.Log ("first quarter: " + first.Name + " " + first.cardLevel + " " + first.Description + " buffBase0: " + first.buffBases[0]);
		}

		foreach (Powerup second in secondQuarCards) {
			Debug.Log ("second quarter: " + second.Name + " " + second.cardLevel + " " + second.Description + " buffBase0: " + second.buffBases[0]);
		}

		foreach (Powerup third in thirdQuarCards) {
			Debug.Log ("third quarter: " + third.Name + " " + third.cardLevel + " " + third.Description + " buffBase0: " + third.buffBases[0]);
		}

		foreach (Powerup fourth in fourthQuarCards) {
			Debug.Log ("fourth quarter: " + fourth.Name + " " + fourth.cardLevel + " " + fourth.Description + " buffBase0: " + fourth.buffBases[0]);
		}
		*/
	}
}
