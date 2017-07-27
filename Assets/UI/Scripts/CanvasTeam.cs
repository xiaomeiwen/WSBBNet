using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using XMLSpace;

public class CanvasTeam : MonoBehaviour {
	private GameObject bballerFloor;
	GameObject playerFront;
	GameObject playerLeft;
	GameObject playerRight;
	GameObject playerGroup;

	// Use this for initialization
	void Start () {
		//default values
		Canvas thisCanvas = GetComponentInParent<Canvas>();
		thisCanvas.enabled = true;
		bballerFloor = GameObject.Find ("BBallerFloor");
//		bballerFloor.GetComponent <SwitchPlayer> ().enabled = false;

//		GameObject contentT = GameObject.Find ("ContentTeam");
	}

	public void ScalePlayerModel () {
		playerFront = GameObject.Find ("Front");
		playerLeft = GameObject.Find ("Left");
		playerRight = GameObject.Find ("Right");
//		
//		playerFront.transform.localScale = new Vector3 (1.8f, 0.3f, 1.8f);
//		playerLeft.transform.localScale = new Vector3 (1.8f, 0.3f, 1.8f);
//		playerRight.transform.localScale = new Vector3 (1.8f, 0.3f, 1.8f);
		playerLeft.SetActive(true);
		playerFront.SetActive (false);
		playerRight.SetActive (false);

		// change playerModel's position and orientation
		playerGroup = GameObject.Find("PlayerGroup");

		Vector3 newPos = new Vector3 (-725f, -350f, 0f);
		playerGroup.GetComponent<RectTransform> ().localPosition = newPos;
		//		Debug.Log (playerGroup.GetComponent<RectTransform> ().localPosition);

		Vector3 frontPos = new Vector3 (0f, 300f, -0.3f);
		Vector3 frontRotate = new Vector3 (0f, -10f, 0f);
		playerLeft.GetComponent<Transform> ().localPosition = frontPos;
		playerLeft.GetComponent<Transform> ().Rotate (frontRotate);

		playerFront.GetComponent<Transform> ().localPosition = frontPos;
		playerFront.GetComponent<Transform> ().Rotate (frontRotate);

		playerRight.GetComponent<Transform> ().localPosition = frontPos;
		playerRight.GetComponent<Transform> ().Rotate (frontRotate);
	}

	GameObject teamIcon;
	GameObject selectedTeamIcon;
	GameObject teamName;

	public void PassTeamInfo() {
		// pass team icon
		teamIcon = GameObject.Find ("TeamIcon");
		selectedTeamIcon = GameObject.Find ("SelectedTeamIcon");
		selectedTeamIcon.GetComponent<Image>().sprite = teamIcon.GetComponent<Image> ().sprite;

		// pass team name
		teamName = GameObject.Find("SelectedTeamName");
		teamName.GetComponent<Text> ().text = SelectTeam.selectedTeamName;
	}
}