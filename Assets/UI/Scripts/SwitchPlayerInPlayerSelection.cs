using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SwitchPlayerInPlayerSelection : MonoBehaviour {
	public GameObject[] choosePlayer;
	public Sprite[] chooseBtn;
	GameObject left;
	GameObject right;
	GameObject front;

	void Start() {
		left = GameObject.Find("Left");
		front = GameObject.Find("Front");
		right = GameObject.Find("Right");
	}

	public void pressPreviousBtn() {
		// current is third player
		if (choosePlayer [2].GetComponent<Image> ().sprite.name == "chosen-player-icon") {
			choosePlayer [0].GetComponent<Image> ().sprite = chooseBtn [1];
			choosePlayer [1].GetComponent<Image> ().sprite = chooseBtn [0];
			choosePlayer [2].GetComponent<Image> ().sprite = chooseBtn [1];
			left.SetActive (false);
			front.SetActive (true);
			right.SetActive (false);
		}
		// current is second player
		else if (choosePlayer [1].GetComponent<Image> ().sprite.name == "chosen-player-icon") {
			choosePlayer [0].GetComponent<Image> ().sprite = chooseBtn [0];
			choosePlayer [1].GetComponent<Image> ().sprite = chooseBtn [1];
			choosePlayer [2].GetComponent<Image> ().sprite = chooseBtn [1];
			left.SetActive (true);
			front.SetActive (false);
			right.SetActive (false);
		}
	}

	public void pressNextBtn() {
		// current is first player
		if (choosePlayer [0].GetComponent<Image> ().sprite.name == "chosen-player-icon") {
			choosePlayer [0].GetComponent<Image> ().sprite = chooseBtn [1];
			choosePlayer [1].GetComponent<Image> ().sprite = chooseBtn [0];
			choosePlayer [2].GetComponent<Image> ().sprite = chooseBtn [1];
			left.SetActive (false);
			front.SetActive (true);
			right.SetActive (false);
		}
		// current is second player
		else if (choosePlayer [1].GetComponent<Image> ().sprite.name == "chosen-player-icon") {
			choosePlayer [0].GetComponent<Image> ().sprite = chooseBtn [1];
			choosePlayer [1].GetComponent<Image> ().sprite = chooseBtn [1];
			choosePlayer [2].GetComponent<Image> ().sprite = chooseBtn [0];
			left.SetActive (false);
			front.SetActive (false);
			right.SetActive (true);
		}
	}
}
