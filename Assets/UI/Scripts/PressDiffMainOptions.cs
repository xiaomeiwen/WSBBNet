using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PressDiffMainOptions : MonoBehaviour {
	public GameObject hiddenPanel;
	public GameObject bballerFloor;
	// this.gameObject.child0 is CardsBtn, child1 is NewsBtn, child2 is PlayBtn, child3 is BlingBtn, child4 is LevelBtn
	float offset;
	float normalWidth;
	float doubleWidth;
	Vector3 scaleUp;
	Vector3 scaleDown;
	Vector3 iconUp;
	Vector3 iconDown;
	Vector2 smallIconSize;
	Vector2 largeIconSize;
	GameObject cardsBtn;
	GameObject newsBtn;
	GameObject mainBtn;
	GameObject blingBtn;
	GameObject levelBtn;
	RectTransform cardsBtnRect;
	RectTransform newsBtnRect;
	RectTransform mainBtnRect;
	RectTransform blingBtnRect;
	RectTransform levelBtnRect;
	GameObject mainLargeIcon;
	GameObject mainSmallIcon;

	GameObject playerGroup;
	Vector3 mainGroupPos;
	Vector3 playerGroupPos;
	// Use this for initialization
	void Start () {
		offset = 100;
		normalWidth = 200;
		doubleWidth = 400;
		scaleUp = new Vector3 (1.5f, 1.5f, 1.0f);
		scaleDown = new Vector3 (1.0f, 1.0f, 1.0f);
		iconUp = new Vector3 (0f, 80f, 0f);
		iconDown = new Vector3 (0f, 40f, 0f);
		smallIconSize = new Vector2 (100f, 100f);
		largeIconSize = new Vector2 (150f, 180f);
		cardsBtn = this.gameObject.transform.GetChild (0).gameObject;
		cardsBtnRect = cardsBtn.GetComponent<RectTransform> ();
		newsBtn = this.gameObject.transform.GetChild (1).gameObject; 
		newsBtnRect = newsBtn.GetComponent<RectTransform> ();
		mainBtn = this.gameObject.transform.GetChild (2).gameObject; 
		mainBtnRect = mainBtn.GetComponent<RectTransform> ();
		blingBtn = this.gameObject.transform.GetChild (3).gameObject; 
		blingBtnRect = blingBtn.GetComponent<RectTransform> ();
		levelBtn = this.gameObject.transform.GetChild (4).gameObject; 
		levelBtnRect = levelBtn.GetComponent<RectTransform> ();
		mainLargeIcon = mainBtn.transform.GetChild (0).gameObject;
		mainSmallIcon = mainBtn.transform.GetChild (2).gameObject;
		playerGroup = GameObject.Find ("PlayerGroup");
		mainGroupPos = new Vector3 (-400f, -190f, 0f);
		playerGroupPos = new Vector3 (-200f, -190f, 0f);
	}

	public void pressCards() {
		// change playIcon
		mainLargeIcon.SetActive(false);
		mainSmallIcon.SetActive (true);

		// when level button was not pressed
		if (cardsBtnRect.rect.width == normalWidth) {
			int pressed = 0;
			hiddenPanel.SetActive (false);
			// get current pressed button index
			for (int i = 0; i < this.gameObject.transform.childCount; i++) {
				if (i != 0 && this.gameObject.transform.GetChild (i).GetComponent<RectTransform> ().rect.width == doubleWidth) {
					GameObject doubledObj = this.gameObject.transform.GetChild (i).gameObject;
					pressed = i;
					RectTransform doubledRect = doubledObj.GetComponent<RectTransform> ();
					// update the text font size of the previous pressed button
					RectTransform txtRect = doubledObj.transform.GetChild (1).GetComponent<RectTransform> ();
					txtRect.localScale = scaleDown;
					// update icon size of the previous pressed button except main button
					if (i != 2) {
						RectTransform iconRect = doubledObj.transform.GetChild (0).GetComponent<RectTransform> ();
						iconRect.localPosition = iconDown;
						iconRect.sizeDelta = smallIconSize;
					}
					// update the size and position of the previous pressed button
					doubledRect.sizeDelta = new Vector2 (normalWidth, doubledRect.rect.height);
					doubledRect.localPosition = new Vector3 (doubledRect.localPosition.x + offset, doubledRect.localPosition.y);
					for (int j = pressed - 1; j > 0; j--) {
						RectTransform updatePosObj = this.gameObject.transform.GetChild (j).GetComponent<RectTransform> ();
						updatePosObj.localPosition = new Vector3 (updatePosObj.localPosition.x + offset * 2, updatePosObj.localPosition.y);
					}
				}
			}
			// update the size and position of level button
			cardsBtnRect.sizeDelta = new Vector2 (doubleWidth, cardsBtnRect.rect.height);
			cardsBtnRect.localPosition = new Vector3 (cardsBtnRect.localPosition.x + offset, cardsBtnRect.localPosition.y);
			// update the text font size of cards button
			RectTransform cardsTxtRect = cardsBtn.transform.GetChild (1).GetComponent<RectTransform> ();
			cardsTxtRect.localScale = scaleUp;
			// update icon size of cards button
			RectTransform cardsIconRect = cardsBtn.transform.GetChild (0).GetComponent<RectTransform> ();
			cardsIconRect.localPosition = iconUp;
			cardsIconRect.sizeDelta = largeIconSize;
		}
	}

	public void pressNews() {
		// change playIcon
		mainLargeIcon.SetActive(false);
		mainSmallIcon.SetActive (true);

		int pressed = 0;
		// when bling button was not pressed
		if (newsBtnRect.rect.width == normalWidth) {
			hiddenPanel.SetActive (false);
			// get current pressed button index
			for (int i = 0; i < this.gameObject.transform.childCount; i++) {
				if (i != 1 && this.gameObject.transform.GetChild (i).GetComponent<RectTransform> ().rect.width == doubleWidth) {
					GameObject doubledObj = this.gameObject.transform.GetChild (i).gameObject;
					pressed = i;
					RectTransform doubledRect = doubledObj.GetComponent<RectTransform> ();
					// update the text font size of the previous pressed button
					RectTransform txtRect = doubledObj.transform.GetChild (1).GetComponent<RectTransform> ();
					txtRect.localScale = scaleDown;
					// update icon size of the previous pressed button except main button
					if (i != 2) {
						RectTransform iconRect = doubledObj.transform.GetChild (0).GetComponent<RectTransform> ();
						iconRect.localPosition = iconDown;
						iconRect.sizeDelta = smallIconSize;
					}
					// update the size and position of the previous pressed button
					doubledRect.sizeDelta = new Vector2 (normalWidth, doubledRect.rect.height);
					if (pressed < 1) {
						doubledRect.localPosition = new Vector3 (doubledRect.localPosition.x - offset, doubledRect.localPosition.y);
					} else {
						doubledRect.localPosition = new Vector3 (doubledRect.localPosition.x + offset, doubledRect.localPosition.y);
						for (int j = pressed - 1; j > 1; j--) {
							RectTransform updatePosObj = this.gameObject.transform.GetChild (j).GetComponent<RectTransform> ();
							updatePosObj.localPosition = new Vector3 (updatePosObj.localPosition.x + offset * 2, updatePosObj.localPosition.y);
						}
					}
				}
			}
			// update the size and position of bling button
			newsBtnRect.sizeDelta = new Vector2 (doubleWidth, newsBtnRect.rect.height);
			if (pressed < 1) {
				newsBtnRect.localPosition = new Vector3 (newsBtnRect.localPosition.x - offset, newsBtnRect.localPosition.y);
			} else {
				newsBtnRect.localPosition = new Vector3 (newsBtnRect.localPosition.x + offset, newsBtnRect.localPosition.y);
			}
			// update the text font size of news button
			RectTransform newsTxtRect = newsBtn.transform.GetChild (1).GetComponent<RectTransform> ();
			newsTxtRect.localScale = scaleUp;
			// update icon size of news button
			RectTransform newsIconRect = newsBtn.transform.GetChild (0).GetComponent<RectTransform> ();
			newsIconRect.localPosition = iconUp;
			newsIconRect.sizeDelta = largeIconSize;
		}
	}

	public void pressMain() {
		// change playIcon
		mainLargeIcon.SetActive(true);
		mainSmallIcon.SetActive (false);
		hiddenPanel.SetActive (true);

		bballerFloor.GetComponent <SwitchPlayer> ().enabled = false;

		int pressed = 0;
		// when bling button was not pressed
		if (mainBtnRect.rect.width == normalWidth) {
			// get current pressed button index
			for (int i = 0; i < this.gameObject.transform.childCount; i++) {
				if (i != 2 && this.gameObject.transform.GetChild (i).GetComponent<RectTransform> ().rect.width == doubleWidth) {
					GameObject doubledObj = this.gameObject.transform.GetChild (i).gameObject;
					pressed = i;
					RectTransform doubledRect = doubledObj.GetComponent<RectTransform> ();
					// update the text font size of the previous pressed button
					RectTransform txtRect = doubledObj.transform.GetChild (1).GetComponent<RectTransform> ();
					txtRect.localScale = scaleDown;
					// update icon size of the previous pressed button except main button
					if (i != 2) {
						RectTransform iconRect = doubledObj.transform.GetChild (0).GetComponent<RectTransform> ();
						iconRect.localPosition = iconDown;
						iconRect.sizeDelta = smallIconSize;
					}
					// update the size and position of the previous pressed button
					doubledRect.sizeDelta = new Vector2 (normalWidth, doubledRect.rect.height);
					if (pressed < 2) {
						doubledRect.localPosition = new Vector3 (doubledRect.localPosition.x - offset, doubledRect.localPosition.y);
						for (int j = pressed + 1; j < 2; j++) {
							RectTransform updatePosObj = this.gameObject.transform.GetChild (j).GetComponent<RectTransform> ();
							updatePosObj.localPosition = new Vector3 (updatePosObj.localPosition.x - offset * 2, updatePosObj.localPosition.y);
						}
					} else {
						doubledRect.localPosition = new Vector3 (doubledRect.localPosition.x + offset, doubledRect.localPosition.y);
						for (int j = pressed - 1; j > 2; j--) {
							RectTransform updatePosObj = this.gameObject.transform.GetChild (j).GetComponent<RectTransform> ();
							updatePosObj.localPosition = new Vector3 (updatePosObj.localPosition.x + offset * 2, updatePosObj.localPosition.y);
						}
					}
				}
			}
			// update the size and position of bling button
			mainBtnRect.sizeDelta = new Vector2 (doubleWidth, mainBtnRect.rect.height);
			if (pressed < 2) {
				mainBtnRect.localPosition = new Vector3 (mainBtnRect.localPosition.x - offset, mainBtnRect.localPosition.y);
			} else {
				mainBtnRect.localPosition = new Vector3 (mainBtnRect.localPosition.x + offset, mainBtnRect.localPosition.y);
			}
			// update the text font size of bling button
			RectTransform playTxtRect = mainBtnRect.transform.GetChild (1).GetComponent<RectTransform> ();
			playTxtRect.localScale = scaleUp;

			playerGroup.GetComponent<RectTransform> ().localPosition = mainGroupPos;
		}
	}
	
	public void pressBling() {
		// change playIcon
		mainLargeIcon.SetActive(false);
		mainSmallIcon.SetActive (true);

		bballerFloor.GetComponent <SwitchPlayer> ().enabled = false;

		int pressed = 0;
		// when bling button was not pressed
		if (blingBtnRect.rect.width == normalWidth) {
			hiddenPanel.SetActive (false);
			// get current pressed button index
			for (int i = 0; i < this.gameObject.transform.childCount; i++) {
				if (i != 3 && this.gameObject.transform.GetChild (i).GetComponent<RectTransform> ().rect.width == doubleWidth) {
					GameObject doubledObj = this.gameObject.transform.GetChild (i).gameObject;
					pressed = i;
					RectTransform doubledRect = doubledObj.GetComponent<RectTransform> ();
					// update the text font size of the previous pressed button
					RectTransform txtRect = doubledObj.transform.GetChild (1).GetComponent<RectTransform> ();
					txtRect.localScale = scaleDown;
					// update icon size of the previous pressed button except main button
					if (i != 2) {
						RectTransform iconRect = doubledObj.transform.GetChild (0).GetComponent<RectTransform> ();
						iconRect.localPosition = iconDown;
						iconRect.sizeDelta = smallIconSize;
					}
					// update the size and position of the previous pressed button
					doubledRect.sizeDelta = new Vector2 (normalWidth, doubledRect.rect.height);
					if (pressed < 3) {
						doubledRect.localPosition = new Vector3 (doubledRect.localPosition.x - offset, doubledRect.localPosition.y);
						for (int j = pressed + 1; j < 3; j++) {
							RectTransform updatePosObj = this.gameObject.transform.GetChild (j).GetComponent<RectTransform> ();
							updatePosObj.localPosition = new Vector3 (updatePosObj.localPosition.x - offset * 2, updatePosObj.localPosition.y);
						}
					} else {
						doubledRect.localPosition = new Vector3 (doubledRect.localPosition.x + offset, doubledRect.localPosition.y);
					}
				}
			}
			// update the size and position of bling button
			blingBtnRect.sizeDelta = new Vector2 (doubleWidth, blingBtnRect.rect.height);
			if (pressed < 3) {
				blingBtnRect.localPosition = new Vector3 (blingBtnRect.localPosition.x - offset, blingBtnRect.localPosition.y);
			} else {
				blingBtnRect.localPosition = new Vector3 (blingBtnRect.localPosition.x + offset, blingBtnRect.localPosition.y);
			}
			// update the text font size of bling button
			RectTransform blingTxtRect = blingBtn.transform.GetChild (1).GetComponent<RectTransform> ();
			blingTxtRect.localScale = scaleUp;
			// update icon size of bling button
			RectTransform blingIconRect = blingBtn.transform.GetChild (0).GetComponent<RectTransform> ();
			blingIconRect.localPosition = iconUp;
			blingIconRect.sizeDelta = largeIconSize;

			playerGroup.GetComponent<RectTransform> ().localPosition = playerGroupPos;
		}
	}

	public void pressLevel() {
		// change playIcon
		mainLargeIcon.SetActive(false);
		mainSmallIcon.SetActive (true);

		bballerFloor.GetComponent <SwitchPlayer> ().enabled = true;

		int pressed = 0;
		// when level button was not pressed
		if (levelBtnRect.rect.width == normalWidth) {
			hiddenPanel.SetActive (false);
			// get current pressed button index
			for (int i = 0; i < this.gameObject.transform.childCount; i++) {
				if (i != 4 && this.gameObject.transform.GetChild (i).GetComponent<RectTransform> ().rect.width == doubleWidth) {
					GameObject doubledObj = this.gameObject.transform.GetChild (i).gameObject;
					RectTransform doubledRect = doubledObj.GetComponent<RectTransform> ();
					pressed = i;
					// update the text font size of the previous pressed button
					RectTransform txtRect = doubledObj.transform.GetChild (1).GetComponent<RectTransform> ();
					txtRect.localScale = scaleDown;
					// update icon size of the previous pressed button except main button
					if (i != 2) {
						RectTransform iconRect = doubledObj.transform.GetChild (0).GetComponent<RectTransform> ();
						iconRect.localPosition = iconDown;
						iconRect.sizeDelta = smallIconSize;
					}
					// update the size and position of the previous pressed button
					doubledRect.sizeDelta = new Vector2 (normalWidth, doubledRect.rect.height);
					doubledRect.localPosition = new Vector3 (doubledRect.localPosition.x - offset, doubledRect.localPosition.y);
					for (int j = pressed + 1; j < 4; j++) {
						RectTransform updatePosObj = this.gameObject.transform.GetChild (j).GetComponent<RectTransform> ();
						updatePosObj.localPosition = new Vector3 (updatePosObj.localPosition.x - offset * 2, updatePosObj.localPosition.y);
					}
				}
			}
			// update the size and position of level button
			levelBtnRect.sizeDelta = new Vector2 (doubleWidth, levelBtnRect.rect.height);
			levelBtnRect.localPosition = new Vector3 (levelBtnRect.localPosition.x - offset, levelBtnRect.localPosition.y);
			// update the text font size of level button
			RectTransform levelTxtRect = levelBtn.transform.GetChild (1).GetComponent<RectTransform> ();
			levelTxtRect.localScale = scaleUp;
			// update icon size of level button
			RectTransform levelIconRect = levelBtn.transform.GetChild (0).GetComponent<RectTransform> ();
			levelIconRect.localPosition = iconUp;
			levelIconRect.sizeDelta = largeIconSize;

			playerGroup.GetComponent<RectTransform> ().localPosition = playerGroupPos;
		}
	}

	public void playGame() {
		hiddenPanel.SetActive(false);
	}
}
