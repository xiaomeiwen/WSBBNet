using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class AssignCards : MonoBehaviour {
	private static int totalImg = 13;
	private static int assignedImgNum = 4;
	public Image[] cardsImgAssignedTo = new Image[assignedImgNum];
	public Image[] cardsCategory = new Image[assignedImgNum];
	public Image[] cardsQuarter = new Image[assignedImgNum];
	public Sprite[] cardsImage = new Sprite[totalImg];
	public Sprite[] cardsCategoryImg = new Sprite[3];
	public Sprite[] cardsQuarterImg = new Sprite[3];
	// Use this for initialization
	void Start () {
		for (int i = 0; i < assignedImgNum; i++) {
			cardsImgAssignedTo [i] = this.gameObject.transform.GetChild(i).GetChild(0).gameObject.GetComponent<Image> ();
			cardsCategory[i] = this.gameObject.transform.GetChild(i).GetChild(0).GetChild(0).gameObject.GetComponent<Image> ();
			cardsQuarter[i] = this.gameObject.transform.GetChild(i).GetChild(0).GetChild(1).gameObject.GetComponent<Image> ();
		}
	}

	void Update () {
//		TouchInput (GetComponent<BoxCollider2D> ());
	}

	// get the button name of seleted pack
	public string btnClicked = "";
	public static string cardsLevel = "";
	public void chooseCardCategory() {
		btnClicked = EventSystem.current.currentSelectedGameObject.name;
	}

	List<int> usedVals = new List<int>();
	public void assignCards() {
		// if select any pack of cards
		if (btnClicked != "") {
			// assign random cards image
			for (int i = 0; i < assignedImgNum; i++) {
				int rand = Random.Range (0, totalImg);
				//			Debug.Log (rand);
				while (usedVals.Contains (rand)) {
					rand = Random.Range (0, totalImg);
				}
				cardsImgAssignedTo [i].sprite = cardsImage [rand];
				usedVals.Add (rand);

				// assign cards category image and quarter image based on the selected pack
				if (btnClicked == "But_Bronze") {
					cardsCategory [i].sprite = cardsCategoryImg [0];
					cardsQuarter [i].sprite = cardsQuarterImg [0];
					cardsLevel = "Bronze";
				} else if (btnClicked == "But_Silver") {
					cardsCategory [i].sprite = cardsCategoryImg [1];
					cardsQuarter [i].sprite = cardsQuarterImg [1];
					cardsLevel = "Silver";
				} else if (btnClicked == "But_Gold") {
					cardsCategory [i].sprite = cardsCategoryImg [2];
					cardsQuarter [i].sprite = cardsQuarterImg [2];
					cardsLevel = "Gold";
				}
			}
		}
	}

//	public void dragCards() {
//		Vector3 pos;
//		pos = new Vector3 (Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position).x, Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position).y, transform.positon.z);
//		transform.position = pos;
//	}
}
