using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Sprites;
using WSBB;

public class ImgFromUI : MonoBehaviour {
	private Image firstCard;
	private Image secCard;
	private Image thirdCard;
	private GameObject cardsData;
	private string[] firstCards = new string[3];
	private string[] secCards = new string[3];
	private string[] thirdCards = new string[3];
	private string[] fourthCards = new string[3];
	private int availCards;
	// Use this for initialization
	void Start () {
		cardsData = GameObject.Find ("DataRecorder");
		this.gameObject.GetComponent<Image> ().enabled = false;
		firstCards = cardsData.GetComponent<DataFromUI> ().firstCards;
		secCards = cardsData.GetComponent<DataFromUI> ().secCards;
		thirdCards = cardsData.GetComponent<DataFromUI> ().thirdCards;
		fourthCards = cardsData.GetComponent<DataFromUI> ().fourthCards;
			
		for (int i = 0; i < 3; i++) {
			if (firstCards [i] != "") {
				this.gameObject.GetComponent<Image> ().enabled = true;
				this.gameObject.transform.GetChild(i).GetComponent<Image> ().enabled = true;
				this.gameObject.transform.GetChild (i).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Cards/" + firstCards [i]);
			} else {
				this.gameObject.transform.GetChild (i).GetComponent<Image> ().enabled = false;
			}
		}

	}


	public void updateImg () {
		// if Scoreboard.quarterNumber == 2;
		this.gameObject.GetComponent<Image> ().enabled = false;
		for (int i = 0; i < 3; i++) {
			if (secCards [i] != "") {
				this.gameObject.GetComponent<Image> ().enabled = true;
				this.gameObject.transform.GetChild (i).GetComponent<Image> ().enabled = true;
				this.gameObject.transform.GetChild (i).GetComponent<Image> ().overrideSprite = Resources.Load ("Cards/" + secCards [i], typeof(Sprite)) as Sprite;
			} else {
//				this.gameObject.transform.GetChild (i).gameObject.SetActive (false);
				this.gameObject.transform.GetChild (i).GetComponent<Image> ().enabled = false;
			}
		}
	}
}
